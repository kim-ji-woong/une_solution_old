using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoiManager : MonoBehaviour {
    [SerializeField]
    List<PoiBehaviour> poiTemplateList = new List<PoiBehaviour>();

    List<PoiBehaviour> poiList = new List<PoiBehaviour>();

    public List<PoiBehaviour> PoiList
    {
        get
        {
            return poiList;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CreatePoi(PoiBehaviour.PoiType poiType, Vector3 location)
    {
        foreach(PoiBehaviour poiTemplate in poiTemplateList)
        {
            if(poiTemplate.CurrentPoiType == poiType)
            {
                GameObject newPoi = GameObject.Instantiate(poiTemplate.gameObject);

                GameObject newPoi3dObject = GameObject.Instantiate(poiTemplate.Poi3dObject);

                PoiBehaviour newPoiBehaviour = newPoi.GetComponent<PoiBehaviour>();

                newPoi.SetActive(true);

                newPoi.transform.parent = poiTemplate.transform.parent;
                newPoi.transform.localScale = new Vector3(1, 1, 1);

                newPoi3dObject.SetActive(true);
                newPoiBehaviour.Poi3dObject = newPoi3dObject;

                //newPoiBehaviour.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnPoiClicked);

                newPoi3dObject.transform.position = location;

                PoiList.Add(newPoiBehaviour);

                Debug.Log("POI created:" + newPoiBehaviour.Id);

                break;
            }            
        }
    }

    
}
