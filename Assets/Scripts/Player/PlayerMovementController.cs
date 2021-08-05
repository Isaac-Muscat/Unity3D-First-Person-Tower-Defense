using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public float walkSpeed = 14f;
    public float sprintSpeed = 20;
    public float gravity = -20f;
    public float groundDistance = 0.4f;
    public float jumpHeight = 2;

    public CharacterController characterController;
    public Transform feet;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;
    
    private float speed = 0;

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(feet.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
        if (isGrounded && Input.GetAxis("Jump")==1)
            velocity.y =  Mathf.Sqrt(jumpHeight*-2f*gravity);


        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * x + transform.forward * z;

        if(Input.GetKey(KeyCode.LeftShift)){
            speed = Mathf.Lerp(speed, sprintSpeed, 0.001f);
        } else {
            speed = Mathf.Lerp(speed, walkSpeed, 0.01f);
        }

        if(Physics.CheckSphere(transform.position, 0.6f, groundMask)) speed = 0;

        characterController.Move(moveDirection * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
        
    }
}
