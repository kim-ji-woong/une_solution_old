using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ButtonState { Normal, Hover, Click }

public class UIEventSystem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CanvasPOI poi = GetComponent<CanvasPOI>();

        if (poi != null)
        {
            poi.Pick();
        }

        /*UnityEngine.UI.Image img = GetComponent<UnityEngine.UI.Image>();

        if (img != null)
        {
            Debug.Log(img.name + " is Clicked");
        }*/
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CanvasPOI poi = GetComponent<CanvasPOI>();

        if (poi != null)
        {
            poi.OnMouseEnter(eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CanvasPOI poi = GetComponent<CanvasPOI>();

        if (poi != null)
        {
            poi.OnMouseExit(eventData);
        }
    }

    private void ChangeImage(UnityEngine.UI.Image img, ButtonState state)
    {
        /*string imgName = img.name;
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
        }*/
    }
}
