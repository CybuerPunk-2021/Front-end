using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject targetObject;
    private Button[] button;
    private GameObject closeImage;

    // Start is called before the first frame update
    void Start()
    {
        //closeImage = GameObject.Find("CloseBack");
        //button[0] = GameObject.Find("LeftRotate").GetComponent<Button>();
        //button[1] = GameObject.Find("RightRotate").GetComponent<Button>();
        //button[2] = GameObject.Find("DeleteButton").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UIClose()
    {
        Destroy(gameObject);
    }

    public void LeftRotate()
    {
        targetObject.transform.Rotate(0, 0, 45f);
    }

    public void RightRotate()
    {
        targetObject.transform.Rotate(0, 0, -45f);
    }

    public void Delete()
    {
        Destroy(targetObject);
        Destroy(gameObject);
    }
}
