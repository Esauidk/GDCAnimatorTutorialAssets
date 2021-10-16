using UnityEngine;

public class MovementController : MonoBehavior{
    private Rigidbody2d rigidbody2D;
    private SpriteRenderer spriteRenderer;

    public void OnEnable(){
        keyPressed += Move;
    }

    public void OnDisable(){
        keyPressed -= Move;
    }

    public void Start(){
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<spriteRenderer>();
    }

    private void Move(KeyCode key){

    }
}