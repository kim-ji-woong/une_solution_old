using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;

public class ButtonClickListener : MonoBehaviour {

    // Use this for initialization

    //GameObject startButton = null;
    //GameObject endButton = null;


    GameObject soundPlayer;



    bool isFireStarted = false;
    float waitFireSeconds = 2.0f;
    ArrayList smokeList = new ArrayList();
    ArrayList fireList = new ArrayList();

    //Button endButton = null;
    Dictionary<string, Transform> dicXminusTranform = new Dictionary<string, Transform>();
    Dictionary<string, Transform> dicXplusTranform = new Dictionary<string, Transform>();

    Dictionary<Transform, Vector3> dicXplusOrgPosition = new Dictionary<Transform, Vector3>();
    Dictionary<Transform, Vector3> dicXminusOrgPosition = new Dictionary<Transform, Vector3>();
    Transform currentFireManTransform = null;
    Transform currentManTransform = null;

    Dictionary<int, Vector3> manPathVectorDic = new Dictionary<int, Vector3>();
    Dictionary<int, Vector3> firemanPathVectorDic = new Dictionary<int, Vector3>();


    Dictionary<int, Vector3> manPath = new Dictionary<int, Vector3>();
    Dictionary<int, Vector3> firemanPath = new Dictionary<int, Vector3>();
    bool moveTranslatePlacement = false;

    Vector3 orgXminusPosition = new Vector3(0f, 0f, 0f);
    Vector3 orgXplusPosition = new Vector3(0f, 0f, 0f);


    GameObject doorObject = null;

    //Vector3 fireManOrgPosition = new Vector3(0.0891f, 0.0576f, -0.0436f);
    Vector3 fireManOrgPosition = new Vector3(0.3014f, 0.212f, -0.178f);
    //Vector3 manOrgPosition = new Vector3(-0.0956f, 0.0576f, 0.697f);
    Vector3 manOrgPosition = new Vector3(-0.33f, 0.212f, -0.067f);

    Vector3 lastManStartPosition = new Vector3(-0.135f, 0.046f, -0.032f);
    Vector3 lastFireManStartPosition = new Vector3(-0.046f, 0.046f, -0.032f);
    Vector3 lastManPosition = new Vector3(-0.135f, 0.0195f, -0.701f);
    Vector3 lastFireManPosition = new Vector3(-0.046f, 0.0195f, -0.701f);

    Vector3 moveForward1F = new Vector3(1.548f, 0.098f, -0.0212f);


    float movingTime = 5.0f;
    float nextTime = 0.0f;
    float m_Speed = 0.12f;

    public static ButtonClickListener Instance { get; private set; }

    int fireManStep = 1;
    public void OnSelectStart()
    {
        OnSelectStartButton();
    }
    public void OnSelectStop()
    {
        OnSelectEndButton();
    }
    public void LogRelativePath ()
    {
        Debug.Log("Path Start ------- ");

        for(int i= 1;i <= manPath.Count; i++)
        {
            int nextKey = i + 1;
            if (manPath.ContainsKey(nextKey))
            {
                Vector3 towardPos;
                manPath.TryGetValue(nextKey, out towardPos);
                Vector3 currentPos;
                manPath.TryGetValue(i, out currentPos);
                Vector3 movementVector = towardPos - currentPos;
                Debug.Log("movementVector   :   " + movementVector.x + "   " + movementVector.y + "   " + movementVector.z);
            }
            else
            {
                Vector3 currentPos;
                manPath.TryGetValue(i, out currentPos);
                Vector3 movementVector = lastManStartPosition - currentPos;                
                Debug.Log("movementVector   :   " + movementVector.x + "   " + movementVector.y + "   " + movementVector.z);

                Vector3 movementlastVector = lastManPosition - lastManStartPosition;
                Debug.Log("movementlastVector   :   " + movementlastVector.x + "   " + movementlastVector.y + "   " + movementlastVector.z);

            }
        }

        for (int i = 1; i <= firemanPath.Count; i++)
        {
            int nextKey = i + 1;
            if (firemanPath.ContainsKey(nextKey))
            {
                Vector3 towardPos;
                firemanPath.TryGetValue(nextKey, out towardPos);
                Vector3 currentPos;
                firemanPath.TryGetValue(i, out currentPos);
                Vector3 movementVector = towardPos - currentPos;
                Debug.Log("Fireman movementVector   :   " + movementVector.x + "   " + movementVector.y + "   " + movementVector.z);
            }
            else
            {
                Vector3 currentPos;
                firemanPath.TryGetValue(i, out currentPos);
                Vector3 movementVector = lastFireManStartPosition - currentPos;
                Debug.Log("Fireman movementVector   :   " + movementVector.x + "   " + movementVector.y + "   " + movementVector.z);

                Vector3 movementlastVector = lastFireManPosition - lastFireManStartPosition;
                Debug.Log("Fireman movementlastVector:   " + movementlastVector.x + "   " + movementlastVector.y + "   " + movementlastVector.z);

            }
        }
    }


