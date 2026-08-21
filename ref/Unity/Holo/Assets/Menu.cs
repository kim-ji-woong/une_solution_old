using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class Menu : MonoBehaviour, IInputClickHandler
{
    MenuManager m_MenuManager = null;

    // Use this for initialization
    void Start () {
		if(m_MenuManager == null)
            m_MenuManager = transform.parent.GetComponent<MenuManager>();

        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.sortingOrder = 4000;
            mr.sharedMaterial.renderQueue = 4000;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnInputClicked(InputClickedEventData eventData)
    {
        m_MenuManager.OnClick(transform.name);
    }

    public void ChangeMaterial(bool bShow)
    {
        MeshRenderer mr = transform.gameObject.GetComponent<MeshRenderer>();
        if(mr != null)
        {
            if(bShow)
                mr.material.color = Color.white;
            else
                mr.material.color = Color.gray;
        }
    }
}
