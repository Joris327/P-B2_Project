using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Spline))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SplineMesh : MonoBehaviour
{
    Spline spline;
    MeshFilter meshFilter;
    
    List<Vector3> vertices;
    
    [SerializeField, Min(2)] float vertexResolution = 10;
    [SerializeField] float roadWidth = 1;
    
    enum Axis { x, y, z }

    void Awake()
    {
        if (!TryGetComponent(out spline)) Debug.LogError(name + ": could not find Spline component.");
        if (!TryGetComponent(out meshFilter)) Debug.LogError(name + ": could not find MeshFilter component.");
    }

    void Update()
    {
        GenerateMesh();
    }
    bool log = true;
    void GenerateMesh()
    {
        vertices = new();
        List<Vector2> uv = new();
        
        for (int i = 0; i < spline.curves.Length; i++)
        {
            BezierCurve curve = spline.curves[i];
            if (i > 0) log = false;
            for (int j = 0; j < vertexResolution; j++)
            {
                Vector3 centrePoint = curve.CalculatePointOnCurve(j / vertexResolution, transform.position) - transform.position;
                //Vector3 nextPos = spline.curves[i].CalculatePointOnCurve((j+1) / vertexResolution, transform.position) - transform.position;
                Vector3 direction = curve.GetDirection(j / vertexResolution, transform);
                
                float progress = j / vertexResolution;
                //float segmentProgress;
                float interpolatedAngle;
                
                // if (progress < 0.33f)
                // {
                //     segmentProgress = progress / 0.33f;
                //     interpolatedAngle = Mathf.LerpAngle(curve.angles[0], curve.angles[1], segmentProgress);//(curve.angles[0] * (1-progress) + curve.angles[1] * progress);
                // }
                // else if (progress < 0.67f)
                // {
                //     segmentProgress = (progress - 0.33f) / 0.33f;
                //     interpolatedAngle = Mathf.LerpAngle(curve.angles[1], curve.angles[2], segmentProgress);
                // }
                // else
                // {
                //     segmentProgress = (progress - 0.67f) / 0.33f;
                //     interpolatedAngle = Mathf.LerpAngle(curve.angles[2], curve.angles[3], segmentProgress);
                // }
                
                interpolatedAngle = Mathf.Lerp(curve.angles[0], curve.angles[1], progress);
                if (log) Debug.Log(interpolatedAngle);

                // centrePoint.z += roadWidth;
                // vertices.Add(centrePoint);
                // uv.Add(new(0, (i % 2) / 2f));
                // centrePoint.z -= roadWidth*2;
                // vertices.Add(centrePoint);
                // uv.Add(new(1, (i % 2) / 2f));
                
                Vector3 angleDir = direction; //forward
                Vector3 axis;
                
                switch (BiggestAxis(direction))
                {
                    case Axis.y: axis = new(-direction.y, direction.x, direction.z); break;
                    default: axis = new(-direction.z, direction.y, direction.x); break;
                }
                
                //angleDir = Quaternion.Euler(0, -90, 0) * angleDir; //90 degrees along global y axis
                angleDir = Quaternion.AngleAxis(90, new(-direction.z, direction.y, direction.x)) * angleDir; //rotate 90 degrees up along a straight angle
                if (log) Debug.Log(angleDir);
                angleDir = Quaternion.AngleAxis(interpolatedAngle, direction) * angleDir; //X degrees along local z axis
                if (log) Debug.Log(angleDir);
                Vector3 cross = Vector3.Cross(direction, angleDir.normalized).normalized;
                //cross = Vector3.Cross(cross, direction).normalized;
                
                Vector3 vertex1 = centrePoint + (cross * roadWidth);
                Vector3 vertex2 = centrePoint - (cross * roadWidth);
                
                vertices.Add(vertex1);
                uv.Add(new(0, (i % 2) / 2f));
                vertices.Add(vertex2);
                uv.Add(new(1, (i % 2) / 2f));
            }
        }
        
        List<int> triangles = new();
        for (int i = 0; i < vertices.Count-2; i+=2)
        {
            triangles.Add(i);
            triangles.Add(i+2);
            triangles.Add(i+1);
            triangles.Add(i+1);
            triangles.Add(i+2);
            triangles.Add(i+3);
        }
        
        if (spline.Loop)
        {
            triangles.Add(vertices.Count-2);
            triangles.Add(0);
            triangles.Add(vertices.Count-1);
            triangles.Add(vertices.Count-1);
            triangles.Add(0);
            triangles.Add(1);
        }
        else
        {
            Vector3 centrePoint = spline.curves[^1].CalculatePointOnCurve(1, transform.position) - transform.position;
            
            centrePoint.z -= roadWidth;
            vertices.Add(centrePoint);
            uv.Add(new(0, 0.5f));
            centrePoint.z += roadWidth*2;
            vertices.Add(centrePoint);
            uv.Add(new(1, 0.5f));
            
            triangles.Add(vertices.Count-4);
            triangles.Add(vertices.Count-2);
            triangles.Add(vertices.Count-3);
            triangles.Add(vertices.Count-4);
            triangles.Add(vertices.Count-1);
            triangles.Add(vertices.Count-2);
        }

        meshFilter.mesh = new()
        {
            name = "Spline Mesh",
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray()
        };
    }
    
    Axis BiggestAxis (Vector3 input)
    {
        if (input.x > input.y && input.x > input.z) return Axis.x;
        if (input.y > input.x && input.y > input.z) return Axis.y;
        if (input.z > input.x && input.z > input.y) return Axis.z;
        return Axis.z;
    }
    
    private void OnDrawGizmos () {
        if (vertices == null) {
			return;
		}
        
		Gizmos.color = Color.black;
		for (int i = 0; i < vertices.Count; i++) {
			Gizmos.DrawSphere(vertices[i] + transform.position, 0.1f);
		}
	}
}
