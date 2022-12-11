using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private int HP;
    [SerializeField] private bool isFacingRight = true;
    [SerializeField] private float horizontal;
    [SerializeField] private bool doubleJump;
    [SerializeField] private bool isGrounded = false;

    [SerializeField] private Animator animator;
    [SerializeField] private Action<Collider2D> action;
    [SerializeField] private GameObject spell;
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private CameraController camera;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
