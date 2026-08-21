using UnityEngine;
using System.Collections;

public class MovingCamera : MonoBehaviour
{
    private string MouseLButton = "MouseLClick";
    private string MouseRButton = "MouseRClick";
    private string MouseMButton = "MouseMClick";

    private Vector3 m_ptLButtonOrigin, m_ptRButtonOrigin, m_ptMButtonOrigin;
    private bool m_isLButtonClicked = false, m_isRButtonClicked = false, m_isMButtonClicked = false;

    public float m_fZoomSpeed = 5.0f;
    
    public bool followDrone = false;
    public Vector3 followingDir;

    private static Vector3 m_vDroneInitPos = new Vector3(422.8595f, 105.84f, 615.4525f);
    private static Vector3 m_vCameraInitPos = new Vector3(450.4063f, 175.6568f, 509.1991f);
    private static Quaternion m_qCameraInitAngle = new Quaternion(-0.1432162f, 0.01744882f, -0.1050205f, -0.9839489f);

    private static float HALF_TOLERANCE = 0.001f;

    private Camera m_mainCamera = null;
    private Drone m_drone = null;

    private Quaternion m_qDroneInitAngle;

    void Start()
    {
        m_mainCamera = Camera.main;
        GameObject drone = GameObject.Find("Drone");

        if (drone != null)
        {
            m_drone = drone.GetComponent<Drone>();
            followingDir = m_vCameraInitPos - m_vDroneInitPos;

            m_qDroneInitAngle = drone.transform.rotation;
        }

        if (followDrone)
        {
            transform.position = m_drone.transform.position + followingDir;
            Rotate(drone.transform.rotation);
        }

        CalcCameraInitPos();

        TreeMaker.Instance.MakeRandom(1000);
    }

    void CalcCameraInitPos()
    {
        Vector3 vRight = CrossProduct(m_drone.transform.up, m_drone.transform.forward);

        Vector3 vBackDir = GetBackwardDirection();
        Vector3 vRightDir = GetRightDirection(vBackDir, vRight);

        followingDir.x = GetDistance(m_drone.transform.position, vBackDir);
        followingDir.y = GetDistance(vBackDir, vRightDir);
        followingDir.z = GetDistance(vRightDir, this.transform.position);

        if (!IsPositive(m_drone.transform.position, vBackDir, -m_drone.transform.forward))
            followingDir.x = -followingDir.x;

        if (!IsPositive(vBackDir, vRightDir, vRight))
            followingDir.y = -followingDir.y;

        if (!IsPositive(vRightDir, this.transform.position, m_drone.transform.up))
            followingDir.z = -followingDir.z;
    }

    private bool IsPositive(Vector3 v1, Vector3 v2, Vector3 vDir)
    {
        Vector3 vPositive = vDir + v1;
        Vector3 vNegative = v1 * 2 - vPositive;

        return GetDistance(v2, vPositive) < GetDistance(v2, vNegative);
    }

    private Vector3 GetBackwardDirection()
    {
        Vector3 vBack = m_drone.transform.position - m_drone.transform.forward;
        //Debug.Log("vBack : " + vBack.x.ToString() + ", " + vBack.y.ToString() + ", " + vBack.z.ToString());
        return GetNearestVertex(this.transform.position, m_drone.transform.position, vBack);
    }

    private Vector3 GetRightDirection(Vector3 vLineBegin, Vector3 vRight)
    {
        Vector3 vLineEnd = vRight + vLineBegin;
        return GetNearestVertex(this.transform.position, vLineBegin, vLineEnd);
    }

    public void Move()
    {
        Vector3 vRightDir = CrossProduct(m_drone.transform.up, m_drone.transform.forward);

        Vector3 vBack = -m_drone.transform.forward - Vector3.zero + m_drone.transform.position;
        vBack = GetLinearVertex(m_drone.transform.position, vBack, followingDir.x);

        Vector3 vRight = vRightDir - Vector3.zero + vBack;
        vRight = GetLinearVertex(vBack, vRight, followingDir.y);

        Vector3 vUp = m_drone.transform.up - Vector3.zero + vRight;
        vUp = GetLinearVertex(vRight, vUp, followingDir.z);
        this.transform.position = vUp;
    }

    public void Rotate(Quaternion droneAngle)
    {
        Vector3 vDroneInit = new Vector3(m_qDroneInitAngle.y, m_qDroneInitAngle.z, m_qDroneInitAngle.w);

        Vector3 vRightDir = CrossProduct(m_drone.transform.up, vDroneInit);
        Vector3 vLeftDir = new Vector3(-vRightDir.x, -vRightDir.y, -vRightDir.z);
        Vector3 vBackDir = new Vector3(-m_drone.transform.forward.x, -m_drone.transform.forward.y, -m_drone.transform.forward.z);
        Vector3 vAngleDir = new Vector3(droneAngle.y, droneAngle.z, droneAngle.w);

        float fAngle = Quaternion.Angle(m_qDroneInitAngle, droneAngle);

        if (GetDistance(vAngleDir, vRightDir) > GetDistance(vAngleDir, vLeftDir))
            fAngle = 360.0f - fAngle;
        if (GetDistance(vAngleDir, m_drone.transform.forward) > GetDistance(vAngleDir, vBackDir))
            fAngle = 360.0f - fAngle;
        
        transform.rotation = Quaternion.AngleAxis(fAngle, m_drone.transform.up) * m_qCameraInitAngle;
    }

