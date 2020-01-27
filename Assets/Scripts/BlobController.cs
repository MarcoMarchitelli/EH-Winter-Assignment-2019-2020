using UnityEngine;

public class BlobController : MonoBehaviour {
    [Header("Refs")]
    public Rigidbody2D rb2D;

    [Header("Params")]
    public float acceleration;
    public float maxSpeed;

    private Vector2 inputVector;
    private float magnitude;
    private Vector2 velocity;

    private void Update () {
        inputVector = new Vector2( Input.GetAxisRaw( "Horizontal" ), Input.GetAxisRaw( "Vertical" ) );
    }

    private void LateUpdate () {
        velocity = inputVector * acceleration * Time.fixedDeltaTime;
        magnitude = velocity.sqrMagnitude;
        magnitude = Mathf.Clamp( magnitude, 0, maxSpeed * maxSpeed );
        rb2D.velocity += velocity.normalized * magnitude;
    }
}