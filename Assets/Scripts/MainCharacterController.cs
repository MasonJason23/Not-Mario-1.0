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
        float castDistance = collider.bounds.extents.y + 0.1f;
        grounded = Physics.Raycast(transform.position, Vector3.down, castDistance);
        Debug.DrawRay(transform.position, Vector3.down * castDistance);
        
        // Move left or right by applying force
        float axis = Input.GetAxis("Horizontal");
        rbody.AddForce(Vector3.right * axis * runForce, ForceMode.Force);

        // Jump with two parameters (character is on the ground and space bar is pressed)
        if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            rbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        else if (rbody.velocity.y > 0f && Input.GetKey(KeyCode.Space))
        {
            rbody.AddForce(Vector3.up * longJumpForce, ForceMode.Force);
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
        
        //
        animComp.SetFloat("Speed", rbody.velocity.magnitude);
    }
}
