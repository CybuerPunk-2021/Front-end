using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainSceneScript : MonoBehaviour
{
    public GameObject View_Home;
    public GameObject View_HomeSnapshot;
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

    public void HometoFriend()
    {
        View_Home.SetActive(false);
        View_Friend.SetActive(true);
    }
    public void FriendtoHome()
    {
        View_Home.SetActive(true);
        View_Friend.SetActive(false);
    }
    public void FriendtoFPage()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;
        socketpp.other_player_uid = current.transform.parent.GetComponent<PrefabUid>().uid;
        socketpp.other_nickname = current.transform.parent.GetComponent<PrefabUid>().nickname;
        Debug.Log(socketpp.other_nickname);
        View_Friend.SetActive(false);
        View_FriendPage.SetActive(true);
        View_FriendPage.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<FriendPageScript>().profile_client_to_server();
    }
    public void FPagetoFriend()
    {
        View_Friend.SetActive(true);
        View_FriendPage.SetActive(false);
    }
   public void FPagetoFAlbum()
    {
        View_FriendPage.SetActive(false);
        View_FriendAlbum.SetActive(true);
    }
    public void FAlbumtoFPage()
    {
        View_FriendPage.SetActive(true);
        View_FriendAlbum.SetActive(false);
    }
    public void HometoFPage()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;
        socketpp.other_player_uid = current.transform.parent.GetComponent<PrefabUid>().uid;
        socketpp.other_nickname = current.transform.parent.GetComponent<PrefabUid>().nickname;
        View_Home.SetActive(false);
        View_FriendPage.SetActive(true);
    }
    public void HometoSearch()
    {
        View_Home.SetActive(false);
        View_Search.SetActive(true);
    }
    public void SearchtoHome()
    {
        View_Home.SetActive(true);
        View_Search.SetActive(false);
    }
    public void SearchtoFPage()
    {
        View_Search.SetActive(false);
        View_FriendPage.SetActive(true);
    }
    public void HometoAlbum()
    {
        View_Home.SetActive(false);
        View_Album.SetActive(true);
    }
    public void AlbumtoHome()
    {
        View_Home.SetActive(true);
        View_Album.SetActive(false);
    }
    public void OnSnapshotScene()
    {
        View_HomeSnapshot.SetActive(true);
    }
    public void OffSnapshotScene()
    {
        View_HomeSnapshot.SetActive(false);
    }
    public void OnAlbumSnapshotScene()
    {
        View_AlbumSnapshot.SetActive(true);
    }
    public void OffAlbumSnapshotScene()
    {
        View_AlbumSnapshot.SetActive(false);
    }
}
