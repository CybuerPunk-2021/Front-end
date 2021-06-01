using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainSceneScript : MonoBehaviour
{
    public GameObject View_Home;
    public GameObject View_HomeSnapshot;
    public GameObject View_EditPage;
    public GameObject View_EditImagePage;
    public GameObject View_Search;
    public GameObject View_Friend;
    public GameObject View_FriendPage;
    public GameObject View_Album;
    public GameObject View_FriendAlbum;
    public GameObject View_AlbumSnapshot;
    Socketpp socketpp;

    void Start()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
    }

    public void ActiveARRealScene()
    {
        if (socketpp.timeStamp == "Not")
        {

        }
        else
        {
            Snapshot_roominfo info = new Snapshot_roominfo();
            info.uid = socketpp.player_uid;
            info.timestamp = socketpp.timeStamp;
            socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(info));
            SceneManager.LoadScene("ARReal");
        }
    }
    public void ActiveARMiniScene()
    {
        if (socketpp.timeStamp == "Not")
        {
        }
        else
        {
            Snapshot_roominfo info = new Snapshot_roominfo();
            info.uid = socketpp.player_uid;
            info.timestamp = socketpp.timeStamp;
            socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(info));
            SceneManager.LoadScene("ARMini");
        }
    }
    public void Active3DEditor()
    {
        if (socketpp.timeStamp == "Not")
        {
            SceneManager.LoadScene("3DEditor");
        }
        else
        {
            Snapshot_roominfo info = new Snapshot_roominfo();
            info.uid = socketpp.player_uid;
            info.timestamp = socketpp.timeStamp;
            socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(info));
            SceneManager.LoadScene("3DEditor");
        }
    }

    //Edit Profile
    public void ActiveEditPage()
    {
        View_EditPage.SetActive(true);
    }
    public void UnActiveEditPage()
    {
        View_EditPage.SetActive(false);
        GameObject.Find("MyPagePannel_Content").GetComponent<MyPageScript>().updateMyPage();
    }

    //Edit Snapshot
    public void ActiveEditSnapshotPage()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;
        socketpp.snapshot_timestamp = current.transform.parent.GetComponent<SnapshotUid>().snapshot_uid;
        socketpp.snapshot_intro = current.transform.parent.GetComponent<SnapshotUid>().snapshot_intro;

        View_EditImagePage.SetActive(true);
        GameObject.Find("EditImagePage").GetComponent<EditSnapshotScript>().UpdateText();
    }
    public void UnActiveEditSnapshotPage()
    {
        View_EditImagePage.SetActive(false);
        GameObject.Find("View_Album_Content").GetComponent<AlbumScript>().UpdateAlbum();
    }

    //Friend Page
    public void ActiveFriend()
    {
        View_Friend.SetActive(true);
        GameObject.Find("View_Follower_Content").GetComponent<FollowerScript>().UpdateFollower();
    }
    public void UnActiveFriend()
    {
        View_Friend.SetActive(false);
    }

    //Follow UnFollow Page
    public void ActiveFriendPage()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;
        socketpp.other_player_uid = current.transform.parent.GetComponent<PrefabUid>().uid;
        socketpp.other_nickname = current.transform.parent.GetComponent<PrefabUid>().nickname;

        View_FriendPage.SetActive(true);

        GameObject.Find("View_Friendpage_Content").GetComponent<FriendPageScript>().profile_client_to_server();
        GameObject.Find("View_Friendpage_Content").GetComponent<FriendPageScript>().loadvisitbook_client_to_server();
    }
    public void UnActiveFriendPage()
    {
        GameObject.Find("View_Friendpage_Content").GetComponent<FriendPageScript>().ErasePost();
        View_FriendPage.SetActive(false);
    }

    //Search Page
    public void ActiveSearch()
    {
        View_Search.SetActive(true);
    }
    public void UnActiveSearch()
    {
        View_Search.SetActive(false);
    }

    //Album Page
    public void ActiveAlbum()
    {
        View_Album.SetActive(true);
        GameObject.Find("View_Album_Content").GetComponent<AlbumScript>().album_client_to_server();
    }
    public void UnActiveAlbum()
    {
        GameObject.Find("View_Album_Content").GetComponent<AlbumScript>().EraseAlbum();
        View_Album.SetActive(false);
    }

    //FriendAlbum Page
    public void ActiveFriendAlbum()
    {
        View_FriendAlbum.SetActive(true);
        GameObject.Find("View_FriendAlbum_Content").GetComponent<FriendAlbumScript>().album_client_to_server();
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
        GameObject.Find("View_HomeSnapshot_Panel").GetComponent<HomeSnapShotScript>().snapshot_client_to_server();
    }
    public void OffSnapshotScene()
    {
        View_HomeSnapshot.SetActive(false);
    }

    //Album Snapshot Page
    public void OnAlbumSnapshotScene()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;
        socketpp.snapshot_timestamp = current.transform.GetComponent<SnapshotUid>().snapshot_uid;
        socketpp.snapshot_intro = current.transform.GetComponent<SnapshotUid>().snapshot_intro;
        socketpp.snapshot_like = current.transform.GetComponent<SnapshotUid>().snapshot_like;

        View_AlbumSnapshot.SetActive(true);
        GameObject.Find("View_AlbumSnapshot_Panel").GetComponent<AlbumSnapshotScript>().album_snapshot_client_to_server();
    }
    public void OffAlbumSnapshotScene()
    {
        View_AlbumSnapshot.SetActive(false);
    }
}