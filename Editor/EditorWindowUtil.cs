using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EditorWindowUtil
{
	static public void SetLabelWidth(float width)
	{
		EditorGUIUtility.labelWidth = width;
	}

	/// <summary>
	/// Create an undo point for the specified object.
	/// </summary>
	static public void RegisterUndo(string name, Object obj) { if (obj != null) UnityEditor.Undo.RecordObject(obj, name); }

	/// <summary>
	/// Create an undo point for the specified objects.
	/// </summary>
	static public void RegisterUndo(string name, params Object[] objects) { if (objects != null && objects.Length > 0) UnityEditor.Undo.RecordObjects(objects, name); }

	/// <summary>
	/// Ensure that the angle is within -180 to 180 range.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public float WrapAngle(float angle)
	{
		while (angle > 180f) angle -= 360f;
		while (angle < -180f) angle += 360f;
		return angle;
	}

	/// <summary>
	/// In the shader, equivalent function would be 'fract'
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public float Wrap01(float val) { return val - Mathf.FloorToInt(val); }

	static void DrawString(string text, Vector3 worldPos, Color? colour = null)
	{
		UnityEditor.Handles.BeginGUI();
		if (colour.HasValue) GUI.color = colour.Value;
		var view = UnityEditor.SceneView.currentDrawingSceneView;
		Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);

		if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0)
		{
			UnityEditor.Handles.EndGUI();
			return;
		}

		Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
		GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);
		UnityEditor.Handles.EndGUI();
	}
}

