using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using MyTools;
using System;
using System.Text;
using System.IO;

public class UUICreater : EditorWindow
{
    [MenuItem("UI/Tools/AUTO_GEN_WINDOWS_CODE")]
    [MenuItem("GameObject/UI/AUTO_GEN_WINDOWS_CODE", false, 0)]
    public static void CreatScript()
    {
        window = (UUICreater)GetWindow(typeof(UUICreater), true, "自动创建UI查询");
        window.minSize = new Vector2(300, 400);
        window.windowsRoot = Application.dataPath + "/Scripts/UI/";
    }
    static UUICreater window;
    string windowsRoot;
    string className;
    bool isCreat = false;
    bool showTemplate = true;
    bool CanExport = true;
    Dictionary<string, string> AllExportName_ComDic = new Dictionary<string, string>();
    GameObject currentGO;
    private void OnGUI()
    {
        if (Selection.activeGameObject == null)
        {
            currentGO = null;
            return;
        }
        if (Selection.activeGameObject != currentGO)
        {
            CanExport = true;
            currentGO = Selection.activeGameObject;
            className = currentGO.name;
            FindAllExportItem(currentGO.transform);
        }
        if (!CanExport)
        {
            EditorUtility.DisplayDialog("Error Message", "有两个导出元素命名一样！\n 退出重新编辑", "Reset", "Console");
            window.Close();
            return;
        }
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        GUILayout.Label("已经确保了元素唯一性！");
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Creat Code Path", GUILayout.Width(110f));
        GUILayout.TextField(windowsRoot);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Code Name", GUILayout.Width(110f));
        GUILayout.TextField(className);
        EditorGUILayout.EndHorizontal();
        isCreat = GUILayout.Toggle(isCreat, "如果是第一次创建，需要选中。   Warnning.... 否则会重构逻辑代码");
        showTemplate = EditorGUILayout.Foldout(showTemplate, "获取并将设置的属性");
        if (showTemplate)
        {
            EditorGUILayout.BeginVertical();
            foreach (var item in AllExportName_ComDic)
            {
                GUILayout.Label(string.Format("         {0}:{1}", item.Key, item.Value));
            }
            EditorGUILayout.EndVertical();
        }
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("创建代码", GUILayout.Height(60), GUILayout.Width(100)))
        {
            if (isCreat)
                WriteModuleClass(string.Format("{0}{1}.cs", windowsRoot, className));
            WriteclassTemplateClass(string.Format("{0}{1}.ui.cs", windowsRoot, className));
            window.Close();
            EditorUtility.DisplayDialog("提示：", className + "已经创建成功！", "确定", "返回");
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        AssetDatabase.Refresh();

    }
    void FindAllExportItem(Transform tran)
    {
        "Dic Clearn".PrintLog();
        AllExportName_ComDic.Clear();
        FindExportItem(tran);
    }

    void FindExportItem(Transform tra)
    {
        if (tra.tag == MyConstItem.ExportTag)
            AddExportName(tra);
        for (int i = 0; i < tra.childCount; i++)
        {
            FindExportItem(tra.GetChild(i));
        }

    }
    void AddExportName(Transform tra)
    {
        var com = GetCompont(tra);
        "Add dic".PrintLog();
        if (com != null)
        {
            if (!AllExportName_ComDic.ContainsKey(tra.name))
                AllExportName_ComDic.Add(tra.name, com.GetType().Name);
            else
            {
                "There Have Two Same Export Item  ++   {0}".ToLogFromat(Color.yellow, tra.name).PrintError();
                CanExport = false;
            }
        }
        else
        {
            if (!AllExportName_ComDic.ContainsKey(tra.name))
                AllExportName_ComDic.Add(tra.name, tra.GetType().Name);
            else
            {
                "There Have Two Same Export Item  ++   {0}".ToLogFromat(Color.yellow, tra.name).PrintError();
                CanExport = false;
            }
        }
    }

    Component GetCompont(Transform tran)
    {
        for (int i = 0; i < allType.Length; i++)
        {
            var com = tran.GetComponent(allType[i]);
            if (com == null)
                continue;
            return com;
        }
        return null;
    }

    #region :: 文本内容
    static Type[] allType = new Type[] //类型顺序: Button Image RawImage Text...
    {
        typeof(Button),
        typeof(Image),
        typeof(RawImage),
        typeof(Text),
    };

    private static string TableTemplates = @"        protected [Component] [name];";
    private static string FindTableTemplates = @"            [name] = FindChild<[Component]>(" + "\"[name]\"" + ");";

    private static string classTemplateFile =
@"using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MyTools;
namespace Windows
{
    [UUIResources(" + "\"[ClassName]\"" + @")]
    partial class [ClassName] : UUIAutoGenWindow
    {
[TableTemplates]
        protected override void InitTemplate()
        {
            base.InitTemplate();
[FindTableTemplates]
           
        }
    }
}";
    private static string moduleClass =
@"using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyTools;
namespace Windows
{
    partial class [ClassName]
    {
        protected override void InitModel()
        {
            base.InitModel();
        }
    }
}";
    #endregion

    StringBuilder sb_module;
    StringBuilder sb_temp;
    void WriteModuleClass(string path)
    {
        sb_module = new StringBuilder();
        sb_module.AppendLine(moduleClass.Replace("[ClassName]", className));
        File.WriteAllText(path, sb_module.ToString());
    }
    void WriteclassTemplateClass(string path)
    {
        File.WriteAllText(path, GetClassContent());
    }
    string GetClassContent()
    {
        sb_temp = new StringBuilder();
        sb_temp.AppendLine(classTemplateFile.Replace("[ClassName]", className));
        StringBuilder sb_TableTemplates = new StringBuilder();
        StringBuilder sb_FindTableTemplates = new StringBuilder();
        foreach (var item in AllExportName_ComDic)
        {
            sb_TableTemplates.AppendLine(TableTemplates.Replace("[Component]", item.Value).Replace("[name]", item.Key));
            sb_FindTableTemplates.AppendLine(FindTableTemplates.Replace("[Component]", item.Value).Replace("[name]", item.Key));
        }
        sb_temp.Replace("[TableTemplates]", sb_TableTemplates.ToString());
        sb_temp.Replace("[FindTableTemplates]", sb_FindTableTemplates.ToString());
        return sb_temp.ToString();
    }
}