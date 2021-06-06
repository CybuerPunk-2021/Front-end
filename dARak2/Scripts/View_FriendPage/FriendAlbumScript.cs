using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class FriendAlbumScript : MonoBehaviour
{
    public GameObject albumImage;
    public int friend_album_snapshot_count = 0;
    Socketpp socketpp;

    // Start is called before the first frame update
    void Awake()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
    }
    void OnEnable()
    {
        EraseAlbum();
        album_client_to_server();
    }

    public void UpdateAlbumBtn()
    {
        album_client_to_server();
    }

    public void EraseAlbum()
    {
        GameObject[] albumsnapshots = GameObject.FindGameObjectsWithTag("AlbumSnapshot");
        foreach (GameObject albumsnapshot in albumsnapshots)
        {
            Destroy(albumsnapshot);
        }
        friend_album_snapshot_count = 0;
    }
    public void album_client_to_server()
    {
        Album_client_to_server album = new Album_client_to_server();
        if (!Directory.Exists(Application.persistentDataPath + "/" + socketpp.other_player_uid.ToString()))
            Directory.CreateDirectory(Application.persistentDataPath + "/" + socketpp.other_player_uid.ToString() + "/");
        album.uid = socketpp.other_player_uid;
        album.count = friend_album_snapshot_count;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(album));
        Album_server_to_client album_snapshot = JsonUtility.FromJson<Album_server_to_client>(socketpp.receiveMsg);
        friend_album_snapshot_count++;
        for (int i = 0; i < 4; i++)
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

        string path = socketpp.other_player_uid.ToString() + "/" + socketpp.other_player_uid.ToString() + "_" + snapshot_timestamp + ".png";
        if (!File.Exists(Application.persistentDataPath + "/" + path))
        {
            socketpp.localDown(path);
            Socketpp.ImgQueue iq = new Socketpp.ImgQueue();
            iq.img = clone_albumImage.GetComponent<Image>();
            iq.path = Application.persistentDataPath + "/" + path;
            socketpp._imgqueue.Add(iq);
        }
        else
        {
            clone_albumImage.GetComponent<Image>().sprite = GameObject.Find("View_Main").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + path);
        }

        GameObject clone_albumImage_btn = clone_albumImage.transform.Find("AlbumImageEditBtn").gameObject;
        clone_albumImage_btn.SetActive(false);
    }
}
