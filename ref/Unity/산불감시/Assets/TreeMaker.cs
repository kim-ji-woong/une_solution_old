using UnityEngine;
using System.Collections;

public class TreeMaker
{
    private static TreeMaker m_treeMaker = null;
    private static bool m_beginMake = false;

    public static TreeMaker Instance
    {
        get
        {
            if (m_treeMaker == null)
                m_treeMaker = new TreeMaker();

            return m_treeMaker;
        }
    }

    private string[] treeTypeNames = new string[]
    {
        "PalmTree_dual-bended_1sided",
        "PalmTree_dual-bended_2sided",
        "PalmTree_dual_ 1sided",
        "PalmTree_dual_ 2sided",
        "PalmTree_single-bended_1sided",
        "PalmTree_single-bended_2sided",
        "PalmTree_single_1sided",
        "PalmTree_single_2sided",
        "PalmTree_trio_1sided",
        "PalmTree_trio_2sided"//,
        //"sycamore",
        //"Sycamore_SmallMaple"
    };

    public GameObject FirePrefab = null;

    public void MakeRandom(int nTreeCount)
    {
        if (m_beginMake)
            return;

        m_beginMake = true;
        RemoveSampleTreeNGrass();
        HideInvisible();

        GameObject terrainObject = GameObject.Find("Terrain");

        if (terrainObject == null)
            return;

        Terrain terrain = terrainObject.GetComponent<Terrain>();
        Vector3 dimension = terrain.terrainData.size;
        //Debug.Log("Terrain Size : " + dimension.x.ToString() + ", " + dimension.y.ToString() + ", " + dimension.z.ToString());
        //Debug.Log("Terrain Position : " + terrain.transform.position.x.ToString() + ", " + terrain.transform.position.y.ToString() + ", " + terrain.transform.position.z.ToString());

        int xCount, yCount;
        int nCellSize = GetTreeCellSize(nTreeCount * 10, (int)dimension.x, (int)dimension.y, out xCount, out yCount);

        if (nCellSize < 0)
        {
            Debug.Log("Can not make trees...\r\n" + nTreeCount.ToString() + " is too many...");
            return;
        }

        int nCount = xCount * yCount;
        Random.seed = (int)System.DateTime.Now.ToBinary();

        RaycastHit hit;
        Vector3 vDir = new Vector3(0, -1, 0);

        System.Collections.Generic.Dictionary<int, int> dicIndeces = new System.Collections.Generic.Dictionary<int, int>();
        int value, nIndexCount = 0;

        for (int i = 0; i < nCount; i++)
        {
            int nIndex = (int)(Random.value * (nCount - 1));

            if (dicIndeces.TryGetValue(nIndex, out value))
                continue;

            int xIndex = nIndex % xCount;
            int zIndex = nIndex / xCount;

            float x = xIndex > 0 ? (xIndex - 1) * nCellSize + nCellSize * 0.5f : nCellSize * 0.5f;
            float z = zIndex > 0 ? (zIndex - 1) * nCellSize + nCellSize * 0.5f : nCellSize * 0.5f;

            Ray ray = new Ray(new Vector3(x, dimension.y + 100.0f, z), vDir);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Mountain"))
                {
                    int nTreeIndex = (int)((treeTypeNames.Length - 1) * Random.value);
                    //Debug.Log("TreeIndex : " + nTreeIndex.ToString());

                    GameObject prefab = GameObject.Find(treeTypeNames[nTreeIndex]);
                    //Object prefab = Resources.Load(treeTypeNames[nTreeIndex]);
                    //Object prefab = UnityEditor.AssetDatabase.LoadAssetAtPath(szPath + treeTypeNames[nTreeIndex] + ".prefab", typeof(GameObject));

                    if (prefab != null)
                    {
                        GameObject clone = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
                        clone.transform.position = hit.point;
                    }
                    
                    dicIndeces[nIndex] = nIndex;

                    if (++nIndexCount >= nTreeCount)
                        break;
                }
            }
        }
    }

    private void HideInvisible()
    {
        GameObject invisible = GameObject.Find("Invisible");
        Transform[] ts = invisible.transform.GetComponentsInChildren<Transform>();

        if (ts != null)
        {
            foreach (Transform tr in ts)
            {
                if (tr.gameObject != null)
                    tr.gameObject.SetActive(false);

                if (tr.gameObject.name == "fx_fire_g")
                    FirePrefab = tr.gameObject;
            }
        }
    }

    private void RemoveSampleTreeNGrass()
    {
        RemoveObject("Grass");
        RemoveObject("풀");
        RemoveObject("나무");
    }

    private void RemoveObject(string strObjName)
    {
        GameObject obj = GameObject.Find(strObjName);

        if (obj != null)
        {
            GameObject.Destroy(obj);
        }
    }

    // width * height 영역에 nCellSize 만큼의 Cell이 들어가도록 하기 위한 최적의 Cell(정사각형) 크기를 얻어온다.
    private int GetTreeCellSize(int nCellSize, int width, int height, out int xCount, out int yCount)
    {
        xCount = yCount = -1;
        int min = width < height ? width : height;

        int rectMinSize = 1, rectMaxSize = min;
        int sum = -1, result = -1;

        for (int rectSize = rectMinSize; rectSize <= rectMaxSize; rectSize++)
        {
            int x = width / rectSize;
            int y = height / rectSize;
            int nCellCount = x * y;

            if (nCellCount >= nCellSize)
            {
                if (sum < 0 || sum > nCellCount)
                {
                    xCount = x;
                    yCount = y;
                    sum = nCellCount;
                    result = rectSize;
                }
            }
        }

        return result;
    }
}
