using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class ButtonUI : MonoBehaviour
{
    Socketpp socketpp;
    public GameObject saveMessege = null;
    //GameObject loading;

    // Start is called before the first frame update
    void Start()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
        //loading = GameObject.Find("Loading");
        //loading.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackScene()
    {
        SceneManager.LoadScene("Main");
        GameObject.Find("MyPagePannel_Content").GetComponent<MyPageScript>().profile_client_to_server();
    }
    
    public void snapshot_save()
    {
        //loading.SetActive(true);
        Snapshot_save save = new Snapshot_save();
        save.uid = socketpp.player_uid;
        save.snapshot_intro = socketpp.snapshot_intro;

        GameObject myRoom = GameObject.Find("Myroom");
        Transform[] roomObj = myRoom.GetComponentsInChildren<Transform>();
        Debug.Log("애들 갯수" + myRoom.transform.childCount);
        int j = 0;
        for (int i = 0; i < roomObj.Length; i++)
        {
            if (!((roomObj[i].name == myRoom.name) || roomObj[i].CompareTag("Untagged")))
                j++;
        }
        string[] ilist = new string[myRoom.transform.childCount];
        j = 0;
        for (int i = 0; i < roomObj.Length; i++)
        {
            if ((roomObj[i].name == myRoom.name) || roomObj[i].CompareTag("Untagged"))
            {
                Debug.Log(roomObj[i].name);
                continue;
            }

            SaveItemList sil = new SaveItemList();
            
            sil.iid = int.Parse(roomObj[i].tag);
            sil.rotation = new float[3] { roomObj[i].eulerAngles.x, roomObj[i].eulerAngles.y, roomObj[i].eulerAngles.z };
            sil.position = new float[3] { roomObj[i].position.x, roomObj[i].position.y, roomObj[i].position.z };
            sil.scale = new float[3] { roomObj[i].localScale.x, roomObj[i].localScale.y, roomObj[i].localScale.z };

            Debug.Log(sil.rotation);

            ilist[j] = JsonUtility.ToJson(sil);
            j++;
        }
        save.item_list = ilist;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(save).Replace("\\", "").Replace("\"{", "{").Replace("}\"", "}"));

        socketpp.player_recent_timestamp = JsonUtility.FromJson<SaveOK>(socketpp.receiveMsg).timestamp;
        socketpp.snapshot_timestamp = JsonUtility.FromJson<SaveOK>(socketpp.receiveMsg).timestamp;

        StartCoroutine(SnapShot());
        StartCoroutine(Wait());
        saveMessege.SetActive(true);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator SnapShot()
    {
        //loading.SetActive(true);
        yield return new WaitForEndOfFrame();
        byte[] imageByte;
        Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, true);
        //Debug.Log((Screen.width, Screen.height));

        Texture2D newTex = new Texture2D(Screen.width, Screen.width);
        for(int y = 0; y < Screen.width; y++)
        {
            for(int x = 0; x < Screen.width; x++)
            {
                newTex.SetPixel(x, y, tex.GetPixel(x, (Screen.height - Screen.width)/2 + y));
            }
        }

        newTex.Apply();
        imageByte = newTex.EncodeToPNG();
        DestroyImmediate(tex);
        DestroyImmediate(newTex);

        string uidstr = socketpp.player_uid.ToString();
        if (!Directory.Exists(Application.persistentDataPath + "/" + uidstr))
            Directory.CreateDirectory(Application.persistentDataPath + "/" + uidstr);

        string filepath = uidstr + "/" + uidstr + "_" + socketpp.player_recent_timestamp + ".png";
        Debug.Log(uidstr);
        File.WriteAllBytes(Application.persistentDataPath + "/" + filepath, imageByte);
        Debug.Log("정상작동");

        socketpp.OnClickFileUpload(filepath, imageByte);
        //Debug.Log("정상작동!");
    }

    /*
    public void snapshot_roominfo()
    {
        Snapshot_roominfo info = new Snapshot_roominfo();
        info.uid = 200;
        info.timestamp = "2016-12-25";
        //Debug.Log(JsonUtility.ToJson(info));
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(info));
        //Debug.Log(socketpp.receiveMsg);
    }*/
}
