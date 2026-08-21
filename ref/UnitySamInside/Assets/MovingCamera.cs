using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using System.IO;
using System.Text;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MovingCamera : MonoBehaviour
{
    private float m_fTopViewDistance = 250.0f;
    private float m_fTopViewAngle = 30.0f;
    
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

        }
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


    private string m_szMainModelName = "SamInside";

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
            Bounds aab = ModelManager.Instance.Model.modelBound;
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
        else if(szViewName == "front")
        {
            Camera mainCamera = Camera.main;
            float asp = mainCamera.aspect;
            Bounds aab = ModelManager.Instance.Model.modelBound;
            float width = aab.max.x - aab.min.x;
            float height = aab.max.y - aab.min.y;
            float depth = aab.max.z - aab.min.z;

            float fov = mainCamera.fieldOfView * 0.5f;
            float len = (width / height > asp) ? width / asp : height;
            len *= 0.5f;
            len = len / Mathf.Tan(fov);

            Vector3 vCamPos = aab.center;
            vCamPos.z -= (len + depth * 0.5f);

            Vector3 newDir = aab.center - vCamPos;
            float flength = newDir.magnitude * 2f / 3f;
            vCamPos.y = flength;

            mainCamera.transform.position = vCamPos;
            mainCamera.transform.forward = (aab.center - vCamPos);           
        }
        else if(szViewName == "rear")
        {
            Camera mainCamera = Camera.main;
            float asp = mainCamera.aspect;
            Bounds aab = ModelManager.Instance.Model.modelBound;
            float width = aab.max.x - aab.min.x;
            float height = aab.max.y - aab.min.y;
            float depth = aab.max.z - aab.min.z;

            float fov = mainCamera.fieldOfView * 0.5f;
            float len = (width / height > asp) ? width / asp : height;
            len *= 0.5f;
            len = len / Mathf.Tan(fov);
            Vector3 vCamPos = aab.center;
            vCamPos.z += (len + depth * 0.5f);

            Vector3 newDir = aab.center - vCamPos;
            float flength = newDir.magnitude * 2f / 3f;
            vCamPos.y = flength;

            mainCamera.transform.position = vCamPos;
            mainCamera.transform.forward = (aab.center - vCamPos);
        }
        else if (szViewName == "left")
        {
            Camera mainCamera = Camera.main;
            float asp = mainCamera.aspect;
            Bounds aab = ModelManager.Instance.Model.modelBound;
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
        else if (szViewName == "right")
        {
            Camera mainCamera = Camera.main;		
			float asp = mainCamera.aspect;
            Bounds aab = ModelManager.Instance.Model.modelBound;
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
        else if( szViewName == "fit")
        {
            Camera mainCamera = Camera.main;		
			float asp = mainCamera.aspect;

            Bounds aab = ModelManager.Instance.Model.modelBound;
            float width = aab.max.x - aab.min.x;

           

            float height = aab.max.y - aab.min.y;
            float depth = aab.max.z - aab.min.z;

            Debug.logger.Log("ModelBound FitView " + width);
            Debug.logger.Log("ModelBound FitView " + height);
            Debug.logger.Log("ModelBound FitView " + depth);

            float fov = mainCamera.fieldOfView * 0.5f;
            float len = ((width / depth) > asp) ? width / asp : depth;
            len *= 0.5f;
            len = len / Mathf.Tan(fov);       
            Vector3 center = aab.center;
            center.z -= (len + depth * 0.5f);
            Vector3 newDir = aab.center - center;
            float flength = newDir.magnitude * 2f / 3f;
            center.y = flength;

            mainCamera.transform.position = center;
            mainCamera.transform.forward = (aab.center - center);
            
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

            string szMsg = string.Format("SendMessage('MainCameraPoisition({0},{1},{2})')", vec.x, vec.y, vec.z);
            proxy.RunPythonScript(szMsg);
        }
    }

    private void ReadAngles()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            Vector3 vec = this.transform.eulerAngles;

            string szMsg = string.Format("SendMessage('MainCameraAngles({0},{1},{2})')", vec.x, vec.y, vec.z);
            proxy.RunPythonScript(szMsg);
        }
    }

    private void ReadDirection()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
            Vector3 vec = this.transform.forward;

            string szMsg = string.Format("SendMessage('MainCameraDirection({0},{1},{2})')", vec.x, vec.y, vec.z);
            proxy.RunPythonScript(szMsg);
        }
    }

    
    // Use this for initialization
    void Awake()
    {
        AddPythonFunction();
    }

    void Start()
    {
        SetView("fit");
    }

    void Update()
    {
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

    private void FindOptimunView(string szObjectName, int nlayer, bool bTopview, float topViewAngle, float topViewDist)
	{
        Camera mainCamera = Camera.main;
        Transform movObj = ModelManager.Instance.Model.transform.Find(szObjectName);
        if( movObj != null)
        {
            MeshRenderer mr = movObj.gameObject.GetComponent<MeshRenderer>();
            if( mr != null)
            {
                Bounds objBound = mr.bounds;
                float scale =  ModelManager.Instance.Model.transform.localScale.x;
				Vector3 vCenter = objBound.center * scale;
				float len = (objBound.extents.magnitude + mainCamera.nearClipPlane) * scale;
                Debug.logger.Log("Object Distance Length = " + len);
                //float len = ( topViewDist + mainCamera.nearClipPlane) * scale;
                
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

                Debug.logger.Log("Object Distance Length = " + (vCenter - vResPos).magnitude);
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
        FindOptimunView(szObjectName, 1, false, m_fTopViewAngle, m_fTopViewDistance);
    }

    private void SetZoomPosition(float x, float y, float z)
    {
        FindOptimunView(new Vector3(x, y, z), 1, false, m_fTopViewAngle, m_fTopViewDistance);
    }
}