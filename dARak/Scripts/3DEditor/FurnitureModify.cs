using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FurnitureModify : MonoBehaviour
{
    private bool isclicked = false;
    private bool isdraged = false;

    //private Vector3 startPos;
    private Vector3 lastPos;

    public GameObject alpha150 = null;
    private GameObject createalpha = null;
    public GameObject realFurniture = null;
    private GameObject createRealFurniture = null;

    GameObject Canvas = null;
    public GameObject furnitureUI = null;
    private GameObject createUI = null;

    Camera Camera;

    private float clickTime;
    public float minClickTime = 0.5f;
    //MeshRenderer meshRenderer;

    // Use this for initialization
    void Awake()
    {
        isclicked = false;
        isdraged = false;
        Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        Canvas = GameObject.Find("Canvas");
        //gameObject.GetComponent<MeshRenderer>().enabled = true;
        if(!(transform.childCount == 1 || transform.childCount == 0))
            Destroy(gameObject.transform.GetChild(0).gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        if (isclicked)
            clickTime += Time.deltaTime;
        else
            clickTime = 0;
    }

    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            isclicked = true;
            createalpha = Instantiate(alpha150, gameObject.transform.position, gameObject.transform.rotation);
        }
    }

    private void OnMouseDrag()
    {
        if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            if (clickTime >= minClickTime)
            {
                // 마우스따라 반투명 캐릭터가 움직임
                isdraged = true;
                var xpos = Input.mousePosition.x;
                var ypos = Input.mousePosition.y;
                lastPos = Camera.ScreenToWorldPoint(new Vector3(xpos, ypos, 101.9f - gameObject.transform.position.y));
                createalpha.transform.position = lastPos;
            }
        }
    }

    private void OnMouseUp()
    {
        if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            isclicked = false;
            //Vector3 Distv = lastPos - startPos;
            //float Dist = Distv.sqrMagnitude;

            createRealFurniture = Instantiate(realFurniture, createalpha.transform.position, createalpha.transform.rotation);
            if (createRealFurniture.transform.position.x < -4.3f)
                createRealFurniture.transform.position = new Vector3(-4.3f, createRealFurniture.transform.position.y, createRealFurniture.transform.position.z);
            if (createRealFurniture.transform.position.x > 4.3f)
                createRealFurniture.transform.localPosition = new Vector3(4.3f, createRealFurniture.transform.position.y, createRealFurniture.transform.position.z);
            if (createRealFurniture.transform.localPosition.z < -7.9f)
                createRealFurniture.transform.localPosition = new Vector3(createRealFurniture.transform.localPosition.x, createRealFurniture.transform.localPosition.y, -7.9f);
            if (createRealFurniture.transform.localPosition.z > 0.8f)
                createRealFurniture.transform.localPosition = new Vector3(createRealFurniture.transform.localPosition.x, createRealFurniture.transform.localPosition.y, 0.8f);

            createRealFurniture.transform.parent = GameObject.Find("Myroom").transform;
            Destroy(createalpha);
            Destroy(gameObject);
            
            if (isdraged == true)
            {
                isdraged = false;
            }
            else
            {
                createUI = Instantiate(furnitureUI, new Vector3(0, 0, 0), Quaternion.identity);
                UIController UIscript = createUI.GetComponent<UIController>();

                UIscript.targetObject = createRealFurniture;
                createUI.transform.SetParent(Canvas.transform, false);
                createUI.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + new Vector3(0, 0, 3f));
                //+new Vector3(0, 45f, 0)
            }
        }
    }
}
