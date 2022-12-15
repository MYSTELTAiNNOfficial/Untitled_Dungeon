using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyController : MonoBehaviour
{
    public List<Transform> points;
    private int manyPoints = 2;
    public int pointID = 0;
    int idChangeValue = 1;

    public Transform target;
    public float speed = 2f;
    public int hp;
    private bool isProvoked = false;
    private float delayTime = 2;
    private float delay;

    public Animator animator;
    public GameObject bullet;

    private void Start()
    {
        animator = GetComponent<Animator>();
        
        hp = PlayerPrefs.GetInt(gameObject.name, hp);
    }

    private void Reset()
    {
        init();
    }
    private void init()
    {
        points = new List<Transform>();

        GetComponent<BoxCollider2D>().isTrigger = true;

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
        if (!isProvoked)
        {
            Move();
        }
        else
        {
            Provoked();

            delay += Time.deltaTime;

            if (delay > delayTime)
            {
                Attack();
                delay = 0;
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

            if (hp > 0)
            {
                hp--;
            }
            else
            {
                Destroy(gameObject);
            }
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

        transform.position = Vector2.MoveTowards(transform.position, goalPoint.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, goalPoint.position) < 0.1f)
        {
            if (pointID == points.Count - 1)
            {
                idChangeValue = -1;
            }

            if (pointID == 0)
            {
                idChangeValue = 1;
            }

            pointID += idChangeValue;
        }

    }

    private void Provoked()
    {
        Vector3 vectorToTarget = target.transform.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 20);
    }

    private void Attack()
    {
        animator.SetTrigger("attack");

        var newSpell = Instantiate(bullet);

        Vector3 aimDirection = (target.transform.position - gameObject.transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg + 35;
        newSpell.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
        newSpell.GetComponent<ShootingBullet>().Cast(this.transform.position, new Vector3(Mathf.Cos(aimDirection.x), Mathf.Sin(aimDirection.y), 0), angle);
    }
}
