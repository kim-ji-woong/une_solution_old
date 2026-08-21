using Assets.Resources.Scripts;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WebInterfaceBehaviour : MonoBehaviour {
    [DllImport("__Internal")]
    private static extern void _OnFinishModelLoading();

    [DllImport("__Internal")]
    private static extern void _SendPoiList(string list);

    [DllImport("__Internal")]
    private static extern void _OnPoiClicked(string id);


    [SerializeField]
    PoiManager poiManager = null;
    [SerializeField]
    MainBehaviour mainBehaviour = null;

    static string[] stringSplitter = { "," };

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetPoiStatus(string data)
    {
        string[] splittedData = data.Split(stringSplitter, System.StringSplitOptions.RemoveEmptyEntries);
    }

    public void CreatePoi(string data)
    {
        string [] splittedData = data.Split(stringSplitter, System.StringSplitOptions.RemoveEmptyEntries);

        if (4 != splittedData.Length)
            return;

        PoiBehaviour.PoiType poiType = PoiBehaviour.GetPoiTypeByName(splittedData[0]);

        if(poiType != PoiBehaviour.PoiType.None)
        {
            Vector3 position = StringToVector3(splittedData[1], splittedData[2], splittedData[3]);

            poiManager.CreatePoi(poiType, position);
        }        
    }

    public static Vector3 StringToVector3(string x,string y,string z)
    {
        float fx,fy,fz;

        if (float.TryParse(x, out fx) && float.TryParse(y, out fy) && float.TryParse(z, out fz))
            return new Vector3(fx, fy, fz);

        return Vector3.zero;
    }

    public void OnFinishModelLoading()
    {
        if(Application.platform == RuntimePlatform.WebGLPlayer)
            _OnFinishModelLoading(); //WEB 호출
    }

    public string GetPoiListString()
    {
        string data = "";

        List<Poi> poiPropertyList = new List<Poi>();

        foreach (PoiBehaviour poi in poiManager.PoiList)
        {
            poiPropertyList.Add(poi.GetPoiProperty());
        }

        //JsonConverter.SerializeObject()

        data = JsonHelper.ToJson(poiPropertyList.ToArray(), false);        

        return data;
    }

    public void RequestPoiList()
    {
        string list = GetPoiListString();

        Debug.Log(list);

        if (Application.platform == RuntimePlatform.WebGLPlayer)
            _SendPoiList(list);
    }

    public void OnPoiClicked(string id)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            _OnPoiClicked(id);
    }

    public void DeletePoi(string id)
    {
        PoiBehaviour poi = FindPoi(id);

        if(null != poi)
        {
            poiManager.PoiList.Remove(poi);

            GameObject.Destroy(poi.gameObject);
            GameObject.Destroy(poi.Poi3dObject);
        }        
    }

    PoiBehaviour FindPoi(string id)
    {
        int index = 0;
        bool isDigit = int.TryParse(id,out index);

        if(isDigit)
        {
            if(index < poiManager.PoiList.Count && index > -1)
            {
                return poiManager.PoiList[index];
            }
        }
        else
        {
            foreach (PoiBehaviour poi in poiManager.PoiList)
            {
                if (id == poi.Id)
                {
                    return poi;
                }
            }
        }       

        return null;
    }

    public void SetPoiVisibility(string data)
    {
        string[] splittedData = data.Split(stringSplitter, System.StringSplitOptions.RemoveEmptyEntries);

        if (splittedData.Length != 2)
            return;

        PoiBehaviour poi = FindPoi(splittedData[0]);

        if (null != poi)
        {
            string trueOrFalse = splittedData[1];

            if (string.Equals(trueOrFalse, "true", StringComparison.OrdinalIgnoreCase))
            {
                poi.gameObject.SetActive( true);
            }
            else if (string.Equals(trueOrFalse, "false", StringComparison.OrdinalIgnoreCase))
            {
                poi.gameObject.SetActive(false);
            }
        }
    }
    
    public void ActivatePoi(string data)
    {
        string[] splittedData = data.Split(stringSplitter, System.StringSplitOptions.RemoveEmptyEntries);

        if (splittedData.Length != 2)
            return;

        PoiBehaviour poi = FindPoi(splittedData[0]);

        if(null != poi)
        {
            string trueOrFalse = splittedData[1];

            if(string.Equals(trueOrFalse, "true", StringComparison.OrdinalIgnoreCase))
            {
                poi.IsSensorActivated = true;
            }
            else if(string.Equals(trueOrFalse, "false", StringComparison.OrdinalIgnoreCase))
            {
                poi.IsSensorActivated = false;
            }
        }
    }
}
