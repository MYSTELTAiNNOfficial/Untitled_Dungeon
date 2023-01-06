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
    public bool isCast = false;

    public int HP;
    public int maxHp = 100;
    public int atkPower;
    public int magicPower;
    public float attackRange = 0.5f;
    private float tempBuffTimer;
    private bool isBuffTemp = false;

    public GameObject spell;
    [SerializeField] private Transform aimCast;
    [SerializeField] private Transform startPoint;
    [SerializeField] private CameraController camera;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private FlyingEnemyController flyingEnemyController;
    [SerializeField] private GolemController golemController;
    [SerializeField] private GameManager gm;
    [SerializeField] private AudioManager am;
    [SerializeField] private Transform checkpointTarget;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        camera = FindObjectOfType<CameraController>();
        gm = FindObjectOfType<GameManager>();
        am = FindObjectOfType<AudioManager>();
        tempBuffTimer = PlayerPrefs.GetFloat("tempBuffTimer", 0);

        checkBuff();

        StartSpawn();

    }

    // Update is called once per frame
    void Update()
    {
        if (isBuffTemp)
        {
            tempBuffTimer -= Time.deltaTime;
            PlayerPrefs.SetFloat("tempBuffTimer", tempBuffTimer);
            Debug.Log(tempBuffTimer);
            if (tempBuffTimer <= 0.0f)
            {
                PlayerPrefs.SetString("isAtkGetTemp", "false");
                PlayerPrefs.SetString("isMagicGetTemp", "false");
                PlayerPrefs.SetInt("hp", HP);
                checkBuff();
                isBuffTemp= false;
            }
        }
        //Check if character on the ground
        isGround();

        //Flip character when try to go left
        Flip();

        //Animator for Run and Walk
        animator.SetFloat("xVelocity", Mathf.Abs(rb2d.velocity.x));
        animator.SetFloat("yVelocity", rb2d.velocity.y);

        if (transform.position.y <= -8.5f)
        {
            HP = 0;
            PlayerPrefs.SetInt("hp", 0);
        }
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
        am.PlayAudio("playerJump");

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
        am.PlayAudio("playerSpell");
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
        Collider2D[] hit = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        am.PlayAudio("playerMelee");

        foreach (Collider2D enemy in hit)
        {
            Debug.Log("We hit " + enemy.name);

            flyingEnemyController = enemy.GetComponent<FlyingEnemyController>();
            enemyController = enemy.GetComponent<EnemyController>();
            golemController = enemy.GetComponent<GolemController>();

            if (flyingEnemyController != null)
            {
                flyingEnemyController.isProvoked = true;
                flyingEnemyController.hit(atkPower);
                flyingEnemyController.timer = 5;
            }

            if (enemyController != null)
            {
                enemyController.isProvoked = true;
                enemyController.hit(atkPower);
                enemyController.timer = 5;
            }

            if (golemController != null)
            {
                golemController.hit(atkPower);
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
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
    public void Die()
    {

        PlayerPrefs.SetInt("hp", 0);
        animator.SetTrigger("Die");
    }

    public void StartSpawn()
    {
        string last = PlayerPrefs.GetString("checkpoint_stage", "");
        string revive = PlayerPrefs.GetString("revive", "false");
        string revivePremium = PlayerPrefs.GetString("revivePremium", "false");
        string continued = PlayerPrefs.GetString("continue", "false");

        PlayerPrefs.SetInt("hp", maxHp);
        if (continued == "true" || revive == "true")
        {
            if (last != "")
            {
                Debug.Log("Masuk Last");
                if (revivePremium == "true")
                {
                    Debug.Log("Masuk Premium");
                    tempBuffTimer = 10f;
                    PlayerPrefs.SetString("isAtkGetTemp", "true");
                    PlayerPrefs.SetString("isMagicGetTemp", "true");
                    checkBuff();
                    isBuffTemp= true;
                }
                if (PlayerPrefs.GetFloat("tempBuffTimer", 0) > 0)
                {
                    isBuffTemp = true;
                    checkBuff();
                    PlayerPrefs.SetString("isAtkGetTemp", "true");
                    PlayerPrefs.SetString("isMagicGetTemp", "true");
                    tempBuffTimer = PlayerPrefs.GetFloat("tempBuffTimer", 0);
                }
                Vector3 check = checkpointTarget.transform.position;
                Vector3 pos = rb2d.transform.position;
                pos.x = check.x;
                pos.y = check.y;
                rb2d.transform.position = pos;
                PlayerPrefs.SetString("revivePremium", "false");
                PlayerPrefs.SetString("revive", "false");
                PlayerPrefs.SetString("continue", "false");
            }
            else
            {
                Debug.Log("Masuk Start");
                if (PlayerPrefs.GetFloat("tempBuffTimer", 0) > 0)
                {
                    isBuffTemp= true;
                    checkBuff();
                    PlayerPrefs.SetString("isAtkGetTemp", "true");
                    PlayerPrefs.SetString("isMagicGetTemp", "true");
                    tempBuffTimer = PlayerPrefs.GetFloat("tempBuffTimer", 0);
                }
                Vector3 start = startPoint.transform.position;
                Vector3 pos = rb2d.transform.position;
                pos.x = start.x;
                pos.y = start.y;
                rb2d.transform.position = pos;
            }
        }
        else
        {
            if (PlayerPrefs.GetFloat("tempBuffTimer", 0) > 0)
            {
                isBuffTemp = true;
                checkBuff();
                PlayerPrefs.SetString("isAtkGetTemp", "true");
                PlayerPrefs.SetString("isMagicGetTemp", "true");
                tempBuffTimer = PlayerPrefs.GetFloat("tempBuffTimer", 0);
            }
            Vector3 start = startPoint.transform.position;
            Vector3 pos = rb2d.transform.position;
            pos.x = start.x;
            pos.y = start.y;
            rb2d.transform.position = pos;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        action.Invoke(collider);
    }

    public void hit(int value)
    {
        if (HP > 0)
        {
            HP -= value;
            am.PlayAudio("playerHit");
            animator.SetTrigger("Hit");
        }
        if (HP < 0)
        {
            HP = 0;
            am.PlayAudio("playerDie");
            Die();
        }
    }

    public void checkBuff()
    {
        if (PlayerPrefs.GetString("isAtkBought") == "true" || PlayerPrefs.GetString("isAtkGetTemp") == "true")
        {
            atkPower = 100;
        }
        else
        {
            atkPower = 25;
        }
        if (PlayerPrefs.GetString("isMagicBought") == "true" || PlayerPrefs.GetString("isMagicGetTemp") == "true")
        {
            magicPower = 100;
        }
        else
        {
            magicPower = 25;
        }
        if (PlayerPrefs.GetString("isHpBought") == "true")
        {
            maxHp = 200;
            int temp = PlayerPrefs.GetInt("hp", 0);
            if (temp == 0)
            {
                HP = maxHp;
            }
            else
            {
                HP = temp;
            }
        }
        else
        {
            maxHp = 100;
            int temp = PlayerPrefs.GetInt("hp", 0);
            if (temp == 0)
            {
                HP = maxHp;
            }
            else
            {
                HP = temp;
            }

        }
    }

    public int getHP()
    {
        if (HP < 0)
        {
            HP = 0;
        }
        return HP;
    }

    public void heal(int value)
    {
        int temp = HP + value;
        if (temp <= maxHp)
        {
            HP = temp;
        }
        else if (temp > maxHp)
        {
            HP = maxHp;
        }
    }

    public int getAP()
    {
        return atkPower;
    }

    public int getMP()
    {
        return magicPower;
    }

    public void DestroyOnDie()
    {
        Destroy(gameObject);
    }
}
