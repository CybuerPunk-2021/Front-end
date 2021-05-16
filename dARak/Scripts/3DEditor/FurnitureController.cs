using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureController : MonoBehaviour
{


    string cmd;
    public GameObject[] furnitureList;
    void Start()
    {
        cmd = GameObject.Find("Socket").GetComponent<Socketpp>().receiveMsg;
        //Debug.Log(cm);
        roominfo info = JsonUtility.FromJson<roominfo>(cmd);
        //Debug.Log(info.item_list[0].position[0]);

        
        for (int i = 0; i < info.item_list.Length; i++)
        {
            Debug.Log(info.item_list[i].iid);
            GameObject furniturePrefab = furnitureList[info.item_list[i].iid];
            GameObject furniture = Instantiate(furniturePrefab);
            furniture.transform.parent = GameObject.Find("Myroom").transform;
            furniture.transform.rotation = Quaternion.Euler(info.item_list[i].rotation[0], info.item_list[i].rotation[1], info.item_list[i].rotation[2]);
            furniture.transform.localPosition = new Vector3(info.item_list[i].position[0], info.item_list[i].position[1], info.item_list[i].position[2]);
            furniture.transform.localScale = new Vector3(info.item_list[i].scale[0], info.item_list[i].scale[1], info.item_list[i].scale[2]);
        }
    }

    void Update()
    {
        
    }
}

