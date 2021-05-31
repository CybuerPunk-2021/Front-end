using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditSnapshotScript : MonoBehaviour
{
    public InputField snapshotTextField;
    Socketpp socketpp;

    // Start is called before the first frame update
    void Start()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateText()
    {
        snapshotTextField.text = socketpp.snapshot_intro;
    }

    public void changeSnapshotDescription_client_to_server()
    {
        ChangeSnapshotDescription_client_to_server change_snapshot = new ChangeSnapshotDescription_client_to_server();
        change_snapshot.uid = socketpp.player_uid;
        change_snapshot.timestamp = socketpp.snapshot_timestamp;
        change_snapshot.introduce = snapshotTextField.text;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(change_snapshot));
    }
    public void deleteSnapshotDescription_client_to_server()
    {
        DeleteSnapshotDescription_client_to_server delete_snapshot = new DeleteSnapshotDescription_client_to_server();
        delete_snapshot.uid = socketpp.player_uid;
        delete_snapshot.timestamp = socketpp.snapshot_timestamp;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(delete_snapshot));
    }
    public void changeSnapshotBtn()
    {
        changeSnapshotDescription_client_to_server();
        GameObject.Find("View_Main").GetComponent<MainSceneScript>().UnActiveEditSnapshotPage();
    }
    public void deleteSnapshotBtn()
    {
        deleteSnapshotDescription_client_to_server();
        GameObject.Find("View_Main").GetComponent<MainSceneScript>().UnActiveEditSnapshotPage();
    }
}
