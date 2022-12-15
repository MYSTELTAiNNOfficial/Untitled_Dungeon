using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Reference:
 * https://www.youtube.com/watch?v=ZBj3LBA2vUY
 * 
 */

public class CameraController : MonoBehaviour
{
    private Vector3 offset = new Vector3(0f, 0f, -10f);
    private float smoothTime = 0.30f;
    private Vector3 velocity = Vector3.zero;

    public Transform target;
    public Vector3 min,max;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = target.position + offset;
        Vector3 boundCamera = new Vector3(
            Mathf.Clamp(targetPos.x, min.x, max.x),
            Mathf.Clamp(targetPos.y, min.y, max.y),
            Mathf.Clamp(targetPos.z, min.z, max.z));
        transform.position = Vector3.SmoothDamp(transform.position, boundCamera, ref velocity, smoothTime);
    }
}