    void Start () {

        soundPlayer = GameObject.Find("SoundPlayer");
        GameObject buildingGroupGameObj = GameObject.Find("BuildingGroup");

        Vector3 deviceposition = Camera.main.transform.position;


        buildingGroupGameObj.transform.position = new Vector3(deviceposition.x, deviceposition.y - 0.3f, deviceposition.z + 0.5f);
        //buildingGroupGameObj.transform.position = Camera.main.transform.up * 0.8f;
        Transform[] allTrans = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform temp in allTrans)
        {
            if (temp.gameObject.tag.Equals("smoke"))
            {
                smokeList.Add(temp.gameObject);
                Debug.Log("Transform Find");
                temp.gameObject.SetActive(false);
            }

            if (temp.gameObject.tag.Equals("Fire"))
            {
                fireList.Add(temp.gameObject);
                Debug.Log("Transform Find");
                temp.gameObject.SetActive(false);
            }

            if (temp.gameObject.tag.Equals("door"))
            {
                doorObject = temp.gameObject;
            }

            if (temp.gameObject.tag.Equals("Xminus"))
            {
                dicXminusTranform.Add(temp.gameObject.name, temp);
                dicXminusOrgPosition.Add(temp, temp.transform.localPosition);
            }
            if (temp.gameObject.tag.Equals("Xplus"))
            {
                dicXplusTranform.Add(temp.gameObject.name, temp);
                dicXplusOrgPosition.Add(temp, temp.transform.localPosition);
            }
            if (temp.gameObject.name.Equals("fireman"))
            {
                currentFireManTransform = temp;
                //fireManOrgPosition = new Vector3(0.0891f, 0.0576f, -0.0436f);
            }
            if (temp.gameObject.name.Equals("man"))
            {
                currentManTransform = temp;
                //manOrgPosition = new Vector3(-0.0956f, 0.0576f, -0.0333f);
            }

        }
               
        //manPath.Add(1, new Vector3(-0.0956f, 0.0576f, 0.697f));
        manPath.Add(1, new Vector3(-0.3294f, 0.2132f, -0.1144f));        //탈출 (시작 위치에서 해당 위치로 처음 움직인다.
        manPath.Add(2, new Vector3(-0.1727f, 0.2132f, -0.1144f));        //문 건너
        manPath.Add(3, new Vector3(-0.0526f, 0.2132f, -0.1144f));       //두번째 방 출구 앞
        manPath.Add(4, new Vector3(0.0769f, 0.2132f, -0.1144f));
        manPath.Add(5, new Vector3(0.1124f, 0.2132f, -0.1751f));
        manPath.Add(6, new Vector3(0.2563f, 0.2132f, -0.1751f));
        //manPath.Add(7, new Vector3(0.0714f, 0.2132f, -0.0511f));


        //firemanPath.Add(1, new Vector3(0.0891f, 0.0576f, -0.0436f));
        firemanPath.Add(1, new Vector3(0.2505f, 0.2132f, -0.1751f));
        firemanPath.Add(2, new Vector3(0.1718f, 0.2132f, -0.1751f));
        firemanPath.Add(3, new Vector3(0.084f, 0.2132f, -0.1144f));
        firemanPath.Add(4, new Vector3(-0.07f, 0.2132f,-0.1144f));      //화재진압 위치
        firemanPath.Add(5, new Vector3(-0.164f, 0.2132f,-0.1144f));      //문열림
        firemanPath.Add(6, new Vector3(-0.2528f, 0.2132f,-0.1144f));        //탈출

