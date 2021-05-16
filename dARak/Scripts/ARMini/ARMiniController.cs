using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GoogleARCore;

public class ARMiniController : MonoBehaviour
{
    public GameObject MiniRoom;
    public GameObject ARCamera;
    public GameObject[] furnitureList;
    string cmd;

    roominfo info;
    //ObjectTransform obj_transform;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

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
            furniture.transform.parent = GameObject.Find("Rot").transform;
            furniture.transform.rotation = Quaternion.Euler(info.item_list[i].rotation[0] + 90, info.item_list[i].rotation[1], info.item_list[i].rotation[2]);
            furniture.transform.localPosition = new Vector3(info.item_list[i].position[0], -info.item_list[i].position[2], info.item_list[i].position[1]);
            furniture.transform.localScale = new Vector3(info.item_list[i].scale[0], info.item_list[i].scale[1], info.item_list[i].scale[2]);
        }
        MiniRoom.SetActive(false);
    }

    void Update()
    {
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }

        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        TrackableHit hit;

        if (Frame.Raycast(touch.position.x, touch.position.y, TrackableHitFlags.PlaneWithinPolygon, out hit))
        {
            MiniRoom.SetActive(true);
            Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);
            MiniRoom.transform.position = hit.Pose.position;
            MiniRoom.transform.rotation = hit.Pose.rotation;

            Vector3 cameraPosition = ARCamera.transform.position;

            cameraPosition.y = hit.Pose.position.y;

            MiniRoom.transform.LookAt(cameraPosition, MiniRoom.transform.up);
            MiniRoom.transform.parent = anchor.transform;
            //MiniRoom.transform.eulerAngles = new Vector3(0, 180, 0);
            /*var anchor = hit.Trackable.CreateAnchor(hit.Pose);
            Instantiate(Portal, hit.Pose.position, hit.Pose.rotation, anchor.transform);*/
        }
    }
    public void BackScene()
    {
        SceneManager.LoadScene("Main");
    }

}
