using UnityEngine;
using System.Collections;

public class Drone : MonoBehaviour
{
    public float movingSpeed = 1.0f;

    private bool m_noMove = false;
    private Vector3 m_prevDir = new Vector3();

    private MovingCamera m_mainCamera = null;
    private Camera m_myCamera = null;
    private bool m_initCamera = false;

    private static float m_fCameraPosition = 10.0f;
    private static int m_nDroneCameraCount = 0;

    private static Camera AddCamera(Drone drone)
    {
        Camera camera = GameObject.Instantiate<Camera>(Camera.main);

        m_nDroneCameraCount++;
        float fCameraScreenSize = 1.0f / m_nDroneCameraCount;

        Camera.main.rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f);

        camera.rect = new Rect((m_nDroneCameraCount - 1) * fCameraScreenSize, 0, fCameraScreenSize, 0.5f);
        camera.transform.position = new Vector3(drone.transform.position.x, drone.transform.position.y - m_fCameraPosition, drone.transform.position.z);
        camera.transform.rotation = drone.transform.rotation;

        return camera;
    }

	// Use this for initialization
	void Start ()
    {
        m_mainCamera = Camera.main.GetComponent<MovingCamera>();
        m_myCamera = AddCamera(this);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!m_initCamera)
        {
            m_initCamera = true;
            Move(0.0f, 0.0f, 0.0f);
        }

        if (m_noMove)
        {
            transform.Translate(-m_prevDir);
            return;
        }

        float xMove = Input.GetAxis("Horizontal");
        float yMove = Input.GetAxis("YMove");
        float zMove = Input.GetAxis("Vertical");

        if (Input.GetAxis("LeftCtrl") != 0.0f)
        {
            if (xMove != 0.0f)
                Roll(xMove);
        }
        else
        {
            if (xMove != 0.0f || yMove != 0.0f || zMove != 0.0f)
            {
                Move(xMove, yMove, zMove);
            }
        }
	}

    void OnTriggerEnter()
    {
        m_noMove = true;

        //Debug.Log("TriggerEnter : " + transform.position.ToString());
        transform.Translate(-m_prevDir);
        //Debug.Log("TriggerEnter After Move : " + transform.position.ToString());
    }

    void OnTriggerExit()
    {
        m_noMove = false;
        //Debug.Log("TriggerExit : " + transform.position.ToString());
    }

    private void Move(float xMove, float yMove, float zMove)
    {
        Vector3 movement = new Vector3(
          movingSpeed * xMove,
          movingSpeed * yMove,
          movingSpeed * zMove);

        //movement *= Time.deltaTime;
        transform.Translate(movement);
        //m_mainCamera.transform.position = transform.position + m_mainCamera.followingDir;
        m_mainCamera.Move();

        m_myCamera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - m_fCameraPosition, this.transform.position.z);
        m_myCamera.transform.rotation = this.transform.rotation;

        //Vector3 vCameraDir = m_mainCamera.transform.position - this.transform.position;
        //Debug.Log("Camera Dir : " + vCameraDir.x.ToString() + ", " + vCameraDir.y.ToString() + vCameraDir.z.ToString());

        m_prevDir.x = xMove;
        m_prevDir.y = yMove;
        m_prevDir.z = zMove;
    }

    private void Roll(float fAngle)
    {
        transform.Rotate(0, fAngle, 0);
        m_mainCamera.Rotate(this.transform.rotation);
        m_mainCamera.Move();

        m_myCamera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - m_fCameraPosition, this.transform.position.z);
        m_myCamera.transform.rotation = this.transform.rotation;
    }
}
