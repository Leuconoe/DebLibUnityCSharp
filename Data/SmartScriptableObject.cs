using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif
using System;
using DebLib;

public class SmartScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    private static readonly string DEFALT_PATH = "game.PreLoadRes.Resources";
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = CreateObject();
            }

            return instance;
        }
    }
    public static void Release()
    {
        if (instance)
        {
            instance = null;
        }
    }
    /// <summary>
    /// 생성하는 부분
    /// </summary>
    /// <param name="m_AssetDataName"></param>
    /// <param name="m_AssetDataPath"></param>
    /// <param name="m_AssetDataExtension"></param>
    /// <returns></returns>
    protected static T CreateObject()
    {
        Type _type = typeof(T);
        string m_AssetDataName = _type.Name+"Data";
        string m_AssetDataPath = DEFALT_PATH;
        if (!string.IsNullOrEmpty(_type.Namespace))
        {
            m_AssetDataPath = _type.Namespace; 
        }
        m_AssetDataPath = m_AssetDataPath.Replace(".", "/");

        T data = Resources.Load(m_AssetDataName, typeof(T)) as T;

        if (data == null)
        {
            // If not found, autocreate the asset object.
            data = ScriptableObject.CreateInstance<T>();
#if UNITY_EDITOR
            string properPath = DebUtil.CombinePath(m_AssetDataPath, m_AssetDataName + ".asset");
            string fullPath = Path.Combine(Application.dataPath + "/", properPath);
            Debug.Log(fullPath);
            DebUtil.CreateDirectoryFromFileName(fullPath);
            AssetDatabase.CreateAsset(data, "Assets/" + properPath);

#endif
        }
        
        return data;
    }
    protected static void updateProperty<T>(ref T property, T value)
    {
        if (Application.isPlaying
#if UNITY_EDITOR
            || EditorApplication.isPlaying
#endif
            )
        {
            //Debug.LogError("NOT ALLOW IN PLAYING MODE - DO NOT SET VALUE!!");
            return;
        }

        if (property != null && !property.Equals(value))
        {
            property = value;
            DirtyEditor();
        }
        else if (property == null)
        {
            property = value;
            DirtyEditor();
        }
    }
    private static void DirtyEditor()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(Instance);
#endif
    }
}

