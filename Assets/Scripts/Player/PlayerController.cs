using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private static readonly int Move = Animator.StringToHash("Move");
    
    [SerializeField] private float rotateSpeed = 100f;
    [SerializeField] private float jumpForce = 5;
    
    private CharacterController _characterController;
    private Animator _animator;
    
    private float _gravity = -9.81f;
    private Vector3 _velocity;
    private float _groundDistance;
    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // 커서 설정
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // 커서 락 해제
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
        HandleMovement();
        ApplyGravity();
    }

    // 사용자 입력 처리 함수
    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (vertical > 0)
        {
            _animator.SetBool(Move, true);
        }
        else
        {
            _animator.SetBool(Move, false);
        }

        Vector3 movement = transform.forward * vertical;
        transform.Rotate(0, horizontal * rotateSpeed * Time.deltaTime, 0);
        
        _characterController.Move(movement * Time.deltaTime);
        
        _groundDistance = GetDistanceToGround();
        Debug.Log("Ground Distance: " + _groundDistance);
        
        if (Input.GetButtonDown("Jump"))
        {
            _velocity.y = Mathf.Sqrt(jumpForce * -2f * _gravity);
        }
    }

    // 중력 적용 함수
    private void ApplyGravity()
    {
        _velocity.y += _gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);
    }
    
    // 바닥과 거리를 계산하는 함수
    private float GetDistanceToGround()
    {
        float maxDistance = 10f;
        if (Physics.Raycast(transform.position, 
                Vector3.down, out RaycastHit hit, maxDistance))
        {
            return hit.distance;
        }
        else
        {
            return maxDistance;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _groundDistance);
    }
}
