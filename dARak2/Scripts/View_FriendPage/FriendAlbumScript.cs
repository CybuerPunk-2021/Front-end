using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendAlbumScript : MonoBehaviour
{
    public GameObject albumImage;
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
    }
    public void album_client_to_server()
    {
        Album_client_to_server album = new Album_client_to_server();
        album.uid = socketpp.other_player_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(album));
        Album_server_to_client album_snapshot = JsonUtility.FromJson<Album_server_to_client>(socketpp.receiveMsg);
        for (int i = 0; i < 50; i++)
        {
            MakeClone(album_snapshot.snapshot[i].snapshot_intro, album_snapshot.snapshot[i].like_num, album_snapshot.snapshot[i].timestamp);
        }
    }

    public void MakeClone(string snapshot_text, int snapshot_like, string snapshot_timestamp)
    {
        GameObject clone_albumImage = Instantiate(albumImage) as GameObject;
        clone_albumImage.transform.SetParent(this.transform);
        clone_albumImage.transform.localPosition = Vector3.zero;
        clone_albumImage.transform.localScale = Vector3.one;

        clone_albumImage.GetComponent<PrefabUid>().uid = socketpp.other_player_uid;
        clone_albumImage.GetComponent<PrefabUid>().nickname = socketpp.other_nickname;
        clone_albumImage.GetComponent<SnapshotUid>().snapshot_uid = snapshot_timestamp;
        clone_albumImage.GetComponent<SnapshotUid>().snapshot_intro = snapshot_text;
        clone_albumImage.GetComponent<SnapshotUid>().snapshot_like = snapshot_like;
        clone_albumImage.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("View_Main").GetComponent<MainSceneScript>().OnAlbumSnapshotScene());
        GameObject clone_albumImage_btn = clone_albumImage.transform.Find("AlbumImageEditBtn").gameObject;
        clone_albumImage_btn.SetActive(false);
    }
}
