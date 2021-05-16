using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Timeline : MonoBehaviour
{
    // Start is called before the first frame update
    public Scrollbar scroll_bar;
    public GameObject snapshot;
    public int snapshot_count = 0;

    Socketpp socketpp;

    void Start()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
    }
    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateBtn()
    {
        timeline_client_to_server();
    }

    public void timeline_client_to_server()
    {
        Timeline_client_to_server timeline = new Timeline_client_to_server();
        timeline.uid = socketpp.player_uid;
        timeline.count = snapshot_count;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(timeline));
        Timeline_server_to_client snapshot = JsonUtility.FromJson<Timeline_server_to_client>(socketpp.receiveMsg);
        //만약에 배열이 공백이 아니면
        for (int i = 0; i < 2; i++)
        {
                MakeClone(snapshot.info[i].uid, snapshot.info[i].nickname);
        }
        snapshot_count++;
    }

    public void MakeClone(int snapshot_uid, string sanpshot_nickname)
    {
        GameObject clone_snapshot = Instantiate(snapshot) as GameObject;
        clone_snapshot.transform.SetParent(this.transform);
        clone_snapshot.transform.localPosition = Vector3.zero;
        clone_snapshot.transform.localScale = Vector3.one;
        clone_snapshot.GetComponent<PrefabUid>().uid = snapshot_uid;
        clone_snapshot.GetComponent<PrefabUid>().nickname = sanpshot_nickname;
        //GameObject profile_image = clone_snapshot.transform.Find("ProfileImage").gameObject;
        //profile_image.GetComponent<Image>().sprite = Resources.Load<Sprite>("");
        GameObject clone_snapshot_text = clone_snapshot.transform.Find("ProfileText").gameObject;
        clone_snapshot_text.GetComponent<Text>().text = sanpshot_nickname;
        //GameObject snapshot_image = clone_snapshot.transform.Find("SnapshotImage").gameObject;
        //snapshot_image.GetComponent<Image>().sprite = Resources.Load<Sprite>("");
    }
}
