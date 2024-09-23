using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public float speed = 3f;
    public float gravity = -9.82f;
    public float jumpHeight = 2.0f;

    private CharacterController controller;

    private Vector3 moveVector;

    private bool isGrounded = false;
    private float horizontalInput;
    private float verticalInput;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        Vector3 moveVelocity = transform.forward * verticalInput + transform.right * horizontalInput;

        Vector3 moveDirection = moveVelocity.normalized;

        float moveSpeed = Mathf.Min(moveVelocity.magnitude, 1.0f) * speed;

        animator.SetFloat("Speed", moveSpeed * 0.1f);

        Vector3 temp = moveDirection * moveSpeed;
        temp.y = moveVector.y;

        moveVector = temp;

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            moveVector.y = Mathf.Sqrt(jumpHeight * -2f * gravity);    //점프 최대 높이
        }

        if(isGrounded && moveVector.y < 0)
        {
            moveVector.y = 0f;
        }

        moveVector.y += gravity * Time.deltaTime;


        controller.Move(moveVector * Time.deltaTime);
    }
}
