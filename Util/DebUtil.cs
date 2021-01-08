using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace DebLib
{
    public sealed class DebUtil
    {
        #region file I/O

        /// <summary>
        /// 파라메터를 앞부분부터 정확한 Path로 더한다
        ///
        /// 수동조합으로 file:///var//abc/d.asset 이렇게 패스가 생성된경우
        /// iOS에서는 var//abc 사이의 슬래시 두개 (//)때문에
        /// WWW 클래스 사용시 문제발생해서 추가
        ///
        /// Path.Combine의 예외동작이 있다. 주의
        /// @see http://sassembla.github.io/Public/2015:02:24%200-32-46/2015:02:24%200-32-46.html
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string CombinePath(params string[] paths)
        {
            string str = paths[0];
            for (int i = 1; i < paths.Length; i++)
            {
                str = Path.Combine(str + "/", paths[i]);
            }
            return str;
        }

        public static System.Uri MakeUri(params string[] paths)
        {
            System.Uri uri = new System.Uri(string.Join("/", paths));
            return uri;
        }

        /// <summary>
        /// 파일 이름을 잘라서 폴더를 생성한다.
        /// 바로 생성하면 접근 오류가 나서.
        /// </summary>
        /// <param name="path">파일의 FullPath</param>
        public static void CreateDirectoryFromFileName(string path)
        {
            System.Uri uri = MakeUri(path);
            CreateDirectory(System.IO.Path.GetDirectoryName(uri.LocalPath));
        }

        /// <summary>
        /// 빈폴더를 지워준다
        /// </summary>
        /// <param name="path"></param>
        public static void RemoveEmptyDirectory(string path)
        {
            foreach (var directory in Directory.GetDirectories(path))
            {
                RemoveEmptyDirectory(directory);
                if (Directory.GetFiles(directory).Length == 0 &&
                    Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                }
            }
        }

        public static void DeleteDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }
            System.IO.DirectoryInfo di = new DirectoryInfo(path);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            Directory.Delete(path);
        }

        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 파일 삭제, 동일 경로에 폴더가 있으면 폴더도 삭제
        /// </summary>
        /// <param name="path"></param>
        public static void RemoveFile(string path)
        {
            if (File.Exists(Path.GetDirectoryName(path)))
            {
                File.Delete(Path.GetDirectoryName(path));
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            DebUtil.CreateDirectoryFromFileName(path);

            if (Directory.Exists(path))
            {
                DebUtil.DeleteDirectory(path);
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// 에디터에서 외부 실행파일을 호출하기 위한 API
        /// 안드로이드 빌드 이후 사인코드 후처리를 위해 사용했었음.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string ExecuteCommand(string command)
        {
#if UNITY_EDITOR_WIN
            try
            {
                //Debug.Log("Running command: " + command);

                System.Diagnostics.ProcessStartInfo procStartInfo =
                new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.RedirectStandardError = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;

                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                proc.WaitForExit();
                string result = proc.StandardOutput.ReadToEnd();
                string error = proc.StandardError.ReadToEnd();

                return result;
            }
            catch (System.Exception e)
            {
                // Log the exception
                Debug.LogError("Got exception: " + e.Message);
                return string.Empty;
            }
#elif UNITY_EDITOR_OSX
            try
            {
            System.Diagnostics.Process proc = new System.Diagnostics.Process ();
            proc.StartInfo.FileName = "/bin/bash";
            proc.StartInfo.Arguments = "-c \" " + command + " \"";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;

            proc.Start ();
            proc.WaitForExit();

            string result = proc.StandardOutput.ReadToEnd();
            string error = proc.StandardError.ReadToEnd();

            return result;
            }
            catch (System.Exception e)
            {
            // Log the exception
            Debug.LogError("Got exception: " + e.Message);
            return string.Empty;
            }
            return "";
#else
                        return "";
#endif
        }

#endif
        private static string _persistentDataPath;

        /// <summary>
        /// persistentDataPath가 캐싱되서 생기는 문제 수정
        /// (저장소 권한 얻기 전과 후의 값이 동일함, 재부팅하면 바뀜)
        /// </summary>
        public static string persistentDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(_persistentDataPath))
                {
                    _persistentDataPath = Application.persistentDataPath;
                    return _persistentDataPath;
#if !UNITY_EDITOR
#if UNITY_ANDROID
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayerActivity"))
                {
                    using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        using (AndroidJavaObject externalFilesDir = currentActivity.Get<AndroidJavaObject>("getExternalFilesDir"))
                        {
                            _persistentDataPath = externalFilesDir.Call<string>("getCanonicalPath");
                        }
                    }
                }
#elif UNITY_IOS
                UnityEngine.iOS.Device.SetNoBackupFlag(_persistentDataPath.EndsWith("/") ? _persistentDataPath : _persistentDataPath + "/");
                setNobackupFlag();
#endif
#endif
                }

                return _persistentDataPath;
            }
        }

