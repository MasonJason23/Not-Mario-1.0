using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainCharacterController : MonoBehaviour
{
    public float runForce = 50f;
    public float jumpForce = 20f;
    public float maxRunSpeed = 60f;
    public float reducingVelocity = 3f;
    public float longJumpForce = 3f;
    public bool grounded;
    public bool rotated;
    public bool hitAbove;
    public static bool gameEndFlag;

    // Coyote Time Variables
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    
    // Jump Buffer Variables
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;
    
    // For User Interface
    [SerializeField] private TextMeshProUGUI coins;
    [SerializeField] private TextMeshProUGUI score;
    private int lives = 3;
    private int coinTotal = 0;
    private int scoreTotal = 0;

    // Components
    private Rigidbody rbody;
    private Collider collider;
    private Animator animComp;
    private float castDistance;
    [SerializeField] private Transform startingPos;
    
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variable components
        rbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        animComp = GetComponent<Animator>();
        castDistance = collider.bounds.extents.y * 0.15f;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameEndFlag)
        {
            return;
        }
        
        // Function that focuses on player movement
        movement();
        
        // Function that focuses on character animation and look direction
        model();
        
        // Function that focuses on player jump mechanics
        jump();

        // Checking if player hit block above itself
        hitAbove = Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, 2.5f);
    }

    void movement()
    {
        // Move left or right by applying force
        float axis = Input.GetAxis("Horizontal");
        rbody.AddForce(Vector3.right * axis * runForce, ForceMode.Force);

        // Capping the character's speed limit
        if (Mathf.Abs(rbody.velocity.x) > maxRunSpeed)
        {
            float newX = maxRunSpeed * Mathf.Sign(rbody.velocity.x);
            rbody.velocity = new Vector3(newX, rbody.velocity.y, rbody.velocity.z);
        }

        // Decaying velocity for main character after force is added to tho left or right
        if (Mathf.Abs(axis) < 0.1f)
        {
            float updatedX = rbody.velocity.x * (1f - Time.deltaTime * reducingVelocity);
            rbody.velocity = new Vector3(updatedX, rbody.velocity.y, rbody.velocity.z);
        }
    }

    void model()
    {
        // Rotates player to look left or right based on arrow keys pressed
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && rbody.velocity.magnitude > 0.1f && !rotated)
        {
            rotated = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else if (rotated && (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)))
        {
            rotated = false;
            transform.Rotate(0f, 180f, 0f);
        }

        // Updates animator controller parameters to do animations when set parameter meets requirements
        if (rbody.velocity.magnitude > 0.1f)
        {
            animComp.SetBool("Moving", true);
        }
        else
        {
            animComp.SetBool("Moving", false);
        }
    }
    
    void jump()
    {
        // Checking to make sure the player is on the ground in order to jump
        grounded = Physics.Raycast(transform.position, Vector3.down, castDistance);

        // Updating coyote time variables
        if (grounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        
        // Updating jump buffer variables
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Jump. Using coyote time and jump buffering + extended jump when space is held
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            rbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            jumpBufferCounter = 0f;
        }
        else if (rbody.velocity.y > 0f && Input.GetKey(KeyCode.Space))
        {
            rbody.AddForce(Vector3.up * longJumpForce, ForceMode.Acceleration);

            coyoteTimeCounter = 0f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("?") && hitAbove)
        {
            if (coinTotal == 99)
            {
                coinTotal = 0;
                lives++;
            }
            
            coinTotal++;
            if (coinTotal.ToString().Length < 2)
            {
                coins.text = "0";
                coins.text += coinTotal.ToString();
            }
            else
            {
                coins.text = coinTotal.ToString();
            }
            
            scoreTotal += 100;
            score.text = "";
            
            while ((scoreTotal.ToString().Length + score.text.Length)  < 6)
            {
                score.text += "0";
            }
            
            score.text += scoreTotal.ToString();
        }
        else if (collision.gameObject.tag.Equals("Brick") && hitAbove)
        {
            Destroy(collision.transform.gameObject);

            scoreTotal += 100;
            score.text = "";
            
            while ((scoreTotal.ToString().Length + score.text.Length)  < 6)
            {
                score.text += "0";
            }
            
            score.text += scoreTotal.ToString();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Goal"))
        {
            Debug.Log("You Win!");
            gameEndFlag = true;
        }
        if (other.gameObject.tag.Equals("Avoid"))
        {
            Debug.Log("You lost a life!");
            lives -= 1;
            transform.position = startingPos.position;
        }
    }
}
