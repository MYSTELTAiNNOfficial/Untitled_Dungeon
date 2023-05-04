using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class FlyingEnemyController : MonoBehaviour
{
    public List<Transform> points;
    private int manyPoints = 2;
    public int pointID = 0;
    int idChangeValue = 1;

    public Transform target;
    public float speed = 2f;
    public int hp;
    public bool isProvoked = false;
    private float delayTime = 2;
    private float delay;
    public int atkPower;
    public bool isFacingRight;
    public float timer = 0;
    public bool isCast;

    System.Random rand = new System.Random();
    public GameObject bullet;
    [SerializeField] private Transform aimBullet;

    public Animator animator;

    public PlayerController playerController;
    public GameManager gm;
    public AudioManager audioManager;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        gm = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();
        rand = new System.Random();
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
        if (!isProvoked && timer < 0)
        {
            Move();
        }
        else
        {
            Provoked();
            if (playerController.getHP() > 0)
            {
                if (Vector3.Distance(playerController.transform.position, transform.position) > 10)
                {
                    timer -= Time.unscaledDeltaTime;
                    if (timer < 0)
                    {
                        isProvoked = false;
                    }
                } else
                {
                    delay += Time.unscaledDeltaTime;

                    if (delay > delayTime)
                    {
                        Attack_Animation();
                        delay = 0;
                    }
                }
            }
            if (playerController.getHP() == 0)
            {
                isProvoked= false;
            }
        }

        PlayerPrefs.SetInt(gameObject.name, hp);
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
        audioManager.PlayAudio("flyingAttack");
        isCast = true;
    }

    private void Attack()
    {
        var newSpell = Instantiate(bullet, aimBullet);
        newSpell.transform.parent = null;
        isCast = false;
    }

    public void hit(int value)
    {
        int currentState = hp - value;
        if (currentState > 0)
        {
            hp = currentState;
            animator.SetTrigger("Hit");
            audioManager.PlayAudio("flyingHit");
        }
        else
        {
            animator.SetTrigger("Die");
            audioManager.PlayAudio("flyingDie");
        }
    }

    public void Die()
    {
        int coin = rand.Next(1, 5);
        int stash = coin + PlayerPrefs.GetInt("coin");
        PlayerPrefs.SetInt("coin", stash);
        gm.setNotif(gameObject.name + " has been killed! Recieving " + coin.ToString() + " coins!");
        Destroy(gameObject);
    }

    public int getAP()
    {
        return atkPower;
    }
}
