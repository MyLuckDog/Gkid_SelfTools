using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyTools;

public class UUIResourcesAttribute : Attribute
{
    public string ResName { get; set; }
    public UUIResourcesAttribute(string res)
    {
        ResName = res;
    }
}

public class UUIAutoGenWindow : UIPanel
{
    protected T FindChild<T>(string name) where T : Component
    {
        return transform.FindChild<T>(name);
    }
    protected override void OnCreat()
    {
        InitTemplate();
    }
    public override void OnOpen()
    {
        base.OnOpen();
        InitModel();
    }

    protected virtual void InitTemplate()
    {

    }
    protected virtual void InitModel()
    {

    }
}
