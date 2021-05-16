using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonUI : MonoBehaviour
{
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

    public void BackScene()
    {
        SceneManager.LoadScene("Main");
    }
    
    public void snapshot_save()
    {
        Snapshot_save save = new Snapshot_save();
        save.uid = 1025063594;
        save.snapshot_intro = "asdf";

        GameObject myRoom = GameObject.Find("Myroom");
        Transform[] roomObj = myRoom.GetComponentsInChildren<Transform>();
        string[] ilist = new string[roomObj.Length - 1];
        int j = 0;
        for (int i = 0; i <= myRoom.transform.childCount; i++)
        {
            if (roomObj[i].name == myRoom.name)
                continue;

            SaveItemList sil = new SaveItemList();
            Debug.Log(roomObj[i].tag);
            sil.iid = int.Parse(roomObj[i].tag);
            sil.rotation = new float[3] { roomObj[i].eulerAngles.x, roomObj[i].eulerAngles.y, roomObj[i].eulerAngles.z };
            sil.position = new float[3] { roomObj[i].position.x, roomObj[i].position.y, roomObj[i].position.z };
            sil.scale = new float[3] { roomObj[i].localScale.x, roomObj[i].localScale.y, roomObj[i].localScale.z };

            ilist[j] = JsonUtility.ToJson(sil);
            Debug.Log(ilist[j]);
            j++;
        }
        save.item_list = ilist;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(save).Replace("\\", "").Replace("\"{", "{").Replace("}\"", "}"));

        socketpp.timeStamp = JsonUtility.FromJson<SaveOK>(socketpp.receiveMsg).timestamp;
    }

    public void snapshot_roominfo()
    {
        Snapshot_roominfo info = new Snapshot_roominfo();
        info.uid = 200;
        info.timestamp = "2016-12-25";
        //Debug.Log(JsonUtility.ToJson(info));
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(info));
        //Debug.Log(socketpp.receiveMsg);
    }
}
