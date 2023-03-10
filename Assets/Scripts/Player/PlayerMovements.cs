using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovements : NetworkBehaviour
{
    [Header("Components")]

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform groundDetector;
    [SerializeField] private Transform cam;
    [SerializeField] private NetworkAnimator bodyAnimator;
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Collider capsuleCollider;
    [SerializeField] private GameObject gunRoot;
    [SerializeField] private GameObject flashlight;


    [SerializeField] private PlayerSound sound;



    [Header("Stats")]
    [SerializeField] private int speed;
    [SerializeField] private int sensibility;


    private float xrot = 0;

    public float maxXRot;

    private int currentSpeed;

    private Transform vehiculeRoot;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }


    bool IsGrounded()
    {
        return Physics.Raycast(groundDetector.position, -Vector3.up, 0.5f);
    }


    void FixedUpdate()
    {
        if (Player.localPlayerInVehicule && Player.localPlayerCanMove)
        {
            if (Player.localPlayerCanMove)
            {
                Player.localDrivenVehicule.UpdateVehicule();
            }
        }
    }

    void Update()
    {
        if (Player.localPlayerInVehicule)
        {
            Player.localDrivenVehicule.CheckZRotation();
            transform.position = vehiculeRoot.position;
            transform.eulerAngles = vehiculeRoot.eulerAngles;
        }

        if (!Player.localPlayerCanMove) return;

        Vector2 PlayerCamMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        xrot = Mathf.Clamp(xrot - PlayerCamMovement.y * sensibility, -maxXRot, maxXRot);
        cam.localEulerAngles = new Vector3(xrot, 0f, 0f);


        if (Player.localPlayerInVehicule) return;

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

        if (moving && !sound.isMoveSoundPlaying)
        {
            sound.CmdSetWalkSound(true);
        }
        else if (!moving && sound.isMoveSoundPlaying)
        {
            sound.CmdSetWalkSound(false);
        }

        transform.Rotate(0f, PlayerCamMovement.x * sensibility, 0f);

        if (gounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(transform.up * 300);
            bodyAnimator.SetTrigger("jump");
        }


        if (Input.GetKeyDown(KeyCode.F))
        {
            flashlight.SetActive(!flashlight.activeInHierarchy);
        }
    }


    public void EnterVehicule(Transform root)
    {
        vehiculeRoot = root;
        transform.eulerAngles = Vector3.zero;
        Command_SetColliderActive(false);
        transform.position = root.position;
        gunRoot.SetActive(false);
        bodyAnimator.SetTrigger("drivingStart");
        Player.localPlayerInVehicule = true;

        if (sound.isMoveSoundPlaying)
        {
            sound.CmdSetWalkSound(false);
        }

    }

    public void ExitVehicule()
    {
        vehiculeRoot = null;
        gunRoot.SetActive(true);
        bodyAnimator.SetTrigger("drivingEnd");
        Player.localPlayerInVehicule = false;
        transform.position += transform.up * 3;
        Command_SetColliderActive(true);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }


    [Command(requiresAuthority = false)]
    void Command_SetColliderActive(bool value)
    {
        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = value;
            rb.isKinematic = !value;
        }

        RpcClient_SetColliderActive(value);
    }

    [ClientRpc]
    void RpcClient_SetColliderActive(bool value)
    {
        capsuleCollider.enabled = value;
        rb.isKinematic = !value;
    }
}
