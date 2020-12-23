namespace DebLib.Resources
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(AssetBundleName))]
    public class AssetBundleNameDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            List<AssetBundleName.cPnT> datas = AssetBundleName.PathAndType;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Sort Name"))
            {
                datas.Sort(
                        delegate (AssetBundleName.cPnT d1, AssetBundleName.cPnT d2)
                        {
                            return d1.Path.CompareTo(d2.Path);
                        }
                        );
                AssetBundleName.PathAndType = datas;
            }

            if (GUILayout.Button("Sort Type"))
            {
                datas.Sort(
                        delegate (AssetBundleName.cPnT d1, AssetBundleName.cPnT d2)
                        {
                            int result = d1.Type.CompareTo(d2.Type);
                            return result != 0 ? result : d1.Path.CompareTo(d2.Path);
                        }
                        );
                AssetBundleName.PathAndType = datas;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            for (int i = 0; i < datas.Count; i++)
            {
                AssetBundleName.cPnT item = datas[i];
                if (string.IsNullOrEmpty(item.Guid))
                {
                    continue;
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
                {
                    datas.RemoveAt(i);
                    AssetBundleName.PathAndType = datas;
                    break;
                }
                EditorGUILayout.LabelField(i + ":", GUILayout.MaxWidth(20));
                string path = item.Path;
                EditorGUILayout.LabelField(path.Replace("Assets/", ""));

                string realPath = AssetDatabase.GUIDToAssetPath(item.Guid);

                AssetImporter assetImporter = AssetImporter.GetAtPath(realPath);
                if (assetImporter != null)
                {
                    EditorGUILayout.LabelField(new GUIContent("[ " + assetImporter.assetBundleName + " ] ", "GUID : " + item.Guid)
                        , GUILayout.MaxWidth(140));
                    item.Type = (AssetBundleName.type)EditorGUILayout.EnumPopup(item.Type, GUILayout.MaxWidth(60));
                    item.Path = assetImporter.assetPath;
                    
                }
                datas[i] = item;

                EditorGUILayout.EndHorizontal();
            }

            if (datas != AssetBundleName.PathAndType)
            {
                AssetBundleName.PathAndType = datas;
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("(Re) Set AssetName"))
            {
                ReSetAssetProcess();
            }

            if (GUILayout.Button("Clear AssetName"))
            {
                NativeFunctionHelper.ShowEditorPopup("경고", "진짜 전체 초기화 하시겠습니까?\n메타파일이 변경됩니다. 메타파일 롤백하면 컴파일 시간이 길어집니다. 저ㅇ말로 길ㅇㅓ집니다.",
                    "예",
                    () =>
                    {
                        ClearName();
                    },
                    " 아니요", null
                    );
            }

            if (GUILayout.Button("regenerate"))
            {
                ClearName();
                ReSetAssetProcess();
            }
        }

        private static void ClearName()
        {
            int clsCnt = 0;
            string[] assetbundleNames = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < assetbundleNames.Length; i++)
            {
                if (AssetDatabase.RemoveAssetBundleName(assetbundleNames[i], true))
                {
                    clsCnt++;
                }
            }
            AssetDatabase.RemoveUnusedAssetBundleNames();
            Debug.Log(clsCnt + "개 이름 지워짐");
        }

        [MenuItem("Assets/AssetBundles/Add AssetBundleName Data")]
        private static void SetAssetBundleName()
        {
            int cnt = 0;
            foreach (var item in Selection.objects)
            {
                bool isExist = false;
                int instanceID = item.GetInstanceID();
                string path = AssetDatabase.GetAssetPath(instanceID);
                foreach (var item2 in AssetBundleName.PathAndType)
                {
                    if (!string.IsNullOrEmpty(item2.Path) && item2.Path.Equals(path))
                    {
                        isExist = true;
                        break;
                    }
                }

                if (!isExist)
                {
                    AssetBundleName.cPnT data = new AssetBundleName.cPnT();
                    data.Path = path;
                    data.Guid = AssetDatabase.AssetPathToGUID(path);
                    List<AssetBundleName.cPnT> l = AssetBundleName.PathAndType;
                    l.Add(data);
                    AssetBundleName.PathAndType = l;
                    cnt++;
                }
            }
            if (cnt != 0)
            {
                Debug.Log(cnt + " add path");
            }
        }

        [MenuItem("Assets/AssetBundles/Clear AssetBundle Name")]
        private static void ClearAssetBundleName()
        {
            foreach (var item in Selection.objects)
            {
                int instanceID = item.GetInstanceID();
                string path = AssetDatabase.GetAssetPath(instanceID);
                AssetImporter assetImporter = AssetImporter.GetAtPath(path);
                if (assetImporter != null && assetImporter.assetBundleName != "")
                {
                    assetImporter.assetBundleName = "";
                }

                List<string> files = new List<string>(Directory.GetFiles(path, "*.*", SearchOption.AllDirectories));    // file
                files.AddRange(new List<string>(Directory.GetDirectories(path, "*.*", SearchOption.AllDirectories)));
                for (int i = 0; i < files.Count; i++)
                {
                    string subPath = files[i].Replace("\\", "/");
                    assetImporter = AssetImporter.GetAtPath(subPath);
                    if (assetImporter != null && assetImporter.assetBundleName != "")
                    {
                        assetImporter.assetBundleName = "";
                    }
                }
            }
        }

        [MenuItem("Assets/AssetBundles/move file from filepath")]
        private static void CreateFolderFromSubFileName()
        {
            foreach (var item in Selection.objects)
            {
                int instanceID = item.GetInstanceID();
                string path = AssetDatabase.GetAssetPath(instanceID);
                List<string> files = new List<string>(Directory.GetFiles(path, "*.*", SearchOption.AllDirectories));    // file
                files.AddRange(new List<string>(Directory.GetDirectories(path, "*.*", SearchOption.AllDirectories)));
                for (int i = 0; i < files.Count; i++)
                {
                    string subPath = files[i].Replace("\\", "/");
                    string fileName = Path.GetFileName(subPath).Split('.')[0];
                    string folderPath = Path.GetDirectoryName(subPath);
                    if (!Directory.Exists(folderPath + "/" + fileName))
                    {
                        AssetDatabase.CreateFolder(folderPath, fileName);
                    }
                    AssetDatabase.MoveAsset(subPath, folderPath + "/" + fileName + "/" + Path.GetFileName(subPath));
                }
            }
        }

        //#if UNITY_EDITOR
        [MenuItem("DebLibs/AssetBundle Name Manager")]
        public static void Edit()
        {
            Selection.activeObject = AssetBundleName.Instance;
        }

        public static void ReSetAssetProcess()
        {
            int cnt = 0;
            List<AssetBundleName.cPnT> existItems = new List<AssetBundleName.cPnT>();
            foreach (var item in AssetBundleName.PathAndType)
            {
                //path는 유효하지 않을 수 있으니 GUID로
                string path = AssetDatabase.GUIDToAssetPath(item.Guid);
                Object asset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
                if (asset != null)
                {
                    existItems.Add(item);
                }
                else
                {
                    continue;
                }

                string assetName = replacePath(asset.name);

                if (asset.GetType().Equals(typeof(MonoScript)))
                {
                    // 스크립트
                    continue;
                }
                else if (asset.GetType().Equals(typeof(DefaultAsset)))
                {
                    switch (item.Type)
                    {
                        case AssetBundleName.type._this:
                            SetAssetBundleName(path, assetName);
                            break;

                        case AssetBundleName.type._sub:
                            {
                                string[] files = System.IO.Directory.GetFiles(path);    // file
                                for (int i = 0; i < files.Length; i++)
                                {
                                    string subPath = files[i];
                                    Object subAsset = AssetDatabase.LoadAssetAtPath(subPath, typeof(Object));
                                    if (subAsset != null)
                                    {
                                        SetAssetBundleName(subPath, subAsset.name);
                                        cnt++;
                                    }
                                }
                                string[] subFolderPath = AssetDatabase.GetSubFolders(path);  //folder
                                for (int i = 0; i < subFolderPath.Length; i++)
                                {
                                    string subPath = subFolderPath[i];
                                    if (isIgnorePath(subPath))
                                    {
                                        continue;
                                    }
                                    SetAssetBundleName(subPath, replacePath(subPath));
                                    cnt++;
                                }
                            }
                            break;

                        case AssetBundleName.type._recursion:
                            {
                                string[] files = System.IO.Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);    // file
                                for (int i = 0; i < files.Length; i++)
                                {
                                    string subPath = files[i].Replace("\\", "/");
                                    if (isIgnorePath(subPath))
                                    {
                                        continue;
                                    }
                                    SetAssetBundleName(subPath, replacePath(subPath).Split('.')[0]);
                                    cnt++;
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
            AssetBundleName.PathAndType = existItems;

            Debuger.Log(cnt + " item set name");
        }

        private static void SetAssetBundleName(string path, string AssetName)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            if (assetImporter == null)
            {
                return;
            }
            if (assetImporter.assetBundleName != AssetName)
            {
                assetImporter.assetBundleName = AssetName;
            }

            //Debug.Log("\"" + path + "\"" + " is Assigned " + "\"" + AssetName + "\"");
        }

        private static string replacePath(string path)
        {
            /*
                        if (path.StartsWith("Assets/Noblesse/Objects/"))
                        {
                            path = path.Replace("Assets/Noblesse/Objects/", "");
                        }

                        if (path.StartsWith("Assets/Noblesse/Resources1/"))
                        {
                            path = path.Replace("Assets/Noblesse/Resources1/", "");
                        }
            */
            for (int i = 0; i < m_replacePathList.Length; i++)
            {
                path = path.Replace(m_replacePathList[i], "");
            }
            return path;
        }

        private static bool isIgnorePath(string path)
        {
            for (int i = 0; i < m_ignorePathList.Length; i++)
            {
                if (path.Contains(m_ignorePathList[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private static string[] m_replacePathList = new string[]
        {
            "Assets/Game/Resources1/",
            "Assets/Game/Resources/",
            "Assets/Game/Res/",
            "Assets/Game/",
        };

        private static string[] m_ignorePathList = new string[]
        {
            "_ignore",
            ".meta",
            ".xls",
            //".mp4",
            "MovieAssets",
            //".cs",
            //".shader",
        };

        private void SetAssetName(string path)
        {
        }

        //#endif
    }
}