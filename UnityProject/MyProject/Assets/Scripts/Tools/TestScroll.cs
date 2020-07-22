using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;

public class TestScroll : MonoBehaviour
{
    private ScrollRect sr;
    public RectTransform Item;
    public int ShowNum = 5;

    List<RectTransform> allItem = new List<RectTransform>();
    int itemIndex = 0;
    [Header("测试显示数据")]
    public int CreatNum = 30;
    private void Start()         //函数调用之前必须设置CreatNum
    {
        Item.gameObject.SetActive(false);
        sr = GetComponent<ScrollRect>();
        sr.onValueChanged.AddListener(SRChange);
        sr.content.anchorMax = new Vector2(0, 1);
        sr.content.anchorMin = new Vector2(0, 0);
        sr.content.pivot = new Vector2(0, 1);
        if (sr.vertical)
        {
            sr.content.anchorMax = new Vector2(1, 1);
            sr.content.anchorMin = new Vector2(0, 1);
            sr.content.pivot = new Vector2(0, 1);
        }
        InitMessage();
    }
    void InitMessage()
    {
        RectTransform selfRect = GetComponent<RectTransform>();
        itemSize = Item.rect.width * Item.localScale.x;
        if (sr.vertical) itemSize = Item.rect.height * Item.localScale.x;
        AllSize = (itemSize) * CreatNum;
        recordValue = sr.content.localPosition.x;
        ShowNum = Mathf.CeilToInt(selfRect.rect.width / Item.rect.width) + 2;
        itemLayOutAdd = new Vector3(-itemSize, 0, 0);
        if (sr.vertical)
        {
            ShowNum = Mathf.CeilToInt(selfRect.rect.height / Item.rect.height) + 2;
            recordValue = sr.content.localPosition.y;
            sr.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemSize * CreatNum);
            itemLayOutAdd = new Vector3(0, itemSize, 0);
        }
        else
        {
            sr.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, selfRect.rect.height);
            sr.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemSize * CreatNum);
        }

        for (int i = 0; i < ShowNum; i++)
        {
            CreatItem(i);
        }

        AllSize = itemSize * (CreatNum - 2) - selfRect.rect.width;
        if (sr.vertical) AllSize = itemSize * (CreatNum - 2) - selfRect.rect.height;
    }
    public float offDis = 0;
    public float recordValue;

    void SRChange(Vector2 pos)
    {
        float Cpos = Mathf.Clamp(sr.content.localPosition.x, -AllSize, 0);
        if (sr.vertical) Cpos = Mathf.Clamp(sr.content.localPosition.y, 0, AllSize);
        offDis += Cpos - recordValue;
        recordValue = Cpos;
        bool enter = offDis > 0 || Mathf.Abs(offDis) > itemSize;
        if (sr.vertical) enter = offDis < 0 || Mathf.Abs(offDis) > itemSize;
        if (enter)
        {
            MoveItem(offDis > 0);
            if (offDis > 0)
            {
                offDis -= itemSize;
            }
            else
            {
                offDis += itemSize;
            }
        }
    }
    public Action<GameObject, int> SetItemMessage;
    /// <summary>
    /// 向上移动为Up     
    /// </summary>
    /// <param name="isUp"></param>
    void MoveItem(bool isUp)
    {
        if (sr.horizontal) isUp = !isUp;
        GameObject go;
        int inde = 0;
        if (isUp)
        {
            go = allItem[itemIndex % ShowNum].gameObject;
            allItem[itemIndex % ShowNum].localPosition -= itemLayOutAdd * ShowNum;
            inde = itemIndex + ShowNum;
            itemIndex++;
        }
        else
        {
            itemIndex--;
            inde = itemIndex;
            allItem[(itemIndex + ShowNum) % ShowNum].localPosition += itemLayOutAdd * ShowNum;
            go = allItem[(itemIndex + ShowNum) % ShowNum].gameObject;
        }
        SetItemMessage?.Invoke(go, inde);
    }

    public float itemSize, AllSize;
    Vector3 itemLayOutAdd;
    public void CreatItem(int index)
    {
        RectTransform itemRect = Instantiate(Item, sr.content);
        allItem.Add(itemRect);
        itemRect.gameObject.SetActive(true);
        LayoutGroup lg = sr.content.GetComponent<LayoutGroup>();
        SetItemMessage?.Invoke(itemRect.gameObject, index);

        if (lg && lg.enabled) return;

        itemRect.anchorMin = new Vector2(0, 1);
        itemRect.anchorMax = new Vector2(0, 1);
        itemRect.pivot = new Vector2(0, 1);
        itemRect.localPosition = -itemLayOutAdd * index;
    }
}
