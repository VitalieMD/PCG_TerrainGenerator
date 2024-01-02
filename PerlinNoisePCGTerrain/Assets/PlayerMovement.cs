using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private float _walkSpeed, _runSpeed, _jumpForce;

    private Vector3 moveDir;

    [SerializeField] private float yOffset;
    [SerializeField] private LayerMask groundMask;
    private Vector3 spherePos;
    [SerializeField] private float gravity = -9.81f;
    private Vector3 velocity;

    private bool running;
    private Animator _animator;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }


    private void Update()
    {
        moveDir = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");


        if ((Input.GetKey(KeyCode.LeftShift)))
        {
            Move(_runSpeed);
            _animator.SetFloat("Speed", 2);
        }
        else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) ||
                  Input.GetKey(KeyCode.D)))
        {
            Move(_walkSpeed);
            _animator.SetFloat("Speed", 1);
        }
        else if (!Input.GetKey(KeyCode.A) || !Input.GetKey(KeyCode.W) || !Input.GetKey(KeyCode.S) ||
                 !Input.GetKey(KeyCode.D))
        {
            _animator.SetFloat("Speed", 0);
        }


        Gravity();
        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space)) velocity.y += _jumpForce;

        _animator.SetBool("isGrounded", IsGrounded());
    }

    private bool IsGrounded()
    {
        var position = transform.position;
        spherePos = new Vector3(position.x, position.y - yOffset, position.z);
        return Physics.CheckSphere(spherePos, _characterController.radius - 0.05f, groundMask);
    }

    private void Gravity()
    {
        if (!IsGrounded())
            velocity.y += gravity * Time.deltaTime;
        else if (velocity.y < 0) velocity.y = -2;

        _characterController.Move(velocity * Time.deltaTime);
    }

    private void Move(float speed)
    {
        _characterController.Move(moveDir.normalized * (speed * Time.deltaTime));
    }
}