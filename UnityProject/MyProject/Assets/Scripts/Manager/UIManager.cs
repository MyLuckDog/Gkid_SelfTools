using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTools;

public class UIManager : MSingleton<UIManager>
{
    private Dictionary<string, UIPanel> panelDic = new Dictionary<string, UIPanel>();
    public Canvas _canvas;
    public override void Awake()
    {
        base.Awake();
        panelDic.Clear();
    }

    public T OpenUIPanel<T>() where T : UIPanel
    {
        string uiName = typeof(T).Name;
        $"Open UI by scrip name{0}".ToLogFromat(uiName).Print();
        T ui = GetUI<T>();
        if (ui == null)
        {
            var go = Resources.Load<GameObject>("AutoUI/" + uiName);
            if (go == null)
            {
                $"{0} panel is null when opening".ToLogFromat(uiName).PrintError();
                return null;
            }
            ui = AddUI<T>(go);
            ui.OnOpen();
        }
        ui.transform.SetAsLastSibling();
        return ui;
    }
    public void CloseAllPanel()
    {
        var dic = panelDic.GetEnumerator();
        while (dic.MoveNext())
        {
            var closeItem = dic.Current.Value;
            CloseUIPanel(closeItem);
        }
    }

    public void CloseUI<T>() where T : UIPanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            var panel = panelDic[panelName];
            if (panel != null)
            {
                panel.OnClose();
                Destroy(panel.gameObject);
            }
            panelDic.Remove(panelName);
        }
        else
        {
            "{0} panel is not controed by UIManager".PrintLog();
        }
    }

    public void CloseUIPanel(UIPanel panel)
    {
        if (panel == null) return;
        panel.OnClose();
        Destroy(panel.gameObject);
        var enumerator = panelDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Value == panel)
            {
                panelDic.Remove(enumerator.Current.Key);
                break;
            }
        }
    }
    T AddUI<T>(GameObject go) where T : UIPanel
    {
        var panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            panelDic.Remove(panelName);
        }
        if (_canvas == null)
        {
            _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            if (_canvas == null)
            {
                "canvas not Find".PrintError();
                return null;
            }
        }
        var panelItem = Instantiate(go, _canvas.transform);
        T panel = panelItem.GetComponent<T>();
        if (!panel) panel = panelItem.AddComponent<T>();
        panelItem.transform.localScale = Vector3.one;
        panelDic.Add(panelName, panel);
        return panel;
    }

    T GetUI<T>() where T : UIPanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;
        return null;
    }
}
