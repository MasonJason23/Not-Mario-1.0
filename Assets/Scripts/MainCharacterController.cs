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
    
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variable components
        rbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        // Checking to make sure the player is on the ground in order to jump
        float castDistance = collider.bounds.extents.y + 0.1f;
        grounded = Physics.Raycast(transform.position, Vector3.down, castDistance);
        
        // Move left or right by applying force
        float axis = Input.GetAxis("Horizontal");
        rbody.AddForce(Vector3.right * axis * runForce, ForceMode.Force);

        // Jump with two parameters (character is on the ground and space bar is pressed)
        if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            rbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        // else if ()
        // {
            /*
             * TODO
             * Implement extended jump when space bar is held
             */
        // }
        
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
