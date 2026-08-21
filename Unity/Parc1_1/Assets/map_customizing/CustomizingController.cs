using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum CustomizingStage
{
    None,
    Spawn,
    Mode_Move,
    Mode_Scale,
    Mode_Rotate
}
public enum TransformKind
{
    Move_X,
    Move_Z,
    Move_Both,
    Scale,
    Rotate
}

public class CustomizingController : MonoBehaviour
{
    public GameObject wallPref;
    public InputField scaleIF;
    public InputField rotateIF;

    private CustomizingStage curStage = CustomizingStage.None;
    private Vector3 floorHit;
    private bool isHitFloor = false;
    private Vector3 firstFHit;
    private Vector3 firstPos;
    private Vector3 rotCenterPos;
    private float firstRot;
    private Wall curSpawnWall = null;
    private Wall curDetectWall = null;
    private Wall curSelectWall = null;
    private Mark curDetectMark = null;
    private Mark curSelectMark = null;
    private Dictionary<string, List<GameObject>> walls = new Dictionary<string, List<GameObject>>();
    private MainModel modelController;

    // Start is called before the first frame update
    void Start()
    {
        modelController = FindObjectOfType<MainModel>();
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Detect();
        UpdateInspector();

        switch (curStage)
        {
            case CustomizingStage.None:

                if (Input.GetMouseButtonDown(0))
                {
                    if (curDetectWall)
                    {
                        curSelectWall = curDetectWall;

                        curSelectWall.Select(CustomizingStage.Mode_Move);

                        curStage = CustomizingStage.Mode_Move;

                        SnapController.Init(walls[modelController.CurrentScene.SceneName], 1.0f);
                    }
                }

                break;
            case CustomizingStage.Spawn:

                curSpawnWall.transform.position = floorHit;

                if (Input.GetMouseButtonDown(0))
                {
                    if (isHitFloor)
                    {
                        if (!walls.ContainsKey(modelController.CurrentScene.SceneName))
                            walls.Add(modelController.CurrentScene.SceneName, new List<GameObject>());
                        walls[modelController.CurrentScene.SceneName].Add(curSpawnWall.gameObject);

                        curSpawnWall = null;
                        curStage = CustomizingStage.None;
                    }
                }

                break;
            case CustomizingStage.Mode_Move:

                if (Input.GetMouseButtonDown(0))
                {
                    if (!curDetectWall)
                    {
                        curStage = CustomizingStage.None;
                        curSelectWall.Select(CustomizingStage.None);
                        curSelectWall = null;
                        break;
                    }
                    else if (curDetectWall != curSelectWall)
                    {
                        curSelectWall.Select(CustomizingStage.None);
                        curSelectWall = curDetectWall;
                        curSelectWall.Select(curStage);

                        break;
                    }

                    if (curDetectMark)
                    {

                        firstFHit = floorHit;
                        firstPos = curSelectWall.transform.position;
                        curSelectMark = curDetectMark;
                        curSelectMark.Select(true);
                    }
                }
                else if (Input.GetMouseButton(0) && curSelectMark)
                {
                    switch (curSelectMark.kind)
                    {
                        case TransformKind.Move_X:
                            {
                                Vector3 newPos = firstPos + curSelectWall.transform.right * Vector3.Dot(curSelectWall.transform.right, floorHit - firstFHit);
                                newPos.x = SnapController.XPos(newPos, curSelectWall.GetComponent<SnapObj>());

                                curSelectWall.transform.position = newPos;
                            }
                            break;
                        case TransformKind.Move_Z:
                            curSelectWall.transform.position = firstPos + curSelectWall.transform.forward * Vector3.Dot(curSelectWall.transform.forward, floorHit - firstFHit);
                            break;
                        case TransformKind.Move_Both:
                            curSelectWall.transform.position = firstPos + floorHit - firstFHit;

                            break;
                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (curSelectMark)
                    {
                        curSelectMark.Select(false);
                        curSelectMark = null;
                    }
                }

                break;
            case CustomizingStage.Mode_Scale:

                if (Input.GetMouseButtonDown(0))
                {
                    if (!curDetectWall)
                    {
                        curStage = CustomizingStage.None;
                        curSelectWall.Select(CustomizingStage.None);
                        curSelectWall = null;
                        break;
                    }
                    else if (curDetectWall != curSelectWall)
                    {
                        curSelectWall.Select(CustomizingStage.None);
                        curSelectWall = curDetectWall;
                        curSelectWall.Select(curStage);

                        break;
                    }

                    if (curDetectMark)
                    {
                        firstFHit = floorHit;
                        firstPos = curDetectMark.transform.position;
                        curSelectMark = curDetectMark;
                        curSelectMark.Select(true);
                    }
                }
                else if (Input.GetMouseButton(0) && curSelectMark)
                {
                    curSelectMark.transform.position = firstPos + curSelectMark.transform.forward * Vector3.Dot(curSelectMark.transform.forward, floorHit - firstFHit);
                    curSelectWall.UpdateScale();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (curSelectMark)
                    {
                        curSelectMark.Select(false);
                        curSelectMark = null;
                    }
                }

                break;
            case CustomizingStage.Mode_Rotate:

                if (Input.GetMouseButtonDown(0))
                {
                    if (!curDetectWall)
                    {
                        curStage = CustomizingStage.None;
                        curSelectWall.Select(CustomizingStage.None);
                        curSelectWall = null;

                        scaleIF.text = "";
                        rotateIF.text = "";
                        break;
                    }
                    else if (curDetectWall != curSelectWall)
                    {
                        curSelectWall.Select(CustomizingStage.None);
                        curSelectWall = curDetectWall;
                        curSelectWall.Select(curStage);

                        break;
                    }

                    if (curDetectMark)
                    {
                        isHitFloor = modelController.CurrentScene.DetectFloor(curDetectMark.transform.position, (curDetectMark.transform.position - Camera.main.transform.position).normalized, out rotCenterPos);

                        firstFHit = floorHit;
                        firstRot = Vector3.SignedAngle(Vector3.right, floorHit - rotCenterPos, Vector3.up);
                        curSelectMark = curDetectMark;
                        curSelectMark.Select(true);
                    }
                }
                else if (Input.GetMouseButton(0) && curSelectMark)
                {
                    float deltaRot = Vector3.SignedAngle(Vector3.right, floorHit - rotCenterPos, Vector3.up) - firstRot;
                    curSelectWall.transform.rotation = Quaternion.Euler(0, deltaRot, 0);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (curSelectMark)
                    {
                        curSelectMark.Select(false);
                        curSelectMark = null;
                    }
                }

                break;
        }


    }

    private void UpdateInspector()
    {
        if (!scaleIF.isFocused && curSelectWall)
        {
            scaleIF.text = curSelectWall.Scale.ToString();
        }

        if (!rotateIF.isFocused && curSelectWall)
        {
            rotateIF.text = curSelectWall.Rotate.ToString();
        }
    }

    private void Detect()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        isHitFloor = modelController.CurrentScene.DetectFloor(ray.origin, ray.direction, out floorHit);

        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(camRay, float.MaxValue);

        float closestDist = float.MaxValue;
        curDetectMark = null;
        foreach (var item in hits)
        {
            Mark curMark = item.collider.GetComponent<Mark>();
            float curDist = (item.point - Camera.main.transform.position).sqrMagnitude;
            if (curMark)
            {
                if (curDist < closestDist)
                {
                    curDetectMark = curMark;
                    closestDist = curDist;
                }
            }
        }

        curDetectWall = null;
        foreach (var item in hits)
        {
            curDetectWall = item.collider.GetComponentInParent<Wall>();
            if (curDetectWall)
            {
                break;
            }
        }

        //if (closestMark)
        //{
        //    tKind = closestMark.kind;
        //    closestMark.Select(true);
        //}
        //else
        //    tKind = TransformKind.None;


    }

