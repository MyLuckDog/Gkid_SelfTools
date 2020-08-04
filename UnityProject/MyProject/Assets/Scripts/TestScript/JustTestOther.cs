using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class JustTestOther : MonoBehaviour
{
    FromOther from = new FromOther();
    public const string Name = "今天";
    public readonly string TheName = "mingtian";
    void deal()
    {
        GetOther get = from;
        
        theList.Take(2);
    }
    List<int> theList = new List<int>();

}
public class GetOther
{
    public string Mes = "不能付钱！";
    public GetOther(string mes)
    {
        Mes = mes;
    }
}
public class FromOther
{
    public string Mes = "完全可以付钱！";

    public static implicit operator GetOther(FromOther from)
    {
        return new GetOther(from.Mes);
    }
}
