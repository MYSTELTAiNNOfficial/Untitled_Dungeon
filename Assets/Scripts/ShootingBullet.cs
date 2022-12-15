using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingBullet : MonoBehaviour
{
    public float speed;

    Vector3 translate;

    void Update()
    {
        transform.Translate(translate * speed * Time.deltaTime);

        if (transform.position.y > 10)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            return;
        }

        Destroy(gameObject);
    }

    public void Cast(Vector3 posAwal, Vector3 direction, float angle)
    {
        //taruh peluru di posisi awal
        transform.localPosition = new Vector3(posAwal.x, posAwal.y, posAwal.z);
        //transform.localPosition += translationVec * speed * 1000;

        // set arah peluru
        translate = new Vector3(direction.x, direction.y, direction.z);

        transform.rotation = Quaternion.Euler(0, 0, angle);

    }
}
