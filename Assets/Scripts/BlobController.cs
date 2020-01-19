using UnityEngine;

public class BlobController : MonoBehaviour {
    [Header("Refs")]
    public Rigidbody2D rb2D;

    [Header("Params")]
    public float speed;

    private Vector2 inputVector;

    private void Update () {
        inputVector = new Vector2( Input.GetAxisRaw( "Horizontal" ), Input.GetAxisRaw( "Vertical" ) );
    }

    private void LateUpdate () {
        rb2D.position += inputVector * speed * Time.fixedDeltaTime;
    }
}