using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using System.IO;
using System.Text;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MovingCamera : MonoBehaviour
{
    private float m_fTopViewDistance = 450.0f;
    private float m_fTopViewAngle = 30.0f;
   
    float currentMovingDistance = 1;
    //Vector3 vOrgPos = new Vector3(-2050.71f,4.827f,-5227.104f);
    Vector3 vOrgPos = new Vector3(67.41f, 400f, -1455f);
    Vector3 endPos = new Vector3(-2051.741f, 1.75f, -5223.9f);
    Vector3 startPos = new Vector3(-2051.741f, 1.91f, -5223.112f);
    Vector3 movingPos = new Vector3(-2051.741f, 1.91f, -5223.112f);         //이 지점으로 이동한 후에 zoomout-zoomin  

    Quaternion zOutRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
    Quaternion qOrg = Quaternion.Euler(new Vector3(50.64f, 0f, 0f));
    Vector3 zInPos = new Vector3();
    bool isZoomoutMoving = false;
    Vector3 zOutPos = new Vector3();

    bool isZoomIn = false;
    float m_Speed = 300f;
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
            //proxy.UserObject.SetVariable("SetZoomObject", new Action<string>(SetZoomObject));
            proxy.UserObject.SetVariable("SetZoomObjectDistance", new Action<float>(SetZoomObjectDistance));
            proxy.UserObject.SetVariable("SetZoomObjectAngle", new Action<float>(SetZoomObjectAngle));
            proxy.UserObject.SetVariable("CameraView", new Action<string>(SetView));
            
            proxy.UserObject.SetVariable("SelectObject", new Action<string>(SelectMesh));
            //proxy.UserObject.SetVariable("SetEarthquake", new Action<int, int>(SetEarthquake));
            proxy.UserObject.SetVariable("CameraZoomoutMoving", new Action<string>(CameraZoomoutMoving));
        }
    }
    

    /*public float shakeDuration = 35.0f;

    //지진 상황 관련 필드 
    public float shakeAmount = 0.35f;
    public float decreaseFactor = 1.0f;
    Vector3 originalPos;
    private bool isEarthquakeMode = false;*/

    Light lightComp = null;
    private int interval = 0;
    public float leftTime = 1.0f;
    private bool onoff_light_emergency = true;
    float timeLeft = 0.3f;
    GameObject zoomInBuilding = null;
    GameObject zoomoutBuilding = null;

    ////지진 시뮬레이션
    //private void SetEarthquake(int amount, int seconds)
    //{
    //    Camera mainCamera = Camera.main;
    //    originalPos = mainCamera.transform.localPosition;
    //    Light[] lights = FindObjectsOfType(typeof(Light)) as Light[];
    //    lightComp = lights[0];
    //    /***
    //     * 삼천포는 광교와 달리 카메라 때문에 shakeAmount가 다름. 
    //     */
    //    switch (amount)
    //    {
    //        case 1: shakeAmount = 1.5f;
    //            break;
    //        case 2: shakeAmount = 2.5f;
    //            break;
    //        case 3: shakeAmount = 3.0f;
    //            break;
    //        case 4: shakeAmount = 3.5f;
    //            break;
    //    }
    //    shakeDuration = (float)seconds;


    //    isEarthquakeMode = true;
    //    ModelManager.Instance.Model.SaveSharedFile("SetEarthquake");


    //}


    void CameraZoomoutMoving(string meshName)
    {
        zoomInBuilding = ModelManager.Instance.Model.transform.Find(meshName).gameObject;
        zoomoutBuilding = GameObject.FindGameObjectWithTag("zoomoutbuilding");
        //zoomoutBuilding = zoomInBuilding;
        
        zInPos = zoomInBuilding.transform.position;
        if (meshName.StartsWith("CV"))
        {
            MeshRenderer mr = zoomInBuilding.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                Bounds objBound = mr.bounds;
                float scale = ModelManager.Instance.Model.transform.localScale.x;
                Vector3 vCenter = objBound.center * scale;
                //zInPos = ModelManager.Instance.Model.transform.Find("B-26").position;
                zInPos = vCenter;
            }
        } 
        zOutPos = zoomoutBuilding.transform.position;
               
        startPos = transform.position;
        isZoomoutMoving = true;

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


    private string m_szMainModelName = "indoor_final_191107";

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
        //SetEarthquake(3, 2);
        //CameraZoomoutMoving("CV-08");
    }
    
    Vector3 lastposition = new Vector3();
    float angleSpeed = 2f;
    bool isLastMoving = false;
    bool isRelaySelection = false;
    int relayCount = 1;
    bool isEarthQuakePost = false;
    public float postTimeLeft = 5.0f;
   
    void Update()
    {
        /*if (isEarthQuakePost)
        {
            
            if (postTimeLeft >= 0)       //지진 신호 처리 후 5초 동안 건물 빨간색 유지.
            {                
                SelectMesh("z140");
                postTimeLeft = postTimeLeft - Time.deltaTime;
            }
            else if (postTimeLeft < 0)
            {               
                postTimeLeft = 5.0f;               
                isEarthQuakePost = false;               
                string szMsg3 = "EarthquakeBeepFinish()";
                Debug.logger.Log(szMsg3);

                if (PassivePipeProxy.Instance != null)
                    PassivePipeProxy.Instance.SendServer(szMsg3);
            }
        }*/
        if (isZoomoutMoving)
        {
           

            float distance = Vector3.Distance(Camera.main.transform.position, vOrgPos);

            Debug.unityLogger.Log("Distance : " + distance);

            if (distance >= 1)
            {
                Vector3 dirCam = (vOrgPos - transform.position).normalized;
                Quaternion rotation = Quaternion.LookRotation(zOutPos - transform.position);
                Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, rotation, Time.deltaTime * angleSpeed);
                //Camera.main.transform.LookAt(new Vector3(0f, 0f, 0f));
                transform.Translate(dirCam * Time.deltaTime * m_Speed, Space.World); ;
                Debug.unityLogger.Log("Camera : " + transform.position + "     Vorgpos :  " + vOrgPos);
            }
            else
            {
                lastposition = transform.position;
                isZoomoutMoving = false;
              
                isZoomIn = true;
            }

        }
        if (isZoomIn)
        {
          
            float distance = Vector3.Distance(Camera.main.transform.position, zInPos);
           
                
            if (distance >= 300)
            {
                Vector3 dirEBuilding = (zInPos - lastposition).normalized;

                Quaternion rotation = Quaternion.LookRotation(zInPos - transform.position);
                Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, rotation, Time.deltaTime * angleSpeed);

                transform.Translate(dirEBuilding * Time.deltaTime * m_Speed, Space.World);

            }
            else
            {
                isZoomIn = false;
                
                //string szMsg = "CollapseBuilding('z06')";       //send buildingID
                //Debug.logger.Log(szMsg);
                //SelectMesh("z06");
                //System.Diagnostics.Trace.WriteLine(szMsg);
                //if (PassivePipeProxy.Instance != null)
                //   PassivePipeProxy.Instance.SendServer(szMsg);     
                //SelectMesh("z140");
            }           
        }
        /*if (isEarthquakeMode)
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
                string szMsg3 = "EarthquakeBeepFinish()";
                Debug.logger.Log(szMsg3);

                if (PassivePipeProxy.Instance != null)
                    PassivePipeProxy.Instance.SendServer(szMsg3);
                //isEarthQuakePost = true;
                //string szMsg2 = "CollapseBuilding('z140')";

                //Debug.logger.Log(szMsg2);

                //System.Diagnostics.Trace.WriteLine(szMsg);
                //if (PassivePipeProxy.Instance != null)
                //    PassivePipeProxy.Instance.SendServer(szMsg2);
                //CameraZoomoutMoving("z140");
                //string szMsg3 = "EarthquakeBeepFinish('')";

                //Debug.logger.Log(szMsg3);
                
                ////System.Diagnostics.Trace.WriteLine(szMsg);
                //if (PassivePipeProxy.Instance != null)
                //    PassivePipeProxy.Instance.SendServer(szMsg3);
               
            }
        }*/
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
        

        float scale = ModelManager.Instance.Model.transform.localScale.x;
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

    public Vector3 FindZoomView(string objectName, int nlayer, float topViewAngle, float topViewDist)
    {
        Transform movObj = ModelManager.Instance.Model.transform.Find(objectName);
        MeshRenderer mr = movObj.gameObject.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            Bounds objBound = mr.bounds;
            float scale = ModelManager.Instance.Model.transform.localScale.x;
            Vector3 vCenter = objBound.center * scale;
            //float len = objBound.extents.magnitude * 4 + mainCamera.nearClipPlane; // scale;
            float len = (topViewDist + Camera.main.nearClipPlane) * scale;


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
            return vResPos;
        }
        return Camera.main.transform.position;
    }
    private void FindOptimunView(string szObjectName, int nlayer, bool bTopview, float topViewAngle, float topViewDist)
	{
        Camera mainCamera = Camera.main;

        Transform movObj = ModelManager.Instance.Model.transform.Find(szObjectName);
        if ( movObj != null)
        {
            MeshRenderer mr = movObj.gameObject.GetComponent<MeshRenderer>();
            if( mr != null)
            {
                Bounds objBound = mr.bounds;
                float scale =  ModelManager.Instance.Model.transform.localScale.x;
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

    /*private void SetZoomObject(string szObjectName)
    {
        FindOptimunView(szObjectName, 1, false, m_fTopViewAngle, m_fTopViewDistance);
    }*/

    private void SelectMesh(string szMeshName)
    {

        //UnityEngine.Debug.Log("szMeshName Message : " + szMeshName);
        //string reMeshName = szMeshName;
        //if (szMeshName.StartsWith("z"))
        //{
        //    reMeshName = szMeshName.Substring(0, 1);
        //}
        //UnityEngine.Debug.Log("select mesh : " + reMeshName);
        //Debug.logger.Log("Select Object : " + szMeshName);
        Transform movObj = ModelManager.Instance.Model.transform.Find(szMeshName);
        
        if (movObj != null)
        {
            
            SelectionModel sm = movObj.gameObject.GetComponent<SelectionModel>();
            if (sm != null)
            {
                sm.SelectObject();
            }
        }
    }

    private void SetZoomPosition(float x, float y, float z)
    {
        FindOptimunView(new Vector3(x, y, z), 1, false, m_fTopViewAngle, m_fTopViewDistance);
    }
}