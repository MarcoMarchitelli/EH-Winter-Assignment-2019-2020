using UnityEngine;
using System;

public class BlobPhysics : MonoBehaviour {
    [Header("Data")]
    public TransformArrayVar blobPhysicsPoints;

    [Header("Parameters")]
    [Range(4,64)] public int detail;
    [Range(0.1f,10)] public float radius;
    [Range(0.1f,10)] public float mainColliderRadius;
    [Range(0.1f,10)] public float secondaryCollidersRaius;
    public float dampen;
    public float frequency;

    private GameObject rbsContainer;
    private Rigidbody2D mainRb;
    private Rigidbody2D[] secondaryRbs;
    private float angleRadians;

    public void Generate () {
        DestroyChildren();
        rbsContainer = new GameObject( "Blob Physics" );
        rbsContainer.transform.SetParent( transform );
        mainRb = rbsContainer.AddComponent<Rigidbody2D>();
        mainRb.freezeRotation = true;
        rbsContainer.AddComponent<CircleCollider2D>().radius = mainColliderRadius;

        angleRadians = Mathf.PI * 2 / detail;
        blobPhysicsPoints.value = new Transform[detail];
        secondaryRbs = new Rigidbody2D[detail];
        for ( int i = 0; i < detail; i++ ) {
            CreateSecondaryRB( i );
        }
        for ( int i = 0; i < detail; i++ ) {
            SetSecondarySprings( i );
        }
    }

    private void DestroyChildren () {
        int childCount = transform.childCount;
        for ( int i = 0; i < childCount; i++ ) {
            if ( Application.isPlaying )
                Destroy( transform.GetChild( 0 ).gameObject );
            else
                DestroyImmediate( transform.GetChild( 0 ).gameObject );
        }
    }

    private void CreateSecondaryRB ( int i ) {
        //position
        Vector2 pos = new Vector2(Mathf.Cos(angleRadians * i), Mathf.Sin(angleRadians * i)) * radius;
        GameObject srb = new GameObject("Secondary RB " + i.ToString());
        srb.transform.position = ( Vector2 ) rbsContainer.transform.position + pos;
        srb.transform.rotation = Quaternion.identity;
        srb.transform.SetParent( transform );

        //rigid body
        Rigidbody2D rb = srb.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        //distance joint
        DistanceJoint2D dj = srb.AddComponent<DistanceJoint2D>();
        dj.autoConfigureDistance = false;
        dj.connectedBody = mainRb;
        dj.maxDistanceOnly = true;
        dj.distance = radius;
        dj.enableCollision = true;

        //spring joint
        SpringJoint2D sj =  srb.AddComponent<SpringJoint2D>();
        sj.connectedBody = mainRb;
        sj.dampingRatio = dampen;
        sj.frequency = frequency;
        sj.enableCollision = true;

        srb.AddComponent<CircleCollider2D>().radius = secondaryCollidersRaius;

        blobPhysicsPoints.value[i] = srb.transform;
        secondaryRbs[i] = rb;
    }

    private void SetSecondarySprings ( int i ) {
        int targetIndex = i + 1 > detail - 1 ? 0 : i + 1;

        SpringJoint2D sj = secondaryRbs[i].gameObject.AddComponent<SpringJoint2D>();
        sj.autoConfigureDistance = true;
        sj.dampingRatio = dampen;
        sj.frequency = frequency;
        sj.connectedBody = secondaryRbs[targetIndex];
        sj.enableCollision = true;
    }
}