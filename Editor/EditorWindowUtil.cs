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
}
