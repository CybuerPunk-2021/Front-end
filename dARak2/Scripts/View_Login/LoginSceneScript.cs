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
    public GameObject PopupPanel;
    public GameObject ToSPanel;

    public InputField login_idField, login_passwordField;
    public InputField sign_idField, sign_usernameField, sign_emailField, sign_passwordField1, sign_passwordField2;
    public InputField auth_authField;
    public InputField search_idField;
    public InputField search_pw_idField, search_pw_emailField;

    Socketpp socketpp;

    // Start is called before the first frame update
    void Awake()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>(); //소켓불러오기
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }

    //로그인하기
    public void login_client_to_server()
    {
        Login_client_to_server login = new Login_client_to_server();
        login.id = login_idField.text;
        login.pw = login_passwordField.text;

        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(login)); //입력된 id 필드 패스워드 필드 값 클라이언트에서 서버로 전달
        Login_server_to_client player = JsonUtility.FromJson<Login_server_to_client>(socketpp.receiveMsg); //서버에서 전달받은 것을 클라이언트 전달
        if (player.action == "True") //로그인 성공
        {
            ActivePopupPanel();
            GameObject.Find("PopUpText").GetComponent<Text>().text = "로그인 중입니다...";
            GameObject.Find("ClosePopUpButton").gameObject.SetActive(false);
            socketpp.player_nickname = player.nickname;
            socketpp.player_uid = player.uid;
            socketpp.player_recent_timestamp = player.timestamp;
            socketpp.player_email = player.email;
            socketpp.player_password = login.pw;
            ActiveMainScene();
        }
        else if (player.action == "False") //로그인 실패
        {
            ActivePopupPanel();
            GameObject.Find("PopUpText").GetComponent<Text>().text = "로그인에 실패하였습니다. 아이디와 비밀번호를 다시 입력하세요";
        }
    }
    //회원가입
    public void signup_client_to_server()
    {
        Signup_client_to_server signup = new Signup_client_to_server();
        signup.id = sign_idField.text.Replace(" ","");
        signup.pw = sign_passwordField1.text.Replace(" ", "");
        signup.nickname = sign_usernameField.text;
        signup.email = sign_emailField.text.Replace(" ", "");
        if (sign_passwordField1.text == sign_passwordField2.text) //비밀번호 재입력 맞는지 확인
        {
            socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(signup)); //id, 비밀번호, 닉네임, 이메일 클라이언트에서 서버로 전달
            Signup_server_to_client signup_server = JsonUtility.FromJson<Signup_server_to_client>(socketpp.receiveMsg); //서버에서 전달받은 것을 클라이언트로 전달
            if (signup_server.action == "dup id") //아이디 중복시
            {
                ActivePopupPanel();
                GameObject.Find("PopUpText").GetComponent<Text>().text = "중복되는 아이디입니다.";
            }
            else if(signup_server.action == "dup nick") //닉네임 중복시
            {
                ActivePopupPanel();
                GameObject.Find("PopUpText").GetComponent<Text>().text = "중복되는 닉네임입니다.";
            }
            else if (signup_server.action == "email send") //이메일 전송 완료시
            {
                ActiveAuthPanel();
            }
            else
            {
                ActivePopupPanel();
                GameObject.Find("PopUpText").GetComponent<Text>().text = "입력 값을 다시 확인해 주세요";
            }
        }
        else if (sign_passwordField1.text != sign_passwordField2.text) //비밀번호 재입력 다를시
        {
            ActivePopupPanel();
            GameObject.Find("PopUpText").GetComponent<Text>().text = "비밀번호가 동일하지 않습니다.";
        }
        //roominfo info = JsonUtility.FromJson<roominfo>(socketpp.receiveMsg);
    }
    //이메일 인증
    public void auth_client_to_server()
    {
        Auth_client_to_server auth = new Auth_client_to_server();
        auth.auth = int.Parse(auth_authField.text.Replace(" ", ""));
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(auth)); //인증필드 입력된 값을 클라이언트에서 서버로 전달
        Auth_server_to_client auth_server = JsonUtility.FromJson<Auth_server_to_client>(socketpp.receiveMsg); //서버에서 전달받은 것을 클라이언트로 전달
        if (auth_server.auth == "True") //인증이 맞으면
        {
            GoSuccessSignup();
            ActivePopupPanel();
            GameObject.Find("PopUpText").GetComponent<Text>().text = "계정이 생성되었습니다";
        }
        else if (auth_server.auth == "False") //인증이 틀리면
        {
            ActivePopupPanel();
            GameObject.Find("PopUpText").GetComponent<Text>().text = "인증번호를 다시 입력해주세요";
        }
    }

    //아이디 찾기
    public void findId_client_to_server()
    {
        FindId_client_to_server findId = new FindId_client_to_server();
        findId.email = search_idField.text.Replace(" ", "");
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(findId)); //이메일필드 입력된 값을 클라이언트에서 서버로 전달
        FindId_server_to_client resultId = JsonUtility.FromJson<FindId_server_to_client>(socketpp.receiveMsg); //서버에서 전달받은 것을 클라이언트로 전달
        if (resultId.result[0] != null) //아이디가 있으면
        {
            ActiveResultPanel();
            GameObject.Find("IdResultText").GetComponent<Text>().text = resultId.result[0];
        }
    }

    //비밀변호 찾기
    public void modifyPw_client_to_sever()
    {
        ModifyPw_client_to_server modifyPw = new ModifyPw_client_to_server();
        modifyPw.id = search_pw_idField.text.Replace(" ", "");
        modifyPw.email = search_pw_emailField.text.Replace(" ", "");
        socketpp.receiveMsg = socketpp.socket(JsonUtility.ToJson(modifyPw)); //아이디 이메일 필드 핍력된 값을 클라이언트에서 서버로 전달
        ModifyPw_server_to_client resultPw = JsonUtility.FromJson<ModifyPw_server_to_client>(socketpp.receiveMsg); //서버에서 전달받은 것을 클라이언트로 전달
        if (resultPw.result == "ok") //아이디와 이메일이 맞으면
        {
            UnActiveSearchPwPanel();
            ActivePopupPanel();
            GameObject.Find("PopUpText").GetComponent<Text>().text = "임시 비밀번호가 메일로 전송되었습니다.";
        }
        else if (resultPw.result == "err") //아이디나 이메일이 틀리면
        {
            ActivePopupPanel();
            GameObject.Find("PopUpText").GetComponent<Text>().text = "올바르지 않는 이메일과 아이디입니다.";
        }
    }


    //로그인 씬 버튼들
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
    public void ActiveToSPanel()
    {
        ToSPanel.SetActive(true);
    }
    public void UnActiveToSPanel()
    {
        ToSPanel.SetActive(false);
    }
    public void ActiveSignupPanel()
    {
        if(GameObject.Find("ToSPanelToggle").GetComponent<Toggle>().isOn == true)
        {
            SignUpPanel.SetActive(true);
        }
        else
        {
            ActivePopupPanel();
            GameObject.Find("PopUpText").GetComponent<Text>().text = "이용약관에 동의하셔야 합니다.";
        }
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
        ToSPanel.SetActive(false);
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
    public void ActivePopupPanel()
    {
        PopupPanel.SetActive(true);
    }
    public void UnActivePopupPanel()
    {
        PopupPanel.SetActive(false);
    }
}