        firemanPath.Add(7, new Vector3(-0.127f, 0.2132f, -0.1144f));        //문 건너
        firemanPath.Add(8, new Vector3(0.005f, 0.2132f, -0.1144f));         //두번째 방 출구 앞
        firemanPath.Add(9, new Vector3(0.1448f, 0.2132f, -0.1144f));        //세번째 방 직선포인트
        firemanPath.Add(10, new Vector3(0.1729f, 0.2132f, -0.1751f));       //세번째 방 출구
        firemanPath.Add(11, new Vector3(0.31f, 0.2132f, -0.1751f));       //2층 마지막 포인트
        //firemanPath.Add(12, new Vector3(0.0891f, 0.2132f, -0.0436f));

        currentFireManTransform.localPosition = fireManOrgPosition;
        currentManTransform.localPosition = manOrgPosition;

        /***** test code ****/

        //OnSelectStartButton();
    }

    public void initialize()
    {
        soundPlayer.SendMessage("StopPlay", SendMessageOptions.DontRequireReceiver);
        isFireManStart = false;
        isFireStarted = false;
        moveTranslatePlacement = false;
        nowClearing = false;
        clearFire = false;
        doorObject.SetActive(true);
        doorOpened = false;
        isManStart = false;
        fireCleared = false;
        manStep = 1;
        fireManStep = 1;
        outGoingBuilding = false;

        currentFireManTransform.localPosition = fireManOrgPosition;
        currentManTransform.localPosition = manOrgPosition;


        foreach (GameObject smoke in smokeList)
        {
            smoke.SetActive(false);
        }

        foreach (GameObject fire in fireList)
        {
            fire.SetActive(false);
        }

        foreach (KeyValuePair<Transform, Vector3> keyValuePair in dicXplusOrgPosition)
        {
            Transform trans = keyValuePair.Key;
            trans.localPosition = keyValuePair.Value;
        }

        foreach (KeyValuePair<Transform, Vector3> keyValuePair in dicXminusOrgPosition)
        {
            Transform trans = keyValuePair.Key;
            trans.localPosition = keyValuePair.Value;
        }


        
        beingInit = false;
        //OnSelectStartButton();
        //startButton.SetActive(true);

    }

    bool isFireManStart = false;
    bool clearFire = false;
    bool fireCleared = false;
    bool nowClearing = false;
    float reachDistance = 0.01f;
    float clearTime = 2.0f;
    bool doorOpened = false;
    bool isManStart = false;
    int manStep = 1;
    bool beingInit = false;
    bool outGoingBuilding = false;
    float outGoingTime = 3.0f;

    // Update is called once per frame
    void Update () {

        if (beingInit)
        {
            initialize();            
            return;
        }
        if (isFireStarted)
        {
            //nothing
            if (Time.time >= nextTime)
            {
                nextTime = Time.time + movingTime;
                moveTranslatePlacement = true;
                isFireStarted = false;
            }
        }
            
        if (moveTranslatePlacement)
        {
            if (Time.time < nextTime)
            {
                //placementTranform.position = transform.up * m_Speed;

                //foreach (KeyValuePair<string, Transform> keyValuePair in dicXplusTranform)
                //{
                //    Transform trans = keyValuePair.Value;
                //    trans.Translate(0, 0, Time.deltaTime * m_Speed, Camera.main.transform);
                //}
                foreach (KeyValuePair<string, Transform> keyValuePair in dicXminusTranform)
                {
                    Transform trans = keyValuePair.Value;
                    //trans.Translate(Time.deltaTime * m_Speed, 0, 0, Camera.main.transform);
                    trans.localPosition = Vector3.MoveTowards(trans.localPosition, moveForward1F, Time.deltaTime * m_Speed * 3.5f);

                }
            }
            else
            {              
                moveTranslatePlacement = false;
                isFireManStart = true;
            }
        }
            
        if (clearFire)
        {
            if (Time.time >= nextTime)
            {
                foreach (GameObject smoke in smokeList)
                {
                    //smoke.SetActive(false);       //연기는 나고 있을 수 있으니깐.
                }

                foreach (GameObject fire in fireList)
                {
                    fire.SetActive(false);
                }
                soundPlayer.SendMessage("StopPlay", SendMessageOptions.DontRequireReceiver);
                doorOpened = true;
                doorObject.SetActive(false);
                clearFire = false;
                nowClearing = false;
                fireCleared = true;
            }
        }
           

        if (isFireManStart && !nowClearing)
        {
            if (fireManStep < firemanPath.Count)
            {
                if (fireManStep == 4 && !fireCleared)
                {
                    nowClearing = true;
                    clearFire = true;
                    nextTime = Time.time + clearTime;
                }

                if (fireManStep == 5 )
                {                                      
                    isManStart = true;
                }

                Vector3 toWards;
                firemanPath.TryGetValue(fireManStep + 1, out toWards);
                
                float distance = Vector3.Distance(toWards, currentFireManTransform.localPosition);
                currentFireManTransform.localPosition = Vector3.MoveTowards(currentFireManTransform.localPosition, toWards, Time.deltaTime * m_Speed);
                                             

                if (distance <= reachDistance)
                {
                    fireManStep++;
                }

            }

        }
            
        if (isManStart && fireManStep >= 6)
        {
            if (manStep < manPath.Count)
            {
                Vector3 toWardsMan;
                manPath.TryGetValue(manStep + 1, out toWardsMan);
                float distanceManPath = Vector3.Distance(toWardsMan, currentManTransform.localPosition);
                currentManTransform.localPosition = Vector3.MoveTowards(currentManTransform.localPosition, toWardsMan, Time.deltaTime * m_Speed);

                if (distanceManPath <= reachDistance)
                {
                    manStep++;
                    //if(manStep == manPath.Count)
                    //{
                    //    isFireManStart = false;
                    //}
                }
            }
            else if((manStep >= manPath.Count) && (fireManStep >= firemanPath.Count))
            {
                foreach (KeyValuePair<Transform, Vector3> keyValuePair in dicXplusOrgPosition)
                {
                    Transform trans = keyValuePair.Key;
                    trans.localPosition = keyValuePair.Value;
                }

                foreach (KeyValuePair<Transform, Vector3> keyValuePair in dicXminusOrgPosition)
                {
                    Transform trans = keyValuePair.Key;
                    trans.localPosition = keyValuePair.Value;
                }
                outGoingBuilding = true;
                isManStart = false;
                nextTime = Time.time + outGoingTime;
                currentManTransform.localPosition = lastManStartPosition;
                currentFireManTransform.localPosition = lastFireManStartPosition;
            }

        }
        if (outGoingBuilding)
        {
            if (Time.time >= nextTime)
            {                  
                float distanceManPath = Vector3.Distance(lastManPosition, currentManTransform.localPosition);
                currentManTransform.localPosition = Vector3.MoveTowards(currentManTransform.localPosition, lastManPosition, Time.deltaTime * m_Speed);

                float distanceFireManPath = Vector3.Distance(lastFireManPosition, currentFireManTransform.localPosition);
                currentFireManTransform.localPosition = Vector3.MoveTowards(currentFireManTransform.localPosition, lastFireManPosition, Time.deltaTime * m_Speed);

                if (distanceManPath <= reachDistance)
                {
                    if (distanceFireManPath <= reachDistance)
                    {
                        outGoingBuilding = false;

                        beingInit = true;
                        OnSelectStartButton();
                    }
                }
            }
        }
              
    }
       
   
    
    public void OnSelectStartButton()
    {
        beingInit = true;
        initialize();
        //Debug.Log("OnClickStartButton");

        //var rigidbody = this.gameObject.AddComponent<Rigidbody>();
        //rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        foreach (GameObject smoke in smokeList)
        {
            smoke.SetActive(true);                
        }

        foreach (GameObject fire in fireList)
        {
            fire.SetActive(true);
        }

        isFireStarted = true;
        soundPlayer.SendMessage("StartPlay", SendMessageOptions.DontRequireReceiver);
        beingInit = false;
        //startButton.SetActive(false);
        //endButton.SetActive(true);

        waitFireSeconds = 2.0f;
        nextTime = Time.time + waitFireSeconds;
        
    }

    public void OnSelectEndButton()
    {
        beingInit = true;
        initialize();
        //endButton.SetActive(false);
        //endButton.gameObject.SetActive(false);
        //startButton.gameObject.SetActive(true);
        //endButton.gameObject.SetActive(false);
    }
}
