using UnityEngine;

public class Splines : MonoBehaviour
{
    [SerializeField] GameObject pointPrefab;
    [SerializeField] Vector3 point1;
    [SerializeField] Vector3 point2;
    [SerializeField] Vector3 point3;
    [SerializeField] Vector3 point4;
    
    [SerializeField] float timeSpeed = 1;
    
    GameObject sphere1;
    GameObject sphere2;
    GameObject sphere3;
    GameObject sphere4;
    GameObject movingPoint;
    float timeVar = 0;
    
    Matrix4x4 characteristicMatrix = new(
        new(1, 0, 0, 0),
        new(-3, 3, 0, 0),
        new(3, -6, 3, 0),
        new(-1, 3, -3, 1)
    );
    
    void Start()
    {
        sphere1 = Instantiate(pointPrefab, point1, pointPrefab.transform.rotation);
        sphere2 = Instantiate(pointPrefab, point2, pointPrefab.transform.rotation);
        sphere3 = Instantiate(pointPrefab, point3, pointPrefab.transform.rotation);
        sphere4 = Instantiate(pointPrefab, point4, pointPrefab.transform.rotation);
        movingPoint = Instantiate(pointPrefab, point1, pointPrefab.transform.rotation);
    }

    void Update()
    {
        //Vector3 tempPoint1 = Vector3.Lerp(sphere1.transform.position, sphere2.transform.position, timeVar);
        //Vector3 tempPoint2 = Vector3.Lerp(sphere2.transform.position, sphere3.transform.position, timeVar);
        //Vector3 tempPoint3 = Vector3.Lerp(sphere3.transform.position, sphere4.transform.position, timeVar);
        
        //Vector3 tempPoint4 = Vector3.Lerp(tempPoint1, tempPoint2, timeVar);
        //Vector3 tempPoint5 = Vector3.Lerp(tempPoint2, tempPoint3, timeVar);
        
        //movingPoint.transform.position = Vector3.Lerp(tempPoint4, tempPoint5, timeVar);
        
        // movingPoint.transform.position = 
        // sphere1.transform.position * (Mathf.Pow(-timeVar, 3) + (3 * Mathf.Pow(timeVar, 2)) - (3 * timeVar) + 1) +
        // sphere2.transform.position * (3 * Mathf.Pow(timeVar, 3) - (6 * Mathf.Pow(timeVar, 2)) + (3 * timeVar)) +
        // sphere3.transform.position * (-3 * Mathf.Pow(timeVar, 3) + (3 * Mathf.Pow(timeVar, 2))) +
        // sphere4.transform.position * Mathf.Pow(timeVar, 3);
        
        Vector4 powersOfT = new(1, timeVar, Mathf.Pow(timeVar, 2), Mathf.Pow(timeVar, 3));
        // Matrix4x4 pointMatrix = new(
        //     new(sphere1.transform.position.x, sphere1.transform.position.y, sphere1.transform.position.z, 1),
        //     new(sphere2.transform.position.x, sphere2.transform.position.y, sphere2.transform.position.z, 1),
        //     new(sphere3.transform.position.x, sphere3.transform.position.y, sphere3.transform.position.z, 1),
        //     new(sphere4.transform.position.x, sphere4.transform.position.y, sphere4.transform.position.z, 1)
        // );
        
        float[,] pointMatrix = {
            {sphere1.transform.position.x, sphere1.transform.position.y, sphere1.transform.position.z},
            {sphere2.transform.position.x, sphere2.transform.position.y, sphere2.transform.position.z},
            {sphere3.transform.position.x, sphere3.transform.position.y, sphere3.transform.position.z},
            {sphere4.transform.position.x, sphere4.transform.position.y, sphere4.transform.position.z}
        };
        
        //Matrix4x4 positionMatrix = characteristicMatrix * pointMatrix;
        Debug.Log(characteristicMatrix[0,1]);
        float[,] positionMatrix = {
            {
                Vector4.Dot(new(characteristicMatrix[0,0], characteristicMatrix[0,1], characteristicMatrix[0,2], characteristicMatrix[0,3]), new(pointMatrix[0,0], pointMatrix[1,0], pointMatrix[2,0], pointMatrix[3,0])),
                Vector4.Dot(new(characteristicMatrix[0,0], characteristicMatrix[0,1], characteristicMatrix[0,2], characteristicMatrix[0,3]), new(pointMatrix[0,1], pointMatrix[1,1], pointMatrix[2,1], pointMatrix[3,1])),
                Vector4.Dot(new(characteristicMatrix[0,0], characteristicMatrix[0,1], characteristicMatrix[0,2], characteristicMatrix[0,3]), new(pointMatrix[0,2], pointMatrix[1,2], pointMatrix[2,2], pointMatrix[3,2]))
            },
            {
                Vector4.Dot(new(characteristicMatrix[1,0], characteristicMatrix[1,1], characteristicMatrix[1,2], characteristicMatrix[1,3]), new(pointMatrix[0,0], pointMatrix[1,0], pointMatrix[2,0], pointMatrix[3,0])),
                Vector4.Dot(new(characteristicMatrix[1,0], characteristicMatrix[1,1], characteristicMatrix[1,2], characteristicMatrix[1,3]), new(pointMatrix[0,1], pointMatrix[1,1], pointMatrix[2,1], pointMatrix[3,1])),
                Vector4.Dot(new(characteristicMatrix[1,0], characteristicMatrix[1,1], characteristicMatrix[1,2], characteristicMatrix[1,3]), new(pointMatrix[0,2], pointMatrix[1,2], pointMatrix[2,2], pointMatrix[3,2]))
            },
            {
                Vector4.Dot(new(characteristicMatrix[2,0], characteristicMatrix[2,1], characteristicMatrix[2,2], characteristicMatrix[2,3]), new(pointMatrix[0,0], pointMatrix[1,0], pointMatrix[2,0], pointMatrix[3,0])),
                Vector4.Dot(new(characteristicMatrix[2,0], characteristicMatrix[2,1], characteristicMatrix[2,2], characteristicMatrix[2,3]), new(pointMatrix[0,1], pointMatrix[1,1], pointMatrix[2,1], pointMatrix[3,1])),
                Vector4.Dot(new(characteristicMatrix[2,0], characteristicMatrix[2,1], characteristicMatrix[2,2], characteristicMatrix[2,3]), new(pointMatrix[0,2], pointMatrix[1,2], pointMatrix[2,2], pointMatrix[3,2]))
            },
            {
                Vector4.Dot(new(characteristicMatrix[3,0], characteristicMatrix[3,1], characteristicMatrix[3,2], characteristicMatrix[3,3]), new(pointMatrix[0,0], pointMatrix[1,0], pointMatrix[2,0], pointMatrix[3,0])),
                Vector4.Dot(new(characteristicMatrix[3,0], characteristicMatrix[3,1], characteristicMatrix[3,2], characteristicMatrix[3,3]), new(pointMatrix[0,1], pointMatrix[1,1], pointMatrix[2,1], pointMatrix[3,1])),
                Vector4.Dot(new(characteristicMatrix[3,0], characteristicMatrix[3,1], characteristicMatrix[3,2], characteristicMatrix[3,3]), new(pointMatrix[0,2], pointMatrix[1,2], pointMatrix[2,2], pointMatrix[3,2]))
            }
        };
        
        Debug.Log("-----------------");
        foreach (float num in positionMatrix)
        {
            Debug.Log(num);
        }
        //Debug.Log(positionMatrix);
        
        //movingPoint.transform.position = new(Vector4.Dot(powersOfT, positionMatrix.GetRow(1)), Vector4.Dot(powersOfT, positionMatrix.GetRow(2)), Vector4.Dot(powersOfT, positionMatrix.GetRow(3)));
        movingPoint.transform.position = new(
            Vector4.Dot(powersOfT, new(positionMatrix[0,0], positionMatrix[1,0], positionMatrix[2,0], positionMatrix[3,0])),
            Vector4.Dot(powersOfT, new(positionMatrix[0,1], positionMatrix[1,1], positionMatrix[2,1], positionMatrix[3,1])),
            Vector4.Dot(powersOfT, new(positionMatrix[0,2], positionMatrix[1,2], positionMatrix[2,2], positionMatrix[3,2]))
        );
        
        timeVar += Time.deltaTime * timeSpeed;
        if (timeVar > 1) timeVar = 0;
    }
}