    void Update()
    {
        // Panning
        if (Input.GetAxis(MouseMButton) != 0)
        {
            if (!m_isMButtonClicked)
            {
                m_isMButtonClicked = true;
                m_ptMButtonOrigin = Input.mousePosition;
            }
            else
            {
                transform.Translate(m_ptMButtonOrigin - Input.mousePosition);
                m_ptMButtonOrigin = Input.mousePosition;
            }
        }
        else
            m_isMButtonClicked = false;
        ////////////////////////////////////////////////////////

        // Orbit
        if (Input.GetAxis(MouseRButton) != 0)
        {
            if (!m_isRButtonClicked)
            {
                m_isRButtonClicked = true;
                m_ptRButtonOrigin = Input.mousePosition;
            }
            else
            {
                float xMove = Input.mousePosition.x - m_ptRButtonOrigin.x;
                float yMove = Input.mousePosition.y - m_ptRButtonOrigin.y;

                float xDegree = 360.0f * xMove / Screen.width;
                float yDegree = 360.0f * yMove / Screen.height;

                transform.Rotate(-yDegree, xDegree, 0.0f);
                m_ptRButtonOrigin = Input.mousePosition;
            }
        }
        else
            m_isRButtonClicked = false;
        ////////////////////////////////////////////////////////

        // Zoom
        float fMouseScroll = Input.GetAxis("Mouse ScrollWheel");

        if (fMouseScroll > 0)
        {
            transform.Translate(0, 0, m_fZoomSpeed);
        }
        else if (fMouseScroll < 0)
            transform.Translate(0, 0, -m_fZoomSpeed);

        if (Input.GetAxis(MouseLButton) != 0)
        {
            if (!m_isLButtonClicked)
            {
                m_isLButtonClicked = true;
                m_ptLButtonOrigin = Input.mousePosition;

                RaycastHit hit;
                Ray ray = m_mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("Mountain"))
                    {
                        GameObject prefab = TreeMaker.Instance.FirePrefab;
                        //Object prefab = Resources.Load("fx_fire_g");
                        //Object prefab = AssetDatabase.LoadAssetAtPath("Assets/fx_fire_g.prefab", typeof(GameObject));

                        if (prefab != null)
                        {
                            GameObject clone = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
                            clone.transform.position = hit.point;
                            clone.SetActive(true);
                        }
                    }
                    /*else if (hit.collider.tag == "Water")
                        Debug.Log("Water Hit");*/
                }
            }
        }
        else
            m_isLButtonClicked = false;
    }

    private float GetDistance(Vector3 v1, Vector3 v2)
    {
        return (float)System.Math.Pow((v2.x - v1.x) * (v2.x - v1.x) + (v2.y - v1.y) * (v2.y - v1.y) + (v2.z - v1.z) * (v2.z - v1.z), 0.5f);
    }

    private Vector3 GetLinearVertex(Vector3 v1, Vector3 v2, float fDistance)
    {
        float fLen = GetDistance(v1, v2);

        if (fLen <= HALF_TOLERANCE)
            return v1;

        Vector3 v3 = v1 + (v2 - v1) * fDistance / fLen;
        return v3;
    }

    private Vector3 CrossProduct(Vector3 v1, Vector3 v2)
    {
        float x = v1.y * v2.z - v1.z * v2.y;
        float y = v1.z * v2.x - v1.x * v2.z;
        float z = v1.x * v2.y - v1.y * v2.x;

        return new Vector3(x, y, z);
    }

    // Return 값 : radian
    private double GetAngle(Vector3 v1, Vector3 vCenter, Vector3 v2)
    {
        // 코사인 제2법칙
        // C²= A²+ B²- 2ABcosΘ
        double a = GetDistance(v1, vCenter);
        double b = GetDistance(v2, vCenter);
        double c = GetDistance(v1, v2);

        double cosData = (a * a + b * b - c * c) / 2 / a / b;
        if (cosData < -1.0) cosData = -1.0;
        else if (cosData > 1.0) cosData = 1.0;

        return System.Math.Acos(cosData);
    }

    // vLineBegin과 vLineEnd를 잇는 무한히 긴 직선위에서 vertex와 가장 가까운 점을 리턴한다.
    private Vector3 GetNearestVertex(Vector3 vertex, Vector3 vLineBegin, Vector3 vLineEnd)
    {
        float fLen1 = GetDistance(vertex, vLineBegin);
        float fLen2 = GetDistance(vertex, vLineEnd);

        if (fLen1 <= HALF_TOLERANCE || fLen2 <= HALF_TOLERANCE)
            return vertex;

        double dAngle = GetAngle(vertex, vLineBegin, vLineEnd);
        float h = (float)(fLen1 * System.Math.Cos(dAngle));

        return GetLinearVertex(vLineBegin, vLineEnd, h);
    }

    /*private void Yaw(float fAngle)
    {
        fAngle *= rotationSpeed * Time.deltaTime;
        transform.Rotate(0, 0, fAngle);
    }

    private void Roll(float fAngle)
    {
        fAngle *= rotationSpeed * Time.deltaTime;
        transform.Rotate(0, fAngle, 0);
    }

    private void Pitch(float fAngle)
    {
        fAngle *= rotationSpeed * Time.deltaTime;
        transform.Rotate(fAngle, 0, 0);
    }

    private void Move(float xMove, float yMove)
    {
        Vector2 movement = new Vector2(
          movingSpeed * xMove,
          movingSpeed * yMove);

        //movement *= Time.deltaTime;
        transform.Translate(movement);
    }

    private void Zoom(float fZoomValue)
    {
        if (fZoomValue > 0)
        {
            if (Camera.main.orthographicSize >= 1.0f)
                Camera.main.orthographicSize--;
        }
        else if (fZoomValue < 0)
            Camera.main.orthographicSize++;
    }*/
}
