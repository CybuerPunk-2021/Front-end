using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.IO;
using System;
public class MainSceneScript : MonoBehaviour
{
    public GameObject View_Home;
    public GameObject View_HomeSnapshot;
    public GameObject View_EditPage;
    public GameObject View_EditImagePage;
    public GameObject View_Search;
    public GameObject View_Friend;
    public GameObject View_FriendPage;
    public GameObject View_FriendFriend;
    public GameObject View_Album;
    public GameObject View_FriendAlbum;
    public GameObject View_AlbumSnapshot;
    Socketpp socketpp;

    void Start()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                SceneManager.LoadScene("Login");
            }
        }
    }

    public Sprite SystemIOFileLoad(string path)
    {
        byte[] byteTexture = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(0, 0);
        if (byteTexture.Length > 0)
        {
            texture.LoadImage(byteTexture);
        }
        Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(.5f, .5f));

        return sprite;
    }

    public string ParseDateTime(string timestamp)
    {
        return DateTime.ParseExact(timestamp.Substring(0, 14), "yyyyMMddHHmmss", null).ToString("G");
    }

    public void ActiveARRealScene()
    {
        Snapshot_roominfo info = new Snapshot_roominfo();
        info.uid = socketpp.other_player_uid;

        if (EventSystem.current.currentSelectedGameObject.name == "ARPannel_RealModeBtn")
        {
            if (socketpp.player_recent_timestamp == "") { return; }
            else
            {
                info.timestamp = socketpp.player_recent_timestamp;
            }
        }
        else
        {
            info.timestamp = socketpp.snapshot_timestamp;
        }
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(info));
        SceneManager.LoadScene("ARReal");
    }

    public void ActiveARMiniScene()
    {
        Snapshot_roominfo info = new Snapshot_roominfo();
        info.uid = socketpp.other_player_uid;

        if (EventSystem.current.currentSelectedGameObject.name == "ARPannel_MiniModeBtn")
        {
            if(socketpp.player_recent_timestamp == "") { return; }
            else {
                info.timestamp = socketpp.player_recent_timestamp;
            }
        }
        else
        {
            info.timestamp = socketpp.snapshot_timestamp;
        }
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(info));
        SceneManager.LoadScene("ARMini");
    }
    public void Active3DEditor()
    {
        if (EventSystem.current.currentSelectedGameObject.name == "View_AlbumSnapshot_Editor3DBtn")
        {
            Snapshot_roominfo info = new Snapshot_roominfo();
            info.uid = socketpp.player_uid;
            info.timestamp = socketpp.snapshot_timestamp;
            socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(info));
        }
        else
        {
            socketpp.snapshot_intro = "설명을 입력하세요.";
        }
        SceneManager.LoadScene("3DEditor");
    }

    //Edit Profile
    public void ActiveEditPage()
    {
        View_Home.SetActive(false);
        View_EditPage.SetActive(true);
    }
    public void UnActiveEditPage()
    {
        View_EditPage.SetActive(false);
        View_Home.SetActive(true);
        GameObject.Find("MyPagePannel_Content").GetComponent<MyPageScript>().ReloadProfile();
    }

    //Edit Snapshot
    public void ActiveEditSnapshotPage()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;
        socketpp.snapshot_timestamp = current.transform.parent.GetComponent<SnapshotUid>().snapshot_uid;
        socketpp.snapshot_intro = current.transform.parent.GetComponent<SnapshotUid>().snapshot_intro;

        View_Album.SetActive(false);
        View_EditImagePage.SetActive(true);
    }
    public void UnActiveEditSnapshotPage()
    {
        View_EditImagePage.SetActive(false);
        View_Album.SetActive(true);
        GameObject.Find("MyPagePannel_Content").GetComponent<MyPageScript>().profile_client_to_server();
    }

    //Follow Unfollow Page
    public void ActiveFriend()
    {
        //View_Home.SetActive(false);
        View_Friend.SetActive(true);
    }
    public void UnActiveFriend()
    {
        View_Friend.SetActive(false);
        //View_Home.SetActive(true);
    }

    //Friend Page
    public void ActiveFriendPage()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;
        socketpp.other_player_uid = current.transform.parent.GetComponent<PrefabUid>().uid;
        socketpp.other_nickname = current.transform.parent.GetComponent<PrefabUid>().nickname;

        View_FriendPage.SetActive(true);
    }
    public void UnActiveFriendPage()
    {
        View_FriendPage.SetActive(false);
    }

    //Post to Friend Page
    public void ActivePostToFriendPage()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;
        socketpp.other_player_uid = current.transform.parent.GetComponent<PrefabUid>().uid;
        socketpp.other_nickname = current.transform.parent.GetComponent<PrefabUid>().nickname;

        View_FriendPage.SetActive(true);
    }
    //FriendPage to Friend Page
    public void ActiveFriendPageToFriendPage()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;
        socketpp.other_player_uid = current.transform.parent.GetComponent<PrefabUid>().uid;
        socketpp.other_nickname = current.transform.parent.GetComponent<PrefabUid>().nickname;
        GameObject.Find("View_Friendpage_Content").GetComponent<FriendPageScript>().reloadFriendPage();
    }

    //FriendFriend Follow Unfollow Page
    public void ActiveFriendFriend()
    {
        View_FriendPage.SetActive(false);
        View_FriendFriend.SetActive(true);
    }
    public void UnActiveFriendFriend()
    {
        View_FriendFriend.SetActive(false);
        View_FriendPage.SetActive(true);
    }

    //Search Page
    public void ActiveSearch()
    {
        //View_Home.SetActive(false);
        View_Search.SetActive(true);
    }
    public void UnActiveSearch()
    {
        View_Search.SetActive(false);
        //View_Home.SetActive(true);
    }

    //Album Page
    public void ActiveAlbum()
    {
        View_Album.SetActive(true);
    }
    public void UnActiveAlbum()
    {
        View_Album.SetActive(false);
    }

    //FriendAlbum Page
    public void ActiveFriendAlbum()
    {
        View_FriendAlbum.SetActive(true);
    }
    public void UnActiveFrendAlbum()
    {
        View_FriendAlbum.SetActive(false);  
    }

    //Snapshot Page
    public void OnSnapshotScene()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;
        socketpp.other_player_uid = current.transform.parent.GetComponent<PrefabUid>().uid;
        socketpp.other_nickname = current.transform.parent.GetComponent<PrefabUid>().nickname;
        socketpp.snapshot_timestamp = current.transform.parent.GetComponent<SnapshotUid>().snapshot_uid;
        socketpp.snapshot_intro = current.transform.parent.GetComponent<SnapshotUid>().snapshot_intro;
        socketpp.snapshot_like = current.transform.parent.GetComponent<SnapshotUid>().snapshot_like;

        View_HomeSnapshot.SetActive(true);
    }
    public void OffSnapshotScene()
    {
        View_HomeSnapshot.SetActive(false);
    }

    //Album Snapshot Page
    public void OnAlbumSnapshotScene()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;
        socketpp.other_player_uid = current.transform.GetComponent<PrefabUid>().uid;
        socketpp.other_nickname = current.transform.GetComponent<PrefabUid>().nickname;
        socketpp.snapshot_timestamp = current.transform.GetComponent<SnapshotUid>().snapshot_uid;
        socketpp.snapshot_intro = current.transform.GetComponent<SnapshotUid>().snapshot_intro;
        socketpp.snapshot_like = current.transform.GetComponent<SnapshotUid>().snapshot_like;

        View_AlbumSnapshot.SetActive(true);
    }
    public void OffAlbumSnapshotScene()
    {
        View_AlbumSnapshot.SetActive(false);
    }
}