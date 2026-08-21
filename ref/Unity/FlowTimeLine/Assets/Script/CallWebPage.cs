using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CallWebPage : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Button btn = GetComponent<Button>();
        Button.ButtonClickedEvent eFunc = new Button.ButtonClickedEvent();

        eFunc.AddListener(CallWeb);
        btn.onClick = eFunc;
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void CallWeb()
    {
        Application.ExternalCall("CallWeb");
    }

}