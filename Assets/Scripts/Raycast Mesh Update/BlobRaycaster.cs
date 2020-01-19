using UnityEngine;
using UnityEditor;

public class BlobRaycaster : MonoBehaviour {
    [Header("Data")]
    public PointArrayVar pointArrayVar;

    [Header("Refs")]
    public Transform pointsContainer;

    [Header("Params")]
    [MinCustom(0.1f)] public float blobRadius;
    [Range(0,64)] public int pointsCount;
    [MinCustom(0.01f)] public float rayLength;
    [MinCustom(0)] public float gravity;
    [Range(0,360)] public float maxNeighbourAngle;
    [MinCustom(0.1f)] public float adjustmentSpeed = 5;

    private float maxPointsDistance;

    private void SetPointsNeighbour () {
        for ( int i = 0; i < pointsCount; i++ ) {
            int leftIndex = i + 1 > pointsCount - 1 ? 0 : i + 1;
            int rightIndex = i - 1 <= 0 ? pointsCount - 1 : i - 1;
            pointArrayVar.value[i].rightNeighbour = pointArrayVar.value[rightIndex];
            pointArrayVar.value[i].leftNeighbour = pointArrayVar.value[leftIndex];
        }
    }

    public void CreatePoints () {
        float angle = Mathf.PI * 2 / pointsCount;

        pointArrayVar.value = new Point[pointsCount];

        for ( int i = 0; i < pointsCount; i++ ) {
            float anglePerPoint = angle * i;
            Vector2 pos = new Vector2(Mathf.Cos(anglePerPoint), Mathf.Sin(anglePerPoint));
            pointArrayVar.value[i] = new Point( pos );
        }

        SetPointsNeighbour();

        maxPointsDistance = Vector2.Distance( pointArrayVar.value[0].position, pointArrayVar.value[1].position );
    }

    #region Monos
    private void Start () {
        CreatePoints();
    }

    private void Update () {
        for ( int i = 0; i < pointsCount; i++ ) {
            Point p = pointArrayVar.value[i];

            //raycasting
            if ( Physics2D.Raycast( p.position, Vector2.down, rayLength ) ) {
                p.airborne = false;
            }
            else {
                p.airborne = true;
            }

            //gravity
            if ( p.airborne ) {
                p.position -= Vector2.up * gravity * Time.deltaTime;
            }
        }
    }

    private void LateUpdate () {
        //min distance
        for ( int i = 0; i < pointsCount; i++ ) {
            Point p = pointArrayVar.value[i];
            for ( int j = 0; j < pointsCount; j++ ) {
                Point p2 = pointArrayVar.value[j];
                if ( p == p2 )
                    return;
                float dist = Vector2.Distance(p.position, p2.position);
                if ( dist < maxPointsDistance ) {
                    Vector2 pos = (p2.position - p.position).normalized * maxPointsDistance;
                    p.position = Vector2.Lerp( p.position, pos, adjustmentSpeed * Time.deltaTime );
                }
            }
        }
    }

    private void OnDrawGizmos () {
        if ( pointArrayVar.value == null )
            return;
        Gizmos.color = Color.green;
        for ( int i = 0; i < pointArrayVar.value.Length; i++ ) {
            Handles.Label( pointArrayVar.value[i].position, i.ToString(), EditorStyles.boldLabel );
            Gizmos.DrawWireSphere( pointArrayVar.value[i].position, .01f );
        }
    }
    #endregion
}

[System.Serializable]
public class Point {
    public Vector2 position;
    public Point leftNeighbour, rightNeighbour;
    public bool airborne;

    public Point ( Vector2 position ) {
        this.position = position;
        leftNeighbour = null;
        rightNeighbour = null;
        airborne = true;
    }

    public Point ( Vector2 position, Point leftNeighbour, Point rightNeighbour ) {
        this.position = position;
        this.leftNeighbour = leftNeighbour;
        this.rightNeighbour = rightNeighbour;
        airborne = false;
    }
}