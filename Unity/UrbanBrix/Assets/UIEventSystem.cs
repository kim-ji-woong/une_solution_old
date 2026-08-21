using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ButtonState { Normal, Hover, Click }
public class UIEventSystem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private static Dictionary<UnityEngine.UI.Image, bool> m_dicViewButtons = new Dictionary<Image, bool>();
    public static Dictionary<UnityEngine.UI.Image, bool> DicViewButtons
    {
        get
        {
            return m_dicViewButtons;
        }
        set
        {
            m_dicViewButtons = value;
        }
    }

    //private bool m_bChecked = false;
    private int m_nDelta = 24;

    private CanvasPOI m_poi = null;

    // Start is called before the first frame update    
    void Start()
    {
        m_poi = GetComponent<CanvasPOI>();

        UnityEngine.UI.Image img = GetComponent<UnityEngine.UI.Image>();
        if (img != null && img.mainTexture != null)
        {
            if (!m_dicViewButtons.ContainsKey(img))
                m_dicViewButtons[img] = false;

            if (img.name == "btnSlideRight")
                img.enabled = false;
            if (img.name == "imgEditMode" || img.name == "imgWallEditMode")
                img.enabled = false;
            if (img.name == "btnText")
                m_dicViewButtons[img] = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (m_poi != null)
        {
            m_poi.OnMouseDown(eventData);
            return;
        }

        UnityEngine.UI.Image img = GetComponent<UnityEngine.UI.Image>();
        if (img != null && img.mainTexture != null)
        {
            if (!m_dicViewButtons.ContainsKey(img))
                return;

            bool bChecked = false;
            string btnName = img.name;
            string orgImageName = img.mainTexture.name;
            if (btnName == "btnPanning" || btnName == "btnOrbit" || btnName == "btnText")
            {
                if (orgImageName.Contains("_click"))
                    m_dicViewButtons[img] = false;
                else
                    m_dicViewButtons[img] = true;

                bChecked = m_dicViewButtons[img];
                if (!bChecked)
                    ChangeImage(img, ButtonState.Normal);
                else
                    ChangeImage(img, ButtonState.Click);
            }
            else
            {
                ChangeImage(img, ButtonState.Click);
            }

            if (btnName == "btnPoiVisible" || btnName == "btnBroadcast" || btnName == "btnManualReport" || btnName == "btnSlideLeft" || btnName == "btnSlideRight")
            {
                if (PassivePipeProxy.Instance != null)
                    PassivePipeProxy.Instance.SendServer(string.Format("OnMessage(OnClick,{0})", img.mainTexture.name));
            }
            else
            {
                MainModel mainModel = ModelManager.Instance.Model;
                if (btnName == "btnHome")
                {
                    //mainModel.AddDoor();
                    mainModel.SelectScene(mainModel.CurrentScene.SceneName);
                    //mainModel.AddIcon();

                    //mainModel.CheckPOIZones();
                    //POIManager.Instance.ShowIconLayers("CCTV", true);
                    //mainModel.EditMode = true;
                    //mainModel.SelectScene("Office-a-15f");

                    //POI TEST
                    //POIManager.Instance.AddIconPOIFile("Fire", @"D:/UnESolution/bin/common12/3AddFirePOI.txt");
                    //POIManager.Instance.ShowIconPOIFile(@"D:/UnESolution/bin/common12/3ShowFirePOI.txt");
                }
                else if (btnName == "btnZoomIn")
                {
                    //mainModel.AddWall();

                    mainModel.ManualZoom(m_nDelta);
                    //POI TEST
                    //POIManager.Instance.ShowIconLayer("Fire", true);
                    //mainModel.AddIcon();
                    //mainModel.SetEditMode(true);

                    //POIManager.Instance.RollBackPOIIcon("");
                    //mainModel.SetEditMode(false);
                    //CustomizingController.Instance.SetWallColor();                    
                }
                else if (btnName == "btnZoomOut")
                {
                    //mainModel.AddSpaceText("한글");
                    //mainModel.SetEditMode(false);
                    //mainModel.AddWall();
                    //mainModel.LoadSpaceTexts(@"C:\UNE\Unity\SpaceText\h01f.txt", "h07f");
                    mainModel.ManualZoom(-m_nDelta);
                }
                else if (btnName == "btnPanning")
                {
                    //mainModel.ChangeFontSpaceText("궁서체", 20, 0);
                    //mainModel.ChangeColorSpaceText("#8000FF");
                    //mainModel.GetWalls(@"C:\UNE\Unity\SpaceText\");

                    if (bChecked)
                    {
                        if (mainModel.Mode == MainModel.MouseWorkMode.ORBIT)
                        {
                            foreach (KeyValuePair<Image, bool> item in m_dicViewButtons)
                            {
                                Image otherButton = item.Key;
                                if (otherButton.name == "btnOrbit")
                                {
                                    ChangeImage(otherButton, ButtonState.Normal);
                                    m_dicViewButtons[otherButton] = false;
                                    break;
                                }
                            }
                        }

                        mainModel.SetMode((int)MainModel.MouseWorkMode.PANNING, true);
                    }
                    else
                        mainModel.SetMode((int)MainModel.MouseWorkMode.PICK, true);
                }
                else if (btnName == "btnOrbit")
                {                    
                    //mainModel.LoadWalls(@"C:\UNE\Unity\Walls\Hotel-6f.txt", "Hotel-6f");

                    if (bChecked)
                    {
                        if (mainModel.Mode == MainModel.MouseWorkMode.PANNING)
                        {
                            foreach (KeyValuePair<Image, bool> item in m_dicViewButtons)
                            {
                                Image otherButton = item.Key;
                                if (otherButton.name == "btnPanning")
                                {
                                    ChangeImage(otherButton, ButtonState.Normal);
                                    m_dicViewButtons[otherButton] = false;
                                    break;
                                }
                            }
                        }

                        mainModel.SetMode((int)MainModel.MouseWorkMode.ORBIT, true);
                    }
                    else
                        mainModel.SetMode((int)MainModel.MouseWorkMode.PICK, true);

                }
                else if (btnName == "btnText")
                {
                    POIManager.Instance.ShowBuildingText(bChecked);

                    CustomSpaceText.bShow = bChecked;
                    CustomSpaceText.Instance.VisibleSpaceText(bChecked);                    
                }
            }            
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (m_poi != null)
        {
            m_poi.OnMouseUp(eventData);
            return;
        }
    
        UnityEngine.UI.Image img = GetComponent<UnityEngine.UI.Image>();
        if (img != null && img.mainTexture != null)
        {
            string btnName = img.name;
            if (btnName != "btnPanning" && btnName != "btnOrbit" && btnName != "btnText")
            {
                ChangeImage(img, ButtonState.Hover);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UnityEngine.UI.Image img = GetComponent<UnityEngine.UI.Image>();
        if (img != null && img.mainTexture != null)
        {
            if (!m_dicViewButtons.ContainsKey(img))
                return;

            bool bChecked = m_dicViewButtons[img];
            if (!bChecked)
                ChangeImage(img, ButtonState.Hover);
        }

        if (m_poi != null)
            m_poi.OnMouseEnter(eventData);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        UnityEngine.UI.Image img = GetComponent<UnityEngine.UI.Image>();
        if (img != null && img.mainTexture != null)
        {
            if (!m_dicViewButtons.ContainsKey(img))
                return;

            bool bChecked = m_dicViewButtons[img];
            if (!bChecked)
                ChangeImage(img, ButtonState.Normal);
        }

        if (m_poi != null)
            m_poi.OnMouseExit(eventData);
    }

    private void ChangeImage(UnityEngine.UI.Image img, ButtonState state)
    {
        string imgName = img.name;
        string changeImageName = "";
        if (state == ButtonState.Normal)
            changeImageName = imgName + "_normal";
        else if (state == ButtonState.Hover)
            changeImageName = imgName + "_hover";
        else if (state == ButtonState.Click)
            changeImageName = imgName + "_click";

        if (changeImageName.Length > 0)
        {
            Sprite sprite = Resources.Load(changeImageName, typeof(Sprite)) as Sprite;
            if (sprite != null)
                img.sprite = sprite;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_poi != null)
        {
            m_poi.OnMouseDrag(eventData);
        }
    }    
}
