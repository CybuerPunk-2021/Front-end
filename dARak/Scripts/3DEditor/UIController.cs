using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{
    public GameObject targetObject;
    private Button[] button;
    private GameObject closeImage;

    // Start is called before the first frame update
    void Start()
    {
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
        targetObject.transform.Rotate(0, -45f, 0, Space.World);

    }

    public void RightRotate()
    {
        targetObject.transform.Rotate(0, 45f, 0, Space.World);
    }

    public void Delete()
    {
        Destroy(targetObject);
        Destroy(gameObject);
    }
}
