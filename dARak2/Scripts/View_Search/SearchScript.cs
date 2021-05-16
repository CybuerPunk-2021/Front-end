using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SearchScript : MonoBehaviour
{
    public GameObject searchFriend;
    public InputField searchInputField;
    public void MakeClone()
    {
        GameObject clone_searchFriend = Instantiate(searchFriend) as GameObject;
        clone_searchFriend.transform.SetParent(this.transform);
        clone_searchFriend.transform.localPosition = Vector3.zero;
        clone_searchFriend.transform.localScale = Vector3.one;
        //GameObject searchFriend_image = clone_snapshot.transform.Find("SearchFriendImage").gameObject;
        //searchFriend_image.GetComponent<Image>().sprite = Resources.Load<Sprite>("");
        GameObject searchFriendt_text = clone_searchFriend.transform.Find("SearchFriendName").gameObject;
        searchFriendt_text.GetComponent<Text>().text = searchInputField.text;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
