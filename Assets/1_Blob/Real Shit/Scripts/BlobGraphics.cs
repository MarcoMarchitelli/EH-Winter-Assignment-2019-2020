using System.Linq;
using UnityEngine;
using System;

public class BlobGraphics : MonoBehaviour {
    [Header("Data")]
    public MeshVar blobMesh;

    [Header("References")]
    public SkinnedMeshRenderer smr;
    public Transform rootBonePositionReference;
    //[SerializeField] private GameObject rootBone = default;

    [Header("Parameters")]
    [Range(3,64)] public int detail;
    [Range(.1f,10)] public float radius;
    [Range(0,1)] public float weightBase;

    private Vector3[] vertices;
    private Vector3[] normals;
    private BoneWeight[] boneWeights;
    private Matrix4x4[] bindPoses;
    private int[] tris;
    private int vertexCount;

    //private void Update () {
    //    //rotate root bone
    //    rootBone.transform.position = rootBonePositionReference.position;
    //    float angle = Vector3.Angle(rootBone.transform.position, BlobPhysics.points[0].position);
    //    rootBone.transform.localEulerAngles = new Vector3( 0, 0, angle );
    //}

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
        bindPoses = new Matrix4x4[BlobPhysics.points.Length];

        smr.bones = BlobPhysics.points;

        boneWeights[0].boneIndex0 = 0;
        boneWeights[0].weight0 = 1;

        for ( int j = 1; j < vertexCount; j++ ) {
            SetClosestBones( j );
        }

        for ( int i = 0; i < bindPoses.Length; i++ ) {
            bindPoses[i] = smr.bones[i].worldToLocalMatrix * smr.transform.localToWorldMatrix;
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
        rootBone.transform.SetParent( transform );
        smr.rootBone = rootBone.transform;
    }

    private void SetClosestBones ( int vertexIndex ) {
        Vector3 vertex = vertices[vertexIndex];
        Vector3 worldVertex = smr.transform.TransformPoint( vertex );

        //order physics points based on distance.
        var bonesRefs = BlobPhysics.points.OrderBy( a => ( worldVertex - a.position).magnitude ).ToArray();

        float GetWeight ( Transform bone, float mul = 1 ) {
            return 1 - Mathf.Clamp01( ( worldVertex - bone.position ).magnitude / weightBase ) * mul;
        }

        Transform currBone = bonesRefs[0];
        boneWeights[vertexIndex].boneIndex0 = Array.IndexOf( BlobPhysics.points, currBone );
        boneWeights[vertexIndex].weight0 = GetWeight( currBone );

        //currBone = bonesRefs[1];
        //boneWeights[vertexIndex].boneIndex1 = Array.IndexOf( BlobPhysics.points, currBone );
        //boneWeights[vertexIndex].weight1 = GetWeight( currBone, .9f );

        //currBone = bonesRefs[2];
        //boneWeights[vertexIndex].boneIndex2 = Array.IndexOf( BlobPhysics.points, currBone );
        //boneWeights[vertexIndex].weight2 = GetWeight( currBone );

        //currBone = bonesRefs[3];
        //boneWeights[vertexIndex].boneIndex3 = Array.IndexOf( BlobPhysics.points, currBone );
        //boneWeights[vertexIndex].weight3 = GetWeight( currBone );
    }
}