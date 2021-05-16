using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureModify : MonoBehaviour
{
    private bool isclicked = false;
    private bool isdraged = false;

    private Vector3 startPos;
    private Vector3 lastPos;

    public GameObject alpha150 = null;
    private GameObject createalpha = null;
    public GameObject realFurniture = null;
    private GameObject createRealFurniture = null;

    GameObject Canvas = null;
    public GameObject furnitureUI = null;
    private GameObject createUI = null;

    Camera Camera;
    //MeshRenderer meshRenderer;

    // Use this for initialization
    void Awake()
    {
        isclicked = false;
        isdraged = false;
        Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        Canvas = GameObject.Find("Canvas");
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        if(transform.childCount != 0)
            Destroy(gameObject.transform.GetChild(0).gameObject);
    }


    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        isclicked = true;
        createalpha = Instantiate(alpha150, transform);
        var xpos = Input.mousePosition.x;
        var ypos = Input.mousePosition.y;
        startPos = Camera.ScreenToWorldPoint(new Vector3(xpos, ypos, 99.5f));
        createalpha.transform.position = startPos;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnMouseDrag()
    {
        if (isclicked == true)
        {
            // 마우스따라 반투명 캐릭터가 움직임
            isdraged = true;
            var xpos = Input.mousePosition.x;
            var ypos = Input.mousePosition.y;
            lastPos = Camera.ScreenToWorldPoint(new Vector3(xpos, ypos, 99.5f));
            createalpha.transform.position = lastPos;
        }
    }

    private void OnMouseUp()
    {
        isclicked = false;
        Vector3 Distv = lastPos - startPos;
        float Dist = Distv.sqrMagnitude;

        createRealFurniture = Instantiate(realFurniture, createalpha.transform.position, createalpha.transform.rotation);
        createRealFurniture.transform.parent = GameObject.Find("Myroom").transform;
        Destroy(createalpha);
        Destroy(gameObject);

        if (isdraged == true && Dist > 0)
        {
            isdraged = false;
        }
        else
        {
            createUI = Instantiate(furnitureUI, new Vector3(0, 0, 0), Quaternion.identity);
            UIController UIscript = createUI.GetComponent<UIController>();

            UIscript.targetObject = createRealFurniture;
            createUI.transform.SetParent(Canvas.transform, false);
            createUI.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1.5f, 0));
        }
    }
}
