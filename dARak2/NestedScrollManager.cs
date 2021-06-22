/*
 * NestedScrollManager 스크립트
 * : Main Scene 좌우 드래그 스크롤 스크립트
 * 스크롤 시, 일정 범위 만큼 스크롤 시 되돌아 오게 함
 * 타임라인, 마이페이지로 이동 시 가장 위로 스크롤 되게 함
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NestedScrollManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Scrollbar scrollbar;
    public Transform contentTr;

    public Slider tapSlider;

    const int SIZE = 3;
    float[] pos = new float[SIZE];
    float distance, targetPos, curPos;
    bool isDrag;
    int targetIndex;

    /* pos[0] = 타임라인
     * pos[1] = AR모드
     * pos[2] = 마이페이지
     */
    void Start()
    {
        distance = 1f / (SIZE - 1);
        for (int i = 0; i < SIZE; i++)
            pos[i] = distance * i;
    }
    void Update()
    {
        tapSlider.value = scrollbar.value;

        if (!isDrag)
        {
            scrollbar.value = Mathf.Lerp(scrollbar.value, targetPos, 0.1f);
        }
    }

    //드래그시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        curPos = setPos();
    }
    //드래그중
    public void OnDrag(PointerEventData eventData)
    {
        isDrag = true;
    }
    //드래그끝
    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        targetPos = setPos();
        if (curPos == targetPos)
        {
            if(eventData.delta.x > 18 && curPos - distance >= 0)
            {
                --targetIndex;
                targetPos = curPos - distance;
            }
            else if(eventData.delta.x < -18 && curPos + distance <= 1.01f)
            {
                ++targetIndex;
                targetPos = curPos + distance;
            }
        }

        for(int i =0; i< SIZE; i++)
        {
            if (contentTr.GetChild(i).GetComponent<ScrollScript>() && curPos != pos[i] && targetPos == pos[i])
            {
                contentTr.GetChild(0).GetChild(3).GetComponent<Scrollbar>().value = 1;
                contentTr.GetChild(2).GetChild(1).GetComponent<Scrollbar>().value = 1;
            }
        }
    }

    //좌 우 드레그 시 타임라인, AR모드, 마이페이지 이동하게 함
    float setPos()
    {
        for (int i = 0; i < SIZE; i++)
        {
            if (scrollbar.value < pos[i] + distance * 0.5f && scrollbar.value > pos[i] - distance * 0.5f)
            {
                targetIndex = i;
                return pos[i];
            }
        }
        return 0;
    }

    //타임라인, 마이페이지 이동 시 최상단으로 이동
    public void TabClick(int n)
    {
        targetIndex = n;
        targetPos = pos[n];
        contentTr.GetChild(0).GetChild(3).GetComponent<Scrollbar>().value = 1;
        contentTr.GetChild(2).GetChild(1).GetComponent<Scrollbar>().value = 1;
    }
}
