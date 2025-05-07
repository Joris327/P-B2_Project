using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spline))]
public class SplineEditor : Editor
{
    const int linesPerCurve = 10;
    
    void OnSceneGUI()
    {
        Spline spline = target as Spline;
        
        Transform handleTransform = spline.transform;
        Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;
        
        for (int i = 0; i < spline.curves.Length; i++)
        {
            BezierCurve curve = spline.curves[i];
            
            Vector3 point0 = ShowPoint(0, spline, curve, handleTransform, handleRotation);
            Vector3 point1 = ShowPoint(1, spline, curve, handleTransform, handleRotation);
            Vector3 point2 = ShowPoint(2, spline, curve, handleTransform, handleRotation);
            Vector3 point3 = ShowPoint(3, spline, curve, handleTransform, handleRotation);
            
            Handles.color = Color.grey;
            Handles.DrawLine(point0, point1);
            Handles.DrawLine(point2, point3);
            Handles.color = Color.white;
            
            Vector3 lineStart = curve.points[0] + spline.transform.position;
            for (int j = 0; j <= linesPerCurve; j++)
            {
                Vector3 lineEnd = spline.CalculatePointOnSpline(j / (1f * linesPerCurve), curve);
                Handles.DrawLine(spline.transform.TransformDirection(lineStart), spline.transform.TransformDirection(lineEnd));
                lineStart = lineEnd;
            }
        }
    }
    
    Vector3 ShowPoint(int index, Spline spline, BezierCurve curve, Transform handleTransform, Quaternion handleRotation)
    {
        Vector3 point = handleTransform.TransformPoint(curve.points[index]);
        
        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Move point");
            EditorUtility.SetDirty(spline);
            curve.points[index] = handleTransform.InverseTransformPoint(point);
        }
        
        return point;
    }
}
