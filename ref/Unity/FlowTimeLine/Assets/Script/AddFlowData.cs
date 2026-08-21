using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AddFlowData : MonoBehaviour
{
    public GameObject gobjPanelFlowDataPrefab = null;
    public RectTransform rectContents = null;
    public Button uiButtonAdd = null;

    private float panelX = 0.0f;
    private float panelY = -55.0f;
    
    // Use this for initialization
    void Start()
    {
        if (uiButtonAdd != null)
        {
            Button.ButtonClickedEvent eFunc = new Button.ButtonClickedEvent();
            eFunc.AddListener(AddFlowDataEmpty);
            uiButtonAdd.onClick = eFunc;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AddFLowDataContent(string strContent)
    {
        if (gobjPanelFlowDataPrefab == null || rectContents == null)
            return;

        GameObject obj = Instantiate(gobjPanelFlowDataPrefab, new Vector2(0.0f, 0.0f), Quaternion.identity) as GameObject;

        RectTransform rect = obj.transform as RectTransform;
        rect.transform.parent = rectContents;
        rect.transform.localPosition = new Vector3(panelX, panelY, 0.0f);

        if (DataManager.CurrentID * (rect.GetHeight() + 5.0f) + 5.0f > rectContents.GetHeight())
        {
            rectContents.SetHeight(DataManager.CurrentID * (rect.GetHeight() + 5.0f) + 5.0f);
            //rectContents.AddHeight(rect.GetHeight() + 5.0f);
        }

        obj.SendMessage("SetContents", strContent, SendMessageOptions.DontRequireReceiver);

        panelY -= (rect.GetHeight() + 5.0f);
    }

    private void AddFlowDataEmpty()
    {
        AddFLowDataContent(DataManager.CurrentID.ToString());
    }

    public void AddFlowUserData(string strContent)
    {
        AddFLowDataContent(strContent);
    }
    
}