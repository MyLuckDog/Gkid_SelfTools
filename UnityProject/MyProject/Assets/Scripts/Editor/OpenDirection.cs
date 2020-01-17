using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class OpenDirection : EditorWindow
{
    static string Path01;
    static string Path02;
    static string Path03;
    [MenuItem("Tools/打开路径")]
    public static void OpenFile()
    {
        OpenDirection file = GetWindow<OpenDirection>("窗口");
        Path01 = Application.temporaryCachePath;
        Path02 = Application.persistentDataPath;
        Path03 = Application.consoleLogPath;
        file.minSize = new Vector2(600, 200);
        file.Show();
    }
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(Path01);
        if (GUILayout.Button("打开temporaryCachePath"))
        {
            System.Diagnostics.Process.Start(Path01);
        }

        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Label(Path02);
        if (GUILayout.Button("打开persistentDataPath"))
            System.Diagnostics.Process.Start(Path02);
        GUILayout.EndHorizontal();


        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.TextField(Path03);
        if (GUILayout.Button("打开consoleLogPath"))
            System.Diagnostics.Process.Start(Path03);
        GUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

    }
}
