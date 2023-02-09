using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovements : MonoBehaviour
{
    [Header("Components")]

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform groundDetector;
    [SerializeField] private Transform cam;
    [SerializeField] private NetworkAnimator bodyAnimator;
    [SerializeField] private Animator gunAnimator;



    [Header("Stats")]
    [SerializeField] private int speed;
    [SerializeField] private int sensibility;


    private float xrot = 0;

    public float maxXRot;

    private int currentSpeed;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }


    bool IsGrounded()
    {
        return Physics.Raycast(groundDetector.position, -Vector3.up, 0.5f);
    }

    void Update()
    {

        if (!Player.localPlayerCanMove) return;

        bool gounded = IsGrounded();
        bodyAnimator.animator.SetBool("grounded", gounded);

        bool running = Input.GetKey(KeyCode.LeftShift);
        bodyAnimator.animator.SetBool("run", running);
        gunAnimator.SetBool("run", running);

        bool moving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        bodyAnimator.animator.SetBool("walk", moving);
        gunAnimator.SetBool("move", moving);


        if (moving)
        {
            currentSpeed = speed + (running ? 5 : 0);
            Vector3 PlayerMovement = cam.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"))) * currentSpeed;
            rb.velocity = new Vector3(PlayerMovement.x, rb.velocity.y, PlayerMovement.z);
        }

        Vector2 PlayerCamMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        transform.Rotate(0f, PlayerCamMovement.x * sensibility, 0f);
        xrot = Mathf.Clamp(xrot - PlayerCamMovement.y * sensibility, -maxXRot, maxXRot);
        cam.localEulerAngles = new Vector3(xrot, 0f, 0f);

        if (gounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(transform.up * 300);
            bodyAnimator.SetTrigger("jump");
        }

    }
}
