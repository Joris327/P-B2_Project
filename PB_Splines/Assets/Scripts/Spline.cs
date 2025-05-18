using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class Spline : MonoBehaviour
{
    [SerializeField] GameObject pointPrefab;
    public bool looping = false;
    public bool mirrored = false;
    [SerializeField] float timeScale = 1;
    public BezierCurve[] curves;
    
    static readonly float4x4 characteristicMatrix = new(
        new( 1,  0,  0,  0),
        new(-3,  3,  0,  0),
        new( 3, -6,  3,  0),
        new(-1,  3, -3,  1)
    );
    
    GameObject movingPoint;
    float timeVar = 0;
    int splineIndex = 0;
    
    void Start()
    {
        if (curves.Length > 0) movingPoint = Instantiate(pointPrefab, curves[0].points[0], pointPrefab.transform.rotation);
    }

    void Update()
    {
        TravelAlongSpline();
    }
    
    void TravelAlongSpline()
    {
        if (curves.Length < 1) return;
        
        movingPoint.transform.position = CalculatePointOnSpline(timeVar, curves[splineIndex]);
        
        timeVar += Time.deltaTime * timeScale;
        
        if (timeVar >= 1)
        {
            timeVar = 0;
            if (splineIndex < curves.Length-1) splineIndex++;
            else splineIndex = 0;
        }
    }
    
    public Vector3 CalculatePointOnSpline(float timeStamp, BezierCurve curve)
    {
        //method 1 ---------------------------------------------------------------------------------------------------------------
        
        //Vector3 tempPoint1 = Vector3.Lerp(sphere1.transform.position, sphere2.transform.position, timeVar);
        //Vector3 tempPoint2 = Vector3.Lerp(sphere2.transform.position, sphere3.transform.position, timeVar);
        //Vector3 tempPoint3 = Vector3.Lerp(sphere3.transform.position, sphere4.transform.position, timeVar);
        
        //Vector3 tempPoint4 = Vector3.Lerp(tempPoint1, tempPoint2, timeVar);
        //Vector3 tempPoint5 = Vector3.Lerp(tempPoint2, tempPoint3, timeVar);
        
        //movingPoint.transform.position = Vector3.Lerp(tempPoint4, tempPoint5, timeVar);
        
        //method 2 ---------------------------------------------------------------------------------------------------------------
        
        // movingPoint.transform.position = 
        // sphere1.transform.position * (Mathf.Pow(-timeVar, 3) + (3 * Mathf.Pow(timeVar, 2)) - (3 * timeVar) + 1) +
        // sphere2.transform.position * (3 * Mathf.Pow(timeVar, 3) - (6 * Mathf.Pow(timeVar, 2)) + (3 * timeVar)) +
        // sphere3.transform.position * (-3 * Mathf.Pow(timeVar, 3) + (3 * Mathf.Pow(timeVar, 2))) +
        // sphere4.transform.position * Mathf.Pow(timeVar, 3);
        
        //method 3 ---------------------------------------------------------------------------------------------------------------
        
        float4 powersOfT = new(1, timeStamp, Mathf.Pow(timeStamp, 2), Mathf.Pow(timeStamp, 3));
        
        Vector3 worldPoint0 = curve.points[0] + transform.position;
        Vector3 worldPoint1 = curve.points[1] + transform.position;
        Vector3 worldPoint2 = curve.points[2] + transform.position;
        Vector3 worldPoint3 = curve.points[3] + transform.position;

        // float[,] pointMatrix = {
        //     { worldPoint0.x, worldPoint0.y, worldPoint0.z },
        //     { worldPoint1.x, worldPoint1.y, worldPoint1.z },
        //     { worldPoint2.x, worldPoint2.y, worldPoint2.z },
        //     { worldPoint3.x, worldPoint3.y, worldPoint3.z },
        // };

        float4x3 pointMatrix = new(
            worldPoint0.x, worldPoint0.y, worldPoint0.z,
            worldPoint1.x, worldPoint1.y, worldPoint1.z,
            worldPoint2.x, worldPoint2.y, worldPoint2.z,
            worldPoint3.x, worldPoint3.y, worldPoint3.z
        );

        // float[,] positionMatrix = {
        //     {
        //         Vector4.Dot(new(characteristicMatrix[0,0], characteristicMatrix[1,0], characteristicMatrix[2,0], characteristicMatrix[3,0]), new(pointMatrix[0,0], pointMatrix[1,0], pointMatrix[2,0], pointMatrix[3,0])),
        //         Vector4.Dot(new(characteristicMatrix[0,0], characteristicMatrix[1,0], characteristicMatrix[2,0], characteristicMatrix[3,0]), new(pointMatrix[0,1], pointMatrix[1,1], pointMatrix[2,1], pointMatrix[3,1])),
        //         Vector4.Dot(new(characteristicMatrix[0,0], characteristicMatrix[1,0], characteristicMatrix[2,0], characteristicMatrix[3,0]), new(pointMatrix[0,2], pointMatrix[1,2], pointMatrix[2,2], pointMatrix[3,2]))
        //     },
        //     {
        //         Vector4.Dot(new(characteristicMatrix[0,1], characteristicMatrix[1,1], characteristicMatrix[2,1], characteristicMatrix[3,1]), new(pointMatrix[0,0], pointMatrix[1,0], pointMatrix[2,0], pointMatrix[3,0])),
        //         Vector4.Dot(new(characteristicMatrix[0,1], characteristicMatrix[1,1], characteristicMatrix[2,1], characteristicMatrix[3,1]), new(pointMatrix[0,1], pointMatrix[1,1], pointMatrix[2,1], pointMatrix[3,1])),
        //         Vector4.Dot(new(characteristicMatrix[0,1], characteristicMatrix[1,1], characteristicMatrix[2,1], characteristicMatrix[3,1]), new(pointMatrix[0,2], pointMatrix[1,2], pointMatrix[2,2], pointMatrix[3,2]))
        //     },
        //     {
        //         Vector4.Dot(new(characteristicMatrix[0,2], characteristicMatrix[1,2], characteristicMatrix[2,2], characteristicMatrix[3,2]), new(pointMatrix[0,0], pointMatrix[1,0], pointMatrix[2,0], pointMatrix[3,0])),
        //         Vector4.Dot(new(characteristicMatrix[0,2], characteristicMatrix[1,2], characteristicMatrix[2,2], characteristicMatrix[3,2]), new(pointMatrix[0,1], pointMatrix[1,1], pointMatrix[2,1], pointMatrix[3,1])),
        //         Vector4.Dot(new(characteristicMatrix[0,2], characteristicMatrix[1,2], characteristicMatrix[2,2], characteristicMatrix[3,2]), new(pointMatrix[0,2], pointMatrix[1,2], pointMatrix[2,2], pointMatrix[3,2]))
        //     },
        //     {
        //         Vector4.Dot(new(characteristicMatrix[0,3], characteristicMatrix[1,3], characteristicMatrix[2,3], characteristicMatrix[3,3]), new(pointMatrix[0,0], pointMatrix[1,0], pointMatrix[2,0], pointMatrix[3,0])),
        //         Vector4.Dot(new(characteristicMatrix[0,3], characteristicMatrix[1,3], characteristicMatrix[2,3], characteristicMatrix[3,3]), new(pointMatrix[0,1], pointMatrix[1,1], pointMatrix[2,1], pointMatrix[3,1])),
        //         Vector4.Dot(new(characteristicMatrix[0,3], characteristicMatrix[1,3], characteristicMatrix[2,3], characteristicMatrix[3,3]), new(pointMatrix[0,2], pointMatrix[1,2], pointMatrix[2,2], pointMatrix[3,2]))
        //     }
        // };

        float4x3 positionMatrix = math.mul(characteristicMatrix, pointMatrix);

        // Vector3 returnValue = new(
        //     Vector4.Dot(powersOfT, new(positionMatrix[0,0], positionMatrix[1,0], positionMatrix[2,0], positionMatrix[3,0])),
        //     Vector4.Dot(powersOfT, new(positionMatrix[0,1], positionMatrix[1,1], positionMatrix[2,1], positionMatrix[3,1])),
        //     Vector4.Dot(powersOfT, new(positionMatrix[0,2], positionMatrix[1,2], positionMatrix[2,2], positionMatrix[3,2]))
        // );

        Vector3 returnValue = math.mul(powersOfT, positionMatrix);
        
        return returnValue;
    }
}
