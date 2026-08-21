using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class InitializeFlowDataDetail : MonoBehaviour
{
    private RectTransform m_rectParent = null;

    public RectTransform rectTitle = null;
    public Button buttonToggle = null;
    
    public GameObject[] gobjDisaterImages = null;
    public GameObject gobjContentBox = null;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetParentRectTransform(RectTransform rect)
    {
        m_rectParent = rect;

        rectTitle.SendMessage("SetParentRectTransform", m_rectParent);
    }

    public void TogglePopup()
    {
        buttonToggle.SendMessage("TogglePopup");
    }

    public void SetContents(object[] arrContents)
    {
        Transform parent = null;
        float x = 160.0f;
        float y = -30.0f;

        foreach (Component component in this.GetComponentsInChildren<RectTransform>())
        {
            if (String.Equals(component.name, "Content"))
            {
                parent = component.transform;
                break;
            }
        }

        if (parent == null)
            return;


        GameObject obj = null;

        // 0 : Disater Image Index
        // 1 : AWS
        // 2 : Temperature
        // 3 : Water
        // 4 : Wind Speed
        // 5 : ETC

        for (int nIndex = 0; nIndex < arrContents.Length; nIndex++)
        {
            switch (nIndex)
            {
                case 0:
                    obj = Instantiate(gobjDisaterImages[Convert.ToInt32(arrContents[nIndex])], new Vector3(), Quaternion.identity) as GameObject;
                    obj.transform.parent = parent;
                    obj.transform.localPosition = new Vector3(30.0f, y, 0.0f);

                    y -= 55;

                    break;

                case 1:
                case 2:
                case 3:
                case 4:
                case 5:

                    obj = Instantiate(gobjContentBox, new Vector3(), Quaternion.identity) as GameObject;
                    obj.transform.parent = parent;
                    obj.transform.localPosition = new Vector3(x, y, 0.0f);

                    (obj.GetComponent<Text>() as Text).text = arrContents[nIndex].ToString();

                    y -= 65;

                    break;

                default:
                    break;
            }
        }

    }

}