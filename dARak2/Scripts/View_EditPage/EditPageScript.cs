using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class EditPageScript : MonoBehaviour
{
    public InputField profileNameField, profileEmailField, profileEmailAuthField, profileIntroduceField, profilePasswordField, profileNewPasswordField;
    Socketpp socketpp;

    // Start is called before the first frame update
    void Awake()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
    }
    void OnEnable()
    {
        profile_client_to_server();
    }

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


    public void ChangeProfileImage()
    {
#if UNITY_EDITOR

#else
            NativeGallery.GetImageFromGallery(callbackForGalleryProfile);
#endif
    }
    private void callbackForGalleryProfile(string path)
    {
        UpdateImageProfile(path);
    }
    void UpdateImageProfile(string path)
    {
        byte[] imageByte = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(250, 250, TextureFormat.RGB24, true);
        texture.LoadImage(imageByte);
        texture.filterMode = FilterMode.Trilinear;
        Texture2D newTexture = ResizeTexture(texture, 250, 250);

        imageByte = newTexture.EncodeToPNG();

        ChangeProfileImage_client_to_server profilechange = new ChangeProfileImage_client_to_server();
        profilechange.uid = socketpp.player_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(profilechange));
        ChangeProfileImage_server_to_client profilechangetime = JsonUtility.FromJson<ChangeProfileImage_server_to_client>(socketpp.receiveMsg);

        string uidstr = socketpp.player_uid.ToString();
        if (!Directory.Exists(Application.persistentDataPath + "/" + uidstr))
            Directory.CreateDirectory(Application.persistentDataPath + "/" + uidstr);

        string filepath = uidstr + "/" + uidstr + "_" + profilechangetime.timestamp + ".png";
        File.WriteAllBytes(Application.persistentDataPath + "/" + filepath, imageByte);

        socketpp.OnClickFileUpload(filepath, imageByte);
    }


    public void ChangeProfileBg()
    {
#if UNITY_EDITOR

#else
            NativeGallery.GetImageFromGallery(callbackForGalleryBg);
#endif
    }
    private void callbackForGalleryBg(string path)
    {
        UpdateImageBg(path);
    }
    void UpdateImageBg(string path)
    {
        byte[] imageByte = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(640, 360, TextureFormat.RGB24, true);
        texture.LoadImage(imageByte);
        texture.filterMode = FilterMode.Trilinear;
        Texture2D newTexture = ResizeTexture(texture, 640, 360);

        imageByte = newTexture.EncodeToPNG();

        ChangeBackgroundImage_client_to_server bgchange = new ChangeBackgroundImage_client_to_server();
        bgchange.uid = socketpp.player_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(bgchange));
        ChangeBackgroundImage_server_to_client bgchangetime = JsonUtility.FromJson<ChangeBackgroundImage_server_to_client>(socketpp.receiveMsg);

        string uidstr = socketpp.player_uid.ToString();
        if (!Directory.Exists(Application.persistentDataPath + "/" + uidstr))
            Directory.CreateDirectory(Application.persistentDataPath + "/" + uidstr);

        string filepath = uidstr + "/" + uidstr + "_" + bgchangetime.timestamp + ".png";
        File.WriteAllBytes(Application.persistentDataPath + "/" + filepath, imageByte);

        socketpp.OnClickFileUpload(filepath, imageByte);
    }

    public void profile_client_to_server()
    {
        Profile_client_to_server profile = new Profile_client_to_server();
        profile.uid = socketpp.player_uid;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(profile));
        Profile_server_to_client myProfile = JsonUtility.FromJson<Profile_server_to_client>(socketpp.receiveMsg);
        profileNameField.text = socketpp.player_nickname;
        profileIntroduceField.text = myProfile.self_intro;
    }

    public void ChangeName()
    {
        ChangeName_client_to_server changename = new ChangeName_client_to_server();
        changename.uid = socketpp.player_uid;
        changename.nickname = profileNameField.text;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(changename));
        socketpp.player_nickname = changename.nickname;
    }
    
    public void ChangeEmail()
    {
        ChangeEmail_client_to_server changeEmail = new ChangeEmail_client_to_server();
        changeEmail.uid = socketpp.player_uid;
        changeEmail.email = profileEmailField.text;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(changeEmail));
    }

    public void ChangeEmailAuth()
    {
        Auth_client_to_server changeEmailAuth = new Auth_client_to_server();
        changeEmailAuth.auth = int.Parse(profileEmailAuthField.text);
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(changeEmailAuth));
    }

    public void ChangePassword()
    {
        ChangePassword_client_to_server changePw = new ChangePassword_client_to_server();
        changePw.uid = socketpp.player_uid;
        changePw.pw = profilePasswordField.text;
        changePw.new_pw = profileNewPasswordField.text;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(changePw));

    }
    
    public void ChangeIntroduce()
    {
        ChangeIntroduce_client_to_server changeIntroduce = new ChangeIntroduce_client_to_server();
        changeIntroduce.uid = socketpp.player_uid;
        changeIntroduce.introduce = profileIntroduceField.text;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(changeIntroduce));
    }
}
