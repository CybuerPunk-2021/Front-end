/*
 * PopupEnd스크립트
 * : 팝업창이 Active된 후 알림 뜬 다음 꺼지도록 하는 스크립트
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupEnd : MonoBehaviour
{
    public GameObject popup;
    void PopUpEnd()
    {
        popup.SetActive(false);
    }
}
