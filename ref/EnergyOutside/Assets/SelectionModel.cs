using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using System.IO;
using System.Text;
using System.Reflection;
using UnityEngine.UI;
//using UnityEditorInternal;

public class SelectionModel : MonoBehaviour
{
    public Collider meshCollider;

    public Bounds boxCollider;
    
    private TextMesh mNameText;

    private GameObject mTextOwner;

    private bool m_bSelect = false;

    private void AddPythonFunction()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null)
        {
        }
    }

    public void ClearSelect()
    {
        m_bSelect = false;
    }

    private void LeaveObject()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null && this.gameObject != null)
        {
            MeshFilter mf = this.gameObject.GetComponent<MeshFilter>();
            if (mf != null)
            //Transform t = gameObject.transform.parent;
            //if (t != null)
            {
                string szMsg = string.Format("LeaveObject(\"{0}\")", mf.name);
                //Debug.logger.Log(szMsg + " : Shared Mesh :" + mf.mesh.name);
                if (PassivePipeProxy.Instance != null)
                    PassivePipeProxy.Instance.SendServer(szMsg);
            }
        }
    }

    private void EnterObject()
    {
        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null && this.gameObject != null)
        {
            MeshFilter mf = this.gameObject.GetComponent<MeshFilter>();
            //Transform t = gameObject.transform.parent;
            if (mf != null)
            {
                string szMsg = string.Format("EnterObject(\"{0}\")", mf.name);
                //Debug.logger.Log(szMsg + " : Shared Mesh :" + mf.mesh.name);
                if(PassivePipeProxy.Instance != null)
                    PassivePipeProxy.Instance.SendServer(szMsg);
            }
        }
    }

    public void SelectObject()
    {
        if (m_bSelect == true)
            return;

        m_bSelect = true;

        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null && this.gameObject != null)
        {
            MeshFilter mf = this.gameObject.GetComponent<MeshFilter>();
            if(mf!= null)
            //Transform t = gameObject.transform.parent;
            //if (t != null)
            {                
                string szMsg = string.Format("SelectObject(\"{0}\")", mf.name);
                //Debug.logger.Log(szMsg + " : Shared Mesh :" + mf.mesh.name);
                if (PassivePipeProxy.Instance != null)
                    PassivePipeProxy.Instance.SendServer(szMsg);
            }
        }
    }

    public void UnselectObject()
    {
        if (m_bSelect == false)
            return;
        m_bSelect = false;

        PythonProxy proxy = PythonProxy.Instance;
        if (proxy != null && this.gameObject != null)
        {
            MeshFilter mf = this.gameObject.GetComponent<MeshFilter>();
            if (mf != null)
            //Transform t = gameObject.transform.parent;
            //if (t != null)
            {
                string szMsg = string.Format("UnSelectObject(\"{0}\")", mf.name);
                //Debug.logger.Log(szMsg);
                if(PassivePipeProxy.Instance!= null)
                    PassivePipeProxy.Instance.SendServer(szMsg);
            }
        }
    }

    private void Awake()
    {
        string szTextMeshName = "";
        MeshFilter mf = this.gameObject.GetComponent<MeshFilter>();

        if (mf != null)
        {
            szTextMeshName = mf.name + "_NameText";
        }

        //if (!gameObject.name.StartsWith("z"))
        //{
        //    if(!gameObject.name.StartsWith("V"))
        //    {
        //        if (gameObject.name == "GGGINFO")
        //        {
        //            int k = 0;
        //            k++;
        //        }
        //        MeshRenderer renderMesh = GetComponent<MeshRenderer>();
        //        if (renderMesh != null)
        //        {
        //            renderMesh.material = ModelManager.Instance.Model.grayMaterial;
        //            Material[] mats = renderMesh.materials;
        //            if (mats != null)
        //            {
        //                for (int i = 0; i < mats.Length; i++)
        //                {
        //                    renderMesh.materials[i] = ModelManager.Instance.Model.grayMaterial;
        //                }
        //            }
        //        }
                
        //    }
            
        //}

        mTextOwner = new GameObject(szTextMeshName);
        //mTextOwner.transform.parent = gameObject.transform;
        mTextOwner.layer = 8;

        
        mNameText = mTextOwner.AddComponent<TextMesh>();
        MeshRenderer render = mTextOwner.GetComponent<MeshRenderer>();

        Material mat = ModelManager.Instance.TextMaterial;
        mat.color = ModelManager.Instance.BuildingNameColor;

        render.material = mat;
        render.materials[0] = mat;

        mNameText.font = ModelManager.Instance.m_TextFont;
        mNameText.anchor = TextAnchor.LowerCenter;
        mNameText.characterSize = 1f;        
        mNameText.fontStyle = FontStyle.Normal;
        mNameText.color = ModelManager.Instance.BuildingNameColor;
                
        if( mf != null)
        {
            string szAliasName = ModelManager.Instance.GetAliasName(mf.name);
            if (szAliasName != "")
                mNameText.text = szAliasName;
        }
        else
        {
            mNameText.text = "";
        }

        render.enabled = false;
       
    }

    public void SetTextColor(Color color)
    {
        MeshRenderer render = mTextOwner.GetComponent<MeshRenderer>();
        if (render != null)
        {
            render.material.color = color;
        }        
        if(mNameText != null)
        {
            mNameText.color = color;
        }
    }

    public void UpdateAliasName()
    {
        MeshFilter mf = this.gameObject.GetComponent<MeshFilter>();
        if (mf != null)
        {           
            string szAliasName = ModelManager.Instance.GetAliasName(mf.name);
            mNameText.text = szAliasName;
        }
        else
        {
            mNameText.text = "";
        }
    }

    void Start()
    {
        AddPythonFunction();
        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        if(mr != null)
        {
            Color mat = mr.material.color;
            originalColor = mat;

            rend = mr;
            material1 = rend.material;
            material2 = ModelManager.Instance.HighlightMaterial;

            orgMats = new Material[rend.materials.Length];
            for (int i = 0; i < orgMats.Length; i++)
            {
                orgMats[i] = rend.materials[i];
            }
            highlightMats = new Material[rend.materials.Length];
            for (int i = 0; i < highlightMats.Length; i++)
            {
                highlightMats[i] = material2;
            }


            BoxCollider temp = gameObject.AddComponent<BoxCollider>();
            boxCollider = temp.bounds;
            temp.enabled = false;

            //int[] nLayers = GetSortingLayerUniqueIDs();
            //if (nLayers != null && nLayers.Length > 1)
                mr.sortingLayerName = "Model";// = 366000623;
            mr.sortingOrder = 100;
        }      

    }

    //public int[] GetSortingLayerUniqueIDs()
    //{
    //    Type internalEditorUtilityType = typeof(InternalEditorUtility);
    //    PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
    //    return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
    //}

    //public string[] GetSortingLayerNames()
    //{
    //    Type internalEditorUtilityType = typeof(InternalEditorUtility);
    //    PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
    //    return (string[])sortingLayersProperty.GetValue(null, new object[0]);
    //}


    private Material[] orgMats;
    private Material[] highlightMats;

    public Material material1;
    public Material material2;
    public float duration = 2.0F;
    public Renderer rend;

    private bool m_bShowText = true;
    public bool ShowText
    {
        get { return m_bShowText; }
        set { m_bShowText = value; }
    }

    void Update()
    {
        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            if (m_bSelect == true)
            {
                rend.materials = highlightMats;
                rend.material = material2;
            }
            else
            {
                rend.materials = orgMats;
                rend.material = material1;
            } 
        }

        if (boxCollider != null)
        {
            Bounds bound = boxCollider;
            Vector3 max = bound.max;
            Vector3 min = bound.min;
            Vector3 textPos2 = new Vector3((max.x + min.x) / 2.0f, max.y, (max.z + min.z) / 2.0f);

            Vector3 dir = Camera.main.transform.position - textPos2;
            float distance = dir.magnitude;
            if (distance < ModelManager.Instance.TextLODDistance)
            {
                MeshRenderer mr2 = mTextOwner.GetComponent<MeshRenderer>();
                if (mr2 != null )
                {
                    if (m_bShowText == true)
                    {
                        mr2.enabled = true;
                    }
                    else
                    {
                        mr2.enabled = false;
                    }

                    if (ModelManager.Instance.FixTextRatio == true)
                    {
                        float ratio = distance / ModelManager.Instance.DistanceRatioText;
                        mNameText.transform.localScale = new Vector3(ratio, ratio, ratio);
                    }

                    Ray ray1 = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));                    
                    Vector3 heading = Camera.main.transform.position + (ray1.direction * 1000000000.0f);
                    mNameText.transform.LookAt(heading);
                                                     
                    mNameText.transform.position = textPos2;
                    

                }

            }
            else
            {
                MeshRenderer mr2 = mTextOwner.GetComponent<MeshRenderer>();
                if (mr2 != null)
                {
                    mr2.enabled = false;
                }
            }
            //MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
            //mr.material.s
        }  

    }   

    void OnDrawGizmos()
    {
        //if( m_bSelect == true)
        //{
        //    Gizmos.color = Color.red;
        //    MeshFilter mf = this.gameObject.GetComponent<MeshFilter>();
        //    if (mf != null)
        //    {
        //        Gizmos.DrawMesh(mf.mesh, transform.position, transform.rotation, transform.parent.localScale);
        //    }                
        //}
    }

    void OnGUI()
    {
              
    }

    bool colorItRed = true;
    bool colorItBlack = false;
    bool colorItBlue = false;
    bool colorItGreen = false;
    bool colorItYellow = false;
    bool colorItWhite = false;
    bool alpha = false;
    float highlightMultiply = 1.5f;//Good at 1.5 
    float alphaMultiply = 0.5f;//0.0 to 1.0

    Color originalColor;

    void OnMouseEnter()
    {
        float x = Input.mousePosition.x;
        float y = (Screen.height - Input.mousePosition.y);
        if((x > 0 && y > 0 )&&(x < Screen.width && y < Screen.height ))
        {
            EnterObject();
        }       
    }

    //void OnMouseOver()
    //{
    //    //float x = Input.mousePosition.x;
    //    //float y = (Screen.height - Input.mousePosition.y);
    //    //if ((x > 0 && y > 0) && (x < Screen.width && y < Screen.height))
    //    //{
    //    //    EnterObject();
    //    //}  
    //}

    void OnMouseExit()
    {
        float x = Input.mousePosition.x;
        float y = (Screen.height - Input.mousePosition.y);
        if ((x > 0 && y > 0) && (x < Screen.width && y < Screen.height))
        {
            LeaveObject();
        }  
    }

    void OnMouseUp()
    {
        //if (meshCollider != null)
        //{
        //    if (Input.GetMouseButtonUp(2))
        //    {
        //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //        RaycastHit hit;
        //        if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
        //        {
        //            if( m_bSelect == false)
        //            {

                       
        //                SelectObject();
        //            }
        //            else
        //            {
        //                UnselectObject();
        //            }
        //        }
        //        else
        //        {
        //            UnselectObject();
        //        }
        //    }
        //    else
        //    {
        //        UnselectObject();
        //    }
        //}
        //else
        //{
        //    UnselectObject();
        //}
    }

 

}
