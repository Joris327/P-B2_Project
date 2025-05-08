using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spline))]
public class SplineEditor : Editor
{
    const int linesPerCurve = 20;
    
    void OnSceneGUI()
    {
        Spline spline = target as Spline;
        
        Transform handleTransform = spline.transform;
        Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;
        
        for (int i = 0; i < spline.curves.Length; i++)
        {
            BezierCurve curve = spline.curves[i];
            
            Vector3 point0 = ShowPoint(0, i, spline, curve, handleTransform, handleRotation);
            Vector3 point1 = ShowPoint(1, i, spline, curve, handleTransform, handleRotation);
            Vector3 point2 = ShowPoint(2, i, spline, curve, handleTransform, handleRotation);
            Vector3 point3 = ShowPoint(3, i, spline, curve, handleTransform, handleRotation);
            
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
    
    Vector3 ShowPoint(int pointIndex, int curveIndex, Spline spline, BezierCurve curve, Transform handleTransform, Quaternion handleRotation)
    {
        Vector3 point = handleTransform.TransformPoint(curve.points[pointIndex]);
        Vector3 oldPoint = point;
        
        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Move point");
            EditorUtility.SetDirty(spline);
            curve.points[pointIndex] = handleTransform.InverseTransformPoint(point);
            
            if (pointIndex == 0 && curveIndex > 0)
            {
                spline.curves[curveIndex-1].points[3] = handleTransform.InverseTransformPoint(point);
            }
            else if (pointIndex == 3 && curveIndex < spline.curves.Length-1)
            {
                spline.curves[curveIndex+1].points[0] = handleTransform.InverseTransformPoint(point);
            }
            else if (spline.looping)
            {
                if (curveIndex == 0 && pointIndex == 0) spline.curves[^1].points[3] = handleTransform.InverseTransformPoint(point);
                else if (curveIndex == spline.curves.Length-1 && pointIndex == 3) spline.curves[0].points[0] = handleTransform.InverseTransformPoint(point);
            }
            
            if (spline.mirrored)
            {
                Vector3 pointDiff = point - oldPoint;
                
                if (pointIndex == 0)
                {
                    curve.points[1] += pointDiff;
                    if (curveIndex > 0) spline.curves[curveIndex-1].points[2] += pointDiff;
                    else if (spline.looping && curveIndex == 0) spline.curves[^1].points[2] += pointDiff;
                }
                else if (pointIndex == 1)
                {
                    Vector3 oppositePos = (curve.points[0] - curve.points[1]) * 2 + curve.points[1];
                    
                    if (spline.looping && curveIndex == 0) spline.curves[^1].points[2] = oppositePos;
                    else if (curveIndex > 0) spline.curves[curveIndex-1].points[2] = oppositePos;
                }
                else if (pointIndex == 2)
                {
                    Vector3 oppositePos = (curve.points[3] - curve.points[2]) * 2 + curve.points[2];
                    
                    if (spline.looping && curveIndex == spline.curves.Length-1) spline.curves[0].points[1] = oppositePos;
                    else if (curveIndex < spline.curves.Length-1) spline.curves[curveIndex+1].points[1] = oppositePos;
                }
                else if (pointIndex == 3)
                {
                    curve.points[2] += pointDiff;
                    if (curveIndex < spline.curves.Length-1) spline.curves[curveIndex+1].points[1] += pointDiff;
                    else if (spline.looping && curveIndex == spline.curves.Length-1) spline.curves[0].points[1] += pointDiff;
                }
            }
        }
        
        return point;
    }
}
