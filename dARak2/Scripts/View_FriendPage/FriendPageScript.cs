using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FriendPageScript : MonoBehaviour
{
    public GameObject ProfileImage, ProfileName, ProfilewallpaperImage, ProfileText, SnapshotsImage;
    public GameObject post;
    public InputField PostInputField;
    public int visitbook_count = 0;
    Socketpp socketpp;
    // Start is called before the first frame update
    void Start()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
        profile_client_to_server();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void profile_client_to_server()
    {
        Profile_client_to_server profile = new Profile_client_to_server();
        profile.uid = socketpp.other_player_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(profile));
        Profile_server_to_client myProfile = JsonUtility.FromJson<Profile_server_to_client>(socketpp.receiveMsg);
        //ProfileImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("");
        ProfileName.GetComponent<Text>().text = socketpp.other_nickname;
        //ProfilewallpaperImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("");
        ProfileText.GetComponent<Text>().text = myProfile.self_intro;
        //SnapshotsImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("");
    }
    public void UpdatePostBtn()
    {
        loadvisitbook_client_to_server();
    }
    public void WritePostBtn()
    {
        writevisitbook_client_to_server();
    }

    public void loadvisitbook_client_to_server()
    {
        Loadvisitbook_client_to_server loadvisitbook = new Loadvisitbook_client_to_server();
        loadvisitbook.uid = socketpp.other_player_uid;
        loadvisitbook.count = visitbook_count;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(loadvisitbook));
        Loadvisitbook_server_to_client visitbook = JsonUtility.FromJson<Loadvisitbook_server_to_client>(socketpp.receiveMsg);
        for (int i = 0; i < 5; i++)
        {
            MakePost(visitbook.visit_book[i].writer_uid, visitbook.visit_book[i].nickname, visitbook.visit_book[i].comment);
        }
        visitbook_count++;
    }

    public void MakePost(int visitbook_uid, string visitbook_nickname, string visitbook_comment)
    {
        GameObject clone_post = Instantiate(post) as GameObject;
        clone_post.transform.SetParent(this.transform);
        clone_post.transform.localPosition = Vector3.zero;
        clone_post.transform.localScale = Vector3.one;
        //GameObject post_profile_image = clone_post.transform.Find("PostImage").gameObject;
        //post_profile_image.GetComponent<Image>().sprite = Resources.Load<Sprite>("");
        GameObject Post_Name = clone_post.transform.Find("PostName").gameObject;
        Post_Name.GetComponent<Text>().text = visitbook_nickname;
        GameObject Post_Answer = clone_post.transform.Find("PostAnswer").gameObject;
        Post_Answer.GetComponent<Text>().text = visitbook_comment;
    }

    public void writevisitbook_client_to_server()
    {
        Writevisitbook_client_to_server writevisitbook = new Writevisitbook_client_to_server();
        writevisitbook.writer_uid = socketpp.player_uid;
        writevisitbook.uid = socketpp.other_player_uid;
        writevisitbook.comment = PostInputField.text;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(writevisitbook));
        Writevisitbook_server_to_client w_visitbook = JsonUtility.FromJson<Writevisitbook_server_to_client>(socketpp.receiveMsg);
        MakePost(writevisitbook.writer_uid, socketpp.player_nickname, PostInputField.text);
    }
}
