using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class EnemyController : MonoBehaviour
{
    public List<Transform> points;
    private int manyPoints = 2;
    public int pointID = 0;
    int idChangeValue = 1;

    public Transform target;
    public float speed = 2f;
    public int hp;
    public bool isProvoked = false;
    private float delayTime = 4;
    private float delay;
    public int atkPower;
    public bool isFacingRight;
    public float timer = 0;
    public float attackDistance;

    System.Random rand = new System.Random();

    [SerializeField] private Transform attackPoint;
    public float attackRange;

    public Animator animator;

    public LayerMask playerLayer;

    public PlayerController playerController;
    public GameManager gm;
    public AudioManager audioManager;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        gm = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Reset()
    {
        init();
    }
    private void init()
    {
        points = new List<Transform>();

        GetComponent<CapsuleCollider2D>().isTrigger = true;

        GameObject root = new GameObject(name + "_Root");

        root.transform.position = transform.position;
        transform.SetParent(root.transform);

        GameObject waypoints = new GameObject("Waypoints");
        waypoints.transform.SetParent(root.transform);
        waypoints.transform.position = root.transform.position;

        for (int i = 0; i < manyPoints; i++)
        {
            GameObject p = new GameObject("Point" + (i + 1));
            p.transform.SetParent(waypoints.transform);
            p.transform.position = root.transform.position;
            points.Add(p.transform);
        }
    }

    private void Update()
    {
        
        if (!isProvoked && timer < 0 && hp > 0)
        {
            Move();
            animator.SetBool("Run", true);
        }
        else if (isProvoked || hp > 0)
        {
            checkHP();
            Provoked();
            if (playerController.getHP() > 0)
            {
                if (Vector3.Distance(playerController.transform.position, transform.position) > 7)
                {
                    timer -= Time.unscaledDeltaTime;
                    if (timer < 0)
                    {
                        isProvoked = false;
                    }
                }
                else
                {
                    Follow();
                }
            }
            if (playerController.getHP() == 0)
            {
                isProvoked = false;
            }
        }
        else
        {
            checkHP();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null && collider.gameObject.tag == "Spell")
        {
            if (!isProvoked)
            {
                isProvoked = true;
            }

            hit(playerController.getMP());
            checkHP();
        }
    }

    private void Move()
    {
        Transform goalPoint = points[pointID];
        if (goalPoint.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        transform.position = Vector2.MoveTowards(transform.position, goalPoint.position, speed * Time.unscaledDeltaTime);

        if (Vector2.Distance(transform.position, goalPoint.position) < 0.1f)
        {
            if (pointID == points.Count - 1)
            {
                idChangeValue = -1;
                isFacingRight = false;
            }

            if (pointID == 0)
            {
                idChangeValue = 1;
                isFacingRight = true;
            }

            pointID += idChangeValue;
        }

    }

    private void Follow()
    {

        float distance2player = Vector2.Distance(transform.position, playerController.transform.position);
        Vector2 target = new Vector2(playerController.transform.position.x, transform.position.y);

        if (distance2player > attackDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.unscaledDeltaTime);
            animator.SetBool("Run", true);
        }
        else if (attackDistance >= distance2player)
        {
            animator.SetBool("Run", false);
            delay += Time.unscaledDeltaTime;

            if (delay > delayTime)
            {
                Attack_Animation();
                delay = 0;
            }

        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void Provoked()

    {
        Vector3 localScale = transform.localScale;
        if (playerController.transform.position.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        if (playerController.transform.position.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
    }

    private void Attack_Animation()
    {
        animator.SetTrigger("Attack");
        audioManager.PlayAudio("enemyAttack");
    }

    private void Attack()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        foreach (Collider2D player in hit)
        {
            Debug.Log("We hit " + player.name);
            if (playerController != null)
            {
                playerController.hit(atkPower);
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

    public void hit(int value)
    {
        int currentState = hp - value;
        Debug.Log(currentState);
        if (currentState > 0)
        {
            hp = currentState;
            animator.SetTrigger("Hit");
            audioManager.PlayAudio("enemyHit");
        }
        else
        {
            Debug.Log("masuk else");
            animator.SetTrigger("Die");
            audioManager.PlayAudio("enemyDie");
        }
    }

    private void checkHP()
    {
        if (hp <= 0)
        {
            animator.SetTrigger("Die");
            audioManager.PlayAudio("enemyDie");
        }
    }

    public void Die()
    {
        int coin = rand.Next(1, 5);
        int stash = coin + PlayerPrefs.GetInt("coin");
        PlayerPrefs.SetInt("coin", stash);
        gm.setNotif(gameObject.name+" has been killed! Recieving " + coin.ToString() + " coins!");
        Destroy(gameObject);
    }

    public int getAP()
    {
        return atkPower;
    }
}