using UnityEngine;
using System.Collections;

public class ZonePolygon : MonoBehaviour
{
    private bool m_bVisible = true;

    private int m_nID = -1;
    public int ID
    {
        get { return m_nID; }
        set { m_nID = value; }
    }

    private Color m_Color;
    public Color Color
    {
        get { return m_Color; }
        set { m_Color = value; }
    }

    private string m_szName = "";
    public string Name
    {
        get { return m_szName; }
        set { m_szName = value; }
    }    

    public void SetColor(Color color)
    {
        MeshRenderer render = gameObject.GetComponent<MeshRenderer>();
        if (render != null)
        {
            render.material.color = color;
        }
    }

    void Start()
    {
    }

    void Update()
    {   
    }

    public void SetVisible(bool bVisible)
    {
        MeshRenderer mr2 = gameObject.GetComponent<MeshRenderer>();
        mr2.enabled = bVisible;
        m_bVisible = bVisible;
    }
}


public class EquipmentZonePolygon : MonoBehaviour
{
    public bool m_bVisible = true;

    private int m_nID = -1;
    public int ID
    {
        get { return m_nID; }
        set { m_nID = value; }
    }

    private Color m_Color;
    public Color Color
    {
        get { return m_Color; }
        set { m_Color = value; }
    }

    private string m_szName = "";
    public string Name
    {
        get { return m_szName; }
        set { m_szName = value; }
    }

    public void SetColor(Color color)
    {
        MeshRenderer render = gameObject.GetComponent<MeshRenderer>();
        if (render != null)
        {
            render.material.color = color;
        }
    }

    void Start()
    {
    }

    void Update()
    {
        MeshRenderer mr2 = gameObject.GetComponent<MeshRenderer>();
        bool bVisible = mr2.enabled;
        if (m_bVisible != bVisible)
        {
            mr2.enabled = m_bVisible;
        }
    }

    public void SetVisible(bool bVisible)
    {
        m_bVisible = bVisible;
        MeshRenderer mr2 = gameObject.GetComponent<MeshRenderer>();
        mr2.enabled = bVisible;
        
    }
}

