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
    
    [SerializeField] float vertexResolution = 10;
    [SerializeField] float roadWidth = 1;

    void Awake()
    {
        if (!TryGetComponent(out spline)) Debug.LogError(name + ": could not find Spline component.");
        if (!TryGetComponent(out meshFilter)) Debug.LogError(name + ": could not find MeshFilter component.");
    }

    void Update()
    {
        GenerateMesh();
    }
    
    void GenerateMesh()
    {
        vertices = new();
        List<Vector2> uv = new();
        
        for (int i = 0; i < spline.curves.Length; i++)
        {
            for (int j = 0; j < vertexResolution; j++)
            {
                Vector3 centrePoint = spline.curves[i].CalculatePointOnSpline(j / vertexResolution, transform.position) - transform.position;
                Vector3 nextPos = spline.curves[i].CalculatePointOnSpline((j+1) / vertexResolution, transform.position) - transform.position;
                
                // centrePoint.z += roadWidth;
                // vertices.Add(centrePoint);
                // uv.Add(new(0, (i % 2) / 2f));
                // centrePoint.z -= roadWidth*2;
                // vertices.Add(centrePoint);
                // uv.Add(new(1, (i % 2) / 2f));
                
                Vector3 cross = Vector3.Cross(spline.curves[i].GetDirection(j / vertexResolution, transform), (nextPos - centrePoint).normalized).normalized;
                cross = Vector3.Cross(cross, nextPos).normalized;
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
            Vector3 centrePoint = spline.curves[^1].CalculatePointOnSpline(1, transform.position) - transform.position;
            
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