#if UNITY_IOS
        /// <summary>
        /// iOS에서 백업플래그때문에 생기는 문제 방지
        /// </summary>
        private static void setNobackupFlag()
        {
        UnityEngine.iOS.Device.SetNoBackupFlag(Application.dataPath.EndsWith("/") ? Application.dataPath : Application.dataPath + "/");
        UnityEngine.iOS.Device.SetNoBackupFlag(Application.persistentDataPath.EndsWith("/") ? Application.persistentDataPath : Application.persistentDataPath + "/");
        UnityEngine.iOS.Device.SetNoBackupFlag(Application.streamingAssetsPath.EndsWith("/") ? Application.streamingAssetsPath : Application.streamingAssetsPath + "/");
        UnityEngine.iOS.Device.SetNoBackupFlag(Application.temporaryCachePath.EndsWith("/") ? Application.temporaryCachePath : Application.temporaryCachePath + "/");
        }
#endif

        #endregion file I/O

        #region compare

        /// <summary>
        /// 소트할 값을 여러개 던져서 일괄처리한다.
        /// </summary>
        /// <param name="param">홀수 d1, 짝수 d2</param>
        /// <returns></returns>
        //public static int sortBy(params System.IComparable[] param)
        //{
        //    int length = param.Length / 2;
        //    for (int i = 0; i < length; i++)
        //    {
        //        System.IComparable d1 = param[2 * i];
        //        System.IComparable d2 = param[(2 * i) + 1];
        //        if (!d1.Equals(d2))
        //        {
        //            return d2.CompareTo(d1);
        //        }
        //    }
        //
        //    return 0;
        //}

        #endregion compare

        #region string

        /// <summary>
        /// 문자열들을 한문자열로 합침
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string CombineString(params object[] args)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < args.Length; i++)
            {
                sb.Append(args[i].ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// 시스템에 맞게 숫자에 콤마,소숫점 처리함
        /// </summary>
        /// <param name="val"></param>
        /// <param name="demicalCount"></param>
        /// <returns></returns>
        public static string NumberAddComma(IFormattable val, int demicalCount = 2)
        {
            //string.Format(System.Globalization.CultureInfo.CurrentCulture , "{0:#.##}",val);
            return val.ToString("#,#0." + string.Concat("#", demicalCount), System.Globalization.CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Json을 이쁘게 재정렬함.
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static string ArrangeJSON(string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
                return "";

            string indent = "   ";
            string result = "";

            long indentDepth = 0;
            string targetStr = "";

            for (int i = 0; i < jsonStr.Length; i++)
            {
                targetStr = jsonStr.Substring(i, 1);
                if (targetStr.Equals("{") || targetStr.Equals("["))
                {
                    result += targetStr + "\n";
                    indentDepth++;
                    for (int j = 0; j < indentDepth; j++) result += indent;
                }
                else if (targetStr.Equals("}") || targetStr.Equals("]"))
                {
                    result += "\n";
                    indentDepth--;
                    for (int j = 0; j < indentDepth; j++) result += indent;
                    result += targetStr;
                }
                else if (targetStr.Equals(","))
                {
                    result += targetStr + "\n";
                    for (int j = 0; j < indentDepth; j++) result += indent;
                }
                else result += targetStr;
            }
            return result;
        }

        #endregion string
    }
}

public static class ClassExtension
{
    #region object
    public static bool IsRealNull(this UnityEngine.Object obj)
    {
        return ReferenceEquals(obj, null);
    }

    public static bool IsFakeNull(this UnityEngine.Object obj)
    {
        return !ReferenceEquals(obj, null) && obj;
    }

    public static bool IsAssigned(this UnityEngine.Object obj)
    {
        return obj;
    }
    #endregion
    #region gameobject
    public static List<GameObject> GetAllChilds(this GameObject Go)
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < Go.transform.childCount; i++)
        {
            list.Add(Go.transform.GetChild(i).gameObject);
        }
        return list;
    }
    #endregion
    #region EDITOR
#if UNITY_EDITOR
    public static void DrawString(string text, Vector3 worldPos, Color? colour = null)
    {
        UnityEditor.Handles.BeginGUI();
        var view = UnityEditor.SceneView.currentDrawingSceneView;
        Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);

        if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0)
        {
            UnityEditor.Handles.EndGUI();
            return;
        }

        Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
        if (!colour.HasValue)
        {
            Color c = GUI.color;
            GUI.color = Color.black;
            GUI.Label(new Rect(screenPos.x - (size.x / 2) - 1, -screenPos.y + view.position.height + 4 - 1, size.x, size.y), text);
            GUI.Label(new Rect(screenPos.x - (size.x / 2) - 1, -screenPos.y + view.position.height + 4 + 1, size.x, size.y), text);
            GUI.Label(new Rect(screenPos.x - (size.x / 2) + 1, -screenPos.y + view.position.height + 4 + 1, size.x, size.y), text);
            GUI.Label(new Rect(screenPos.x - (size.x / 2) + 1, -screenPos.y + view.position.height + 4 - 1, size.x, size.y), text);
            GUI.color = c;
        }
        if (colour.HasValue) GUI.color = colour.Value;
        GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);
        UnityEditor.Handles.EndGUI();
    }
#endif
    #endregion
}
