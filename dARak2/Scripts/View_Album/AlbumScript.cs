using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AlbumScript : MonoBehaviour
{
    public GameObject albumImage;
    public void MakeClone()
    {
        GameObject clone_albumImage = Instantiate(albumImage) as GameObject;
        clone_albumImage.transform.SetParent(this.transform);
        clone_albumImage.transform.localPosition = Vector3.zero;
        clone_albumImage.transform.localScale = Vector3.one;
        //clone_albumImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("");
    }
    // Start is called before the first frame update
    void Start()
    {
        MakeClone();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
