using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class FriendAlbumScript : MonoBehaviour
{
    public GameObject albumImage;
    public int friend_album_snapshot_count = 0; //친구 스냅샷 리스트 번호
    Socketpp socketpp;

    // Start is called before the first frame update
    void Awake()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>(); //소켓 불러오기
    }
    //켜질시
    void OnEnable()
    {
        EraseAlbum(); //앨범내 스냅샷 Prefab삭제
        album_client_to_server();//앨범 스냅샷 불러오기
    }
    // 더보기 버튼 클릭 시 앨범 스냅샷 불러오기
    public void UpdateAlbumBtn()
    {
        album_client_to_server();
    }
    //앨범 스냅샷 Prefab삭제
    public void EraseAlbum()
    {
        GameObject[] albumsnapshots = GameObject.FindGameObjectsWithTag("AlbumSnapshot");
        foreach (GameObject albumsnapshot in albumsnapshots)
        {
            Destroy(albumsnapshot);
        }
        friend_album_snapshot_count = 0; //리스트 번호 0으로 초기화
    }
    //앨범 스냅샷 불러오기
    public void album_client_to_server()
    {
        Album_client_to_server album = new Album_client_to_server();
        if (!Directory.Exists(Application.persistentDataPath + "/" + socketpp.other_player_uid.ToString()))
            Directory.CreateDirectory(Application.persistentDataPath + "/" + socketpp.other_player_uid.ToString() + "/");//플레이어 uid 폴더 없을 시 생성
        album.uid = socketpp.other_player_uid;
        album.count = friend_album_snapshot_count;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(album));//현재 사용자 uid와 스냅샷 리스트 번호 클라이언트에서 서버로 전달
        Album_server_to_client album_snapshot = JsonUtility.FromJson<Album_server_to_client>(socketpp.receiveMsg);//서버에서 전달받은 것을 클라이언트로 전달
        friend_album_snapshot_count++; //리스트 번호 증가
        for (int i = 0; i < 4; i++)
        {
            MakeClone(album_snapshot.snapshot[i].snapshot_intro, album_snapshot.snapshot[i].like_num, album_snapshot.snapshot[i].timestamp, album_snapshot.snapshot[i].size);//서버에서 클라이언트로 가져온 정보로 앨범 스냅샷 생성
        }
    }
    //앨범 스냅샷 prefab생성
    public void MakeClone(string snapshot_text, int snapshot_like, string snapshot_timestamp, int snapshot_size)
    {
        GameObject clone_albumImage = Instantiate(albumImage) as GameObject;
        clone_albumImage.transform.SetParent(this.transform);
        clone_albumImage.transform.localPosition = Vector3.zero;
        clone_albumImage.transform.localScale = Vector3.one;

        clone_albumImage.GetComponent<PrefabUid>().uid = socketpp.other_player_uid; //앨범이미지 uid = 다른 사용자 uid
        clone_albumImage.GetComponent<PrefabUid>().nickname = socketpp.other_nickname; //앨범이미지 닉네임 = 다른 사용자 닉네임
        clone_albumImage.GetComponent<SnapshotUid>().snapshot_uid = snapshot_timestamp; //앨범이미지 타임스템프
        clone_albumImage.GetComponent<SnapshotUid>().snapshot_intro = snapshot_text; //앨범이미지 설명
        clone_albumImage.GetComponent<SnapshotUid>().snapshot_like = snapshot_like; //앨범 좋아요 정보
        clone_albumImage.GetComponent<Button>().onClick.AddListener(() => GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().OnAlbumSnapshotScene());//앨범이미지 클릭 시 앨범스냅샷 Scene실행
        //앨범 이미지 불러오기
        string path = socketpp.other_player_uid.ToString() + "/" + socketpp.other_player_uid.ToString() + "_" + snapshot_timestamp + ".png";
        if (!File.Exists(Application.persistentDataPath + "/" + path))
        {
            socketpp.localDown(path);
            Socketpp.ImgQueue iq = new Socketpp.ImgQueue();
            iq.img = clone_albumImage.GetComponent<Image>();
            iq.path = Application.persistentDataPath + "/" + path;
            iq.size = snapshot_size;
            socketpp._imgqueue.Add(iq);
        }
        else
        {
            while (true)
            {
                FileInfo info = new FileInfo(Application.persistentDataPath + "/" + path);
                if (info.Length == snapshot_size)
                {
                    clone_albumImage.GetComponent<Image>().sprite = GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().SystemIOFileLoad(Application.persistentDataPath + "/" + path);
                    break;
                }
            }
        }

        GameObject clone_albumImage_btn = clone_albumImage.transform.Find("AlbumImageEditBtn").gameObject;
        clone_albumImage_btn.SetActive(false);//앨범 이미지 수정 버튼 끄기
    }
}
