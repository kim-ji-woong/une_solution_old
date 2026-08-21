using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidInterfaceBehaviour : MonoBehaviour {

    [SerializeField]
    PoiManager poiManager = null;
    [SerializeField]
    MainBehaviour mainBehaviour = null;
    List<AndroidPoiBehaviour> poiList = new List<AndroidPoiBehaviour>();

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    private int detectedSensorNum = 2;

    public void ToggleFire()
    {
        mainBehaviour.ToggleNavigationMode();
    }
    public void TogglePath()
    {
        mainBehaviour.TogglePathMode();
    }

    public void ExitUnityProcess()
    {
        Application.Quit();
    }

    public void CreateFireDetectorPoi(GameObject parent, Vector3 position)
    {
        GameObject fireDetector = null;
        Transform[] allTrans = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform temp in allTrans)
        {
            if (temp.gameObject.tag.Equals("firedetector"))
            {
                fireDetector = temp.gameObject;
            }
        }

                 
        if(fireDetector != null)
        {
            GameObject newPoi = GameObject.Instantiate(fireDetector);

            AndroidPoiBehaviour newPoiBehaviour = newPoi.GetComponent<AndroidPoiBehaviour>();

            newPoi.transform.parent = parent.transform;
            newPoi.transform.position = position;
            newPoi.SetActive(true);

            poiList.Add(newPoiBehaviour);
        }
        
    }


    public void SetFireDetectPoiVisibility(int numOfSensor, bool active)
    {
        AndroidPoiBehaviour poi = poiList[numOfSensor];       //알람 울릴 센서만 (시연용)

        if (poi != null)
        {
            if (active)
            {
                poi.gameObject.SetActive(true);
            }
            else
            {
                poi.gameObject.SetActive(false);
            }
        }
    }

    public void ActivateFireDetectPoi(int numOfSensor, bool active)
    {
        AndroidPoiBehaviour poi = poiList[numOfSensor];       //알람 울릴 센서만 (시연용)

        if (poi != null)
        {
            if (active)
            {
                poi.IsSensorActivated = true;
            }
            else
            {
                poi.IsSensorActivated = false;
            }
        }
    }

}
