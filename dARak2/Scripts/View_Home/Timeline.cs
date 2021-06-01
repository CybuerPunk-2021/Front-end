using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Timeline : MonoBehaviour
{
    public GameObject snapshot;
    public int snapshot_count = 0;

    Socketpp socketpp;

    // Start is called before the first frame update
    void Start()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
        timeline_client_to_server();
    }
    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateBtn()
    {
        timeline_client_to_server();
    }

    public void LikeBtn(string timeline_snapshot_like)
    {
        Debug.Log(timeline_snapshot_like);
        if(timeline_snapshot_like == "True")
            likeSnapshot_client_to_server("delete");
        else if(timeline_snapshot_like == "False")
            likeSnapshot_client_to_server("add");
    }

    public void likeSnapshot_client_to_server(string like_type)
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;
        LikeSnapshot_client_to_server likeSnapshot = new LikeSnapshot_client_to_server();
        likeSnapshot.type = like_type;
        likeSnapshot.from_uid = socketpp.player_uid;
        likeSnapshot.to_uid = current.transform.parent.GetComponent<PrefabUid>().uid;
        likeSnapshot.timestamp = current.transform.parent.GetComponent<SnapshotUid>().snapshot_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(likeSnapshot));
        LikeSnapshot_server_to_client likeSnapshotResult = JsonUtility.FromJson<LikeSnapshot_server_to_client>(socketpp.receiveMsg);
        if(likeSnapshotResult.action == "ok")
        {
            Debug.Log("success");
            if(like_type == "add")
            {
                current.transform.parent.GetComponent<SnapshotUid>().snapshot_like++;
                current.GetComponent<Button>().onClick.RemoveAllListeners();
                current.GetComponent<Button>().onClick.AddListener(() => LikeBtn("True"));
                current.GetComponent<Image>().color = new Color(200, 200 / 255, 200 / 255);
            }
            else if(like_type == "delete")
            {
                current.transform.parent.GetComponent<SnapshotUid>().snapshot_like--;
                current.GetComponent<Button>().onClick.RemoveAllListeners();
                current.GetComponent<Button>().onClick.AddListener(() => LikeBtn("False"));
                current.GetComponent<Image>().color = new Color(30 / 255, 225 / 255, 200 / 255);
            }
        }
    }

    public void timeline_client_to_server()
    {
        Timeline_client_to_server timeline = new Timeline_client_to_server();
        timeline.uid = socketpp.player_uid;
        timeline.count = snapshot_count;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(timeline));
        Timeline_server_to_client timeline_snapshot = JsonUtility.FromJson<Timeline_server_to_client>(socketpp.receiveMsg);
        snapshot_count++;
        for (int i = 0; i < 4; i++)
        {
            MakeClone(timeline_snapshot.info[i].uid, timeline_snapshot.info[i].nickname, timeline_snapshot.info[i].like_num, timeline_snapshot.info[i].timestamp, timeline_snapshot.info[i].snapshot_intro, timeline_snapshot.info[i].like);
        }
    }

    public void MakeClone(int timeline_snapshot_user_uid, string timeline_snapshot_nickname, int timeline_snapshot_like_num, string timeline_snapshot_timestamp, string timeline_snapshot_text, string timeline_snapshot_like)
    {
        GameObject clone_snapshot = Instantiate(snapshot) as GameObject;
        clone_snapshot.transform.SetParent(this.transform);
        clone_snapshot.transform.localPosition = Vector3.zero;
        clone_snapshot.transform.localScale = Vector3.one;

        clone_snapshot.GetComponent<PrefabUid>().uid = timeline_snapshot_user_uid;
        clone_snapshot.GetComponent<PrefabUid>().nickname = timeline_snapshot_nickname;
        clone_snapshot.GetComponent<SnapshotUid>().snapshot_uid = timeline_snapshot_timestamp;
        clone_snapshot.GetComponent<SnapshotUid>().snapshot_intro = timeline_snapshot_text;
        clone_snapshot.GetComponent<SnapshotUid>().snapshot_like = timeline_snapshot_like_num;

        GameObject clone_snapshot_profile = clone_snapshot.transform.Find("ProfileImage").gameObject;
        clone_snapshot_profile.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("View_Main").GetComponent<MainSceneScript>().ActiveFriendPage());
        //GameObject profile_image = clone_snapshot.transform.Find("ProfileImage").gameObject;
        //profile_image.GetComponent<Image>().sprite = Resources.Load<Sprite>("");
        GameObject clone_snapshot_snapshot = clone_snapshot.transform.Find("SnapshotImage").gameObject;
        clone_snapshot_snapshot.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("View_Main").GetComponent<MainSceneScript>().OnSnapshotScene());
        //GameObject snapshot_image = clone_snapshot.transform.Find("SnapshotImage").gameObject;
        //snapshot_image.GetComponent<Image>().sprite = Resources.Load<Sprite>("");
        GameObject clone_snapshot_text = clone_snapshot.transform.Find("ProfileText").gameObject;
        clone_snapshot_text.GetComponent<Text>().text = timeline_snapshot_nickname;
        GameObject clone_snapshot_likebtn = clone_snapshot.transform.Find("SnapshotLikeButton").gameObject;
        clone_snapshot_likebtn.GetComponent<Button>().onClick.AddListener(() => LikeBtn(timeline_snapshot_like));
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
