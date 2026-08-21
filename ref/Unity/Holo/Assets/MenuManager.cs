using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    private ModelManager m_ModelManager = null;

    private int m_nMoving = -1;
    private int m_nMovingCnt = 0;

    private bool m_bVisible = false;
    public bool visible { get { return m_bVisible; } }

	// Use this for initialization
	void Start () {
        if (m_ModelManager == null)
            m_ModelManager = GameObject.Find("Model").GetComponent<ModelManager>();

        MeshRenderer mr = transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.sortingOrder = 4000;
            mr.sharedMaterial.renderQueue = 4000;
        }
        Visible(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (m_ModelManager == null)
            return;

        if (m_nMoving != -1)
        {
            m_ModelManager.ModelTranslate(m_nMoving);

            ++m_nMovingCnt;
            if (m_nMovingCnt > 20)
                m_nMoving = -1;
        }
	}

    public void OnClick(string name)
    {
        if (name == "MenuFirst")
        {
            if (m_ModelManager.IsMoving)
                return;

            m_ModelManager.modelType = 0;
            m_ModelManager.KeyDown();
            m_ModelManager.Move();
        }
        else if (name == "MenuZoom")
        {
            if (m_ModelManager.IsMoving)
                return;

            m_ModelManager.modelType = 1;
            m_ModelManager.KeyDown();
            m_ModelManager.Move();
        }
        else if (name == "MenuUnderLoad")
        {
            if (m_ModelManager.IsMoving)
                return;

            m_ModelManager.modelType = 2;
            m_ModelManager.KeyDown();
            m_ModelManager.Move();
        }
        else if (name == "MenuCollision")
        {
            if (m_ModelManager.IsMoving)
                return;

            m_ModelManager.modelType = 4;
            m_ModelManager.KeyDown();
            m_ModelManager.Move();
        }
        else if (name == "MenuForward")
        {
            if (m_ModelManager.IsMoving)
                return;

            m_nMovingCnt = 0;
            m_nMoving = 0;
        }
        else if (name == "MenuBack")
        {
            if (m_ModelManager.IsMoving)
                return;

            m_nMovingCnt = 0;
            m_nMoving = 1;
        }
        else if (name == "MenuLeft")
        {
            if (m_ModelManager.IsMoving)
                return;

            m_nMovingCnt = 0;
            m_nMoving = 2;
        }
        else if (name == "MenuRight")
        {
            if (m_ModelManager.IsMoving)
                return;

            m_nMovingCnt = 0;
            m_nMoving = 3;
        }

        else if (name == "MenuShowTerrain")
        {
            bool bShow = m_ModelManager.ShowHide(0);
            Menu menu = transform.GetChild(9).GetComponent<Menu>();
            menu.ChangeMaterial(bShow);
        }
        else if (name == "MenuShowUnder")
        {
            bool bShow = m_ModelManager.ShowHide(1);
            Menu menu = transform.GetChild(10).GetComponent<Menu>();
            menu.ChangeMaterial(bShow);
        }
        else if (name == "MenuShowBase")
        {
            bool bShow = m_ModelManager.ShowHide(2);
            Menu menu = transform.GetChild(11).GetComponent<Menu>();
            menu.ChangeMaterial(bShow);
        }
        else if (name == "MenuShowNew")
        {
            bool bShow = m_ModelManager.ShowHide(3);
            Menu menu = transform.GetChild(12).GetComponent<Menu>();
            menu.ChangeMaterial(bShow);
        }
        Visible(false);
    }

    public void Visible(bool bShow)
    {
        m_bVisible = bShow;

        // 위치 지정
        if(bShow)
        {
            m_nMoving = -1;
            SetPos();

            Menu menu = transform.GetChild(9).GetComponent<Menu>();
            menu.ChangeMaterial(m_ModelManager.IsActive(ModelManager.ModelIndex.model_over));

            menu = transform.GetChild(10).GetComponent<Menu>();
            menu.ChangeMaterial(m_ModelManager.IsActive(ModelManager.ModelIndex.model_under));

            menu = transform.GetChild(11).GetComponent<Menu>();
            menu.ChangeMaterial(m_ModelManager.IsActive(ModelManager.ModelIndex.model_base));

            menu = transform.GetChild(12).GetComponent<Menu>();
            menu.ChangeMaterial(m_ModelManager.IsActive(ModelManager.ModelIndex.model_new));
        }

        //gameObject.SetActive(bShow);

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(bShow);
            Renderer r = child.gameObject.GetComponent<Renderer>();
            if (r != null)
                r.enabled = bShow;

            VisibleChild(child, bShow);
        }
    }

    private void VisibleChild(Transform parent, bool bShow)
    {
        foreach (Transform child in parent)
        {
            child.gameObject.SetActive(bShow);
            Renderer r = child.gameObject.GetComponent<Renderer>();
            if (r != null)
                r.enabled = bShow;

            VisibleChild(child, bShow);
        }
    }

    private void SetPos()
    {
        transform.position = Camera.main.transform.position + (Camera.main.transform.forward * 1.5f);
        transform.forward = Camera.main.transform.forward;
        //foreach (Transform child in transform)
        //{
        //    child.LookAt(Camera.main.transform.position);
        //}
    }
}
