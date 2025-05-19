using UnityEngine;
using Unity.Mathematics;

public class Spline : MonoBehaviour
{
    [SerializeField] bool loop = false;
    public bool Loop { get { return loop; } }
    
    [SerializeField] bool mirrored = false;
    public bool Mirrored { get { return mirrored; } }
    
    public BezierCurve[] curves;
}
