using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Resources.Scripts;
using System.IO;

public class MainBehaviour : MonoBehaviour
{
    [SerializeField]
    GameObject baseWallObject = null;
    [SerializeField]
    GameObject baseFloorObject = null;
    [SerializeField]
    GameObject baseDoorObject = null;
    [SerializeField]
    GameObject baseWindowObject = null;
    [SerializeField]
    GameObject baseCurtainWallObject = null;
    [SerializeField]
    GameObject mobileUiCanvas = null;
    [SerializeField]
    GameObject floorButton = null;
    [SerializeField]
    GameObject mainCamera = null;
    [SerializeField]
    GameObject navigationPath = null;
    [SerializeField]
    GameObject fire = null;
    [SerializeField]
    GameObject plane = null;
    [SerializeField]
    List<Material> navigationModeMaterialList = new List<Material>();
    [SerializeField]
    WebInterfaceBehaviour webInterfaceBehaviour = null;
    [SerializeField]
    AndroidInterfaceBehaviour androidInterfaceBehaviour = null;
    //[SerializeField]
    //List<>

    List<GameObject> originalModelList = new List<GameObject>();
    List<Material> originalModelMaterialList = new List<Material>();

    Dictionary<GameObject, List<GameObject>> modelListMap = new Dictionary<GameObject, List<GameObject>>();

    bool isFinishModelLoading = false;

    List<PoiBehaviour> poiList = new List<PoiBehaviour>();

    GameObject Waypoint = null;
    GameObject manObject = null;

    private int b1index = 0;

    // Use this for initialization
    void Start()
    {

        if (Application.platform == RuntimePlatform.Android)
        {
            mobileUiCanvas.SetActive(true);
        }
        else
            mobileUiCanvas.SetActive(false);

        originalModelList.Add(baseWallObject);
        originalModelList.Add(baseFloorObject);
        originalModelList.Add(baseDoorObject);
        originalModelList.Add(baseWindowObject);
        originalModelList.Add(baseCurtainWallObject);

        foreach (GameObject obj in originalModelList)
        {
            modelListMap.Add(obj, new List<GameObject>());
        }

        foreach (GameObject originalModelObject in originalModelList)
        {
            originalModelMaterialList.Add(originalModelObject.GetComponent<MeshRenderer>().material);
        }


        //GameObject wallObject = GameObject.Instantiate(baseBuildingObject);
        //wallObject.name = "wall";        

        //GameObject floorObject = GameObject.Instantiate(baseBuildingObject);
        //floorObject.name = "floor";


        //GameObject doorObject = GameObject.Instantiate(baseDoorObject);
        //doorObject.name = "door";

        //if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WebGLPlayer)
        //{
        string dataPath = Application.persistentDataPath;

        string dataRootFolder = "Data5";

        TextAsset text = Resources.Load(dataRootFolder+@"\faceInfo") as TextAsset;
        modelListMap[baseWallObject] = Util.createModelByLevel(baseWallObject, text.text, false);

        text = Resources.Load(dataRootFolder+@"\floorinfo") as TextAsset;
        modelListMap[baseFloorObject] = Util.createModelByLevel(baseFloorObject, text.text, false);

        text = Resources.Load(dataRootFolder+@"\doorInfo") as TextAsset;
        modelListMap[baseDoorObject] = Util.createModelByLevel(baseDoorObject, text.text);

        text = Resources.Load(dataRootFolder+@"\windowInfo") as TextAsset;
        modelListMap[baseWindowObject] = Util.createModelByLevel(baseWindowObject, text.text);

        text = Resources.Load(dataRootFolder+@"\curtainWallInfo") as TextAsset;
        modelListMap[baseCurtainWallObject] = Util.createModelByLevel(baseCurtainWallObject, text.text);

        isFinishModelLoading = true;
        //////////////////////////////////////////////////////////////////////////////////////////////
        //string textData = File.ReadAllText(@"C:/Users/Public/Documents/faceinfo.txt");
        //modelListMap[baseWallObject] = Util.createModelByLevel(baseWallObject, textData, false);

        //textData = File.ReadAllText(@"C:/Users/Public/Documents/floorinfo.txt");
        //modelListMap[baseFloorObject] = Util.createModelByLevel(baseFloorObject, textData, false);

        //textData = File.ReadAllText(@"C:/Users/Public/Documents/doorInfo.txt");
        //modelListMap[baseDoorObject] = Util.createModelByLevel(baseDoorObject, textData);

        //textData = File.ReadAllText(@"C:/Users/Public/Documents/windowInfo.txt");
        //modelListMap[baseWindowObject] = Util.createModelByLevel(baseWindowObject, textData);

        //textData = File.ReadAllText(@"C:/Users/Public/Documents/curtainWallInfo.txt");
        //modelListMap[baseCurtainWallObject] = Util.createModelByLevel(baseCurtainWallObject, textData);

        Bounds totalBounds = new Bounds();
        bool isFirst = true;

        foreach (GameObject model in modelListMap[baseWallObject])
        {
            Bounds modelBounds = model.GetComponent<MeshFilter>().mesh.bounds;
            if (isFirst)
                totalBounds = modelBounds;
            else
                totalBounds.Encapsulate(modelBounds);
        }

        //mainCamera.transform.position = totalBounds.center + new Vector3(100,100,100);
        
        mainCamera.transform.position = new Vector3(433.8507f, 225.873f, 267.07f);       //by hypark : 화면 줌아웃 하기 위해. 위치 변경. 카메라. 
        mainCamera.transform.rotation = Quaternion.Euler(new Vector3(52.2f, -151.85f, 0f));

        GameObject.Find("NavigationPositionGroup").transform.position = totalBounds.center;

        //plane.transform.position = new Vector3(totalBounds.center.x, totalBounds.min.y-0.1f, totalBounds.center.z);
        plane.transform.position = new Vector3(totalBounds.center.x, -31, totalBounds.center.z);

        fire.transform.position = new Vector3(fire.transform.position.x + totalBounds.center.x, fire.transform.position.y,
            fire.transform.position.z + totalBounds.center.z);

        CreateLevelButtons();
        MakeTest();     //시연용으로 static한 부분을 처리.

    }

