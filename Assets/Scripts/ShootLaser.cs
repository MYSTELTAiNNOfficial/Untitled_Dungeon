using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootLaser : MonoBehaviour
{
    private bool isHit = false;
    private void Start()
    {

    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            return;
        }
        isHit = true;
    }

    public void destroyItem()
    {
        Destroy(gameObject);
    }
}
