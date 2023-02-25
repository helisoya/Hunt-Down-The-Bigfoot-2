using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class BigfootAI : NetworkBehaviour
{

    [Header("Stats")]
    [SerializeField] private float speed;
    [SerializeField] private float runSpeed;
    [SyncVar] private int currentHealth;
    [SerializeField] private int maxHealth;
    [SerializeField] private float walkRadius;
    [SerializeField] private int damage;
    [SerializeField] private int maxBulletsBeforeFlee = 3;


    [Header("Components")]
    [SerializeField] private NetworkAnimator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private NetworkSound sound;
    [SerializeField] private AudioSource moveSound;


    public static BigfootAI instance;

    private bool fleeAtAllCost = false;
    private bool rage = false;
    private Transform playerTarget;
    private int currentBullets = 0;




    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, agent.destination);
    }


    void Start()
    {
        instance = this;

        if (!isServer)
        {
            enabled = false;
        }
        else
        {
            currentBullets = 0;
            currentHealth = maxHealth;
            animator.animator.SetBool("moving", true);
            agent.speed = speed;
        }

        PlayerGUI.instance.UpdateBigfootHealth(maxHealth, currentHealth);
    }

    void Update()
    {
        if (currentHealth == 0)
        {
            return;
        }

        if (rage)
        {
            if (playerTarget == null)
            {
                Flee();
            }

            agent.SetDestination(playerTarget.position);

            if (agent.remainingDistance <= 1.5f)
            {
                playerTarget.GetComponent<PlayerHealth>().TakeDamage(damage);
                Flee();
            }

        }
        else if (agent.remainingDistance <= 0.2f)
        {
            FindNextWalkPosition();
        }
    }


    void Flee()
    {
        currentBullets = 0;
        rage = false;
        FindNextWalkPosition();
        fleeAtAllCost = true;
        agent.speed = runSpeed;
    }


    void FindNextWalkPosition()
    {
        Vector3 randomDirection;
        Vector3 waypoint = new Vector3(-5, -5, -5);

        NavMeshPath path = new NavMeshPath();
        while (!(agent.CalculatePath(waypoint, path) && path.status == NavMeshPathStatus.PathComplete))
        {
            randomDirection = new Vector3(Random.Range(0, 500), 0, Random.Range(0, 500));
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1))
            {
                waypoint = hit.position;
            }
        }


        agent.SetDestination(waypoint);
        agent.isStopped = false;

        if (fleeAtAllCost)
        {
            fleeAtAllCost = false;
            agent.speed = speed;
        }


    }

    IEnumerator BigfootDeath()
    {
        moveSound.enabled = false;
        yield return new WaitForSeconds(5);

        Player.localPlayerCanMove = false;
        PlayerGUI.instance.OpenWinScreen();
    }

    [ClientRpc]
    void RpcClient_Death()
    {
        StartCoroutine(BigfootDeath());
    }


    [Command(requiresAuthority = false)]
    public void TakeDamage(int dmg)
    {
        currentHealth = Mathf.Clamp(currentHealth - dmg, 0, maxHealth);
        RpcClient_UpdateHealth(currentHealth, maxHealth);

        sound.CmdAddSound("BigfootHurt");

        if (currentHealth == 0)
        {
            // Die Sequence
            agent.isStopped = true;
            animator.SetTrigger("dead");
            RpcClient_Death();
        }
        else if (rage)
        {
            currentBullets++;
            if (currentBullets >= maxBulletsBeforeFlee)
            {
                Flee();
            }
        }
        else if (!fleeAtAllCost && !rage)
        {
            rage = true;
            playerTarget = Player.GetRandomPlayer().transform;
        }
    }


    [ClientRpc]
    void RpcClient_UpdateHealth(int currentHealth, int maxHealth)
    {
        this.currentHealth = currentHealth;
        PlayerGUI.instance.UpdateBigfootHealth(maxHealth, currentHealth);
    }

}
