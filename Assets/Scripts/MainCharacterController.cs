using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainCharacterController : MonoBehaviour
{
    // Public Variables
    public float runForce = 10f;
    public float jumpForce = 10f;
    public float maxRunSpeed = 6f;
    public float reducingVelocity = 3f;
    public float longJumpForce = 9f;
    public bool grounded;
    public bool rotated;
    public bool hitAbove;
    public static bool gameEndFlag;
    public bool nextLevel;

    // Components
    public AudioClip jumpAudioClip;
    public AudioClip coinAudioClip;
    public AudioClip breakBrickAudioClip;

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
    private AudioSource audioSource;
    private float castDistance;
    [SerializeField] private Transform startingPos;
    
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variable components
        rbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        animComp = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        castDistance = collider.bounds.extents.y * 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (nextLevel)
        {
            SceneManager.LoadScene("Scenes/LevelParser2", LoadSceneMode.Single);
        }
        
        if (gameEndFlag)
        {
            animComp.SetBool("Moving", false);
            animComp.SetBool("Turbo", false);
            return;
        }
        
        // Checking if player hit block above itself
        hitAbove = Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, 2.5f);

        // Function that focuses on player movement
        movement();
        
        // Function that focuses on character animation and look direction
        model();
        
        // Function that focuses on player jump mechanics
        jump();
    }

    void movement()
    {
        // Move left or right by applying force
        float axis = Input.GetAxis("Horizontal");
        rbody.AddForce(Vector3.right * axis * runForce, ForceMode.Force);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            maxRunSpeed = 12f;
        }
        else
        {
            maxRunSpeed = 6f;
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
        if (!grounded)
        {
            animComp.SetBool("Grounded", false);
        }
        else
        {
            animComp.SetBool("Grounded", true);
        }
        
        if (rbody.velocity.magnitude > 0.1f)
        {
            animComp.SetBool("Moving", true);
        }
        else
        {
            animComp.SetBool("Moving", false);
        }
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            animComp.SetBool("Turbo", true);
        }
        else
        {
            animComp.SetBool("Turbo", false);
        }
    }
    
    void jump()
    {
        // Checking to make sure the player is on the ground in order to jump
        // grounded = Physics.Raycast(transform.position, Vector3.down, castDistance);
        // Now using collision to detect if grounded instead of raycast
        
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
            audioSource.clip = jumpAudioClip;
            audioSource.Play();
            
            rbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            jumpBufferCounter = 0f;
        }
        else if (rbody.velocity.y > 0f && Input.GetKey(KeyCode.Space))
        {
            rbody.AddForce(Vector3.up * longJumpForce, ForceMode.Force);

            coyoteTimeCounter = 0f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("?") && hitAbove)
        {
            audioSource.clip = coinAudioClip;
            audioSource.Play();
            
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
            audioSource.clip = breakBrickAudioClip;
            audioSource.Play();
            
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

    private void OnCollisionStay(Collision collisionInfo)
    {
        grounded = true;
    }

    private void OnCollisionExit(Collision other)
    {
        grounded = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Goal"))
        {
            Debug.Log("You Win!");
            nextLevel = true;
        }
        if (other.gameObject.tag.Equals("Avoid"))
        {
            Debug.Log("You lost a life!");
            lives -= 1;
            transform.position = startingPos.position;
        }
    }
}
