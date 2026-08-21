using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HUX.Buttons;
using HoloToolkit.Unity.InputModule;

public class InputHandleButton : MonoBehaviour, IInputClickHandler, IInputHandler
{
    MenuManager m_MenuManager = null;

	// Use this for initialization
	void Start () {
        m_MenuManager = transform.parent.GetComponent<MenuManager>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnInputClicked(InputClickedEventData eventData)
    {
        Debug.Log("OnInputClicked");
    }

    public void OnInputUp(InputEventData eventData)
    {
        ModelManager model = GameObject.Find("Model").GetComponent<ModelManager>();

        if (this.name == "ShowAll")
        {
            model.ShowModel(ModelManager.ModelIndex.model_over, true);
            model.ShowModel(ModelManager.ModelIndex.model_under, true);
            model.ShowModel(ModelManager.ModelIndex.model_base, true);
        }
        else if (this.name == "ShowOver")
        {
            model.ShowModel(ModelManager.ModelIndex.model_over, true);
            model.ShowModel(ModelManager.ModelIndex.model_under, false);
            model.ShowModel(ModelManager.ModelIndex.model_base, false);
        }
        else if (this.name == "ShowUnder")
        {
            model.ShowModel(ModelManager.ModelIndex.model_over, false);
            model.ShowModel(ModelManager.ModelIndex.model_under, true);
            model.ShowModel(ModelManager.ModelIndex.model_base, true);
        }
        else if (this.name == "Orbit")
            model.SetMouseMode(1);
        else if (this.name == "Pan")
            model.SetMouseMode(2);
        else if (this.name == "Scale")
            model.ScaleChange();

        m_MenuManager.Visible(false);
    }

    public void OnInputDown(InputEventData eventData)
    {
        Debug.Log("OnInputDown");
    }
}
