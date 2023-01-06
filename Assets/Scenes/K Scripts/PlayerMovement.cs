using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Resource: https://www.youtube.com/watch?v=f473C43s8nE
// Dave / Game Development on YouTube
public class PlayerMovement : MonoBehaviour
{
    //Add movement depth for combat. E.g. block, dash, sprint, hitstun, input lag, items

    [Header("Movement")]
    public float moveSpeed;
    public PlayerHistory history;
    public GameManager gameManager;

    [Header("Sprinting")]
    public float sprintSpeed;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public bool canSprint = true;
    public bool IsSprinting => canSprint && Input.GetKey(sprintKey);

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Dodge")]
    public bool canDodge = true;
    public KeyCode firstButtonPressed;
    public float timeOfFirstButton;
    public float dodgeForce;
    public float dodgeJumpForce;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask ground;
    bool grounded;
    public float groundDrag;

    [Header("Teleports")]
    public GameObject teleportToCave;
    public GameObject teleportToLand;

    public Transform orientation;

    public LevelGeneration levelGenerator;
    public Camera mainCamera;
    public GameObject tempCamPrefab;
    public GameObject tempCam;
    private GameObject starFollowed = null;
    private bool movementSuppressed;


    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private float pushPower = 1f;

    private void Start()
    {
        starFollowed = null;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
    }

    private void Update()
    {
        if (starFollowed)
        {
            if (!starFollowed.GetComponent<StarMovement>().inAnimation())
            {
                starFollowed = null;
                unfollowStar();
            }
        }
        if (movementSuppressed) return;

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);

        MyInput();
        SpeedControl();
        
        if(grounded)
        {
            rb.drag = groundDrag;
            canDodge = true;
        }
        else
        {
            rb.drag = 0;
            canDodge = false;
        }

        if(checkDoubleTap(KeyCode.W) && canDodge)
        {
            dodge(KeyCode.W);
        }
        
        if(checkDoubleTap(KeyCode.A) && canDodge)
        {
            dodge(KeyCode.A);
        }

        if(checkDoubleTap(KeyCode.S) && canDodge)
        {
            dodge(KeyCode.S);
        }

        if(checkDoubleTap(KeyCode.D) && canDodge)
        {
            dodge(KeyCode.D);
        }

        
    }

    private void FixedUpdate()
    {
        if (!movementSuppressed)
        {
            MovePlayer();
        }
            
    }

    public void setSuppressed(bool s) { movementSuppressed = s; }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    public void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(grounded)
            rb.AddForce(moveDirection.normalized * (IsSprinting ? sprintSpeed : moveSpeed) * 10f, ForceMode.Force);
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * (IsSprinting ? sprintSpeed : moveSpeed) * 10f * airMultiplier, ForceMode.Force);
    }
    
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x,0f, rb.velocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    void onControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic) return;
        Vector3 dir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.AddForceAtPosition(dir * pushPower, transform.position, ForceMode.Impulse);
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log(other.tag);
        Debug.Log(other.gameObject.tag);
        switch (other.tag)
        {  
            case "End":
                FindObjectOfType<GameManager>().GameOver();
                break;
            case "Win":
                Cursor.lockState = CursorLockMode.None;
                Debug.Log(Cursor.lockState.ToString());
                FindObjectOfType<GameManager>().WinGame();
                break;
            case "Star":
                Debug.Log("Player ran into a star");
                other.GetComponent<Animator>().SetTrigger("Fly");
                break;
            case "Teleport":
                Debug.Log("Player ran into a teleport");
                gameManager.setGameArea(gameArea.cave);
                rb.transform.position = teleportToCave.transform.position;
                // orientation.transform.position = teleportToCave.transform.position;
                break;
            case "TeleportMain":
                Debug.Log("Player ran into a teleport");
                gameManager.setGameArea(gameArea.first);
                rb.transform.position = teleportToLand.transform.position;
                // orientation.transform.position = teleportToCave.transform.position;
                break;
            case "Star2":
                other.gameObject.GetComponent<StarMovement>().onTouch();
                levelGenerator.updateGates();
                followStar(other.gameObject);
                break;
	    }
    }

    void onCollisionEnter(Collision other)
    {
        if (other.collider.tag == "fogGate")
        {
            (int z, int x) levelSize = levelGenerator.levelCoordSize();
            Vector2 relativePosition = new Vector3(transform.position.z/levelSize.z, transform.position.x/levelSize.x);
            Destroy(levelGenerator.voronoiDiagram.bisectors[levelGenerator.voronoiDiagram.nearestGate(relativePosition)].gateObject);
        }
    }


    // Resource for Dodging: https://www.youtube.com/watch?v=pRzb9qi8a8Q
    // Firemind on YouTube
    private bool checkDoubleTap(KeyCode key)
    {
        if(Input.GetKeyDown(key) && firstButtonPressed == key)
        {
            //Is this supposed to be alphanumeric 0?
            //no it's 0 on keyboard (can always change this)
            //it is a dummy save
            firstButtonPressed = KeyCode.P;
            if (Time.time - timeOfFirstButton < 0.3f)
            {
                return true;
            }
        }

        if(Input.GetKeyDown(key) && firstButtonPressed != key)
        {
            firstButtonPressed = key;
            timeOfFirstButton = Time.time;
            return false;
        }

        //What's it supposed to return as default?
        //supposed to return false
        //there is only one case when a button is pressed twice
        //and it must be pressed twice quickly
        return false;
    }

    public Vector3 findVectorForDirection(KeyCode directionKey)
    {
        if(directionKey == KeyCode.W)
        {
            return transform.forward;
        }

        if(directionKey == KeyCode.S)
        {
            return -transform.forward;
        }

        Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
        Vector3 left = Vector3.Cross(transform.forward.normalized, up.normalized);

        if(directionKey == KeyCode.A) //Left Vector
        {
            return left;
        }

        return -left;
    }

    public void dodge(KeyCode direction)
    {
        float force = dodgeForce;

        if (readyToJump)
        {
            force = force / 5;
        }

        Vector3 directionVector = findVectorForDirection(direction);

        rb.drag = 0f;

        rb.AddForce(new Vector3(0f, dodgeJumpForce / 2, 0f), ForceMode.Impulse);
        rb.AddForce(new Vector3(directionVector.x * force, directionVector.y * force, directionVector.z * force), ForceMode.Impulse);
        canDodge = false;
    }

    public void followStar(GameObject star)
    {
        mainCamera.GetComponent<Camera>().enabled = true;
        tempCam = Instantiate(tempCamPrefab, mainCamera.transform.position, mainCamera.transform.rotation);
        tempCam.GetComponent<FollowObject>().obj = star;
        starFollowed = star;
    }
    public void unfollowStar()
    {
        mainCamera.GetComponent<Camera>().enabled = true;
        Destroy(tempCam);
    }
}