using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginSceneScript : MonoBehaviour
{
    public GameObject SignUpPanel;
    public GameObject PasswordPanel;
    public GameObject LoginPanel;
    public GameObject AuthPanel;

    public InputField login_idField, login_passwordField;
    public InputField sign_idField, sign_usernameField, sign_emailField, sign_passwordField1, sign_passwordField2;
    public InputField auth_authField;
    public InputField search_emailField;

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
            GoAuthScene();
        }
        //roominfo info = JsonUtility.FromJson<roominfo>(socketpp.receiveMsg);
    }
    public void auth_client_to_server()
    {
        Auth_client_to_server auth = new Auth_client_to_server();
        auth.auth = int.Parse(auth_authField.text);
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(auth));
        GoSucessScene();
    }

    public void ActiveMainScene()
    {   
        SceneManager.LoadScene("Main");
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
    public void SearchBtn()
    {
        if ("" != search_emailField.text)
        {
            GoLoginScene();
        }
    }


    public void GoLoginScene()
    {
        SignUpPanel.SetActive(false);
        PasswordPanel.SetActive(false);
        LoginPanel.SetActive(true);
    }
    public void GoSignUpScene()
    {
        SignUpPanel.SetActive(true);
        LoginPanel.SetActive(false);
    }
    public void GoPasswordScene()
    {
        PasswordPanel.SetActive(true);
        LoginPanel.SetActive(false);
    }
    public void GoAuthScene()
    {
        SignUpPanel.SetActive(false);
        AuthPanel.SetActive(true);
    }
    public void GoBackSignUpScene()
    {
        SignUpPanel.SetActive(true);
        AuthPanel.SetActive(false);
    }
    public void GoSucessScene()
    {
        AuthPanel.SetActive(false);
        LoginPanel.SetActive(true);
    }
}
