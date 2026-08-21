using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ApplyCurrentTime : MonoBehaviour
{
    public Text uiTextNowTime = null;

    // Use this for initialization
    void Start()
    {
        if (uiTextNowTime == null)
        {
            uiTextNowTime = this.GetComponent<Text>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (uiTextNowTime != null)
        {
            uiTextNowTime.text = String.Format("Current time is at... {0}.", DateTime.Now.ToString("yyyy-MM-dd H:M:ss"));
        }
    }
}