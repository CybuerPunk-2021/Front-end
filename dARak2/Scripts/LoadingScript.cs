/*
 * Loading스크립트
 * 로딩 시 실행되는 스크립트
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScript : MonoBehaviour
{
    public GameObject View_Loading;
    public IEnumerator OneFrame()
    {
        View_Loading.SetActive(true);
        yield return null;
    }
}
