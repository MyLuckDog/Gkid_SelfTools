using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel : MonoBehaviour
{

    private void Awake()
    {
        OnCreat();
    }
    /// <summary>
    /// Awake 状态启动，启动中...
    /// </summary>
    protected virtual void OnCreat()
    {

    }
    /// <summary>
    /// 创建成功时启动
    /// </summary>
    public virtual void OnOpen()
    {

    }
    protected virtual void Close()
    {
        if (UIManager.S != null)
            UIManager.S.CloseUIPanel(this);
    }
    public virtual void OnClose()
    {

    }
    protected virtual void OnUpdate()
    {

    }
 
}
