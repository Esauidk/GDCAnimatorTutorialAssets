using UnityEngine;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(SpriteRenderer))]
public class Movement : MonoBehaviour {
    public float speed;
    public float jumpPower;

    [SerializeField]
    private Collider2D groundCollider;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;


    public void Start(){
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Update(){
        Move();
    }

    // A and D move the object left and right at veloctiy of (1/-1 * speed)
    // Pressing space makes the object jump at speed *only if the ground collider is toucing the "Ground" layer
    private void Move(){
        if(Input.GetKey(KeyCode.D)){
            rb.velocity = new Vector2(speed, rb.velocity.y);
        }else if(Input.GetKey(KeyCode.A)){
            rb.velocity = new Vector2(-speed, rb.velocity.y);
        }else{
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        
        if(Input.GetKeyDown(KeyCode.Space) && groundCollider.IsTouchingLayers(LayerMask.NameToLayer("Ground"))){
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        }
    }
}