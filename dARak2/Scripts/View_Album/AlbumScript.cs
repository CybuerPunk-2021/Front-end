using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlbumScript : MonoBehaviour
{
    public GameObject albumImage;
    private int snapshot_count = 0;
    Socketpp socketpp;

    // Start is called before the first frame update
    void Start()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
        album_client_to_server();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateAlbum()
    {
        EraseAlbum();
        album_client_to_server();
    }

    public void EraseAlbum()
    {
        GameObject[] albumsnapshots = GameObject.FindGameObjectsWithTag("AlbumSnapshot");
        foreach (GameObject albumsnapshot in albumsnapshots)
        {
            Destroy(albumsnapshot);
        }
        snapshot_count = 0;
    }
    public void album_client_to_server()
    {
        Timeline_client_to_server album = new Timeline_client_to_server();
        album.uid = socketpp.player_uid;
        album.count = snapshot_count;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(album));
        Timeline_server_to_client snapshot = JsonUtility.FromJson<Timeline_server_to_client>(socketpp.receiveMsg);
        for (int i = 0; i < 50; i++)
        {
            MakeClone(snapshot.info[i].uid, snapshot.info[i].nickname, snapshot.info[i].timestamp, snapshot.info[i].snapshot_intro, snapshot.info[i].like);
        }
        snapshot_count++;
    }

    public void MakeClone(int snapshot_user_uid, string snapshot_nickname, string snapshot_timestamp, string snapshot_text, string snapshot_like)
    {
        GameObject clone_albumImage = Instantiate(albumImage) as GameObject;
        clone_albumImage.transform.SetParent(this.transform);
        clone_albumImage.transform.localPosition = Vector3.zero;
        clone_albumImage.transform.localScale = Vector3.one;

        clone_albumImage.GetComponent<PrefabUid>().uid = snapshot_user_uid;
        clone_albumImage.GetComponent<PrefabUid>().nickname = snapshot_nickname;
        clone_albumImage.GetComponent<SnapshotUid>().snapshot_uid = snapshot_timestamp;
        clone_albumImage.GetComponent<SnapshotUid>().snapshot_intro = snapshot_text;
        clone_albumImage.GetComponent<SnapshotUid>().snapshot_like = snapshot_like;
        clone_albumImage.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("View_Main").GetComponent<MainSceneScript>().OnAlbumSnapshotScene());
        GameObject clone_albumImage_btn = clone_albumImage.transform.Find("AlbumImageEditBtn").gameObject;
        clone_albumImage_btn.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("View_Main").GetComponent<MainSceneScript>().ActiveEditSnapshotPage());
    }
}
