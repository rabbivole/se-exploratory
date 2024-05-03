using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    // later this will become part of PlayerState, most likely
    public float speed = 6f;
    public Vector3 velocity = Vector3.zero; // for gravity implementation
    public bool isGrounded;
    public float jumpHeight = 2f;

    public Transform groundCheck;
    public float groundDistance = 0.1f; // radius of sphere used for grounded checking
    public LayerMask groundMask;
    public float gravity = -9.81f;

    public float turnSmoothingTime = 0.1f;
    private float turnSmoothVelocity;

    // debug
    private float GROUNDED_DISTANCE = 0.03f;

    // animation stuff
    public Animator animator;

    private void Start()
    {
        // to only situationally allow camera control
        CinemachineCore.GetInputAxis = CustomInputAxis;
    }

    // Intercepts mouse movement and only allows the camera to be orbited if a mouse button
    // is held down. 
    // todo: camera zoom
    private float CustomInputAxis(string axisName)
    {
        if (axisName.Equals("Mouse X"))
        {
            // must be holding down a mouse button to move camera
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                return Input.GetAxis("Mouse X");
            }
            else
            {
                return 0;
            }
        }
        else if (axisName.Equals("Mouse Y"))
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                return Input.GetAxis("Mouse Y");
            }
            else
            {
                return 0;
            }
        }
        Debug.Log(axisName);
        return Input.GetAxis(axisName);
    }

    void Update()
    {
        //isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        isGrounded = IsGrounded();
        // tell animator whether we're grounded or not
        animator.SetBool("Grounded", isGrounded);

        // account for gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (isGrounded && velocity.y < 0)
        {
            // todo: if the groundDistance gets smaller, can this be 0?
            velocity.y = 0; 
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // turn to face movement direction
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothingTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            // actually move the character
            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        }
        AnimateRunning(direction.magnitude);


        // jump handling
        // todo: this gives you WACKY air acceleration which i don't like
        // todo: we shouldn't keep trying to move upwards if we bonk our head on something
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            StartCoroutine(AnimateJump());
        }
        
        // debug
        if (Input.mouseScrollDelta.y != 0)
        {
            Debug.Log("mwheel delta: " + Input.mouseScrollDelta);
        }
    }

    private void AnimateRunning(float movementMagnitude)
    {
        // tell the animator whether we're moving or not so it can turn running on/off
        if (movementMagnitude >= 0.1f)
        {
            animator.SetFloat("Speed_f", 0.6f);
        }
        else
        {
            animator.SetFloat("Speed_f", 0);
        }
    }

    private IEnumerator AnimateJump()
    {
        animator.SetBool("Jump_b", true);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("Jump_b", false);
    }

    private bool IsGrounded()
    {
        // we're going to cast 5 rays in basically a + from the character's feet downwards.
        // in order, these are N, E, CENTER, W, S:
        Vector3[] feetRayOrigins =
        {
            new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.2f),
            new Vector3(transform.position.x + 0.3f, transform.position.y, transform.position.z),
            new Vector3(transform.position.x, transform.position.y, transform.position.z),
            new Vector3(transform.position.x - 0.3f, transform.position.y, transform.position.z),
            new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.2f)
        };

        foreach (Vector3 origin in feetRayOrigins)
        {
            if (Physics.Raycast(origin, Vector3.down, GROUNDED_DISTANCE, groundMask))
            {
                return true;
            }
        }
        Debug.Log("in midair!");
        return false;
    }
}
