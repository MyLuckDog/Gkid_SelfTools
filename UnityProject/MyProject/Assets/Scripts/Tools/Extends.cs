using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MyTools
{
    public static class Extends
    {
        public static void PrintError(this string mesg)
        {
            new MyLogger(mesg).PrintError();
        }
        public static void PrintWarnning(this string mesg)
        {
            new MyLogger(mesg).PrintWarnning();
        }
        public static void PrintLog(this string mesg, Color? color = null)
        {
            new MyLogger(mesg).DoColor(color).Print();
        }
        public static MyLogger ToLogFromat(this string mesg, Color? col = null, params object[] paras)
        {
            return new MyLogger(mesg).Format(paras).DoColor(col);
        }
        public static MyLogger ToLogFromat(this string mesg, params object[] paras)
        {
            return new MyLogger(mesg).Format(paras);
        }

        public static T FindChild<T>(this Transform tran, string name) where T : Component
        {
            if (tran.name == name)
                return tran.GetComponent<T>();
            else
                return FindInAllChild<T>(tran, name);
        }
        static T FindInAllChild<T>(this Transform tran, string name) where T : Component
        {
            if (tran.childCount > 0)
            {
                for (int i = 0; i < tran.childCount; i++)
                {
                    T com = null;
                    if (tran.GetChild(i).name == name && tran.GetChild(i).tag == MyConstItem.ExportTag)
                        com = tran.GetChild(i).GetComponent<T>();
                    else
                        com = tran.GetChild(i).FindInAllChild<T>(name);
                    if (com != null)
                        return com;
                }
            }
            return null;
        }
        public static List<int> FindAllIndex<T>(this List<T> all, T item) where T : IComparable<T>
        {
            List<int> IndexList = new List<int>();
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].CompareTo(item) == 0)
                {
                    IndexList.Add(i);
                }
            }
            return IndexList;
        }
    }
}
public static class MyConstItem
{
    public static string ExportTag = "Export";
}

