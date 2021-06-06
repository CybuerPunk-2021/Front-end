using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMessege : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SaveMessegeEnd()
    {
        GameObject saveMessege = GameObject.Find("SaveMessege");
        saveMessege.SetActive(false);
    }
}
