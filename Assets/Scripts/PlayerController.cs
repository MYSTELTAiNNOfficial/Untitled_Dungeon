using System;
using UnityEngine;
using UnityEngine.Diagnostics;

public class PlayerController : MonoBehaviour
{
    /* Reference:
     * https://www.youtube.com/watch?v=K1xZ-rycYY8
     * https://www.youtube.com/watch?v=RdhgngSUco0
     * 
     */

    public float moveSpeed;
    public float jumpPower;
    public Animator animator;
    private bool isFacingRight = true;
    private bool doubleJump;
    private bool isGrounded = false;
    private Rigidbody2D rb2d;
    public Action<Collider2D> action;
    public GameObject spell;
    public bool isCast = false;

    public int HP;
    public int atkPower;
    public int magicPower;

    [SerializeField] private Transform aimCast;
    [SerializeField] private Transform startPoint;
    [SerializeField] private CameraController camera;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        camera = FindObjectOfType<CameraController>();

        HP = PlayerPrefs.GetInt("hp", HP);

        StartSpawn();
    }

    // Update is called once per frame
    void Update()
    {

        //Check if character on the ground
        isGround();

        //Flip character when try to go left
        Flip();

        //Animator for Run and Walk
        animator.SetFloat("xVelocity", Mathf.Abs(rb2d.velocity.x));
        animator.SetFloat("yVelocity", rb2d.velocity.y);

        //Auto save current positions
        PlayerPrefs.SetFloat("playerPosX", rb2d.transform.position.x);
        PlayerPrefs.SetFloat("playerPosY", rb2d.transform.position.y);
    }

    public void Move(float direction)
    {
        Vector3 velocity = rb2d.velocity;
        velocity.x = direction * moveSpeed;
        rb2d.velocity = velocity;
    }

    public void Jump(float direction)
    {
        Vector3 velocity = rb2d.velocity;

        if (isGrounded || doubleJump)
        {
            animator.SetBool("Jump", true);
            velocity.y = direction * jumpPower;
            rb2d.velocity = velocity;
            doubleJump = !doubleJump;
        }
        else
        {
            doubleJump = false;
        }
    }

    public void Cast_Animation()
    {
        animator.SetTrigger("Cast");
        isCast = true;
    }

    public void Cast_Attack()
    {
        var newSpell = Instantiate(spell, aimCast);
        newSpell.transform.parent = null;
        isCast = false;
    }

    public void Melee_Animation()
    {
        animator.SetTrigger("Attack");
        isCast = true;
    }

    public void Melee_Attack()
    {
        
    }

    private void Flip()
    {
        if (isFacingRight && Input.GetAxisRaw("Horizontal") < 0f || !isFacingRight && Input.GetAxisRaw("Horizontal") > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    public void isGround()
    {
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.1f, groundLayer);
        if (colliders.Length > 0)
        {
            isGrounded = true;
        }

        animator.SetBool("Jump", !isGrounded);
    }

    public void StartSpawn()
    {

        Vector3 start = startPoint.transform.position;
        Vector3 pos = rb2d.transform.position;
        pos.x = PlayerPrefs.GetFloat("playerPosX", start.x);
        pos.y = PlayerPrefs.GetFloat("playerPosY", start.y);
        rb2d.transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        action.Invoke(collider);
    }

    public void hit()
    {
        HP--;
    }

    public int getHP()
    {
        return HP;
    }

    public void heal()
    {
        HP++;
    }

    public int getAP()
    {
        return atkPower;
    }

    public int getMP()
    {
        return magicPower;
    }
}
