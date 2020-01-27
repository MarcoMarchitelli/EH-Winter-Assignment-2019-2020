using System.Linq;
using UnityEngine;

public class BlobGraphics : MonoBehaviour {
    [Header("Data")]
    public TransformArrayVar blobPhysicsPoints;
    public MeshVar blobMesh;

    [Header("References")]
    public SkinnedMeshRenderer smr;
    public Transform rootBonePositionReference;

    [Header("Parameters")]
    [Range(3,64)] public int detail;
    [Range(.1f,10)] public float radius;

    private Vector3[] vertices;
    private Vector3[] normals;
    private BoneWeight[] boneWeights;
    private Matrix4x4[] bindPoses;
    private int[] tris;
    private int vertexCount;
    private GameObject rootBone;

    private void Update () {
        //rotate root bone
        rootBone.transform.position = rootBonePositionReference.position;
        float angle = Vector3.Angle(rootBone.transform.position, blobPhysicsPoints.value[0].position);
        rootBone.transform.localEulerAngles = new Vector3( 0, 0, angle );
    }

    public void GenerateMesh () {
        blobMesh.value = new Mesh();
        blobMesh.value.name = "Blob";

        float anglePerVertex = Mathf.PI * 2 / detail;
        vertexCount = detail + 1;
        vertices = new Vector3[vertexCount];
        normals = new Vector3[vertexCount];
        int trisCount = detail * 3;
        tris = new int[trisCount];

        Vector3 center = smr.transform.position;

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

        blobMesh.value.vertices = vertices;
        blobMesh.value.triangles = tris;
        blobMesh.value.normals = normals;

        CreateBones();

        smr.sharedMesh = blobMesh.value;
    }

    private void CreateBones () {
        boneWeights = new BoneWeight[vertexCount];
        bindPoses = new Matrix4x4[blobPhysicsPoints.value.Length];

        CreateRootBone();
        smr.bones = blobPhysicsPoints.value;

        for ( int j = 1; j < vertexCount; j++ ) {
            SetClosestBones( j );
        }

        blobMesh.value.bindposes = bindPoses;
        blobMesh.value.boneWeights = boneWeights;
    }

    private void CreateRootBone () {
        if ( transform.childCount > 0 )
            if ( Application.isPlaying ) {
                Destroy( transform.GetChild( 0 ).gameObject );
            }
            else {
                DestroyImmediate( transform.GetChild( 0 ).gameObject );
            }



        GameObject rootBone = new GameObject("Root Bone");
        smr.rootBone = rootBone.transform;
    }

    private void SetClosestBones ( int vertexIndex ) {
        Vector3 vertex = vertices[vertexIndex];

        blobPhysicsPoints.value.OrderBy( a => ( a.position - vertex ).sqrMagnitude );

        int currentIndex = 0;
        Transform currBone = blobPhysicsPoints.value[currentIndex];

        float minDist = (vertex - currBone.position).sqrMagnitude;

        boneWeights[vertexIndex].boneIndex0 = currentIndex;
        boneWeights[vertexIndex].weight0 = Mathf.Clamp01( minDist / ( currBone.position - vertex ).sqrMagnitude );
        bindPoses[currentIndex] = smr.bones[currentIndex].worldToLocalMatrix * smr.transform.localToWorldMatrix;

        currentIndex = 1;
        currBone = blobPhysicsPoints.value[currentIndex];
        boneWeights[vertexIndex].boneIndex1 = currentIndex;
        boneWeights[vertexIndex].weight1 = Mathf.Clamp01( minDist / ( currBone.position - vertex ).sqrMagnitude );
        bindPoses[currentIndex] = smr.bones[currentIndex].worldToLocalMatrix * smr.transform.localToWorldMatrix;

        currentIndex = 2;
        currBone = blobPhysicsPoints.value[currentIndex];
        boneWeights[vertexIndex].boneIndex2 = currentIndex;
        boneWeights[vertexIndex].weight2 = Mathf.Clamp01( minDist / ( currBone.position - vertex ).sqrMagnitude );
        bindPoses[currentIndex] = smr.bones[currentIndex].worldToLocalMatrix * smr.transform.localToWorldMatrix;

        currentIndex = 3;
        currBone = blobPhysicsPoints.value[currentIndex];
        boneWeights[vertexIndex].boneIndex3 = currentIndex;
        boneWeights[vertexIndex].weight3 = Mathf.Clamp01( minDist / ( currBone.position - vertex ).sqrMagnitude );
        bindPoses[currentIndex] = smr.bones[currentIndex].worldToLocalMatrix * smr.transform.localToWorldMatrix;
    }
}