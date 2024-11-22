using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController _controller;
    private Transform _camera;
    private Animator _animator;

    private float _horizontal;
    private float _vertical;
    [SerializeField] private float _movimentSpeed = 5;
    private  float _turnSmoothVelocity;
    [SerializeField] private float _turnSmoothTime = 0.5f;

    [SerializeField] private float _jumpHeight = 1;

     [SerializeField] private float  _gravity = -10f;
    [SerializeField] private Vector3 _playerGravity;

     [SerializeField] Transform _sensorPosition;
    [SerializeField] float _sensorRadius = 0.5f;
    [SerializeField] LayerMask _groundLayer;
    private bool _hasJumped = false;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _camera = Camera.main.transform;
        _animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");

         Movement();

         if(Input.GetButtonDown("Jump") && IsGrounded())
        {
            Jump();
        }

         Gravity();
    }
     void Jump()
    {
        _playerGravity.y = Mathf.Sqrt(_jumpHeight * -2 * _gravity);

            if (!_hasJumped)
            {
                _animator.SetBool("IsJumping", true);
                _hasJumped = true;
            }
    }
    void Movement()
    {
         Vector3 direction = new Vector3(_horizontal, 0, _vertical);

        _animator.SetFloat("VelZ", direction.magnitude);
        _animator.SetFloat("VelX", 0); 

        if(direction != Vector3.zero)
         {
             float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _camera.eulerAngles.y; 

            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime); 

             transform.rotation = Quaternion.Euler(0, smoothAngle, 0); 

             Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

             _controller.Move(moveDirection * _movimentSpeed * Time.deltaTime);
         }
    }
    void Gravity()
    {
        if (!IsGrounded())
        {
            _playerGravity.y += _gravity * Time.deltaTime;
        }
        else if (IsGrounded() && _playerGravity.y < 0)
        {
            _playerGravity.y = -1;

            _animator.SetBool("IsJumping", false);
            _hasJumped = false;
        }

        _controller.Move(_playerGravity * Time.deltaTime);
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(_sensorPosition.position, _sensorRadius, _groundLayer);
    }
}
