using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ButtonState { Normal, Hover, Click }
public class UIEventSystem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
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

    // Start is called before the first frame update    
    void Start()
    {
        UnityEngine.UI.Image img = GetComponent<UnityEngine.UI.Image>();
        if (img != null && img.mainTexture != null)
        {
            if (!m_dicViewButtons.ContainsKey(img))
                m_dicViewButtons[img] = false;

            if (img.name == "btnSlideRight")
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

            if (btnName == "btnPoiVisible" || btnName == "btnManualReport" || btnName == "btnSlideLeft" || btnName == "btnSlideRight")
            {
                if (PassivePipeProxy.Instance != null)
                    PassivePipeProxy.Instance.SendServer(string.Format("OnMessage(OnClick,{0})", img.mainTexture.name));
            }
            else
            {
                MainModel mainModel = ModelManager.Instance.Model;
                if (btnName == "btnHome")
                    mainModel.SelectScene(mainModel.CurrentScene.SceneName);
                else if (btnName == "btnZoomIn")
                    mainModel.ManualZoom(m_nDelta);
                else if (btnName == "btnZoomOut")
                    mainModel.ManualZoom(-m_nDelta);
                else if (btnName == "btnPanning")
                {
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
                    POIManager.Instance.ShowBuildingText(bChecked);
            }            
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
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

        Debug.Log(changeImageName);

        if (changeImageName.Length > 0)
        {
            Sprite sprite = Resources.Load(changeImageName, typeof(Sprite)) as Sprite;
            if (sprite != null)
                img.sprite = sprite;
        }
    }
}