    void MakeTest()
    {
        GameObject b1f_object = GameObject.Find("B1F BIMS plan");

        manObject = GameObject.Find("man");
        manObject.transform.parent = b1f_object.transform;

        Waypoint = GameObject.FindGameObjectWithTag("waypoint");
        Waypoint.transform.parent = b1f_object.transform;
        Waypoint.SetActive(false);

        fire.transform.parent = b1f_object.transform;
        fire.transform.position = new Vector3(297.6f, -11.5f, 105.1f);

        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(297.6f, -4.8f, 92.6f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(297.6f, -4.8f, 118.3f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(375.7f, -4.8f, 113.1f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(324.1f, -4.8f, 92.6f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(359.3f, -4.8f, 113.1f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(394f, -4.8f, 113.1f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(376.6f, -4.8f, 102.1f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(326.8f, -4.8f, 114.6f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(374.8f, -4.8f, 145.5f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(415.4f, -4.8f, 145.5f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(419.9f, -4.8f, 159.2f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(410.3f, -4.8f, 167.2f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(420.2f, -4.8f, 172f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(420.2f, -4.8f, 182.8f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(420.6f, -4.8f, 193.9f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(419.1f, -4.8f, 204.4f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(419.1f, -4.8f, 215.8f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(419.1f, -4.8f, 226.5f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(419.1f, -4.8f, 237.1f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(419.1f, -4.8f, 248.3f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(419.1f, -4.8f, 259.2f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(419.1f, -4.8f, 269.9f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(419.1f, -4.8f, 280.6f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(419.1f, -4.8f, 291.5f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(419.1f, -4.8f, 302f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(375.7f, -4.8f, 88.6f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(353f, -4.8f, 88.6f));
        androidInterfaceBehaviour.CreateFireDetectorPoi(b1f_object, new Vector3(396.7f, -4.8f, 93.1f));

        androidInterfaceBehaviour.SetFireDetectPoiVisibility(0, true);
        androidInterfaceBehaviour.SetFireDetectPoiVisibility(1, true);
        androidInterfaceBehaviour.SetFireDetectPoiVisibility(2, true);



    }


    // Update is called once per frame
    void Update()
    {
        if(isFinishModelLoading)
        {
            webInterfaceBehaviour.OnFinishModelLoading();

            isFinishModelLoading = false;
        }
    }

    float floorAnimationElapsedTime = 0.0f;
    float floorAnimationSpeed = 0.04f;
    Vector3 floorAnimationVelocity = new Vector3();
    float floorAnimationSmooth = 100.0f;

    void FixedUpdate()
    {
        if (isBuildingSelectionAnimation)
        {
            GameObject[] levelObjects = GameObject.FindGameObjectsWithTag("Level");

            int counter = 0;

            bool isEnd = false;
            floorAnimationElapsedTime += Time.deltaTime * floorAnimationSpeed;

            foreach (GameObject levelObject in levelObjects)
            {
                levelObject.transform.position = Vector3.Lerp(levelObject.transform.position, floorTargetPosition[counter], floorAnimationElapsedTime);


                if (floorAnimationElapsedTime >= 1.0f)
                {
                    isEnd = true;
                }

                counter++;
            }

            if (isEnd)
            {
                isBuildingSelectionAnimation = false;
                floorAnimationVelocity = Vector3.zero;
                floorAnimationElapsedTime = 0.0f;
            }                
        }
    }


    private void CreateLevelButtons()
    {
        GameObject[] levelObjects = GameObject.FindGameObjectsWithTag("Level");

        //create all button
        _createLevelButton(-1, "All");

        int counter = 0;

        foreach (GameObject levelObject in levelObjects)
        {
            _createLevelButton(counter, levelObject.name);

            counter++;
        }
    }

    private void _createLevelButton(int index, string buttonText)
    {
        GameObject floorButtonClone = GameObject.Instantiate(floorButton);

        floorButtonClone.transform.GetChild(0).gameObject.GetComponent<Text>().text = buttonText;


        if(buttonText.Equals("B1F BIMS plan"))      //버튼
        {
            b1index = index;
        }


        floorButtonClone.transform.position = new Vector3(floorButton.transform.position.x,
            floorButton.transform.position.y + index * 60, floorButton.transform.position.z);

        floorButtonClone.transform.localScale = new Vector3(0.8f, 1.42f, 1.11f);

        floorButtonClone.transform.parent = floorButton.transform.parent;

        floorButtonClone.SetActive(true);

        floorButtonClone.GetComponent<Button>().onClick.AddListener(delegate { OnBuildingLevelButtonClick(index); });
    }

    int selectedFloorIndex = 0;
    bool isBuildingSelectionAnimation = false;
    List<Vector3> floorOriginalPosition = new List<Vector3>();
    List<Vector3> floorTargetPosition = new List<Vector3>();


     


    public void OnBuildingLevelButtonClick(int buttonIndex)
    {
        //toggle floor
        selectedFloorIndex = buttonIndex;
        isBuildingSelectionAnimation = true;

        //floorOriginalPosition.Clear();

        GameObject[] levelObjects = GameObject.FindGameObjectsWithTag("Level");

        int counter = 0;
        float farDistance = 1000.0f;
        floorTargetPosition.Clear();

        foreach (GameObject levelObject in levelObjects)
        {
            if(floorOriginalPosition.Count < levelObjects.Length)
                floorOriginalPosition.Add(levelObject.transform.position);

            if(counter == selectedFloorIndex || -1 == selectedFloorIndex)
            {
                floorTargetPosition.Add(new Vector3(floorOriginalPosition[counter].x,
                    floorOriginalPosition[counter].y, floorOriginalPosition[counter].z));
            }
            else
            {
                if (counter % 2 == 0)
                {
                    floorTargetPosition.Add(new Vector3(floorOriginalPosition[counter].x + farDistance,
                        floorOriginalPosition[counter].y, floorOriginalPosition[counter].z));
                }
                else
                    floorTargetPosition.Add(new Vector3(floorOriginalPosition[counter].x - farDistance,
                        floorOriginalPosition[counter].y, floorOriginalPosition[counter].z));
            }            

            counter++;
        }
    }

    public void Cascade(int value)
    {
        GameObject[] levelObjects = GameObject.FindGameObjectsWithTag("Level");

        int counter = 0;

        foreach (GameObject levelObject in levelObjects)
        {
            levelObject.transform.position = new Vector3(levelObject.transform.position.x - counter * 15.0f,
                levelObject.transform.position.y + counter * 5, levelObject.transform.position.z - counter * 15.0f);

            counter++;
        }
    }

    bool isNavigationMode = false;

    public void ToggleNavigationMode()
    {
        if (navigationPath.GetComponent<NavigationPathBehaviour>().IsLineAnimation)
        {
            return;
        }

        isNavigationMode = !isNavigationMode;
        androidInterfaceBehaviour.ActivateFireDetectPoi(0, isNavigationMode);
        SetNavigationMode(isNavigationMode);
    }

    /***** temp code by hypark : 시연용 2018.12.06 *****/
    private bool isActiveEscapePath = false;
    public void TogglePathMode()
    {
        
        isActiveEscapePath = !isActiveEscapePath;
        if (isActiveEscapePath) Waypoint.SetActive(true);
        else Waypoint.SetActive(false);

    }

    /***  지하 1층 뷰를 바로 보여주기 위해 임시로 넣은 함수 ***/
    private void Temp_ViewB1F()
    {
        mainCamera.transform.position = new Vector3(402.3226f, 146.329f, 231.77f);       //by hypark : 화면 줌아웃 하기 위해. 위치 변경. 카메라. 
        mainCamera.transform.rotation = Quaternion.Euler(new Vector3(49.65f, -154.5f, 0f));
        OnBuildingLevelButtonClick(b1index);

    }


    private void SetNavigationMode(bool navigationMode)
    {
        if (navigationMode)
        {
            if (5 != navigationModeMaterialList.Count || 5 != originalModelList.Count)
                return;

            Temp_ViewB1F();
            fire.SetActive(true);
            mainCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            plane.SetActive(false);

            //change material 
            if(isXrayOn())
                changeModelMaterialByType(navigationModeMaterialList);

            //navigationPath.GetComponent<NavigationPathBehaviour>().StartAnimation();
            //navigationPath.SetActive(true);
        }
        else
        {
            //일반 모드
            //navigationPath.GetComponent<NavigationPathBehaviour>().TerminateAnimation();


            //navigationPath.SetActive(false);
            fire.SetActive(false);
            plane.SetActive(true);
            mainCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
            navigationPath.GetComponent<VolumetricLines.VolumetricMultiLineBehavior>().LineWidth = 0.0f;
            //restore material 
            if (isXrayOn())
                changeModelMaterialByType(originalModelMaterialList);
        }
    }

    bool isXrayOn()
    {
        return GameObject.Find("X-Ray Mode").GetComponent<Toggle>().isOn;
    }

    private void changeModelMaterialByType(List<Material> modelMaterialList)
    {
        int counter = 0;
        //originalModelMaterialList.Clear();

        foreach (GameObject originalModelObject in originalModelList)
        {
            List<GameObject> modelList = modelListMap[originalModelObject];

            foreach (GameObject model in modelList)
            {
                model.GetComponent<MeshRenderer>().material = modelMaterialList[counter];
            }

            counter++;
        }
    }

    public void GotoCamera(GameObject target)
    {
        
        mainCamera.GetComponent<CameraMovement>().GoTo(target.transform.position, GameObject.Find("NavigationPositionGroup").transform.position);
    }

    
}
