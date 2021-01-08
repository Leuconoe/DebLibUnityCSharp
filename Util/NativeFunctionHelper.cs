using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
//using EasyMobile;
public class NativeFunctionHelper : MonoBehaviour
{
    private static NativeFunctionHelper instance;
    public static NativeFunctionHelper sharedInstance()
    {
        if (instance == null)
        {
            GameObject go = new GameObject("NativeManager");
            instance = go.gameObject.AddComponent<NativeFunctionHelper>();
            DontDestroyOnLoad(instance);

            //NativeUI.IsShowingAlert();

        }
        return instance;
    }

    public void ShowPopup(string title, string message, Action action1)
    {
        ShowPopup(title, message, "OK", action1);
    }
    public void ShowPopup(string title, string message, string btn1, Action action1)
    {
        if (action1 == null)
        {
            action1 = EmptyFunction;
        }

#if UNITY_EDITOR
        ShowEditorPopup(title, message, btn1, action1, string.Empty, null);
#else
/*
         EasyMobile.NativeUI.Alert(title, message, btn1).OnComplete += (buttonIndex) => 
        {
            bool isFistButtonClicked = buttonIndex == 0;
            bool isSecondButtonClicked = buttonIndex == 1;
            bool isThirdButtonClicked = buttonIndex == 2;

            if (isFistButtonClicked) { if (action1 != null) action1.Invoke(); } ;
            if (isSecondButtonClicked) { };
            if (isThirdButtonClicked) { };
        };
        */
#endif




    }
    public void ShowPopup(string title, string message, string btn1, Action action1, string btn2, Action action2)
    {
        if (action1 == null)
        {
            action1 = EmptyFunction;
        }
        if (action2 == null)
        {
            action2 = EmptyFunction;
        }
#if UNITY_EDITOR
        ShowEditorPopup(title, message, btn1, action1, btn2, action2);
#else
/*
        EasyMobile.NativeUI.ShowTwoButtonAlert(title, message, btn1, btn2).OnComplete += (buttonIndex) =>
        {
            bool isFistButtonClicked = buttonIndex == 0;
            bool isSecondButtonClicked = buttonIndex == 1;
            bool isThirdButtonClicked = buttonIndex == 2;

            if (isFistButtonClicked) { if (action1 != null) action1.Invoke(); };
            if (isSecondButtonClicked) { if (action2 != null) action2.Invoke(); };
            if (isThirdButtonClicked) { };
        };
        */
#endif


    }

    // action이 null일경우에 대한 처리.
    private void EmptyFunction()
    {

    }

    public void ShowToast(string msg)
    {
#if UNITY_EDITOR
        showEditorToast(msg);
#elif UNITY_ANDROID
        //AndroidNativeFunctions.ShowToast(msg);
#elif UNITY_IOS
        ShowPopup("Notice", msg, "OK", null);
#endif
    }
    public static void showEditorToast(string msg)
    {
#if UNITY_EDITOR
        GUIContent con = new GUIContent("******TOASTMESSAGE******\n" + msg);
        System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
        System.Type type = assembly.GetType("UnityEditor.GameView");
        UnityEditor.EditorWindow gameview = UnityEditor.EditorWindow.GetWindow(type);
        if (gameview != null)
        {
            gameview.ShowNotification(con);
        }

        if (UnityEditor.SceneView.currentDrawingSceneView != null)
        {
            UnityEditor.SceneView.currentDrawingSceneView.ShowNotification(con);
        }
#endif
    }

    public static void ShowEditorPopup(string title, string message, string btn1, Action action1, string btn2 = "", Action action2 = null)
    {
#if UNITY_EDITOR

        if (UnityEditor.EditorUtility.DisplayDialog(title, message, btn1, btn2))
        {
            if (action1 != null) action1.Invoke();
        }
        else
        {
            if (action2 != null) action2.Invoke();
        }
#else
        Debug.LogError("not found type");
#endif
    }

}