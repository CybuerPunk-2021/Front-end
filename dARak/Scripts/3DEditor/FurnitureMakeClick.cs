using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FurnitureMakeClick : MonoBehaviour
{
    private bool isclicked = false;

    public GameObject alpha150 = null;
    private GameObject createalpha = null;
    public GameObject realFurniture = null;
    public GameObject[] item_list;
    public string cmd;

    Camera Camera;

    private void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {   
        isclicked = false;
        Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnMouseDown()
    {
        if (!IsPointerOverUIObject())
        {
            isclicked = true;
            createalpha = Instantiate(alpha150, gameObject.transform.position, gameObject.transform.rotation);
        }
    }

    private void OnMouseDrag()
    {
        if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            if (isclicked)
            {
                // 마우스따라 반투명 캐릭터가 움직임
                var xpos = Input.mousePosition.x;
                var ypos = Input.mousePosition.y;
                Vector3 mousepos = Camera.ScreenToWorldPoint(new Vector3(xpos, ypos, 101.9f - gameObject.transform.position.y));
                createalpha.transform.position = mousepos;
                //createalpha.transform.position = new Vector3(mousepos.x, mousepos.y, gameObject.transform.position.z);
            }
        }
    }
    //-4.3 ~ 4.3,  0.8~-7.84
    private void OnMouseUp()
    {
        if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            isclicked = false;
            //Quaternion rotation = Quaternion.identity;
            //rotation.eulerAngles = new Vector3(realFurniture.transform.rotation.x, realFurniture.transform.rotation.y, realFurniture.transform.rotation.z);
            //Debug.Log(rotation);
            GameObject a = Instantiate(realFurniture, createalpha.transform.position, createalpha.transform.rotation);
            
            if (a.transform.position.x < -4.3f)
                a.transform.position = new Vector3(-4.3f, a.transform.position.y, a.transform.position.z);
            if (a.transform.position.x > 4.3f)
                a.transform.localPosition = new Vector3(4.3f, a.transform.position.y, a.transform.position.z);
            if (a.transform.localPosition.z < -7.9f)
                a.transform.localPosition = new Vector3(a.transform.localPosition.x, a.transform.localPosition.y, -7.9f);
            if (a.transform.localPosition.z > 0.8f)
                a.transform.localPosition = new Vector3(a.transform.localPosition.x, a.transform.localPosition.y, 0.8f);

            a.transform.parent = GameObject.Find("Myroom").transform;
            Destroy(createalpha);
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
