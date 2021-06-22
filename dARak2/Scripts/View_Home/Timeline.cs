using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System.Linq;

public class Timeline : MonoBehaviour
{
    public GameObject snapshot;
    public int snapshot_count = 0; //타임라인 스냅샷 리스트 값
    public GameObject popup;
    Socketpp socketpp;

    // Start is called before the first frame update
    void Awake()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>(); //소켓불러오기
        timeline_client_to_server(); //타임라인 스냅샷 가져오기
    }
    //켜질시
    void OnEnable()
    {
        //EraseTimeline();
        //timeline_client_to_server();
    }

    //타임라인 새로고침
    public void TimelineReload()
    {
        popup.transform.GetChild(0).GetComponent<Text>().text = "새로고침 되었습니다";
        popup.SetActive(true);
        EraseTimeline(); //타임라인 Prefab지우기
        timeline_client_to_server(); //타임라인 스냅샷 가져오기
    }

    //타임라인 스냅샷 업데이트
    public void UpdateBtn()
    {
        timeline_client_to_server(); //타임라인 스냅샷 가져오기
    }
    //타임라인 Prefab지우기
    public void EraseTimeline()
    {
        GameObject[] timelineSnapshots = GameObject.FindGameObjectsWithTag("Snapshot");
        foreach (GameObject timelineSnapshot in timelineSnapshots)
        {
            Destroy(timelineSnapshot); //타임라인스냅샷 삭제
        }
        snapshot_count = 0; //스냅샷 리스트 0으로 초기화
    }

    //좋아요 버튼
    public void LikeBtn(string timeline_snapshot_like)
    {
        Debug.Log(timeline_snapshot_like);
        if(timeline_snapshot_like == "True") //이미 좋아요상태이면 좋아요 삭제, 좋아요 상태 아니면 좋아요 추가
            likeSnapshot_client_to_server("delete");
        else if(timeline_snapshot_like == "False")
            likeSnapshot_client_to_server("add");
    }
    //스냅샷 좋아요
    public void likeSnapshot_client_to_server(string like_type)
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;
        LikeSnapshot_client_to_server likeSnapshot = new LikeSnapshot_client_to_server();
        likeSnapshot.type = like_type;
        likeSnapshot.from_uid = socketpp.player_uid;
        likeSnapshot.to_uid = current.transform.parent.GetComponent<PrefabUid>().uid;
        likeSnapshot.timestamp = current.transform.parent.GetComponent<SnapshotUid>().snapshot_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(likeSnapshot)); //좋아요 정보, 사용자uid, 대상 uid 타임스탬프값을 클라이언트에서 서버로 전달
        LikeSnapshot_server_to_client likeSnapshotResult = JsonUtility.FromJson<LikeSnapshot_server_to_client>(socketpp.receiveMsg); //서버에서 전달받은 것을 클라이언트로 전달
        if(likeSnapshotResult.action == "ok") //좋아요 ok
        {
            Debug.Log("success");
            if(like_type == "add") //add상태면
            {
                current.transform.parent.GetComponent<SnapshotUid>().snapshot_like++;
                current.GetComponent<Button>().onClick.RemoveAllListeners();
                current.GetComponent<Button>().onClick.AddListener(() => LikeBtn("True"));
                current.GetComponent<Image>().color = new Color(200, 200 / 255, 200 / 255);
            }
            else if(like_type == "delete") //delete상태면
            {
                current.transform.parent.GetComponent<SnapshotUid>().snapshot_like--;
                current.GetComponent<Button>().onClick.RemoveAllListeners();
                current.GetComponent<Button>().onClick.AddListener(() => LikeBtn("False"));
                current.GetComponent<Image>().color = new Color(30 / 255, 225 / 255, 200 / 255);
            }
        }
    }

    //타임라인 불러오기
    public void timeline_client_to_server()
    {
        Timeline_client_to_server timeline = new Timeline_client_to_server();
        timeline.uid = socketpp.player_uid;
        timeline.count = snapshot_count;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(timeline)); //사용자 uid, 스냅샷 리스트 번호를 클라이언트에서 서버로 전달
        Timeline_server_to_client timeline_snapshot = JsonUtility.FromJson<Timeline_server_to_client>(socketpp.receiveMsg); //서버에서 전달받은 것을 클라이언트로 전달
        snapshot_count++; //스냅샷 리스트 번호 증가

        //스냅샷 이미지 가져오기
        CheckProfileImage_client_to_server checkprofile = new CheckProfileImage_client_to_server();
        int[] uid_list = new int[timeline_snapshot.info.Length];
        for (int i = 0; i < uid_list.Length; i++)
        {
            uid_list[i] = timeline_snapshot.info[i].uid;
        }
        checkprofile.uid = uid_list.Distinct().ToArray();
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(checkprofile));
        CheckProfileImage_server_to_client checkprofiletime = JsonUtility.FromJson<CheckProfileImage_server_to_client>(socketpp.receiveMsg);
        Dictionary<int, string> timedict = new Dictionary<int, string>();
        Dictionary<int, int> sizedict = new Dictionary<int, int>();
        for (int i = 0; i < checkprofile.uid.Length; i++)
        {
            timedict.Add(checkprofile.uid[i], checkprofiletime.timestamp[i]);
            sizedict.Add(checkprofile.uid[i], checkprofiletime.size[i]);
        }


        for (int i = 0; i < 4; i++)
        {
            MakeClone(timeline_snapshot.info[i].uid, timeline_snapshot.info[i].nickname, timeline_snapshot.info[i].like_num, timeline_snapshot.info[i].timestamp, timeline_snapshot.info[i].snapshot_intro, timeline_snapshot.info[i].like, timeline_snapshot.info[i].size, timedict[timeline_snapshot.info[i].uid], sizedict[timeline_snapshot.info[i].uid]); //스냅샷 정보로 스냅샷 Prefab생성
        }
    }
    
    //스냅샷 Prefab생성
    public void MakeClone(int timeline_snapshot_user_uid, string timeline_snapshot_nickname, int timeline_snapshot_like_num, string timeline_snapshot_timestamp, string timeline_snapshot_text, string timeline_snapshot_like, int timeline_snapshot_size, string timeline_profile_timestamp, int timeline_profile_size)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/" + timeline_snapshot_user_uid.ToString())) 
            Directory.CreateDirectory(Application.persistentDataPath + "/" + timeline_snapshot_user_uid.ToString() + "/"); //플레이어uid폴더 없을 시 생성
        GameObject clone_snapshot = Instantiate(snapshot) as GameObject;
        clone_snapshot.transform.SetParent(this.transform);
        clone_snapshot.transform.localPosition = Vector3.zero;
        clone_snapshot.transform.localScale = Vector3.one;

        //스냅샷 정보 기록
        clone_snapshot.GetComponent<PrefabUid>().uid = timeline_snapshot_user_uid;
        clone_snapshot.GetComponent<PrefabUid>().nickname = timeline_snapshot_nickname;
        clone_snapshot.GetComponent<SnapshotUid>().snapshot_uid = timeline_snapshot_timestamp;
        clone_snapshot.GetComponent<SnapshotUid>().snapshot_intro = timeline_snapshot_text;
        clone_snapshot.GetComponent<SnapshotUid>().snapshot_like = timeline_snapshot_like_num;

        GameObject clone_snapshot_profile = clone_snapshot.transform.Find("ProfileImage").gameObject;
        clone_snapshot_profile.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().ActiveFriendPage()); //프로필 이미지에 페이지로 가기 추가
        
        //스냅샷 프로필 이미지 불러오기
        string profile_path = timeline_snapshot_user_uid.ToString() + "/" + timeline_snapshot_user_uid.ToString() + "_" + timeline_profile_timestamp + ".png";
        
        if (!File.Exists(Application.persistentDataPath + "/" + profile_path))
        {
            socketpp.localDown(profile_path);
            Socketpp.ImgQueue iq = new Socketpp.ImgQueue();
            iq.img = clone_snapshot_profile.GetComponent<Image>();
            iq.path = Application.persistentDataPath + "/" + profile_path;
            iq.size = timeline_profile_size;
            socketpp._imgqueue.Add(iq);
        }
        else
        {
            while (true)
            {
                FileInfo info = new FileInfo(Application.persistentDataPath + "/" + profile_path);
                if (info.Length == timeline_profile_size)
                {
                    clone_snapshot_profile.GetComponent<Image>().sprite = GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + profile_path);
                    break;
                }
            }
        }


        GameObject clone_snapshot_snapshot = clone_snapshot.transform.Find("SnapshotImage").gameObject;
        clone_snapshot_snapshot.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().OnSnapshotScene()); //스냅샷 이미지에 페이지로 가기 추가

        //스냅샷 페이지 이미지 불러오기
        string path = timeline_snapshot_user_uid.ToString() + "/" + timeline_snapshot_user_uid.ToString() + "_" + timeline_snapshot_timestamp + ".png";
        if (!File.Exists(Application.persistentDataPath + "/" + path))
        {
            socketpp.localDown(path);
            Socketpp.ImgQueue iq = new Socketpp.ImgQueue();
            iq.img = clone_snapshot_snapshot.GetComponent<Image>();
            iq.path = Application.persistentDataPath + "/" + path;
            iq.size = timeline_snapshot_size;
            socketpp._imgqueue.Add(iq);
        }
        else
        {
            clone_snapshot_snapshot.GetComponent<Image>().sprite = GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + path);
        }

        GameObject clone_snapshot_text = clone_snapshot.transform.Find("ProfileText").gameObject;
        clone_snapshot_text.GetComponent<Text>().text = timeline_snapshot_nickname; //스냅샷 텍스트 불러오기
        GameObject clone_snapshot_time = clone_snapshot.transform.Find("SnapshotTime").gameObject;
        clone_snapshot_time.GetComponent<Text>().text = GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().ParseDateTime(timeline_snapshot_timestamp); //스냅샷 타임스탬프 불러오기
        GameObject clone_snapshot_likebtn = clone_snapshot.transform.Find("SnapshotLikeButton").gameObject;
        clone_snapshot_likebtn.GetComponent<Button>().onClick.AddListener(() => LikeBtn(timeline_snapshot_like)); //스냅샷 좋아요 버튼 추가
        if(timeline_snapshot_like == "True")
        {
            clone_snapshot_likebtn.GetComponent<Image>().color = new Color(200, 200 / 255, 200 / 255);
        }
        else if(timeline_snapshot_like == "False")
        {
            clone_snapshot_likebtn.GetComponent<Image>().color = new Color(30 / 255, 225 / 255, 200 / 255);
        }
    }
}