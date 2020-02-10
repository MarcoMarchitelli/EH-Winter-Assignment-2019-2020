using UnityEngine;
using System.Collections;
using Unity.Collections;

public class MiniCirclesGenerator : MonoBehaviour {
    [Header( "Data" )]
    public MeshVar meshVar;

    [Header("Refs")]
    public SkinnedMeshRenderer smr;
    public Rigidbody2D blobRb2D;

    [Header("Params")]
    [Range(3,64)] public int meshDetail = 32;
    [MinCustom(0)] public float meshRadius;
    [Range(0,32)] public int circlesCount;
    [MinCustom(0)] public float circlesRadius;
    public bool autoGen;

    private Vector3[] vertices;
    private Vector3[] normals;
    private BoneWeight[] boneWeights;
    private Matrix4x4[] bindPoses;
    //private byte[] bonesPerVertex;
    private int[] tris;
    private int vertexCount;

    private Transform[] circles;

    #region Privates
    private void GenerateMesh () {
        circlesCount = 4;

        if ( meshVar.value == null ) {
            meshVar.value = Instantiate<Mesh>( new Mesh() );
            meshVar.value.name = "Blob";
        }
        else {
            meshVar.value.Clear();
        }


        float anglePerVertex = Mathf.PI * 2 / meshDetail;
        vertexCount = meshDetail + 1;
        vertices = new Vector3[vertexCount];
        normals = new Vector3[vertexCount];
        int trisCount = meshDetail * 3;
        tris = new int[trisCount];

        Vector3 center = smr.transform.position;

        vertices[0] = center;
        normals[0] = center - transform.forward;

        for ( int i = 1; i < vertexCount; i++ ) {
            float angle = anglePerVertex * i;
            vertices[i] = new Vector2( Mathf.Sin( angle ), Mathf.Cos( angle ) ) * meshRadius;
            normals[i] = vertices[i] - transform.forward;
        }

        for ( int i = 0; i < meshDetail; i++ ) {
            if ( i == meshDetail - 1 ) {
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

        meshVar.value.vertices = vertices;
        meshVar.value.triangles = tris;
        meshVar.value.normals = normals;

        smr.sharedMesh = meshVar.value;
    }

    private void DestroyAllChildren () {
        int childCount = smr.transform.childCount;
        for ( int i = 0; i < childCount; i++ ) {
            if ( Application.isPlaying )
                Destroy( smr.transform.GetChild( i ).gameObject );
            else
                DestroyImmediate( smr.transform.GetChild( i ).gameObject );
        }
    }

    private void CreateCircles () {
        circles = new Transform[circlesCount];
        float anglePerCircle = Mathf.PI * 2 / circlesCount;
        float miniRadius = meshRadius - circlesRadius;

        for ( int i = 0; i < circlesCount; i++ ) {
            float angle = anglePerCircle * i;
            Vector3 pos = smr.transform.position + new Vector3( Mathf.Cos(angle), Mathf.Sin(angle), 0 ) * miniRadius;
            GameObject circle = new GameObject( "Circle Collider " + i );
            circle.AddComponent<CircleCollider2D>().radius = circlesRadius;
            circle.AddComponent<Rigidbody2D>().gravityScale = 0;
            circle.AddComponent<SpringJoint2D>().connectedBody = blobRb2D;
            circle.AddComponent<DistanceJoint2D>();
            circle.transform.position = pos;
            circle.transform.parent = smr.transform;
            circle.transform.localRotation = Quaternion.identity;
            circles[i] = circle.transform;
        }

        for ( int i = 0; i < circlesCount; i++ ) {
            int nextIndex = i + 1 < circlesCount ? i + 1 : 0;
            circles[i].GetComponent<DistanceJoint2D>().connectedBody = circles[nextIndex].GetComponent<Rigidbody2D>();
        }
    }

    private void CreateBones () {
        boneWeights = new BoneWeight[vertexCount];
        bindPoses = new Matrix4x4[circlesCount];
        //bonesPerVertex = new byte[vertexCount];
        //for ( int i = 0; i < vertexCount; i++ ) {
        //    bonesPerVertex[i] = ( byte ) circlesCount;
        //}
        smr.bones = circles;
        for ( int j = 0; j < vertexCount; j++ ) {
            SetClosestBones( j );
        }
        meshVar.value.bindposes = bindPoses;
        meshVar.value.boneWeights = boneWeights;
        //meshVar.value.SetBoneWeights( new NativeArray<byte>( bonesPerVertex, Allocator.Temp ), new NativeArray<BoneWeight1>( boneWeights, Allocator.Temp ) );
    }

    //TODO: make first 4 bones NOT just first.
    private void SetClosestBones ( int vertexIndex ) {
        Vector3 vertex = vertices[vertexIndex];

        float distance = Vector3.Distance(vertex, circles[0].position);
        boneWeights[vertexIndex].boneIndex0 = 0;
        boneWeights[vertexIndex].weight0 = Mathf.Clamp01( 1 / distance );
        bindPoses[0] = smr.bones[0].worldToLocalMatrix * smr.transform.localToWorldMatrix;

        distance = Vector3.Distance( vertex, circles[1].position );
        boneWeights[vertexIndex].boneIndex1 = 1;
        boneWeights[vertexIndex].weight1 = Mathf.Clamp01( 1 / distance );
        bindPoses[1] = smr.bones[1].worldToLocalMatrix * smr.transform.localToWorldMatrix;

        distance = Vector3.Distance( vertex, circles[2].position );
        boneWeights[vertexIndex].boneIndex2 = 2;
        boneWeights[vertexIndex].weight2 = Mathf.Clamp01( 1 / distance );
        bindPoses[2] = smr.bones[2].worldToLocalMatrix * smr.transform.localToWorldMatrix;

        distance = Vector3.Distance( vertex, circles[3].position );
        boneWeights[vertexIndex].boneIndex3 = 3;
        boneWeights[vertexIndex].weight3 = Mathf.Clamp01( 1 / distance );
        bindPoses[3] = smr.bones[3].worldToLocalMatrix * smr.transform.localToWorldMatrix;
    }
    #endregion

    public void Generate () {
        GenerateMesh();

        DestroyAllChildren();

        CreateCircles();

        CreateBones();
    }

    private void Start () {
        Generate();
    }
}