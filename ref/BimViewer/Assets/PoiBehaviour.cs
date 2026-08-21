using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Resources.Scripts;
public class PoiBehaviour : MonoBehaviour {

    [SerializeField]
    WebInterfaceBehaviour webInterfaceBehaviour = null;
    public enum PoiType
    {
        None = 0,
        FireDetector,
        EscapeGuide,
        BroadcastDevice,
        CollapseDetector,
        RequireRescuePerson
    }    

    static string [] PoiNames = { "없음", "화재감지기", "대피유도등", "재난방송장치", "붕괴감지기", "요구조자" };
    static string[] PoiTypeNames = { "None", "FireDetector", "EscapeGuide", "BroadcastDevice", "CollapseDetector", "RequireRescuePerson" };

    string id = "";

    bool isSensorActivated = false;

    [SerializeField]
    PoiType currentPoiType = PoiType.None;
    //[SerializeField]
    [SerializeField]
    GameObject poi3dObject = null;

    UnityEngine.UI.Image poiImage = null;

    public GameObject Poi3dObject
    {
        get
        {
            return poi3dObject;
        }

        set
        {
            poi3dObject = value;
        }
    }

    public PoiType CurrentPoiType
    {
        get
        {
            return currentPoiType;
        }

        set
        {
            currentPoiType = value;
        }
    }

    public string Id
    {
        get
        {
            return id;
        }
    }

    public bool IsSensorActivated
    {
        get
        {
            return isSensorActivated;
        }

        set
        {
            isSensorActivated = value;

            if(!isSensorActivated)
            {
                //reset color
                poiImage.color = new Color(1.0f, 1.0f, 1.0f);
            }
        }
    }
    void Awake()
    {
        id = Guid.NewGuid().ToString();
        this.gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnPoiClicked);
    }

    // Use this for initialization
    void Start () {		

        poiImage = this.gameObject.GetComponent<UnityEngine.UI.Image>();        
    }

    float currentColorPosition = 1.0f;

    //double timeElapsed = 0.0f;

    float colorPositionStep = -0.1f;

    void FixedUpdate()
    {
        if (IsSensorActivated)
        {
            currentColorPosition += colorPositionStep;

            if (Mathf.Abs(currentColorPosition) >= 1.0f)
            {
                colorPositionStep = colorPositionStep * -1.0f;
            }

            Color currentColor = new Color(1.0f, currentColorPosition, currentColorPosition);

            poiImage.color = currentColor;
        }
    }

    // Update is called once per frame
    void Update () {
        Vector3 ViewportPosition = Camera.main.WorldToViewportPoint(Poi3dObject.transform.position);

        if (ViewportPosition.z >= 0.0f)
        {
            RectTransform canvasTransform = GetComponentInParent<RectTransform>();

            canvasTransform.anchorMin = ViewportPosition;
            canvasTransform.anchorMax = ViewportPosition;

            Vector2 WorldObject_ScreenPosition = new Vector2(
                                                     ((ViewportPosition.x * canvasTransform.sizeDelta.x) - (canvasTransform.sizeDelta.x * 0.5f)),
                                                     ((ViewportPosition.y * canvasTransform.sizeDelta.y) - (canvasTransform.sizeDelta.y * 0.5f)));


            RectTransform transform = GetComponent<RectTransform>();

            transform.anchoredPosition = WorldObject_ScreenPosition;
        }
    }

    public string GetPoiTypeName()
    {
        return PoiTypeNames[(int)CurrentPoiType];
    }

    public string GetPoiName()
    {
        return PoiNames[(int)CurrentPoiType];
    }

    public static PoiType GetPoiTypeByName(string poiTypeName)
    {
        int counter = 0;

        foreach(string s in PoiTypeNames)
        {
            if (s == poiTypeName)
                return (PoiType)counter;

            counter++;
        }

        return PoiType.None;
    }

    public Poi GetPoiProperty()
    {
        Poi poi = new Poi();
        poi.Id = Id;
        poi.PoiName = GetPoiName();
        poi.PoiTypeName = GetPoiTypeName();
        poi.X = this.gameObject.transform.position.x;
        poi.Y = this.gameObject.transform.position.y;
        poi.Z = this.gameObject.transform.position.z;

        poi.Visible = this.gameObject.activeSelf;
        poi.Activate = IsSensorActivated;

        return poi;
    }

    public void OnPoiClicked()
    {
        Debug.Log(this.gameObject.GetComponent<PoiBehaviour>().Id + " clicked");

        webInterfaceBehaviour.OnPoiClicked(id);
    }
}
