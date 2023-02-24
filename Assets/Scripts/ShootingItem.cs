using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingItem : MonoBehaviour
{
    public float speed;
    public Animator animator;
    public AudioManager audioManager;
    private bool isHit = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        if (isHit)
        {
            transform.Translate(transform.right * transform.localScale.x * 0);
        }
        else
        {
            transform.Translate(transform.right * transform.localScale.x * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player" || collider.gameObject.tag == "Bullet" || collider.gameObject.tag == "Laser")
        {
            return;
        }
        isHit= true;
        transform.Translate(transform.right * transform.localScale.x * 0 );
        animator.SetTrigger("Hit");
        audioManager.PlayAudio("spellExplode");
    }

    public void destroyItem()
    {
        Destroy(gameObject);
    }
}
