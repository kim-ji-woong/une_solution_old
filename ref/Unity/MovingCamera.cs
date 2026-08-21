using UnityEngine;
using System.Collections;

public class MovingCamera : MonoBehaviour
{
    private string MouseLButton = "Fire1";
    private string MouseRButton = "Fire2";
    private string MouseMButton = "Fire3";

    private Vector3 m_ptLButtonOrigin, m_ptRButtonOrigin, m_ptMButtonOrigin;
    private bool m_isLButtonClicked = false, m_isRButtonClicked = false, m_isMButtonClicked = false;

    public float m_fZoomSpeed = 5.0f;

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
                transform.Translate(Input.mousePosition - m_ptMButtonOrigin);
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
