using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public float runSpeed = 1000f;
    public float jumpStrength = 20000f;

    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public bool jump = false;
    public bool hang = false;
    public bool dash = false;
    public bool slide = false;
    public bool backflip = false;
    public bool falling = false;

    public bool hasJumped = false;
    public bool hasDashed = false;

    private bool flipSprite = false;
    private Vector3 initialPosition;

    // For level designing.
    public bool overrideInitialPosition;
    public Vector3 overrideInitialPositionTarget;

    private float defaultGravity;
    public float defaultHangTime = 1f;
    private float hangTime;

    public float verticalDashStrength = 1.5f;
    public float horizontalDashStrength = 1.2f;
    public float backflipStrength = 1.8f;

    public int deaths = 0;
    public Text uiDeaths;

    public int diamonds = 0;
    public Text uiDiamonds;

    public Text uiInfo;
    public float uiInfoDisplayTime = 3f;
    public float uiInfoDisplayTimeDefault = 3f;

    private AudioSource audioSource;
    public AudioClip audioJump;
    public AudioClip audioDash;
    public AudioClip audioBackflip;
    public AudioClip audioDeath;
    public AudioClip audioDiamond;
    public AudioClip audioSlide;
    public AudioClip audioBonfireActivated;
    public AudioClip audioDoorActivated;
    public AudioClip audioTransform;

    public Camera mainCamera;
    public float shake = 0f;
    public float shakeAmount = 0.7f;
    public float shakeDecreaseFactor = 1f;

    private float death = 0f;
    public float deathRespawnTime = 2f;

    public bool godMode = false;

	// Use this for initialization
	void Start () {

        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        initialPosition = transform.position;
        defaultGravity = rigidBody.gravityScale;
        hangTime = defaultHangTime;
        audioSource = GetComponent<AudioSource>();

        if(overrideInitialPosition)
        {
            initialPosition = overrideInitialPositionTarget;
            transform.position = initialPosition;
        }

    }

    void Reset()
    {
        jump = false;
        hang = false;
        dash = false;
        slide = false;
        falling = false;
        hangTime = defaultHangTime;
        transform.position = initialPosition;
        rigidBody.velocity = new Vector2(0, 0);
        rigidBody.gravityScale = defaultGravity;

        if (uiDeaths != null)
        {
            uiDeaths.text = "Deaths: " + deaths;
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (rigidBody == null || spriteRenderer == null || animator == null) return;

        if(death > 0f)
        {
            death -= Time.deltaTime;
            animator.Play("Player_Transform");
            return;
        }
        else if (death < 0)
        {
            death = 0f;
            audioSource.PlayOneShot(audioTransform);
        }

        float hAxis = Input.GetAxisRaw("Horizontal");

        if (!hang)
        {
            if (hAxis > 0) flipSprite = false;
            if (hAxis < 0) flipSprite = true;
            rigidBody.AddForce(Vector3.right * hAxis * runSpeed);
        }
        
        if(Mathf.Abs(hAxis) < .01f && Mathf.Abs(rigidBody.velocity.x) > 10f && rigidBody.velocity.y == 0 )
        {
            animator.Play("Player_Slide");

            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(audioSlide);
            }
        }
        else if( Mathf.Abs(rigidBody.velocity.x) > 10f && rigidBody.velocity.y == 0)
        {
            animator.Play("Player_Run");
        }

        if (Input.GetButtonDown("Jump") && jump)
        {
            dash = true;
        }
        else if(Input.GetButtonDown("Jump") && Input.GetAxisRaw("Vertical") < 0f)
        {
            backflip = true;
        }
        else if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        if(uiDiamonds != null)
        {
            uiDiamonds.text = "Diamonds: " + diamonds;
        }

        if (shake > 0)
        {
            var random = Random.insideUnitCircle * shakeAmount;
            mainCamera.transform.position += new Vector3(random.x, random.y, 0);
            shake -= Time.deltaTime * shakeDecreaseFactor;
        }
        else
        {
            shake = 0f;
        }

        if(Mathf.Abs(rigidBody.velocity.x) < 0.5f)
        {
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        }

        Debug.Log("velocity.x: " + rigidBody.velocity.x + ", velocity.y: " + rigidBody.velocity.y);
    }

    void FixedUpdate()
    {

        spriteRenderer.flipX = flipSprite;

        if(jump && !hasJumped)
        {
            shake = .15f;
            hasJumped = true;
            animator.Play("Player_Jump");
            rigidBody.AddForce(Vector3.up * jumpStrength);
            rigidBody.gravityScale = defaultGravity;
            hang = false;
            audioSource.PlayOneShot(audioJump);
        }

        if(backflip && !hasJumped && !hang)
        {
            shake = .3f;
            hasJumped = true;
            animator.Play("Player_Backflip");
            rigidBody.AddForce(Vector3.up * jumpStrength * backflipStrength);
            audioSource.PlayOneShot(audioBackflip);
        }

        if(dash && !hasDashed)
        {
            shake = .3f;
            hasDashed = true;
            var axisH = Input.GetAxisRaw("Horizontal");
            audioSource.PlayOneShot(audioDash);

            if(axisH == 0)
            {
                rigidBody.AddForce(Vector3.up * jumpStrength * verticalDashStrength);
                animator.Play("Player_DashVertical");
            }
            else
            {
                rigidBody.AddForce(Vector3.right * axisH * jumpStrength * horizontalDashStrength);
                animator.Play("Player_DashHorizontal");
            }
            
        }

        if(hang && hangTime > 0)  
        {
            animator.Play("Player_WallGrab");
            rigidBody.velocity = new Vector2(0, 0);
            jump = false;
            hasJumped = false;
            hangTime -= Time.fixedDeltaTime;
        }
        
        if(hang && hangTime <= 0)
        {
            rigidBody.gravityScale = defaultGravity;
            hang = false;
            hasJumped = true;
        }

        if(Mathf.Abs(rigidBody.velocity.y) > 1f && !backflip)
        {
            falling = true;
            animator.Play("Player_Fall");
        }

        if(falling && Mathf.Abs(rigidBody.velocity.y) <= 1f)
        {
            falling = false;
            animator.Play("Player_Idle");
        }

        if(uiInfoDisplayTime <= 0f && uiInfo.text.Length > 0)
        {
            uiInfo.text = "";
        }
        else if(uiInfoDisplayTime > 0f)
        {
            uiInfoDisplayTime -= Time.fixedDeltaTime;
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "FloorCollider" 
            || collision.gameObject.tag == "Floor" 
            || (godMode && collision.gameObject.name == "Spikes"))
        {
            hasJumped = false;
            jump = false;
            hasDashed = false;
            dash = false;
            backflip = false;
            falling = false;
            hangTime = defaultHangTime;
            animator.Play("Player_Idle");
        }

        if((collision.gameObject.name == "OutOfLevelCollider" 
            || collision.gameObject.name == "SpikeCollider" 
            || collision.gameObject.name == "FinishCollider" 
            || collision.gameObject.tag == "Spike") && !godMode)
        {
            shake = 0.5f;
            audioSource.PlayOneShot(audioDeath);
            deaths++;
            death = deathRespawnTime;
            Reset();
        }

        if( (collision.gameObject.name == "WallCollider" || collision.gameObject.tag == "Wall") && (jump || backflip || falling))
        {
            rigidBody.gravityScale = 0;
            hang = true;
            falling = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Diamond")
        {
            Debug.Log("Diamong hit!");
            Destroy(collider.gameObject);
            audioSource.PlayOneShot(audioDiamond);
            diamonds++;
            if(diamonds == 15)
            {
                SetInfoText("You can now open the door to the east!");
            }
        }

        if(collider.gameObject.tag == "Door")
        {
            if(diamonds < 15)
            {
                SetInfoText("You need 15 diamonds!");
            }
            else
            {
                Destroy(collider.gameObject);
                diamonds -= 15;
                audioSource.PlayOneShot(audioDoorActivated);
            }
        }

        if(collider.gameObject.tag == "Bonfire")
        {
            if(initialPosition != collider.gameObject.transform.position)
            {
                SetInfoText("You will now respawn here!");
                initialPosition = collider.gameObject.transform.position;
                audioSource.PlayOneShot(audioBonfireActivated);
            }
        }
    }

    void SetInfoText(string message)
    {
        uiInfoDisplayTime = uiInfoDisplayTimeDefault;
        uiInfo.text = message;
    }
}
