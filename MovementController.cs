using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementController : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameControls controls;
    private CheckSurroundings check;
    private DashAbility dash;
    private GroundSmashAbility groundSmash;
    [SerializeField]
    private SpriteRenderer sr = null;

    [SerializeField]
    private Animator animator = null;

    #region Movement
    bool right = true;
    bool AllowMovement = true;
    [Header("Movement Settings")]
    public float moving_speed;
    public float movementForce;
    private bool temptDisable;
    private float temptTimeCounter;
    #endregion

    #region Jump
    [Header("Jump Settings")]
    public float Jump_Force;
    public float gravityChange;

    //used for jumping
    private bool Jumped;
    public float jumptime;
    public float jumpCooldown;
    public float wallJumpFallTime;
    public float wallJumpCoyoteTime;
    public float wallFallGravityChange;
    public float wallJumpForce;
    public float normalCoyoteTime;
    float jumpercounter,currentJumpCooldown,wallCoyoteCounter,wallJumpFallCounter,normalCoyoteCounter;
    bool hasJumped, wallJump,wallCoyoteCounting,wallResting,normalCoyoteCouting,hasCoyoted;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = GameControls.instance;
        dash = gameObject.GetComponent<DashAbility>();
        groundSmash = GetComponent<GroundSmashAbility>();
        if(controls == null){
            Debug.LogError("This gameobject does not have a controller attach");
        }
        check = gameObject.GetComponentInChildren<CheckSurroundings>();
        if(check == null){
            Debug.LogError("This object does not have the collision prefab");
        }
        GM.instance.setMovementSpeed(moving_speed);
        GM.instance.setJumpForce(Jump_Force);
        GM.instance.setJumpTime(jumptime);
        GM.instance.setWallJumpForce(wallJumpForce);
    }

    void Update(){

        //Debug.Log("WallJump: " + wallJump);
        //Debug.Log("Gravity: " + rb.gravityScale);
        //Jump
        if(currentJumpCooldown > 0){
                currentJumpCooldown -= Time.deltaTime;
        }else{
            hasJumped = false;
        }

        if(jumpercounter > 0){
            jumpercounter -= Time.deltaTime;
                
        }
        if(wallCoyoteCounter > 0){
            wallCoyoteCounter -= Time.deltaTime;
        }
        if(wallJumpFallCounter > 0){
            wallJumpFallCounter -= Time.deltaTime;
        }

        if(normalCoyoteCounter > 0 && normalCoyoteCouting){
            normalCoyoteCounter -= Time.deltaTime;
        }else if(normalCoyoteCouting){
            normalCoyoteCouting = false;
        }

        if(temptTimeCounter > 0 & temptDisable){
            temptTimeCounter -= Time.deltaTime;
        }else if(temptDisable){
            EnableMovement();
            temptDisable = false;
        }
        animator.SetFloat("Speed_x", Mathf.Abs(rb.velocity.x));
        animator.SetBool("Jumped", Jumped);
        //animator.SetBool("Right",right);
        animator.SetBool("OnGround",check.Check_Ground());
        animator.SetFloat("Velocity_Y",rb.velocity.y);
    }
    void FixedUpdate(){
        if(GM.instance.Player_control){
            Basic_Movement();
            Jump();
        }
        
    }

    void Basic_Movement()
    {
        if(AllowMovement){
            //Pre: Are you giving any input left or right?
            //Post: Do movement functions
            if(controls.movementInput.x != 0){
                int controlDirection = (int)(controls.movementInput.x/Mathf.Abs(controls.movementInput.x));
                //Pre: Are not you dashing or slowing down from a dash?
                //Post: let the player be able to move left and right
                if(!check.Check_Forward_Obstacle(controls.movementInput.x > 0)){
                    rb.AddForce(controlDirection*transform.right*movementForce);
                    if(controls.movementInput.x > 0){
                        if(rb.velocity.x < 0){
                            rb.velocity = new Vector2(0f,rb.velocity.y);
                        }
                        rb.velocity = new Vector2(Mathf.Min(rb.velocity.x,GM.instance.getMovementSpeed()),rb.velocity.y);
                    }else{
                        if(rb.velocity.x > 0){
                            rb.velocity = new Vector2(0f,rb.velocity.y);
                        }
                        rb.velocity = new Vector2(Mathf.Max(rb.velocity.x,-GM.instance.getMovementSpeed()),rb.velocity.y);
                    }
                }
                if(right && controlDirection < 0){
                    right = false;
                    sr.flipX = true;
                    this.transform.GetChild(5).transform.localRotation = Quaternion.Euler(0,270f,0f);
                }else if(controlDirection > 0){
                    right = true;
                    sr.flipX = false;
                    this.transform.GetChild(5).transform.localRotation = Quaternion.Euler(0,90f,0f);
                }
                    
               /* 
                //Pre: Are you only moving the joystick left or right?
                //Post: Attack directions for left and right will register
                if(controls.movementInput.y > -.1 && controls.movementInput.y < .1f){
                    attack_capsule.gameObject.transform.eulerAngles = new Vector3(0,0,90f+(-90f*controlDirection));     
                }*/
            //Pre: Are you not giving input for left or right?
            //Post: x direction velocity will be zero  
            }else{
                rb.velocity = new Vector2(0,rb.velocity.y);
            }

            /*
            //Pre: Are you giving input up or down?
            //Post: attack directions for up and down will register
            if(controls.movementInput.y > 0){
                attack_capsule.gameObject.transform.eulerAngles = new Vector3(0,0,90f);
            }else if(controls.movementInput.y <0){
                attack_capsule.gameObject.transform.eulerAngles = new Vector3(0,0,270f);
            }*/
            
        }
        
    
    }

    void Jump(){
        if(AllowMovement){
            if(controls.jumpInput && (check.Check_Ground() | normalCoyoteCouting) && !wallJump){
                //Debug.Log("Case 1");
                if(!Jumped){
                    if(transform.eulerAngles.z != 0){
                        transform.eulerAngles = new Vector3(transform.rotation.x,transform.rotation.y,0f);
                        //Debug.Log("Rotated");
                    }
                    Jumped = true;
                    hasJumped = true;
                    rb.velocity = new Vector2(rb.velocity.x,GM.instance.getJumpForce());
                    jumpercounter = GM.instance.getJumpTime();
                    rb.gravityScale = gravityChange;
                }
            //Pre: Still have enough time in jump counter
            //Post: Continue jumping until counter runs out
            }else if(controls.jumpInput && !wallJump){
                //Debug.Log("Case 4");
                if(Jumped && jumpercounter < 0){
                    Jumped = false;
                    rb.gravityScale = 1f;
                }
            //Pre: Must be on the ground
            //Post: can jump goes on a short cooldown and you can jump and dash(if used in the air) again
            }else if(check.Check_Ground()){
                //Debug.Log("Case 5");
                if(hasJumped){
                    currentJumpCooldown = jumpCooldown;
                }
                hasCoyoted = false;
            //Post: In all other cases, you are no longer jumping 
            }else if(!check.Check_Ground() && !wallJump && !hasCoyoted){
                hasCoyoted = true;
                normalCoyoteCouting = true;
                normalCoyoteCounter = normalCoyoteTime;
            }else if(!wallJump){
                //Debug.Log("Case 6");
                Jumped = false;
                rb.gravityScale = 1f;
            }
            //Pre: If either wall jumping or on a wall without touching the ground
            //Post: Initate wall jump and keep track of wall fall
            if( wallJump || (!check.Check_Ground() && check.Check_Next_Obstacle())){
                //Debug.Log("Touched a wall");
                if(check.checkNextJumpable()){
                    //Debug.Log("Case 2");
                    //Debug.Log("Touched Jumpable Wall");
                    if(!wallJump){
                        transform.rotation = Quaternion.Euler(transform.rotation.x,transform.rotation.y,0f);
                        wallJump = true;
                        rb.velocity = new Vector2(0f,0f);
                        rb.gravityScale = 0f; 
                        wallCoyoteCounting = false;
                        wallResting = false;
                    }else{
                        if(!wallResting){
                            wallJumpFallCounter = wallJumpFallTime;
                            wallResting = true;
                        }else{
                            if(wallJumpFallCounter < 0){
                                rb.gravityScale = wallFallGravityChange;
                            }
                        }
                    }
                //Pre: If not touching a wall but currently wall jumping
                //Post: Start coyote jump, giving extra time to wall jump even if not technically on th wall
                }else if(wallJump){
                    //Debug.Log("Case 3");
                    if(!wallCoyoteCounting){
                        wallCoyoteCounter = wallJumpCoyoteTime;
                        wallCoyoteCounting = true;
                    }else{
                        rb.gravityScale = 1f;
                        if(wallCoyoteCounter > 0){
                            if(controls.jumpInput){
                                int direction = 1;
                                if(!right){
                                    direction = -1;
                                }
                                rb.AddForce((direction*transform.right+transform.up).normalized * wallJumpForce*1000);
                                wallJump = false;
                                Jumped = false;
                            }
                            
                        }else{
                            wallJump = false;
                            Jumped = false;
                            
                        }
                    }
                }
            }
        }
    }

    public void DisableMovement(){
        AllowMovement = false;
    }

    public void EnableMovement(){
        AllowMovement = true;
    }

    public void tempDisableMovement(float time){
        DisableMovement();
        temptDisable = true;
        temptTimeCounter = time;
    }

    public int getDirection(){
        if(right){
            return 1;
        }else{
            return -1;
        }
    }
    
}
