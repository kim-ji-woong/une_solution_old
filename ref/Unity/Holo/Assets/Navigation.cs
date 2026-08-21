using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class Navigation : MonoBehaviour, INavigationHandler
{
    public enum MouseMode
    {
        mode_None = 0,
        mode_orbit,
        mode_pan
    }

    private MouseMode m_mouseMode = MouseMode.mode_None;
    private float RotationSensitivity = 10.0f;
    private Vector3 m_MousePosCur;
    private Vector3 m_MousePosPrev;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetMouseMode(int nMode)
    {
        m_mouseMode = (MouseMode)nMode;
    }

    public void OnNavigationStarted(NavigationEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnNavigationUpdated(NavigationEventData eventData)
    {
        m_MousePosCur = Input.mousePosition;

        if (m_mouseMode == MouseMode.mode_orbit)
        {
            float rotationFactor = eventData.CumulativeDelta.x * RotationSensitivity;
            transform.Rotate(new Vector3(0, -1 * rotationFactor, 0));
            //UpdateRotation();
        }
        m_MousePosPrev = m_MousePosCur;
    }

    public void OnNavigationCompleted(NavigationEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnNavigationCanceled(NavigationEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    private void UpdateRotation()
    {
        //Vector3 PtDiff = m_MousePosCur - m_MousePosPrev;
        //if (PtDiff.x == 0 && PtDiff.y == 0)
        //    return;

        //float pitch = (-0.5f * PtDiff.x);
        //float yaw = (-0.5f * PtDiff.y);

        //if (yAngle + yaw < 5)
        //    yaw = 5 - yAngle;
        //else if (yAngle + yaw > 85)
        //    yaw = 85 - yAngle;

        //xAngle += pitch;
        //yAngle += yaw;

        //Camera.main.transform.RotateAround(mOrbitCenter, Camera.main.transform.right, yaw);
        //Camera.main.transform.RotateAround(mOrbitCenter, -Vector3.up, pitch);
    }
}
