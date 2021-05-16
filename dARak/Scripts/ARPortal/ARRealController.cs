using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GoogleARCore;

#if UNITY_EDITOR
using input = GoogleARCore.InstantPreviewInput;
#endif

public class ARRealController : MonoBehaviour
{
    //private List<DetectedPlane> m_NewTrackedPlanes = new List<DetectedPlane>();

    public GameObject Portal;
    public GameObject ARCamera;
    public GameObject[] furnitureList;
    string cmd;

    roominfo info;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        cmd = GameObject.Find("Socket").GetComponent<Socketpp>().receiveMsg;
        //Debug.Log(cm);
        roominfo info = JsonUtility.FromJson<roominfo>(cmd);
        //Debug.Log(info.item_list[0].position[0]);


        for (int i = 0; i < info.item_list.Length; i++)
        {
            GameObject furniturePrefab = furnitureList[info.item_list[i].iid];
            GameObject furniture = Instantiate(furniturePrefab);
            furniture.transform.parent = GameObject.Find("Room").transform;
            
            furniture.transform.rotation = Quaternion.Euler(info.item_list[i].rotation[0] + 90, info.item_list[i].rotation[1], info.item_list[i].rotation[2]);
            furniture.transform.localPosition = new Vector3(-info.item_list[i].position[0], -info.item_list[i].position[2], -info.item_list[i].position[1] - 5.5f);
            furniture.transform.localScale = new Vector3(info.item_list[i].scale[0], info.item_list[i].scale[1], info.item_list[i].scale[2]);
        }
        GameObject.Find("PortalPlane").GetComponent<PortalManager>().enabled = true;
        Portal.SetActive(false);
    }

    void Update()
    {
        Touch touch;
        if(Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        TrackableHit hit;

        if(Frame.Raycast(touch.position.x, touch.position.y, TrackableHitFlags.PlaneWithinPolygon, out hit))
        { 
            
            Portal.SetActive(true);
            Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);
            Portal.transform.position = hit.Pose.position;
            Portal.transform.rotation = hit.Pose.rotation;

            Vector3 cameraPosition = ARCamera.transform.position;

            cameraPosition.y = hit.Pose.position.y;

            Portal.transform.LookAt(cameraPosition, Portal.transform.up);

            Portal.transform.parent = anchor.transform;
            
            /*var anchor = hit.Trackable.CreateAnchor(hit.Pose);
            Instantiate(Portal, hit.Pose.position, hit.Pose.rotation, anchor.transform);*/
    }
    }

    public void BackScene()
    {
        SceneManager.LoadScene("Main");
    }
}
