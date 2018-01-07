using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveEditor : Editor {
    private BezierCurve curve;
    private Transform handleTransform;
    private Quaternion handleRotation;

    private const float handleSize = 0.04f;
    private const float pickSize = 0.06f;

    private int selectedPointIndex = -1;
    private int selectedCurveIndex = -1;

    private void OnSceneGUI()
    {
        curve = target as BezierCurve;
        handleTransform = curve.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                         handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = ShowPoint(0);
        for (int i = 1; i < curve.ControlPointCount; i += 3)
        {
            Vector3 p1 = ShowPoint(i);
            Vector3 p2 = ShowPoint(i + 1);
            Vector3 p3 = ShowPoint(i + 2);

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
            p0 = p3;
        }
    }

    private Vector3 ShowPoint(int point_index)
    {
        Vector3 point = handleTransform.TransformPoint(curve.GetControlPoint(point_index));
        Handles.color = Color.white;

        float size = HandleUtility.GetHandleSize(point);
        if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotCap)) {
            selectedPointIndex = point_index;
            float t = 0;
            curve.ComputeTimeIndex(ref t, out selectedCurveIndex);
            Repaint();
        }

        if (selectedPointIndex == point_index) {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curve, "Move Point");
                EditorUtility.SetDirty(curve);
                curve.SetControlPoint(point_index, handleTransform.InverseTransformPoint(point));
            }
        }

        return point;
    }

    public override void OnInspectorGUI()
    {
        curve = target as BezierCurve;
        if (selectedPointIndex >= 0 && selectedPointIndex < curve.ControlPointCount) {
            DrawSelectedPointInspector();
        }
        if (GUILayout.Button("Add Curve")) {
            Undo.RecordObject(curve, "Add Curve");
            curve.AddCurve();
            EditorUtility.SetDirty(curve);
        }
    }

    private void DrawSelectedPointInspector()
    {
        GUILayout.Label("Selected Point");
        EditorGUI.BeginChangeCheck();
        Vector3 point = EditorGUILayout.Vector3Field("Position", curve.GetControlPoint(selectedPointIndex));

		float time = 0;
		if (selectedPointIndex > 0 && (selectedPointIndex % 3) == 0) {
			time = EditorGUILayout.FloatField("Time", curve.GetTime(selectedCurveIndex));
		}

        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(curve, "Move Point");
            EditorUtility.SetDirty(curve);
            curve.SetControlPoint(selectedPointIndex, point);

			if (selectedPointIndex > 0 && (selectedPointIndex % 3) == 0) {
				curve.SetTime ((selectedPointIndex / 3) - 1, time);
			}
        }
    }
}
