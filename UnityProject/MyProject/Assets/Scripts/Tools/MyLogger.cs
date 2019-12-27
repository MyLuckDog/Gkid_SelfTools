using UnityEngine;

public class MyLogger
{
    public enum LogLevel
    {
        LOG = 0,
        WARNNING,
        ERROR,
    }
    LogLevel level;
    public string LogMessage;
    private Color32? color;
    public MyLogger(string log)
    {
        LogMessage = log;
        level = LogLevel.LOG;
    }
    public MyLogger()
    {
        level = LogLevel.LOG;
    }
    public MyLogger AsLog()
    {
        level = LogLevel.LOG;
        return this;
    }
    public MyLogger AsWarnning()
    {
        level = LogLevel.WARNNING;
        return this;
    }
    public MyLogger AsError()
    {
        level = LogLevel.ERROR;
        return this;
    }

    public MyLogger DoColor(Color? col)
    {
        if (col != null)
            color = col;
        return this;
    }
    public MyLogger Format(params object[] paras)
    {
        this.paras = paras;
        return this;
    }
    private object[] paras;
    public override string ToString()
    {
        var mess = LogMessage;
        if (paras != null && paras.Length > 0)
            mess = string.Format(LogMessage, paras);
        return mess;
    }
    string GetColor()
    {
        var co = this.color.Value;
        return co.r.ToString("X2") + co.g.ToString("X2") + co.b.ToString("X2") + co.a.ToString("X2");
    }

    public void PrintError()
    {
        this.AsError().Print();
    }
    public void PrintWarnning()
    {
        this.AsWarnning().Print();
    }
    public void Print()
    {
        var mes = ToString();
        switch (level)
        {
            case LogLevel.LOG:
                if (color.HasValue)
                {
#if UNITY_EDITOR
                    if (mes.Length < 100) Debug.Log("<color=#" + GetColor() + ">" + mes + "</color>");
                    else Debug.Log(mes);
#else
                    Debug.Log(mes);
#endif
                }
                Debug.Log(mes);
                break;
            case LogLevel.WARNNING:
                Debug.LogWarning(mes);
                break;
            case LogLevel.ERROR:
                Debug.LogError(mes);
                break;
        }
    }
}
