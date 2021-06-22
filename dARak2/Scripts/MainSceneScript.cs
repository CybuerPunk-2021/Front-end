/*
 * MainSceneScript
 * : 씬 전환, 화면 전환 스크립트 및 파일로드, 시간 스크립트
 */

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
    public GameObject View_Loading;
    Socketpp socketpp;
    LoadingScript loadingscript;
    void Start()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>(); //소켓불러오기
        loadingscript = transform.parent.gameObject.GetComponent<LoadingScript>(); //로딩스크립트
    }
    //켜질때 로딩화면 끈다
    private void OnEnable()
    {
        View_Loading.SetActive(false);
    }

    //뒤로가기 버튼 누를 시, 로그인 화면으로 이동
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

    //내부저장소 이미지 파일 로드 스크립트
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

    //타임스탬프 파싱 스크립트, 2021 06 23 01 01 12 형식으로 출력
    public string ParseDateTime(string timestamp)
    {
        return DateTime.ParseExact(timestamp.Substring(0, 14), "yyyyMMddHHmmss", null).ToString("G");
    }

    //AR Real 씬 버튼 누를시 실행
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

    //AR Mini 씬 버튼 누를시 실행
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

    //3D 에디터 씬 버튼 누를시 실행
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


    //Edit Profile 켜지고 꺼질시
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
        /*
        View_Home.SetActive(true);
        GameObject.Find("MyPagePannel_Content").GetComponent<MyPageScript>().ReloadProfile();
        View_Loading.SetActive(false);*/
    }

    //Edit Snapshot 켜지고 꺼질시
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

    //Follow Unfollow Page 켜지고 꺼질시
    public void ActiveFriend()
    {
        View_Friend.SetActive(true);
    }
    public void UnActiveFriend()
    {
        View_Friend.SetActive(false);
    }

    //Friend Page 켜지고 꺼질시
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

    //Post to Friend Page 켜질시
    public void ActivePostToFriendPage()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;
        socketpp.other_player_uid = current.transform.parent.GetComponent<PrefabUid>().uid;
        socketpp.other_nickname = current.transform.parent.GetComponent<PrefabUid>().nickname;

        View_FriendPage.SetActive(true);
    }
    //FriendPage to Friend Page 켜질시
    public void ActiveFriendPageToFriendPage()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;
        socketpp.other_player_uid = current.transform.parent.GetComponent<PrefabUid>().uid;
        socketpp.other_nickname = current.transform.parent.GetComponent<PrefabUid>().nickname;
        GameObject.Find("View_Friendpage_Content").GetComponent<FriendPageScript>().reloadFriendPage();
    }

    //FriendFriend Follow Unfollow Page 켜지고 꺼질시
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

    //Search Page 켜지고 꺼질시
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

    //Album Page 켜지고 꺼질시
    public void ActiveAlbum()
    {
        View_Album.SetActive(true);
    }
    public void UnActiveAlbum()
    {
        View_Album.SetActive(false);
    }

    //FriendAlbum Page 켜지고 꺼질시
    public void ActiveFriendAlbum()
    {
        View_FriendAlbum.SetActive(true);
    }
    public void UnActiveFrendAlbum()
    {
        View_FriendAlbum.SetActive(false);  
    }

    //Snapshot Page 켜지고 꺼질시
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

    //Album Snapshot Page 켜지고 꺼질시
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

    //친구 페이지에서 스냅샷 켜질시
    public void ActiveSnapshotSceneToFriendPage()
    {
        View_HomeSnapshot.SetActive(false);
        View_FriendPage.SetActive(true);
    }
    /*
    IEnumerator UnActiveEditPageLoad()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
    }

    IEnumerator ActiveFriendLoad()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        View_Friend.SetActive(true);
        View_Loading.SetActive(false);
    }*/
}