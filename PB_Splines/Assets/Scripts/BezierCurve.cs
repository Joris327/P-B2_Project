using UnityEngine;

[System.Serializable]
public class BezierCurve
{
    public Vector3[] points = {
        new(), new(), new(), new()
    };
    
    public BezierCurve(Vector3 pPoint0, Vector3 pPoint1, Vector3 pPoint2, Vector3 pPoint3)
    {
        points[0] = pPoint0;
        points[1] = pPoint1;
        points[2] = pPoint2;
        points[3] = pPoint3;
    }
    
    public BezierCurve() {}
}
