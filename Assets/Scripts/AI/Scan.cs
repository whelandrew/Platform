using UnityEngine;
using Trig;
public class Scan : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;

    private Mesh mesh;    
    private Vector3 origin = Vector3.zero;
    private float startingAngle = 0;

    public float viewDistance=10f;

    private float fov = 50f;
    private int rayCount = 50;

    private void Start()
    {
        this.enabled = false;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh; 
    }

    private void Update()
    {
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for(int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex = origin + Angles.GetVectorFromAngle(angle) * viewDistance;
            //check if view is blocked   
            RaycastHit2D hit = Physics2D.Raycast(origin, Angles.GetVectorFromAngle(angle), viewDistance, layerMask);
            if (hit.collider != null)
            {
                vertex = hit.point;
            }            
            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            //counterclockwise for Unity
            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }    

    public void SetupScan(Vector3 _startingPos, float _startingAngle, int facing)
    {
        this.enabled = true;
        origin = _startingPos;                
        startingAngle = _startingAngle;
        fov = fov * facing;
    }
}
   