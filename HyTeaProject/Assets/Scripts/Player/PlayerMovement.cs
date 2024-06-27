using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")] 
    public float moveSpeed;
    public float groundDrag;

    [Header("Jumping")] 
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool _readyToJump;

    [Header("Ground Check")] 
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool _grounded;
    
    public Transform orientation;

    private float _horizontalInput;
    private float _verticalInput;

    private Vector3 _moveDirection;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    private void Update()
    {
        _grounded = IsPlayerGrounded();
        
        MyInput();
        SpeedControl();
        
        _rb.drag = _grounded ? groundDrag : 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private bool IsPlayerGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
    }

    private void MyInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Space) && _readyToJump && _grounded ) { CallJump(); }
    }

    private void MovePlayer()
    {
        _moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;

        if (_grounded) _rb.AddForce(_moveDirection.normalized * (moveSpeed * 10f), ForceMode.Force);
        else _rb.AddForce(_moveDirection.normalized * (moveSpeed * 10f * airMultiplier), ForceMode.Force);
    }

    private void SpeedControl()
    {
        var velocity = _rb.velocity;
        Vector3 flatVec3 = new Vector3(velocity.x, 0f, velocity.z);

        if (!(flatVec3.magnitude > moveSpeed)) return;
        
        Vector3 limitedVel = flatVec3.normalized * moveSpeed;
        _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
    }

    private void CallJump()
    {
        _readyToJump = false;
        Jump();
        StartCoroutine(ResetJump(jumpCooldown));
    }
    
    private void Jump()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        
        _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private IEnumerator ResetJump(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        _readyToJump = true;
    }
    
    
}
