using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToggleRectTransform : MonoBehaviour
{
    public RectTransform RectTransformPanel = null;

    void Start()
    {
        Button btn = GetComponent<Button>();
        Button.ButtonClickedEvent eFunc = new Button.ButtonClickedEvent();

        eFunc.AddListener(ClickButton);
        btn.onClick = eFunc;
    }

    private void ClickButton()
    {
        TogglePanel();
    }

    public void TogglePanel()
    {
        RectTransformPanel.gameObject.SetActive(!RectTransformPanel.gameObject.activeSelf);
    }

}