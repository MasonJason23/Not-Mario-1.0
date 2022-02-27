using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterController : MonoBehaviour
{
    public float runForce = 50f;
    public float jumpForce = 20f;
    public float maxRunSpeed = 60f;
    public float reducingVelocity = 3f;
    public float longJumpForce = 3f;
    public bool grounded;
    public bool rotated;

    // Coyote Time Variables
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    
    // Jump Buffer Variables
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private Rigidbody rbody;
    private Collider collider;
    private Animator animComp;
    
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variable components
        rbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        animComp = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Checking to make sure the player is on the ground in order to jump
        float castDistance = collider.bounds.extents.y * 0.1f;
        grounded = Physics.Raycast(transform.position, Vector3.down, castDistance);

        // Move left or right by applying force
        float axis = Input.GetAxis("Horizontal");
        rbody.AddForce(Vector3.right * axis * runForce, ForceMode.Force);

        // Rotates player to look left or right based on arrow keys pressed
        if (Input.GetKey(KeyCode.LeftArrow) && rbody.velocity.magnitude > 0.1f && !rotated)
        {
            rotated = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else if (rotated && Input.GetKey(KeyCode.RightArrow))
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
            rbody.AddForce(Vector3.up * longJumpForce, ForceMode.Force);

            coyoteTimeCounter = 0f;
        }
        
        
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
}
