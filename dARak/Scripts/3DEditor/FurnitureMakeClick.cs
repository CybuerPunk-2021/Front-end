using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        isclicked = true;
        createalpha = Instantiate(alpha150, gameObject.transform);
        var xpos = Input.mousePosition.x;
        var ypos = Input.mousePosition.y;
        Vector3 mousepos = Camera.ScreenToWorldPoint(new Vector3(xpos, ypos, 99.5f));
        createalpha.transform.position = mousepos;
    }

    private void OnMouseDrag()
    {
        if (isclicked == true)
        {
            // 마우스따라 반투명 캐릭터가 움직임
            var xpos = Input.mousePosition.x;
            var ypos = Input.mousePosition.y;
            Vector3 mousepos = Camera.ScreenToWorldPoint(new Vector3(xpos, ypos, 99.5f));
            createalpha.transform.position = new Vector3(mousepos.x, mousepos.y, gameObject.transform.position.z);
        }
    }

    private void OnMouseUp()
    {
        isclicked = false;
        GameObject a = Instantiate(realFurniture, createalpha.transform.position, Quaternion.identity);
        a.transform.parent = GameObject.Find("Myroom").transform;
        Destroy(createalpha);
    }
}
