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
    public int HP;

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

    public void Cast()
    {
        animator.SetTrigger("Cast1");

        var newSpell = Instantiate(spell);
        newSpell.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
        newSpell.GetComponent<ShootingItem>().Cast(this.transform.position, new Vector3(Mathf.Cos(Aim().x), Mathf.Sin(Aim().y), 0), AimAngle());
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

    public Vector3 Aim()
    {
        Vector3 mousePos = camera.gameObject.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);

        Vector3 aimDirection = (mousePos - gameObject.transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg + 35;

        return aimDirection;
    }

    public float AimAngle()
    {
        float angle = Mathf.Atan2(Aim().y, Aim().x) * Mathf.Rad2Deg;
        return angle;
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
}
