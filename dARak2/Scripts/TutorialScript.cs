using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TutorialScript : MonoBehaviour
{
    public GameObject View_Home;
    public GameObject View_Tutorial;
    Socketpp socketpp;
    // Start is called before the first frame update
    void Awake()
    {
        socketpp = GameObject.Find("Socket").GetComponent<Socketpp>();
        if(socketpp.tutorial_flag == 0) //튜토리얼 실행여부 확인후, 튜토리얼 실행 안했으면 실행 했으면 실행안함
        {
            AvaliableTutorial();
        }
        else
        {
            View_Tutorial.SetActive(false);
            View_Home.SetActive(true);
        }
    }

    //튜토리얼 다시 안보기 체크 여부를 확인해서, 다시 안보기 되어있으면 튜토리얼 종료, 아닌 경우 튜토리얼을 처음 실행했기에 tutorial_flag = 1
    public void AvaliableTutorial()
    {
        if (PlayerPrefs.HasKey("tutorial")){
            OffTutorial();
        }
        else
        {
            socketpp.tutorial_flag = 1;
        }
    }   

    //튜토리얼 다시 안보기 버튼 체크 되어있으면, PlayerPrefs의 tutorial에 1을 넣음
    public void CheckTutorial()
    {
        if (GameObject.Find("TutorialToggle").GetComponent<Toggle>().isOn == true)
        {
            PlayerPrefs.SetInt("tutorial", 1);
        }
    }
    
    //튜토리얼 종료 버튼시 실행
    public void OffTutorial()
    {
        CheckTutorial();
        View_Tutorial.SetActive(false);
        View_Home.SetActive(true);
    }
}
