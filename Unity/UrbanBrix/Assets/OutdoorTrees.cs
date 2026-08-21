using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutdoorTrees : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        MainModel mainModel = ModelManager.Instance.Model;
        if (mainModel.IsOutdoorView)
        {
            this.gameObject.SetActive(true);            
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
