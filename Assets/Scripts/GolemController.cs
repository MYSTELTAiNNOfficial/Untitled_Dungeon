    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GolemController : MonoBehaviour
{
    public int health = 1500;
    public int currentHealth;
    public Slider healthBar;

    public int atkPower;
    public float attackDelay = 3;
    private float cooldown = 3;

    public bool flip;
    public bool facingRight = false;
    public bool isAttacking = false;

    public PlayerController playerController;
    private Rigidbody2D rb2d;

    public GameObject laser;
    [SerializeField] private Transform firePoint;
    public GameObject shockwave;
    [SerializeField] private Transform slamPoint;

    public Animator animator;
    System.Random rand = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = health;
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerController = FindObjectOfType<PlayerController>();
        rand = new System.Random();
    }

    // Update is called once per frame
    void Update()
    {
        //face
        if (playerController.transform.position.x > gameObject.transform.position.x)
        {
            gameObject.transform.localScale = new Vector3(7, 7, 7);
        }else if(playerController.transform.position.x < gameObject.transform.position.x)
        {
            gameObject.transform.localScale = new Vector3(-7, 7, 7);
        }

        //healthbar
        healthBar.value = currentHealth;
        Debug.Log(currentHealth);


        attackDelay -= Time.deltaTime;
        if(attackDelay <= 0f)
        {
            Debug.Log("If Masuk");
            attackDelay = cooldown;
            Attack();
        }
    }


    private void Attack()
    {
        int randomNumber = rand.Next(1, 3);

        if(randomNumber == 1)
        {
            animator.SetTrigger("Laser");
            ShootLaser();
            Debug.Log("Tembak laser");
        }else
        {
            animator.SetTrigger("Slam");
            Debug.Log("Pukul tanah");
        }
    }

    private void ShootLaser()
    {
        var newLaser = Instantiate(laser, firePoint);
        newLaser.transform.parent = null;
    }

    private void ShockWave()
    {

        var newSlam = Instantiate(shockwave, slamPoint);
        newSlam.transform.parent = null;
    }

    public void hit(int value)
    {
        currentHealth = health - value;
        if (currentHealth > 0)
        {
            health = currentHealth;
            Debug.Log("Hit");
            Debug.Log(currentHealth.ToString());
        }
        else
        {
            animator.SetTrigger("Die");
        }
    }

    public int getAP()
    {
        return atkPower;
    }
}
