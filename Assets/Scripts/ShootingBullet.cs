using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingBullet : MonoBehaviour
{
    public float speed;
    public Animator animator;
    private bool isHit = false;
    private void Start()
    {
        animator = GetComponent<Animator>();
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
        if (collider.gameObject.tag == "Enemy" || collider.gameObject.tag == "Spell")
        {
            return;
        }
        isHit = true;
        transform.Translate(transform.right * transform.localScale.x * 0);
        animator.SetTrigger("Hit");
    }

    public void destroyItem()
    {
        Destroy(gameObject);
    }
}
