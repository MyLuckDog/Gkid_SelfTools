using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 应用子类继承该类
/// </summary>
public class TestItem : MonoBehaviour
{
    //test
    public Text OnlyText;

    public virtual void InitMess(string name, int index)
    {
        if (OnlyText == null) OnlyText = GetComponentInChildren<Text>(true);

        OnlyText.text = $"{index}  名字 {name}";
    }

    public bool MoveNext()
    {
        throw new System.NotImplementedException();
    }
}


