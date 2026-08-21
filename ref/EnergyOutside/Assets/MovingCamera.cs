using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using System.IO;
using System.Text;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MovingCamera : MonoBehaviour
{
    private float m_fTopViewDistance = 1.0f;
    private float m_fTopViewAngle = 30.0f;

    //Vector3 vOrgPos = new Vector3(-2050.71f,4.827f,-5227.104f);
    Vector3 vOrgPos = new Vector3(-2051.698f, 5f, -5220.08f);
    Vector3 endPos = new Vector3(-2051.741f, 1.75f, -5223.9f);
    Vector3 startPos = new Vector3(-2051.741f, 1.91f, -5223.112f);
    Vector3 movingPos = new Vector3(-2051.741f, 1.91f, -5223.112f);         //이 지점으로 이동한 후에 zoomout-zoomin  
   
    Quaternion zOutRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
    Quaternion qOrg = Quaternion.Euler(new Vector3(50.64f, 0f, 0f));
    Vector3 zInPos = new Vector3();
    bool isZoomoutMoving = false;
    Vector3 zOutPos = new Vector3();

    bool isZoomIn = false;
    float m_Speed = 1.5f;


    private void AddPythonFunction()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {

            proxy.UserObject.SetVariable("SetMainModel", new Action<string>(SetMainModel));
            
            proxy.UserObject.SetVariable("CameraTranslate", new Action<float, float, float>(MoveTo));
            proxy.UserObject.SetVariable("CameraPosition", new Action(ReadPosition));
            proxy.UserObject.SetVariable("CameraAngles", new Action(ReadAngles));
            proxy.UserObject.SetVariable("CameraDirection", new Action(ReadDirection));

            proxy.UserObject.SetVariable("SetCameraPosition", new Action<float, float, float>(SetCameraPosition));
            proxy.UserObject.SetVariable("SetCameraAngles", new Action<float, float, float>(SetCameraAngles));
            proxy.UserObject.SetVariable("SetCameraDirection", new Action<float, float, float>(SetCameraDirection));

            proxy.UserObject.SetVariable("SetZoomPosition", new Action<float, float, float>(SetZoomPosition));
            proxy.UserObject.SetVariable("SetZoomObject", new Action<string>(SetZoomObject));
            proxy.UserObject.SetVariable("SetZoomObjectDistance", new Action<float>(SetZoomObjectDistance));
            proxy.UserObject.SetVariable("SetZoomObjectAngle", new Action<float>(SetZoomObjectAngle));
            proxy.UserObject.SetVariable("CameraView", new Action<string>(SetView));           
            proxy.UserObject.SetVariable("SelectObject", new Action<string>(SelectMesh));           
            proxy.UserObject.SetVariable("SetEarthquake", new Action<int, int>(SetEarthquake));

        }
    }
    public float shakeDuration = 35.0f;

    //지진 상황 관련 필드 
    public float shakeAmount = 0.005f;
    public float decreaseFactor = 1.0f;
    Vector3 originalPos;
    private bool isEarthquakeMode = false;

    Light lightComp = null;
    private int interval = 0;
    public float leftTime = 1.0f;
    private bool onoff_light_emergency = true;
    float timeLeft = 0.3f;
    GameObject zoomInBuilding = null;
    GameObject zoomoutBuilding = null;
    float zoomindistance = 1.1f;
    bool isAllBuildingSelect = false;
    //지진 시뮬레이션
    private void SetEarthquake(int amount, int seconds)
    {
        Camera mainCamera = Camera.main;
        originalPos = mainCamera.transform.localPosition;        
        Light[] lights = FindObjectsOfType(typeof(Light)) as Light[];
        lightComp = lights[0];
       
        switch (amount)
        {
            case 1: shakeAmount = 0.008f;
                break;
            case 2 : shakeAmount = 0.011f;
                break;
            case 3: shakeAmount = 0.015f;
                break;
            case 4: shakeAmount = 0.018f;
                break;
        }
        shakeDuration = (float) seconds;
        
        
        isEarthquakeMode = true;
        ModelManager.Instance.Model.SaveSharedFile("SetEarthquake");   
    }

    
    void CameraZoomoutMoving(string meshName)
    {
        zoomInBuilding = ModelManager.Instance.Model.transform.Find(meshName).gameObject;
        zoomoutBuilding = GameObject.FindGameObjectWithTag("zoomoutbuilding");
      

        zInPos = zoomInBuilding.transform.position;           
        zOutPos = zoomoutBuilding.transform.position;       
        startPos = transform.position;
        isZoomoutMoving = true;

    }
    void EarthquakeCameraZoomMoving(string meshName)
    {
        zoomindistance = 3f;
        isAllBuildingSelect = true;
        CameraZoomoutMoving(meshName);
    }

    private void SetCameraPosition(float x, float y, float z)
    {
        Camera mainCamera = Camera.main;
        mainCamera.transform.position = new Vector3(x, y, z);
    }

    private void SetCameraAngles(float x, float y, float z)
    {
        Camera mainCamera = Camera.main;
        mainCamera.transform.eulerAngles = new Vector3(x, y, z);
    }

    private void SetCameraDirection(float x, float y, float z)
    {
        Camera mainCamera = Camera.main;
        mainCamera.transform.forward = new Vector3(x, y, z);
    }


    private string m_szMainModelName = "Energy03";

    private void SetMainModel(string szName)
    {
        m_szMainModelName = szName;
    }


    private void SetView(string szViewName)
    {
        szViewName = szViewName.ToLower();
        if( szViewName == "top")
        {
            Camera mainCamera = Camera.main;		
			float asp = mainCamera.aspect;
            Transform movObj = ModelManager.Instance.Model.transform.Find(m_szMainModelName);
            if( movObj != null)
            {
                MeshRenderer mr = movObj.gameObject.GetComponent<MeshRenderer>();
                if( mr != null)
                {
                    Bounds aab = mr.bounds;

                    float width = aab.max.x - aab.min.x;
			        float height = aab.max.y - aab.min.y;				
			        float depth = aab.max.z - aab.min.z;

			        float len = (width/height > asp) ? width/asp : height;
			        len *= 0.5f;

			        float fov = mainCamera.fieldOfView * 0.5f;
			        len = len / Mathf.Tan(fov);

			        Vector3 vPosCam = aab.center;
                    vPosCam.y += (len + width);

                    mainCamera.transform.position = vPosCam;
                    mainCamera.transform.forward = (aab.center - vPosCam);
                }
            }
        }
        else if(szViewName == "front")
        {
            Camera mainCamera = Camera.main;
            float asp = mainCamera.aspect;
            Transform movObj = ModelManager.Instance.Model.transform.Find(m_szMainModelName);
            if (movObj != null)
            {
                MeshRenderer mr = movObj.gameObject.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    Bounds aab = mr.bounds;
                    float width = aab.max.x - aab.min.x;
                    float height = aab.max.y - aab.min.y;
                    float depth = aab.max.z - aab.min.z;

                    float fov = mainCamera.fieldOfView * 0.5f;
                    float len = (width / height > asp) ? width / asp : height;
                    len *= 0.5f;

                    Vector3 vCamPos = aab.center;
                    vCamPos.z -= (len + depth * 0.5f);

                    Vector3 newDir = aab.center - vCamPos;
                    float flength = newDir.magnitude * 2f / 3f;
                    vCamPos.y = flength;

                    mainCamera.transform.position = vCamPos;
                    mainCamera.transform.forward = (aab.center - vCamPos);
                }
            }
        }
        else if(szViewName == "rear")
        {
            Camera mainCamera = Camera.main;
            float asp = mainCamera.aspect;
            Transform movObj = ModelManager.Instance.Model.transform.Find(m_szMainModelName);
            if (movObj != null)
            {
                MeshRenderer mr = movObj.gameObject.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    Bounds aab = mr.bounds;
                    float width = aab.max.x - aab.min.x;
                    float height = aab.max.y - aab.min.y;
                    float depth = aab.max.z - aab.min.z;

                    float fov = mainCamera.fieldOfView * 0.5f;
                    float len = (width / height > asp) ? width / asp : height;
                    len *= 0.5f;

                    Vector3 vCamPos = aab.center;
                    vCamPos.z += (len + depth * 0.5f);

                    Vector3 newDir = aab.center - vCamPos;
                    float flength = newDir.magnitude * 2f / 3f;
                    vCamPos.y = flength;

                    mainCamera.transform.position = vCamPos;
                    mainCamera.transform.forward = (aab.center - vCamPos);
                }
            }
        }
        else if (szViewName == "left")
        {
            Camera mainCamera = Camera.main;
            float asp = mainCamera.aspect;
            Transform movObj = ModelManager.Instance.Model.transform.Find(m_szMainModelName);
            if (movObj != null)
            {
                MeshRenderer mr = movObj.gameObject.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    Bounds aab = mr.bounds;
                    float width = aab.max.x - aab.min.x;
                    float height = aab.max.y - aab.min.y;
                    float depth = aab.max.z - aab.min.z;

                    float len = (width / height > asp) ? width / asp : height;
                    len *= 0.5f;
                    float fov = mainCamera.fieldOfView * 0.5f;
                    len = len / Mathf.Tan(fov);

                    Vector3 center = aab.center;
                    Vector3 yCenter = aab.center;
                    yCenter.z -= (len + depth * 0.5f);
                    center.x -= (len + width * 0.5f);
                    Vector3 newDir = aab.center - center;

                    Vector3 newDir2 = aab.center - yCenter;
                    float flength = newDir2.magnitude * 2f / 3f;
                    center.y = flength;

                    newDir = aab.center - center;

                    mainCamera.transform.position = center;
                    mainCamera.transform.forward = newDir;
                }
            }
        }
        else if (szViewName == "right")
        {
            Camera mainCamera = Camera.main;		
			float asp = mainCamera.aspect;
            Transform movObj = ModelManager.Instance.Model.transform.Find(m_szMainModelName);
            if( movObj != null)
            {
                MeshRenderer mr = movObj.gameObject.GetComponent<MeshRenderer>();
                if( mr != null)
                {
                    Bounds aab = mr.bounds;
                    float width = aab.max.x - aab.min.x;
			        float height = aab.max.y - aab.min.y;				
			        float depth = aab.max.z - aab.min.z;

			        float len = (width/height > asp) ? width/asp : height;
			        len *= 0.5f;

			        float fov = mainCamera.fieldOfView * 0.5f;
			        len = len / Mathf.Tan(fov);
                    			        
                    Vector3 yCenter = aab.center;                    
                    yCenter.z -= (len + depth * 0.5f);                                       

                    Vector3 newDir2 = aab.center - yCenter;
                    float flength = newDir2.magnitude * 2f / 3f;

                    Vector3 camPos = aab.center;
                    camPos.x += (len + width * 0.5f);
                    camPos.y = flength;

                    Vector3 newDir = aab.center - camPos;
                    mainCamera.transform.position = camPos;
                    mainCamera.transform.forward = newDir;
                }
            }
        }
        else if( szViewName == "fit")
        {
            Camera mainCamera = Camera.main;		
			float asp = mainCamera.aspect;
            Transform movObj = ModelManager.Instance.Model.transform.Find(m_szMainModelName);
            if (movObj != null)
            {
                MeshRenderer mr = movObj.gameObject.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    Bounds aab = mr.bounds;
                    float width = aab.max.x - aab.min.x;
                    float height = aab.max.y - aab.min.y;
                    float depth = aab.max.z - aab.min.z;

                    float fov = mainCamera.fieldOfView * 0.5f;
                    float len = (width / height > asp) ? width / asp : height;
                    len *= 0.5f;
                    
                    Vector3 center = aab.center;
                    center.z -= (len + depth * 0.5f);
                    Vector3 newDir = aab.center - center;
                    float flength = newDir.magnitude * 2f / 3f;
                    center.y = flength;

                    mainCamera.transform.position = center;
                    mainCamera.transform.forward = (aab.center - center);
                }
            }
        }
    }

    private void MoveTo(float x, float y, float z)
    {
        this.transform.Translate(new Vector3(x, y, z));
    }

    private void ReadPosition()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            Vector3 vec = this.transform.position;

            ModelManager.Instance.Model.SaveSharedFile("CameraPosition", vec.x, vec.y, vec.z);

            //string szMsg = string.Format("SendMessage('MainCameraPoisition({0},{1},{2})')", vec.x, vec.y, vec.z);
            //proxy.RunPythonScript(szMsg);
        }
    }

    private void ReadAngles()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            Vector3 vec = this.transform.eulerAngles;
            ModelManager.Instance.Model.SaveSharedFile("CameraAngles", vec.x, vec.y, vec.z);
            //string szMsg = string.Format("SendMessage('MainCameraAngles({0},{1},{2})')", vec.x, vec.y, vec.z);
            //proxy.RunPythonScript(szMsg);
        }
    }

    private void ReadDirection()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            Vector3 vec = this.transform.forward;
            ModelManager.Instance.Model.SaveSharedFile("CameraDirection", vec.x, vec.y, vec.z);
            //string szMsg = string.Format("SendMessage('MainCameraDirection({0},{1},{2})')", vec.x, vec.y, vec.z);
            //proxy.RunPythonScript(szMsg);
        }
    }

    private float orgIntensity = 0.37f;
    // Use this for initialization
    void Awake()
    {
        AddPythonFunction();
       
    }

    void Start()
    {
       // SetEarthquake(3, 2);
       // CameraZoomoutMoving("z03");
    }

    
    Vector3 lastposition = new Vector3();
    float angleSpeed = 1f;
   
    bool isEarthQuakePost = false;

    public float postTimeLeft = 5.0f;

    void Update()
    {
        if (isZoomoutMoving)
        {          
            float distance = Vector3.Distance(transform.position, vOrgPos);

            Debug.logger.Log("Distance : " + distance);

            if (distance >= 0.1)
            {
                Vector3 dirCam = (vOrgPos - transform.position).normalized;
                Quaternion rotation = Quaternion.LookRotation(zOutPos - transform.position);
                transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, rotation, Time.deltaTime * angleSpeed);                
                transform.Translate(dirCam * Time.deltaTime * m_Speed, Space.World); ;
                Debug.logger.Log("Camera : " + transform.position + "     Vorgpos :  " + vOrgPos);
            }
            else
            {
                lastposition = transform.position;
                isZoomoutMoving = false; 
                isZoomIn = true;
            }

        }
        else if (isZoomIn)
        {            
            float distance = Vector3.Distance(transform.position, zInPos);
            // float distance = hit1.distance;
            Debug.logger.Log("zInPos : " + zInPos);
            if (distance >= zoomindistance)            //zoomin시 해당 건물과 거리.
            {
                Vector3 dirEBuilding = (zInPos - vOrgPos).normalized;
                Quaternion rotation = Quaternion.LookRotation(zInPos - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * angleSpeed);
                transform.Translate(dirEBuilding * Time.deltaTime * m_Speed, Space.World);

                //Camera.main.transform.position = vPos;
                // Debug.logger.Log("2. dirCam :" + dirCam + "   " + "dir :" + dir + "   " + "vPos :" + vPos + "   ");

            }
            else
            {
                isZoomIn = false;
                if (isAllBuildingSelect)
                {
                    isEarthQuakePost = true;
                }
            }
           
        }

        if (isEarthQuakePost)
        {
            if (postTimeLeft >= 0)       //지진 신호 처리 후 5초 동안 건물 빨간색 유지.
            {
                string[] allbuildings = { "z01", "z02", "z03", "New_Tank", "z05", "z06", "z04-1", "z04-2", "z08", "z05-2", "z10" };

                foreach (string meshname in allbuildings)
                {
                    SelectMeshWithZoomMoving(meshname);
                }
                postTimeLeft = postTimeLeft - Time.deltaTime;
            }
            else if (postTimeLeft < 0)
            {
                isAllBuildingSelect = false;
                postTimeLeft = 5.0f;
                zoomindistance = 1.1f;
                isEarthQuakePost = false;
                ModelManager.Instance.Model.AllClearBuildings();
                string szMsg3 = "EarthquakeBeepFinish()";
                Debug.logger.Log(szMsg3);
                                
                if (PassivePipeProxy.Instance != null)
                    PassivePipeProxy.Instance.SendServer(szMsg3);
            }
        }

        

        if (isEarthquakeMode)
        {
            Camera mainCamera = Camera.main;

            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                if (onoff_light_emergency)
                {
                    lightComp.intensity = 3.7f;
                    lightComp.color = new Color(0.9f, 0.1f, 0.1f);
                    onoff_light_emergency = false;
                    timeLeft = 0.8f;
                }
                else
                {
                    lightComp.intensity = orgIntensity;
                    lightComp.color = Color.white;
                    onoff_light_emergency = true;
                    timeLeft = 0.2f;
                }
            }

            if (shakeDuration > 0)
            {
                if (interval == 2)
                {
                    mainCamera.transform.localPosition = originalPos + UnityEngine.Random.insideUnitSphere * shakeAmount;
                    shakeDuration -= Time.deltaTime * decreaseFactor;
                    interval = 0;
                }

                interval++;
            }
            else
            {
                shakeDuration = 0f;
                mainCamera.transform.localPosition = originalPos;
                lightComp.intensity = orgIntensity;
                lightComp.color = Color.white;

                isEarthquakeMode = false;

                string szMsg = "EarthquakeFinished()";
                Debug.logger.Log(szMsg);
                //System.Diagnostics.Trace.WriteLine(szMsg);
                if (PassivePipeProxy.Instance != null)
                    PassivePipeProxy.Instance.SendServer(szMsg);

                EarthquakeCameraZoomMoving("z03");
                
                


            }
        }
    }

    private void OnMouseDown()
    {
    }

    private void OnMouseUp()
    {

    }
    void LateUpdate()
    {
    
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    private void FindOptimunView(Vector3 vPos, int nlayer, bool bTopview, float topViewAngle, float topViewDist)
    {

        Camera mainCamera = Camera.main;


        float scale = 1.0f;// ModelManager.Instance.Model.transform.localScale.x;
        Vector3 vCenter = vPos;
        //float len = objBound.extents.magnitude * 4 + mainCamera.nearClipPlane; // scale;
        float len = (topViewDist + mainCamera.nearClipPlane) * scale;

        if (bTopview)
        {
            Vector3 unitY = new Vector3(0f, 1f, 0f);
            Vector3 unitX = new Vector3(1f, 0f, 0f);
            Quaternion q1 = Quaternion.AngleAxis((float)-topViewAngle, unitY);
            Quaternion q2 = Quaternion.AngleAxis((float)-90, unitX);

            float dist = (float)(topViewDist * scale);
            if (topViewDist == 0)
                dist = len;

            mainCamera.transform.rotation = (q1 * q2);
            mainCamera.transform.position = (vCenter + unitY * dist);
            return;
        }

        Vector3[] pos = new Vector3[4];
        bool[] bRes = new bool[4];
        float[] dist2 = new float[4];
        pos[0] = new Vector3(0f, 1f, 1f);
        pos[1] = new Vector3(1f, 1f, 0f);
        pos[2] = new Vector3(0f, 1f, -1f);
        pos[3] = new Vector3(-1f, 1f, 0f);
        for (int i = 0; i < 4; i++)
        {
            bRes[i] = false;
        }
        for (int i = 0; i < 4; i++)
        {
            pos[i] = (pos[i].normalized * len) + vCenter;
            Ray ray = new Ray();
            ray.origin = vCenter;
            Vector3 dir = pos[i] - vCenter;
            dir.y = 0;
            ray.direction = dir.normalized;

            RaycastHit hit1;
            if (Physics.Raycast(ray, out hit1, Mathf.Infinity))
            {
                if (hit1.distance <= Math.Cos(Math.PI * 0.5f * 0.5f) * len)
                {
                    bRes[i] = true;
                    dist2[i] = hit1.distance;
                }
            }
        }

        Vector3 vResPos = new Vector3();
        if (!bRes[0] && !bRes[1] && !bRes[2] && !bRes[3])
        {
            vResPos = pos[0];
        }
        else if (bRes[0] && bRes[1] && bRes[2] && bRes[3])
        {
            int sel = 0;
            float selDist = 0.0f;
            for (int i = 0; i < 4; i++)
            {
                if (i == 0)
                {
                    sel = 0;
                    selDist = dist2[i];
                }
                else
                {
                    if (selDist < dist2[i])
                    {
                        selDist = dist2[i];
                        sel = i;
                    }
                }
            }
            vResPos = pos[sel];
        }
        else if (bRes[0])
        {
            if (!bRes[2]) vResPos = pos[2];
            else if (!bRes[1]) vResPos = pos[1];
            else vResPos = pos[3];
        }
        else if (bRes[1])
        {
            if (!bRes[3]) vResPos = pos[3];
            else if (!bRes[2]) vResPos = pos[2];
            else vResPos = pos[0];
        }
        else if (bRes[2])
        {
            if (!bRes[0]) vResPos = pos[0];
            else if (!bRes[1]) vResPos = pos[1];
            else vResPos = pos[3];
        }
        else if (bRes[3])
        {
            if (!bRes[1]) vResPos = pos[1];
            else if (!bRes[2]) vResPos = pos[2];
            else vResPos = pos[0];
        }
        mainCamera.transform.position = vResPos;
        mainCamera.transform.forward = (vCenter - vResPos);
            
        
    }

    private void FindOptimunView(string szObjectName, int nlayer, bool bTopview, float topViewAngle, float topViewDist)
	{
        Camera mainCamera = Camera.main;
                   
        Transform movObj = ModelManager.Instance.Model.transform.Find(szObjectName);
        if( movObj != null)
        {
            if( movObj.childCount > 0)
            {
                movObj = movObj.GetChild(0);
            }

            MeshRenderer mr = null;
            if (szObjectName == "_z10-1")
            {
                Transform t = movObj.FindChild("z10-1");
                mr = t.gameObject.GetComponent<MeshRenderer>();                
            }
            else
            {
                mr = movObj.gameObject.GetComponent<MeshRenderer>();
            }
            
            if( mr != null)
            {
                Bounds objBound = mr.bounds;
                float scale =  ModelManager.Instance.Model.transform.localScale.x;
                if( scale > 1.0f)
                {
                    scale = 1.0f;
                }
				Vector3 vCenter = objBound.center * scale;
				//float len = objBound.extents.magnitude * 4 + mainCamera.nearClipPlane; // scale;
                float len = ( topViewDist + mainCamera.nearClipPlane) * scale;
                
                if(bTopview)
				{
                    Vector3 unitY = new Vector3(0f, 1f, 0f);
                    Vector3 unitX = new Vector3(1f, 0f, 0f);
                    Quaternion q1 = Quaternion.AngleAxis((float)-topViewAngle, unitY);
                    Quaternion q2 = Quaternion.AngleAxis((float)-90, unitX);
					
					float dist = (float)(topViewDist * scale);
					if(topViewDist == 0)
						dist = len;

					mainCamera.transform.rotation = (q1 * q2);
					mainCamera.transform.position = (vCenter + unitY * dist);
					return;
				}

                Vector3 [] pos = new Vector3[4];
				bool [] bRes = new bool[4];
				float [] dist2 = new float[4];
				pos[0] = new Vector3(0f, 1f, 1f);
				pos[1] = new Vector3(1f, 1f, 0f);
				pos[2] = new Vector3(0f, 1f, -1f);
				pos[3] = new Vector3(-1f, 1f, 0f);
				for(int i = 0; i < 4; i++)
				{
					bRes[i] = false;
				}
				for(int i = 0; i < 4; i++)
				{                    
                    pos[i] = (pos[i].normalized * len) + vCenter;
					Ray ray = new Ray();
					ray.origin = vCenter;
					Vector3 dir = pos[i] - vCenter;
					dir.y = 0;
					ray.direction = dir.normalized;

                    RaycastHit hit1;   
                    if (Physics.Raycast(ray, out hit1, Mathf.Infinity))
                    {               
                        if( hit1.distance <= Math.Cos(Math.PI *0.5f * 0.5f) * len)
                        {
                            bRes[i] = true;
                            dist2[i] = hit1.distance;
                        }                       
                    }	
                }
		
                Vector3 vResPos = new Vector3();
				if(!bRes[0] && !bRes[1] && !bRes[2] && !bRes[3])
				{
					vResPos = pos[0];
				}
				else if(bRes[0] && bRes[1] && bRes[2] && bRes[3])
				{
					int sel = 0;
                    float selDist = 0.0f;
					for(int i = 0; i < 4; i++)
					{
						if(i == 0)
						{
							sel = 0;
                            selDist = dist2[i];
						}
						else
						{
                            if (selDist < dist2[i])
							{
                                selDist = dist2[i];
								sel = i;
							}
						}
					}
					vResPos = pos[sel];
				}
				else if(bRes[0])
				{
					if(!bRes[2])      vResPos = pos[2];
					else if(!bRes[1]) vResPos = pos[1];
					else			  vResPos = pos[3];
				}
				else if(bRes[1])
				{
					if(!bRes[3])      vResPos = pos[3];
					else if(!bRes[2]) vResPos = pos[2];
					else			  vResPos = pos[0];
				}
				else if(bRes[2])
				{
					if(!bRes[0])      vResPos = pos[0];
					else if(!bRes[1]) vResPos = pos[1];
					else			  vResPos = pos[3];
				}
				else if(bRes[3])
				{
					if(!bRes[1])	  vResPos = pos[1];
					else if(!bRes[2]) vResPos = pos[2];
					else			  vResPos = pos[0];
				}
				mainCamera.transform.position = vResPos;
				mainCamera.transform.forward = (vCenter - vResPos);
            }           
        }
    }

    private void SetZoomObjectDistance(float fDistance)
    {
        m_fTopViewDistance = fDistance;
    }
    
    private void SetZoomObjectAngle(float fAngle)
    {
        m_fTopViewAngle = fAngle;
    }

    private void SetZoomObject(string szObjectName)
    {
        if (szObjectName == "z10-1")
            szObjectName = "_z10-1";        

        FindOptimunView(szObjectName, 1, false, m_fTopViewAngle, m_fTopViewDistance);
    }

    /*
    * 지진 상황에서 ZoomMoving을 하기 위한 함수.내부에서만 사용
    * 광교에서는 모든 빌딩을 빨갛게 선택되도록 요구사항대로.
    */
    private void SelectMeshWithZoomMoving(string szMeshName)
    {
        Debug.logger.Log("Select Object : " + szMeshName);
        Transform movObj = ModelManager.Instance.Model.transform.Find(szMeshName);
        if (!isAllBuildingSelect)
            CameraZoomoutMoving(szMeshName);
        if (movObj != null)
        {
            if (szMeshName == "_z10-1")
            {
                Transform t = movObj.FindChild("z10-1");
                SelectionModel smt = t.gameObject.GetComponent<SelectionModel>();
                if (smt != null)
                {
                    smt.SelectObject();
                }
                Transform t2 = movObj.FindChild("z10-2");
                SelectionModel smt2 = t2.gameObject.GetComponent<SelectionModel>();
                if (smt2 != null)
                {
                    smt2.SelectObject();
                }
            }
            else
            {
                if (movObj.childCount > 0)
                {
                    for (int i = 0; i < movObj.childCount; i++)
                    {
                        Transform t = movObj.GetChild(i);
                        SelectionModel smt = t.gameObject.GetComponent<SelectionModel>();
                        if (smt != null)
                        {
                            smt.SelectObject();
                        }
                    }
                }
                else
                {
                    SelectionModel sm = movObj.gameObject.GetComponent<SelectionModel>();
                    if (sm != null)
                    {
                        sm.SelectObject();
                    }
                    else
                    {
                        Transform t = movObj.FindChild(szMeshName);
                        SelectionModel smt = t.gameObject.GetComponent<SelectionModel>();
                        if (smt != null)
                        {
                            smt.SelectObject();
                        }
                        else
                        {
                            Debug.logger.Log("Select Object : " + szMeshName + " Not Found");
                        }
                    }
                }

            }
        }
        else
        {
            Debug.logger.Log("Select Object : " + szMeshName + " Not Found");
        }
    }

    private void SelectMesh(string szMeshName)
    {
        
        Debug.logger.Log("Select Object : " + szMeshName);
        Transform movObj = ModelManager.Instance.Model.transform.Find(szMeshName);        
        if (movObj != null)
        {
            if (szMeshName == "_z10-1")
            {               
                Transform t = movObj.FindChild("z10-1");
                SelectionModel smt = t.gameObject.GetComponent<SelectionModel>();
                if (smt != null)
                {
                    smt.SelectObject();
                }
                Transform t2 = movObj.FindChild("z10-2");
                SelectionModel smt2 = t2.gameObject.GetComponent<SelectionModel>();
                if (smt2 != null)
                {
                    smt2.SelectObject();
                }
            }
            else
            {
                if(movObj.childCount > 0)
                {    
                    for( int i = 0 ; i < movObj.childCount ; i++)
                    {
                        Transform t = movObj.GetChild(i);
                        SelectionModel smt = t.gameObject.GetComponent<SelectionModel>();
                        if (smt != null)
                        {
                            smt.SelectObject();
                        }
                    }                    
                }
                else
                {
                    SelectionModel sm = movObj.gameObject.GetComponent<SelectionModel>();
                    if (sm != null)
                    {
                        sm.SelectObject();
                    }
                    else
                    {
                        Transform t = movObj.FindChild(szMeshName);
                        SelectionModel smt = t.gameObject.GetComponent<SelectionModel>();
                        if (smt != null)
                        {
                            smt.SelectObject();
                        }
                        else
                        {
                            Debug.logger.Log("Select Object : " + szMeshName + " Not Found");
                        }
                    }
                }
               
            }
        }
        else
        {
            Debug.logger.Log("Select Object : " + szMeshName + " Not Found");
        }
    }

    private void SetZoomPosition(float x, float y, float z)
    {
        FindOptimunView(new Vector3(x, y, z), 1, false, m_fTopViewAngle, m_fTopViewDistance);
    }
}