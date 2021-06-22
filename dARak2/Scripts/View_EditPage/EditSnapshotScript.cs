using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditSnapshotScript : MonoBehaviour
{
    public InputField snapshotTextField;
    public GameObject popup;
    Socketpp socketpp;

    // Start is called before the first frame update
    void Awake()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>(); //소켓 불러오기
    }
    //켜질시
    void OnEnable()
    {
        snapshotTextField.text = socketpp.snapshot_intro; //스냅샷 수정창 텍스트 = 스냅샷 설명
    }

    //스냅샷 설명 수정
    public void changeSnapshotDescription_client_to_server()
    {
        ChangeSnapshotDescription_client_to_server change_snapshot = new ChangeSnapshotDescription_client_to_server();
        change_snapshot.uid = socketpp.player_uid;
        change_snapshot.timestamp = socketpp.snapshot_timestamp;
        change_snapshot.introduce = snapshotTextField.text;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(change_snapshot)); //사용자 uid, 스냅샷 타임스탬프, 스냅샷 텍스트 필드 값 클라이언트에서 서버로 전달
    }

    //스냅샷 삭제
    public void deleteSnapshotDescription_client_to_server()
    {
        DeleteSnapshotDescription_client_to_server delete_snapshot = new DeleteSnapshotDescription_client_to_server();
        delete_snapshot.uid = socketpp.player_uid;
        delete_snapshot.timestamp = socketpp.snapshot_timestamp;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(delete_snapshot)); //사용자 uid, 스냅샷 타임스탬프 클라이언트에서 서버로 전달
    }

    //스냅샷 수정 버튼
    public void changeSnapshotBtn()
    {
        changeSnapshotDescription_client_to_server(); //스냅샷 설명 수정
        GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().UnActiveEditSnapshotPage();
        popup.transform.GetChild(0).GetComponent<Text>().text = "수정 완료";
        popup.SetActive(true);
    }

    //스냅샷 삭제 버튼
    public void deleteSnapshotBtn()
    {
        deleteSnapshotDescription_client_to_server(); //스냅샷 삭제
        GameObject.Find("MasterCanvas").GetComponent<MainSceneScript>().UnActiveEditSnapshotPage();
        popup.transform.GetChild(0).GetComponent<Text>().text = "삭제 완료";
        popup.SetActive(true);
    }
}
