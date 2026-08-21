using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InitializeFlowData : MonoBehaviour
{
    public Text uiTextFlowTime = null;
    public Text UiTextFlowDataContent = null;
    public Button uiButtonPopDetail = null;

    public GameObject gobjPopDetail = null;

    private GameObject gobjPopupInstance = null;


    private int m_nID = -1;

    // Use this for initialization
    void Start()
    {
        this.gameObject.transform.SetAsFirstSibling();

        m_nID = DataManager.CreateID();
        
        if (uiTextFlowTime != null)
        {
            uiTextFlowTime.text = DateTime.Now.ToShortTimeString();
        }

        RectTransform rect = this.transform as RectTransform;
        rect.offsetMin = new Vector2(5.0f, rect.offsetMin.y);
        rect.offsetMax = new Vector2(-5.0f, rect.offsetMax.y);

        if (uiButtonPopDetail != null)
        {
            Button.ButtonClickedEvent eFunc = new Button.ButtonClickedEvent();
            eFunc.AddListener(TogglePopup);
            uiButtonPopDetail.onClick = eFunc;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetContents(string strContents)
    {
        if (UiTextFlowDataContent != null)
        {
            UiTextFlowDataContent.text = strContents;
        }
    }

    private void TogglePopup()
    {
        if (gobjPopupInstance == null)
        {
            RectTransform rect = this.gameObject.transform as RectTransform;
            Vector2 vParentAchoredPosition = rect.offsetMin;

            gobjPopupInstance = Instantiate(gobjPopDetail, new Vector2(0.0f, 0.0f), Quaternion.identity) as GameObject;
            gobjPopupInstance.transform.parent = this.gameObject.transform.parent;
            gobjPopupInstance.SendMessage("SetParentRectTransform", this.transform.parent as RectTransform, SendMessageOptions.DontRequireReceiver);
            gobjPopupInstance.SendMessage("SetContents", new object[] { m_nID % 3 , "AWS", "Temperature", "Water", "Wind Speed", "ETC"}, SendMessageOptions.DontRequireReceiver);

            RectTransform rectPopup = gobjPopupInstance.transform as RectTransform;
            rectPopup.anchoredPosition = new Vector2(
                vParentAchoredPosition.x + (rectPopup.GetWidth() / 2.0f) + uiTextFlowTime.rectTransform.GetWidth(),
                vParentAchoredPosition.y - (rectPopup.GetHeight() / 2.0f) + (uiTextFlowTime.rectTransform.GetHeight() / 2.0f)
                );
        }
        else
        {
            bool bVisible = gobjPopupInstance.activeSelf;

            DestroyImmediate(gobjPopupInstance);
            gobjPopupInstance = null;

            if (bVisible == false)
                TogglePopup();

        }
    }

}