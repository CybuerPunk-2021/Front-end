using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Profile_img_request_size_to_server
{
    public string action = "profile_img_request_size";
    public int uid;
}

[System.Serializable]
public class Profile_img_request_size_to_client
{
    public string action = "profile_img_request_size";
    public int size;
    public string timestamp;
}

[System.Serializable]
public class Profile_img_request
{
    public string action = "profile_img_request";
    public int uid;
}
//Login Scene
[System.Serializable]
public class Signup_client_to_server
{
    public string action = "signup";
    public string id;
    public string pw;
    public string nickname;
    public string email;
}

[System.Serializable]
public class Auth_client_to_server
{
    public string action = "email auth";
    public int auth;
}

[System.Serializable]
public class Login_client_to_server
{
    public string action = "login";
    public string id;
    public string pw;
}

[System.Serializable]
public class Login_server_to_client
{
    public string action;
    public string nickname;
    public int uid;
}

//TimelineScene
[System.Serializable]
public class Timeline_client_to_server
{
    public string action = "home";
    public int uid;
    public int count;
}


[System.Serializable]
public class Timeline_server_to_client
{
    public string action = "homeinfo";
    public Info[] info;
}


[System.Serializable]
public class Info
{
    public int uid;
    public string nickname;
    public string like;
    public string timestamp;
    public string snapshot_intro;
}


[System.Serializable]
//MypageScene
public class Profile_client_to_server
{
    public string action = "profile_info";
    public int uid;
}


[System.Serializable]
public class Profile_server_to_client
{
    public string action = "profile_info";
    public int follower;
    public string self_intro;
    public Snapshot snapshot_info;
}


[System.Serializable]
public class Snapshot
{
    public string snapshot_intro;
    public int like_num;
    public string timestamp;
}


[System.Serializable]
public class Loadvisitbook_client_to_server
{
    public string action = "visit_book_request";
    public int uid;
    public int count;
    public string type = "comment";
}


[System.Serializable]
public class Loadvisitbook_server_to_client
{
    public string action = "visit_book_request";
    public Visitbook[] visit_book;
}


[System.Serializable]
public class Visitbook
{
    public int writer_uid;
    public string nickname;
    public string comment;
    public string timestamp;
}


[System.Serializable]
public class Writevisitbook_client_to_server
{
    public string action = "visit_book_write";
    public int writer_uid;
    public int uid;
    public string comment;
    public string type = "comment";
}

[System.Serializable]
public class Writevisitbook_server_to_client
{
    public string action = "visit_book_write";
    public string timestamp;
}

//FollowScene
[System.Serializable]
public class Followscene_client_to_server
{
    public string action = "follower";
    public int uid;
}


[System.Serializable]
public class Followscene_server_to_client
{
    public string action = "follower";
    public Follower[] follower;
}


[System.Serializable]
public class Follower
{
    public int uid;
    public string nickname;
}


[System.Serializable]
public class Followingscene_client_to_server
{
    public string action = "following";
    public int uid;
}


[System.Serializable]
public class Followingscene_server_to_client
{
    public string action = "following";
    public Following[] following;
}


[System.Serializable]
public class Following
{
    public int uid;
    public string nickname;
}


[System.Serializable]
public class Followadd_client_to_server
{
    public string action = "follow";
    public int from_uid;
    public int to_uid;
}


[System.Serializable]
public class Followdelete_client_to_server
{
    public string action = "follow_del";
    public int from_uid;
    public int to_uid;
}



[System.Serializable]
public class Snapshot_roominfo
{
    public string action = "snapshot_roominfo";
    public int uid;
    public string timestamp;
}


[System.Serializable]
public class roominfo
{
    public string action = "snapshot_roominfo";
    public SaveItemList[] item_list;
}


[System.Serializable]
public class Snapshot_save
{
    public string action = "snapshot_save";
    public int uid;
    public string snapshot_intro;
    public string[] item_list;
}


[System.Serializable]
public class ObjectTransform
{
    public string category;
    public int iid;
    public float[] rotation;
    public float[] position;
    public float[] scale;
}


[System.Serializable]
public class SaveItemList
{
    public int iid;
    public float[] rotation;
    public float[] position;
    public float[] scale;
}

[System.Serializable]
public class SaveOK
{
    public string action;
    public string timestamp;
}

public class ClassCollection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
