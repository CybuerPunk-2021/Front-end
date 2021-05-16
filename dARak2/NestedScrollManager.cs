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

    void Start()
    {
        distance = 1f / (SIZE - 1);
        for (int i = 0; i < SIZE; i++)
            pos[i] = distance * i;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        curPos = setPos();
    }
    public void OnDrag(PointerEventData eventData)
    {
        isDrag = true;
    }
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
    void Update()
    {
        tapSlider.value = scrollbar.value;

        if (!isDrag)
        {
            scrollbar.value = Mathf.Lerp(scrollbar.value, targetPos, 0.1f);
        }
    }

    public void TabClick(int n)
    {
        targetIndex = n;
        targetPos = pos[n];
        contentTr.GetChild(0).GetChild(3).GetComponent<Scrollbar>().value = 1;
        contentTr.GetChild(2).GetChild(1).GetComponent<Scrollbar>().value = 1;
    }
}
