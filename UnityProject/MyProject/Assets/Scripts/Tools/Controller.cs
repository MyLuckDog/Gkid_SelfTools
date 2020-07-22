using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public TestScroll Scroll;
    public List<ItemData> allData = new List<ItemData>();
    public int messageCount = 50;
    void Awake()
    {
        for (int i = 0; i < messageCount; i++)
        {
            allData.Add(new ItemData(i, $"过去的第{i}天"));
        }
        Scroll.CreatNum = messageCount;
        Scroll.SetItemMessage = ShowItemMessage;
    }

    void ShowItemMessage(GameObject go, int index)
    {
        go.GetComponent<TestItem>().InitMess(allData[index].Name, index);
    }
}
public class ItemData
{
    public int Index;
    public string Name;
    public ItemData(int index, string name)
    {
        Index = index;
        Name = name;
    }
}