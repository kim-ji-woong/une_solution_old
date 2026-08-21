using UnityEngine;
using System.Collections;

public class TextPOI : MonoBehaviour 
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
    
    public void SetColor(Color color)
    {
        MeshRenderer render = gameObject.GetComponent<MeshRenderer>();
        if (render != null)
        {
            render.material.color = color;
        }
    }

    public bool ReversLOD = false;

    private string m_strSceneName = "";
    public string SceneName
    {
        get { return m_strSceneName; }
        set { m_strSceneName = value; }
    }

	void Start ()
    {
    }
	
	void Update () 
    {
        MainModel mainModel = ModelManager.Instance.Model;

        if (mainModel == null)
        {
            Hide();
            return;
        }

        Assets.UnityScene scene = mainModel.CurrentScene;

        if (scene == null)
        {
            Hide();
            return;
        }

        // 현재 선택된 층의 Text만 나오도록 한다..
        if (scene.SceneName != m_strSceneName)
        {
            Hide();
            return;
        }

        if( m_bVisible == true)
        {  
            Vector3 dir = Camera.main.transform.position - gameObject.transform.position;
            float distance = dir.magnitude;
            if (ReversLOD == true)
            {
                if (distance > ModelManager.Instance.TextLODDistance)
                {
                    MeshRenderer mr2 = gameObject.GetComponent<MeshRenderer>();
                    mr2.enabled = true;

                    if (ModelManager.Instance.FixTextRatio == true)
                    {
                        float ratio = distance / ModelManager.Instance.DistanceRatioText;
                        gameObject.transform.localScale = new Vector3(ratio, ratio, ratio);
                    }
                    Ray ray1 = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
                    Vector3 heading = Camera.main.transform.position + (ray1.direction * 1000000000.0f);
                    gameObject.transform.LookAt(heading);
                }
                else
                {
                    MeshRenderer mr2 = gameObject.GetComponent<MeshRenderer>();
                    mr2.enabled = false;
                }
            }
            else
            {
                if (distance <= ModelManager.Instance.TextLODDistance)
                {
                    MeshRenderer mr2 = gameObject.GetComponent<MeshRenderer>();
                    mr2.enabled = true;

                    if (ModelManager.Instance.FixTextRatio == true)
                    {
                        float ratio = distance / ModelManager.Instance.DistanceRatioText;
                        gameObject.transform.localScale = new Vector3(ratio, ratio, ratio);
                    }
                    Ray ray1 = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
                    Vector3 heading = Camera.main.transform.position + (ray1.direction * 1000000000.0f);
                    gameObject.transform.LookAt(heading);
                }
                else
                {
                    MeshRenderer mr2 = gameObject.GetComponent<MeshRenderer>();
                    mr2.enabled = false;
                }
            }

            if (mainModel.RotatePOI)
                gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y + 90, gameObject.transform.eulerAngles.z);
        }        
	}

    public void SetVisible(bool bVisible)
    {
        MeshRenderer mr2 = gameObject.GetComponent<MeshRenderer>();
        mr2.enabled = bVisible;
        m_bVisible = bVisible;
    }	

    private void Hide()
    {
        MeshRenderer mr2 = gameObject.GetComponent<MeshRenderer>();
        mr2.enabled = false;
    }
}
