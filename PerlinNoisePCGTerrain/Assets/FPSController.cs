using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public Transform orientation;
    private float hInput;
    private float vInput;

    private Vector3 moveDir;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        
    }

    private void Update()
    {
        Inputs();
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void Inputs()
    {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");
    }

    void PlayerMovement()
    {
        moveDir = orientation.forward * vInput + orientation.right * hInput;
        rb.AddForce(moveDir.normalized * speed,ForceMode.Force);
    }
}
