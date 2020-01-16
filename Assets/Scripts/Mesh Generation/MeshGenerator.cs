using UnityEngine;
using UnityEditor;

public class MeshGenerator : MonoBehaviour {
    [Header("References")]
    public MeshFilter mf;

    [Header("Parameters")]
    [Range(3,64)] public int detail = 32;
    [Range(1,99)] public float radius = 1;
    public bool autoGen;
    public bool gizmos;

    [Header("Events")]
    [Tooltip("Passes the generated mesh")]
    public UnityMeshEvent OnMeshGeneration;

    private Vector3[] vertices;
    private int[] tris;
    private Vector3[] normals;
    private Vector2[] uvs;
    private int vertexCount;

    public void GenerateMesh () {
        Mesh mesh = new Mesh();
        mesh.name = "Blob";

        float anglePerVertex = Mathf.PI * 2 / detail;
        vertexCount = detail + 1;
        vertices = new Vector3[vertexCount];
        normals = new Vector3[vertexCount];
        int trisCount = detail * 3;
        tris = new int[trisCount];

        Vector3 center = mf.transform.position;

        vertices[0] = center;
        normals[0] = center - transform.forward;

        for ( int i = 1; i < vertexCount; i++ ) {
            float angle = anglePerVertex * i;
            vertices[i] = new Vector2( Mathf.Sin( angle ), Mathf.Cos( angle ) ) * radius;
            normals[i] = vertices[i] - transform.forward;
        }

        for ( int i = 0; i < detail; i++ ) {
            if ( i == detail - 1 ) {
                tris[i * 3] = 0;
                tris[i * 3 + 1] = i + 1;
                tris[i * 3 + 2] = 1;
            }
            else {
                tris[i * 3] = 0;
                tris[i * 3 + 1] = i + 1;
                tris[i * 3 + 2] = i + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.normals = normals;

        mf.mesh = mesh;

        OnMeshGeneration.Invoke( mesh );
    }

    private void OnDrawGizmos () {
        if ( !gizmos )
            return;

        for ( int i = 0; i < vertexCount; i++ ) {
            Handles.Label( vertices[i], i.ToString(), EditorStyles.boldLabel );
        }

        Gizmos.color = Color.blue;
        for ( int i = 0; i < vertexCount; i++ ) {
            Gizmos.DrawLine( vertices[i], normals[i] );
        }
    }
}