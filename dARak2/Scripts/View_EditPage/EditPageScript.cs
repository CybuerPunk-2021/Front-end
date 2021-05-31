using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditPageScript : MonoBehaviour
{
    public InputField profileNameField, profileEmailField, profileEmailAuthField, profileIntroduceField, profilePasswordField, profileNewPasswordField;
    Socketpp socketpp;

    // Start is called before the first frame update
    void Start()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
        profile_client_to_server();
    }

    // Update is called once per frame
    void Update()
    {
        
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
