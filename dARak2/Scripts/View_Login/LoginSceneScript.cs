using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginSceneScript : MonoBehaviour
{
    public GameObject LoginPanel;
    public GameObject AuthPanel;
    public GameObject SignUpPanel;
    public GameObject IdPanel;
    public GameObject IdResultPanel;
    public GameObject PasswordPanel;


    public InputField login_idField, login_passwordField;
    public InputField sign_idField, sign_usernameField, sign_emailField, sign_passwordField1, sign_passwordField2;
    public InputField auth_authField;
    public InputField search_idField;
    public InputField search_pw_idField, search_pw_emailField;

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

    public void login_client_to_server()
    {
        Login_client_to_server login = new Login_client_to_server();
        login.id = login_idField.text;
        login.pw = login_passwordField.text;

        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(login));
        Login_server_to_client player = JsonUtility.FromJson<Login_server_to_client>(socketpp.receiveMsg);
        if (player.action == "True")
        {
            socketpp.player_nickname = player.nickname;
            socketpp.player_uid = player.uid;
            ActiveMainScene();
        }
    }

    public void signup_client_to_server()
    {
        Signup_client_to_server signup = new Signup_client_to_server();
        signup.id = sign_idField.text;
        signup.pw = sign_passwordField1.text;
        signup.nickname = sign_usernameField.text;
        signup.email = sign_emailField.text;
        if (sign_passwordField1.text == sign_passwordField2.text)
        {
            socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(signup));
            Signup_server_to_client signup_server = JsonUtility.FromJson<Signup_server_to_client>(socketpp.receiveMsg);
            if (signup_server.action == "dup id" || signup_server.action == "dup nick")
            {
                //에러 메시지 뜨게 한다.
            }
            else if(signup_server.action == "email send"){
                ActiveAuthPanel();
            }
        }
        //roominfo info = JsonUtility.FromJson<roominfo>(socketpp.receiveMsg);
    }
    public void auth_client_to_server()
    {
        Auth_client_to_server auth = new Auth_client_to_server();
        auth.auth = int.Parse(auth_authField.text);
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(auth));
        Auth_server_to_client auth_server = JsonUtility.FromJson<Auth_server_to_client>(socketpp.receiveMsg);
        if(auth_server.auth == "True")
        {
            GoSuccessSignup();
        }
        else
        {
            //인증번호 재입력하게 한다
        }
    }
    public void findId_client_to_server()
    {
        FindId_client_to_server findId = new FindId_client_to_server();
        findId.email = search_idField.text;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(findId));
        FindId_server_to_client resultId = JsonUtility.FromJson<FindId_server_to_client>(socketpp.receiveMsg);
        if (resultId.result[0] != null)
        {
            ActiveResultPanel();
            GameObject.Find("IdResultText").GetComponent<Text>().text = resultId.result[0];
        }
    }
    public void modifyPw_client_to_sever()
    {
        ModifyPw_client_to_server modifyPw = new ModifyPw_client_to_server();
        modifyPw.id = search_pw_idField.text;
        modifyPw.email = search_pw_emailField.text;
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(modifyPw));
        UnActiveSearchPwPanel();
        ModifyPw_server_to_client resultPw = JsonUtility.FromJson<ModifyPw_server_to_client>(socketpp.receiveMsg);
        if (resultPw.result == "ok")
        {
            UnActiveSearchPwPanel();
        }
        else
        {
            //아이디나 비밀번호에 문제가 있습니다 라고 팝업 필요
        }
    }

    public void LoginBtn()
    {
        login_client_to_server();
    }
    public void SignupBtn()
    {
        signup_client_to_server();
    }
    public void AuthBtn()
    {
        auth_client_to_server();
    }
    public void SearchIdBtn()
    {
        findId_client_to_server();
    }
    public void SearchPwBtn()
    {
        modifyPw_client_to_sever();
    }
    public void ActiveMainScene()
    {
        SceneManager.LoadScene("Main");
    }

    public void ActiveSignupPanel()
    {
        SignUpPanel.SetActive(true);
    }
    public void UnActiveSignupPanel()
    {
        SignUpPanel.SetActive(false);
    }
    public void ActiveAuthPanel()
    {
        AuthPanel.SetActive(true);
    }
    public void UnActiveAuthPanel()
    {
        AuthPanel.SetActive(false);
    }
    public void GoSuccessSignup()
    {
        SignUpPanel.SetActive(false);
        AuthPanel.SetActive(false);
    }
    public void ActiveSearchIdPanel()
    {
        IdPanel.SetActive(true);
    }
    public void UnActiveSearchIdPanel()
    {
        IdPanel.SetActive(false);
    }
    public void ActiveResultPanel()
    {
        IdPanel.SetActive(false);
        IdResultPanel.SetActive(true);
    }
    public void UnActiveResultPanel()
    {
        IdResultPanel.SetActive(false);
    }
    public void GoSuccessSearchPw()
    {
        IdResultPanel.SetActive(false);
        PasswordPanel.SetActive(true);
    }
    public void ActiveSearchPwPanel()
    {
        PasswordPanel.SetActive(true);
    }
    public void UnActiveSearchPwPanel()
    {
        PasswordPanel.SetActive(false);
    }
}
