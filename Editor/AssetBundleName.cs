namespace DebLib.Resources
{
#if UNITY_EDITOR
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    //#if UNITY_EDITOR
    using UnityEditor;
    using System.Text;
    using Object = UnityEngine.Object;
    //#endif

    //#if UNITY_EDITOR
    [InitializeOnLoad]
    //#endif
    [Serializable]
    public class AssetBundleName : SmartScriptableObject<AssetBundleName>
    {


        public enum type
        {
            _this,
            _sub,
            _recursion,
        }
        [Serializable]
        public class cPnT
        {
            [SerializeField]
            private string m_path;

            public string Path
            {
                get { return m_path; }
                set { updateProperty(ref m_path, value); }
            }
            [SerializeField]
            private type m_type;

            public type Type
            {
                get { return m_type; }
                set { updateProperty(ref m_type, value); }
            }
            [SerializeField]
            private string m_guid;

            public string Guid
            {
                get { return m_guid; }
                set { updateProperty(ref m_guid, value); }
            }
        }
        [SerializeField]
        private List<cPnT> m_pathAndType = new List<cPnT>();
        public static List<cPnT> PathAndType
        {
            get { return Instance.m_pathAndType; }
            set {
                Instance.m_pathAndType = value;
                EditorUtility.SetDirty(Instance);
            }
        }
    }
#endif
}