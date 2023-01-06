    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GolemController : MonoBehaviour
{
    public int maxHealth = 1500;
    public int health = 1500;
    public int currentHealth;
    public Slider healthBar;

    public int atkPower;
    public float attackDelay = 3;
    private float cooldown = 3;

    public PlayerController playerController;
    private Rigidbody2D rb2d;
    private bool isDie = false;

    public GameObject laser;
    [SerializeField] private Transform firePoint;
    public GameObject shockwave;
    [SerializeField] private Transform slamPoint;

    public Animator animator;
    public AudioManager audioManager;
    System.Random rand = new System.Random();
    public GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        healthBar.maxValue = maxHealth;
        currentHealth = health;
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerController = FindObjectOfType<PlayerController>();
        rand = new System.Random();
        audioManager = FindObjectOfType<AudioManager>();
        gm = FindObjectOfType<GameManager>();
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
        healthBar.value = health;


        if (!isDie)
        {
            attackDelay -= Time.deltaTime;
            if (attackDelay <= 0f)
            {
                Debug.Log("If Masuk");
                attackDelay = cooldown;
                Attack();
            }
        }
    }


    private void Attack()
    {
        int randomNumber = rand.Next(1, 3);

        if(randomNumber == 1)
        {
            animator.SetTrigger("Laser");
            ShootLaser();
            audioManager.PlayAudio("golemLaser");
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

    private void CallSlamAudio()
    {
        audioManager.PlayAudio("golemSlam");
    }

    private void CallShockWaveAudio()
    {
        audioManager.PlayAudio("golemShockwave");
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
            isDie = true;
        }
    }

    public int getAP()
    {
        return atkPower;
    }

    private void Die()
    {
        int coin = rand.Next(20, 50);
        int stash = coin + PlayerPrefs.GetInt("coin");
        PlayerPrefs.SetInt("coin", stash);
        gm.setNotif(gameObject.name + " has been killed! Recieving " + coin.ToString() + " coins!");
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null && collider.gameObject.tag == "Spell")
        {
            hit(playerController.getMP());
        }
    }
}
