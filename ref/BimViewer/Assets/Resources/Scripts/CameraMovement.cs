using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TouchControlsKit;

public class CameraMovement : MonoBehaviour
{

    public float speed = 2.0f;
    public float zoomSpeed = 5.0f;

    public float minX = -360.0f;
    public float maxX = 360.0f;

    public float minY = -45.0f;
    public float maxY = 45.0f;

    public float sensX = 100.0f;
    public float sensY = 100.0f;

    float rotationY = 0.0f;
    float rotationX = 0.0f;

    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    public float rotationSmoothTime = 0.3F;
    private Vector3 rotationVelocity = Vector3.zero;

    bool isAutoMoving = false;

    private void Start()
    {
        rotationX = transform.localEulerAngles.y;
        rotationY = -transform.localEulerAngles.x;

        //Browser에서 키보드 입력을 먹는걸 방지.
        //if(Application.platform == RuntimePlatform.WebGLPlayer)
#if UNITY_WEBGL
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            WebGLInput.captureAllKeyboardInput = false;
#endif
    }

    Vector3 startPosition = new Vector3();
    Vector3 gotoPosition = new Vector3();

    Vector3 lookAtTarget = new Vector3();

    float gotoElapsed = 0.0f;
    float gotoSpeed = 0.0f;

    float lookAtSpeed = 0.3f;
    public float autoMoveSmooth = 100.0f;
    private Vector3 autoMoveVelocity = Vector3.zero;
    private float damping = 100;



    void FixedUpdate()
    {
        if (isAutoMoving)
        {
            //Vector3 toPosition = Vector3.Lerp(startPosition, gotoPosition, gotoElapsed);

            transform.position =  Vector3.SmoothDamp(transform.position, gotoPosition, ref autoMoveVelocity, autoMoveSmooth,100.0f);

            //gotoElapsed += gotoSpeed;

            Vector3 lTargetDir = lookAtTarget - transform.position;
            //lTargetDir.y = 0.0f;
            Quaternion toRotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lTargetDir), Time.time * lookAtSpeed);

            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * damping);

            //transform.LookAt(lookAtTarget);

            if ((transform.position - gotoPosition).magnitude < 0.1f && autoMoveVelocity.magnitude < 0.1f && 
                (transform.rotation.eulerAngles - toRotation.eulerAngles).magnitude < 0.1f)
            {
                isAutoMoving = false;

                rotationX = transform.localEulerAngles.y;
                rotationY = -transform.localEulerAngles.x;
            }                
        }
    }

    void Update()
    {
        if(isAutoMoving)
        {
            return;
        }
        else
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Vector3 prePosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Vector3 preAngle = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);

            float scroll = Input.GetAxis("Mouse ScrollWheel");

            Vector3 toPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            toPosition += (transform.forward * scroll * zoomSpeed);


            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    toPosition += transform.right * speed;
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    toPosition += -transform.right * speed;
                }
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    toPosition += transform.forward * speed;
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    toPosition += -transform.forward * speed;
                }
            }



            if (Application.platform == RuntimePlatform.Android)
            {
                Vector2 move = TCKInput.GetAxis("Joystick") * speed; // NEW func since ver 1.5.5

                float oldY = transform.position.y;

                toPosition += transform.right * move.x + transform.forward * move.y;

                toPosition = new Vector3(toPosition.x, oldY, toPosition.z);

                Vector2 rotate = TCKInput.GetAxis("Touchpad");

                rotationX += rotate.x * sensX;
                rotationY += rotate.y * sensY;
                rotationY = Mathf.Clamp(rotationY, minY, maxY);
                rotationX = Mathf.Clamp(rotationX, minX, maxX);

                Vector3 newRotation = new Vector3(-rotationY, rotationX, 0);
                transform.localEulerAngles = newRotation;
            }
            else
            {
                
                if (Input.GetMouseButton(0))
                {
                    rotationX += Input.GetAxis("Mouse X") * sensX;
                    rotationY += Input.GetAxis("Mouse Y") * sensY;
                    rotationY = Mathf.Clamp(rotationY, minY, maxY);
                    rotationX = Mathf.Clamp(rotationX, minX, maxX);

                    Vector3 newRotation = new Vector3(-rotationY, rotationX, 0);
                    transform.localEulerAngles = newRotation;
                    //transform.localEulerAngles = Vector3.Slerp(transform.localEulerAngles, newRotation, Time.deltaTime * 2.0f);            
                }
                else if (Input.GetMouseButton(1))
                {
                    float mouseX = -Input.GetAxis("Mouse X") * speed;
                    float mouseZ = -Input.GetAxis("Mouse Y") * speed;

                    float oldY = transform.position.y;

                    toPosition += transform.right * mouseX + transform.forward * mouseZ;

                    toPosition = new Vector3(toPosition.x, oldY, toPosition.z);

                    //transform.position = new Vector3(transform.position.x, oldY, transform.position.z);            
                    //transform.position = Vector3.Slerp(transform.position, toPosition, Time.time);
                }
                else if (Input.GetMouseButton(2))
                {
                    //float mouseX = -Input.GetAxis("Mouse X") * speed;
                    float mouseZ = -Input.GetAxis("Mouse Y") * speed;

                    float oldX = transform.position.x;
                    float oldZ = transform.position.z;

                    toPosition += transform.up * mouseZ;

                    toPosition = new Vector3(oldX, toPosition.y, oldZ);
                }
            }


            transform.position = Vector3.SmoothDamp(transform.position, toPosition, ref velocity, smoothTime);
        }        
    }

    public void GoTo(Vector3 position,Vector3 lookAt)
    {
        startPosition = transform.position;
        gotoPosition = position;
        lookAtTarget = lookAt;
        gotoElapsed = 0.0f;
        gotoSpeed = 0.01f;
        isAutoMoving = true;
    }
}