    public void BT_SpawnWall()
    {
        curSelectMark = null;
        curSelectWall = null;

        if (!curSpawnWall)
        {
            curSpawnWall = Instantiate(wallPref).GetComponent<Wall>();
        }

        curStage = CustomizingStage.Spawn;
    }
    public void BT_MoveMode()
    {
        if (curSelectWall)
        {
            curSelectWall.Select(CustomizingStage.Mode_Move);
            curStage = CustomizingStage.Mode_Move;

            SnapController.Init(walls[modelController.CurrentScene.SceneName], 1.0f);
        }
    }
    public void BT_RotateMode()
    {
        if (curSelectWall)
        {
            curSelectWall.Select(CustomizingStage.Mode_Rotate);
            curStage = CustomizingStage.Mode_Rotate;
        }
    }
    public void BT_ScaleMode()
    {
        if (curSelectWall)
        {
            curSelectWall.Select(CustomizingStage.Mode_Scale);
            curStage = CustomizingStage.Mode_Scale;
        }
    }

    public void IF_ScaleEnd()
    {
        if (CustomFunction.IsNumber(scaleIF.text) && curSelectWall)
        {
            float scaleValue = float.Parse(scaleIF.text);

            if (scaleValue > 0.0f)
            {
                curSelectWall.ScaleFromCenter(scaleValue);
            }
        }
    }
    public void IF_RotateEnd()
    {
        if (CustomFunction.IsNumber(rotateIF.text) && curSelectWall)
        {
            float rotValue = float.Parse(rotateIF.text);

            curSelectWall.transform.rotation = Quaternion.Euler(0, rotValue, 0);
        }
    }
}
