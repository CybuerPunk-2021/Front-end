using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class EditPageScript : MonoBehaviour
{
    public InputField profileNameField, profileEmailField, profileEmailAuthField, profileIntroduceField, profilePasswordField, profileNewPasswordField;
    public GameObject popup;
    Socketpp socketpp;

    // Start is called before the first frame update
    void Awake()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>(); //소켓불러오기
    }
    //켜질시
    void OnEnable()
    {
        profile_client_to_server(); //프로필 정보 가져오기
    }

    //이미지 크기 축소 후 텍스처 화질 높이기
    public static Texture2D ResizeTexture(Texture2D source, int sizex, int sizey)
    {
        //*** Get All the source pixels
        Color[] aSourceColor = source.GetPixels(0);
        Vector2 vSourceSize = new Vector2(source.width, source.height);

        //*** Calculate New Size
        float xWidth = sizex;
        float xHeight = sizey;

        //*** Make New
        Texture2D oNewTex = new Texture2D((int)xWidth, (int)xHeight, TextureFormat.RGBA32, false);

        //*** Make destination array
        int xLength = (int)xWidth * (int)xHeight;
        Color[] aColor = new Color[xLength];

        Vector2 vPixelSize = new Vector2(vSourceSize.x / xWidth, vSourceSize.y / xHeight);

        //*** Loop through destination pixels and process
        Vector2 vCenter = new Vector2();
        for (int ii = 0; ii < xLength; ii++)
        {
            //*** Figure out x&y
            float xX = (float)ii % xWidth;
            float xY = Mathf.Floor((float)ii / xWidth);

            //*** Calculate Center
            vCenter.x = (xX / xWidth) * vSourceSize.x;
            vCenter.y = (xY / xHeight) * vSourceSize.y;

            //*** Average
            //*** Calculate grid around point
            int xXFrom = (int)Mathf.Max(Mathf.Floor(vCenter.x - (vPixelSize.x * 0.5f)), 0);
            int xXTo = (int)Mathf.Min(Mathf.Ceil(vCenter.x + (vPixelSize.x * 0.5f)), vSourceSize.x);
            int xYFrom = (int)Mathf.Max(Mathf.Floor(vCenter.y - (vPixelSize.y * 0.5f)), 0);
            int xYTo = (int)Mathf.Min(Mathf.Ceil(vCenter.y + (vPixelSize.y * 0.5f)), vSourceSize.y);

            //*** Loop and accumulate
            Color oColorTemp = new Color();
            float xGridCount = 0;
            for (int iy = xYFrom; iy < xYTo; iy++)
            {
                for (int ix = xXFrom; ix < xXTo; ix++)
                {

                    //*** Get Color
                    oColorTemp += aSourceColor[(int)(((float)iy * vSourceSize.x) + ix)];

                    //*** Sum
                    xGridCount++;
                }
            }

            //*** Average Color
            aColor[ii] = oColorTemp / (float)xGridCount;
        }

        //*** Set Pixels
        oNewTex.SetPixels(aColor);
        oNewTex.Apply();

        //*** Return
        return oNewTex;
    }

    //이미지 프로필 변경
    public void ChangeProfileImage()
    {
#if UNITY_EDITOR

#else
            NativeGallery.GetImageFromGallery(callbackForGalleryProfile); //구글 네이티브 갤러리에서 이미지 가져오기
#endif

        popup.transform.GetChild(0).GetComponent<Text>().text = "수정 완료";
        popup.SetActive(true);
    }
    //갤러리 사진 이미지 경로 가져오기
    private void callbackForGalleryProfile(string path)
    {
        UpdateImageProfile(path);
    }
    //이미지 경로로 프로필 업데이트
    void UpdateImageProfile(string path)
    {
        //가져온 이미지로 크기 변경 후 PNG로 변경
        byte[] imageByte = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(250, 250, TextureFormat.RGB24, true);
        texture.LoadImage(imageByte);
        texture.filterMode = FilterMode.Trilinear;
        Texture2D newTexture = ResizeTexture(texture, 250, 250);

        imageByte = newTexture.EncodeToPNG();
        
        ChangeProfileImage_client_to_server profilechange = new ChangeProfileImage_client_to_server();
        profilechange.uid = socketpp.player_uid;
        profilechange.size = imageByte.Length;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(profilechange)); //사용자 uid와 이미지 크기로 클라이언트에서 서버 보내기
        ChangeProfileImage_server_to_client profilechangetime = JsonUtility.FromJson<ChangeProfileImage_server_to_client>(socketpp.receiveMsg); //서버에서 전달받은 것을 클라이언트로 전달
        socketpp.player_profile_timestamp = profilechangetime.timestamp;

        string uidstr = socketpp.player_uid.ToString();
        if (!Directory.Exists(Application.persistentDataPath + "/" + uidstr))
            Directory.CreateDirectory(Application.persistentDataPath + "/" + uidstr);

        string filepath = uidstr + "/" + uidstr + "_" + profilechangetime.timestamp + ".png";
        File.WriteAllBytes(Application.persistentDataPath + "/" + filepath, imageByte);

        socketpp.OnClickFileUpload(filepath, imageByte);
    }


    //이미지 프로필 배경 변경
    public void ChangeProfileBg()
    {
#if UNITY_EDITOR

#else
            NativeGallery.GetImageFromGallery(callbackForGalleryBg); //구글 네이티브 갤러리에서 이미지 가져오기
#endif
        popup.transform.GetChild(0).GetComponent<Text>().text = "수정 완료";
        popup.SetActive(true);
    }
    //갤러리 사진 이미지 경로 가져오기
    private void callbackForGalleryBg(string path)
    {
        UpdateImageBg(path);
    }
    //이미지 경로로 프로필 배경 업데이트
    void UpdateImageBg(string path)
    {
        //가져온 이미지로 크기 변경 후 PNG로 변경
        byte[] imageByte = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(640, 360, TextureFormat.RGB24, true);
        texture.LoadImage(imageByte);
        texture.filterMode = FilterMode.Trilinear;
        Texture2D newTexture = ResizeTexture(texture, 640, 360);

        imageByte = newTexture.EncodeToPNG();

        ChangeBackgroundImage_client_to_server bgchange = new ChangeBackgroundImage_client_to_server();
        bgchange.uid = socketpp.player_uid;
        bgchange.size = imageByte.Length;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(bgchange)); //사용자 uid와 이미지 크기로 클라이언트에서 서버 보내기
        ChangeBackgroundImage_server_to_client bgchangetime = JsonUtility.FromJson<ChangeBackgroundImage_server_to_client>(socketpp.receiveMsg);//서버에서 전달받은 것을 클라이언트로 전달

        string uidstr = socketpp.player_uid.ToString();
        if (!Directory.Exists(Application.persistentDataPath + "/" + uidstr))
            Directory.CreateDirectory(Application.persistentDataPath + "/" + uidstr);

        string filepath = uidstr + "/" + uidstr + "_" + bgchangetime.timestamp + ".png";
        File.WriteAllBytes(Application.persistentDataPath + "/" + filepath, imageByte);

        socketpp.OnClickFileUpload(filepath, imageByte);
    }

    //프로필 정보 가져오기
    public void profile_client_to_server()
    {
        Profile_client_to_server profile = new Profile_client_to_server();
        profile.uid = socketpp.player_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(profile)); //사용자 uid 클라이언트에서 서버로 전달
        Profile_server_to_client myProfile = JsonUtility.FromJson<Profile_server_to_client>(socketpp.receiveMsg); //서버에서 전달받은 것을 클라이언트로 전달
        profileNameField.text = socketpp.player_nickname; //이름 필드 = 사용자 이름
        profileIntroduceField.text = myProfile.self_intro; //설명 필드 = 자기설명
    }

    //이름 변경
    public void ChangeName()
    {
        if (socketpp.player_nickname != profileNameField.text && profileNameField.text != null) 
        {
            ChangeName_client_to_server changename = new ChangeName_client_to_server();
            changename.uid = socketpp.player_uid;
            changename.nickname = profileNameField.text;
            socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(changename)); //사용자 uid  닉네임 클라이언트에서 서버로 전달
            socketpp.player_nickname = changename.nickname;
            popup.transform.GetChild(0).GetComponent<Text>().text = "수정 완료";
            popup.SetActive(true);
        }
        else
        {
            popup.transform.GetChild(0).GetComponent<Text>().text = "변경 실패";
            popup.SetActive(true);
        }
    }

    public string changePlanEmail;
    //이메일 변경
    public void ChangeEmail()
    {
        if (socketpp.player_email != profileEmailField.text && profileEmailField.text != null)
        {
            ChangeEmail_client_to_server changeEmail = new ChangeEmail_client_to_server();
            changeEmail.uid = socketpp.player_uid;
            changeEmail.email = profileEmailField.text;
            socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(changeEmail)); //사용자 uid  이메일 클라이언트에서 서버로 전달
            changePlanEmail = changeEmail.email;
            popup.transform.GetChild(0).GetComponent<Text>().text = "번호 전송";
            popup.SetActive(true); 
        }
        else
        {
            popup.transform.GetChild(0).GetComponent<Text>().text = "변경 실패";
            popup.SetActive(true);
        }
    }

    //이메일변경 인증
    public void ChangeEmailAuth()
    {
        Auth_client_to_server changeEmailAuth = new Auth_client_to_server();
        changeEmailAuth.auth = int.Parse(profileEmailAuthField.text);
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(changeEmailAuth)); //이메일 인증번호 필드 값 클라이언트에서 서버로 전달
        Auth_server_to_client emailChangeResult = JsonUtility.FromJson<Auth_server_to_client>(socketpp.receiveMsg); //서버에서 받은 것을 클라이언트로 전달

        if (emailChangeResult.auth == "True") //인증번호 맞으면
        {
            socketpp.player_email = changePlanEmail;
            popup.transform.GetChild(0).GetComponent<Text>().text = "수정 완료";
        }
        else
            popup.transform.GetChild(0).GetComponent<Text>().text = "변경 실패";
        popup.SetActive(true);
    }

    //비밀번호 변경
    public void ChangePassword()
    {
        if (socketpp.player_password == profilePasswordField.text && profilePasswordField.text != null && profileNewPasswordField.text != null && profilePasswordField.text != profileNewPasswordField.text)
        {
            ChangePassword_client_to_server changePw = new ChangePassword_client_to_server();
            changePw.uid = socketpp.player_uid;
            changePw.pw = profilePasswordField.text;
            changePw.new_pw = profileNewPasswordField.text;
            socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(changePw)); //사용자 uid 기존비밀번호 새로운 비밀번호 클라이언트에서 서버로 전달
            socketpp.player_password = changePw.pw;
            popup.transform.GetChild(0).GetComponent<Text>().text = "수정 완료";
            popup.SetActive(true);
        }
        else
        {
            popup.transform.GetChild(0).GetComponent<Text>().text = "변경 실패";
            popup.SetActive(true);
        }
    }
    //자기소개 변경
    public void ChangeIntroduce()
    {
        ChangeIntroduce_client_to_server changeIntroduce = new ChangeIntroduce_client_to_server();
        changeIntroduce.uid = socketpp.player_uid;
        changeIntroduce.introduce = profileIntroduceField.text;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(changeIntroduce)); //사용자 uid 설명 필드 클라이언트에서 서버로 전달
        popup.transform.GetChild(0).GetComponent<Text>().text = "수정 완료";
        popup.SetActive(true);
    }
}
