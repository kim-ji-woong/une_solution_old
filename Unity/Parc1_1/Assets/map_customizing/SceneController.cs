using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    private MainModel mainScript;

    public InputField inputField;

    private string[] sceneStrNames = new string[34];

    // Start is called before the first frame update
    void Start()
    {
        mainScript = FindObjectOfType<MainModel>();

        sceneStrNames[0] = "Outdoor_Temp";
        sceneStrNames[1] = "h01f";
        sceneStrNames[2] = "h02f";
        sceneStrNames[3] = "h03f";
        sceneStrNames[4] = "h04f";
        sceneStrNames[5] = "h05f";
        sceneStrNames[6] = "h06f";
        sceneStrNames[7] = "h07f";
        sceneStrNames[8] = "h08f";
        sceneStrNames[9] = "h09f";
        sceneStrNames[10] = "h10f";
        sceneStrNames[11] = "h11f";
        sceneStrNames[12] = "h12f";
        sceneStrNames[13] = "h13f";
        sceneStrNames[14] = "h14f";
        sceneStrNames[15] = "h15f";
        sceneStrNames[16] = "h16f";
        sceneStrNames[17] = "h17f";
        sceneStrNames[18] = "h18f";
        sceneStrNames[19] = "h19f";
        sceneStrNames[20] = "h20f";
        sceneStrNames[21] = "h21f";
        sceneStrNames[22] = "h22f";
        sceneStrNames[23] = "h23f";
        sceneStrNames[24] = "h24f";
        sceneStrNames[25] = "h25f";
        sceneStrNames[26] = "h26f";
        sceneStrNames[27] = "h27f";
        sceneStrNames[28] = "h28f";
        //sceneStrNames[29] = UnityScene indoor29 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_29F", "h29f", "h29f_zone", new Vector3(0.0f, 91.63f, 0.0f), false, true, 0.5f, 89);
        //sceneStrNames[30] = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_29.5F", "h29_02f", "h29-2f_zone", new Vector3(0.0f, 94.51f, 0.0f), false, true, 0.5f, 92.5f, indoor29);
        //sceneStrNames[31] = UnityScene indoor30 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_30F", "h30f", "h30f_zone", new Vector3(0.0f, 96.83f, 0.0f), false, true, 0.5f, 96);
        //sceneStrNames[32] = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_31F", "h31f", "h31f_zone", new Vector3(0.0f, 101.31f, 0.0f), false, true, 0.5f, 98, indoor30);
        //sceneStrNames[33] = UnityScene indoor32 = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_32F", "h32f", "h32f_zone", new Vector3(0.0f, 105.14f, 0.0f), false, true, 0.5f, 103);
        //sceneStrNames[34] = SetIndoorScene(m_dicSceneIndoors, dicCameraDatas, "Camera_Hotel_33F", "h33f", "h33f_zone", new Vector3(0.0f, 108.66f, 0.0f), false, true, 0.5f, 105.5f, indoor32);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void BT_Indoor()
    {
        int idx = Convert.ToInt32(inputField.text);

        //mainScript.EditMode = idx != 0;

        mainScript.SelectScene(sceneStrNames[idx]);
    }
}
