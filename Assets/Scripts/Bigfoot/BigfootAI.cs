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

    [Header("Components")]
    [SerializeField] private NetworkAnimator animator;
    [SerializeField] private NavMeshAgent agent;


    public static BigfootAI instance;

    private bool fleeAtAllCost = false;
    private bool rage = false;
    private Transform playerTarget;



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
            currentHealth = maxHealth;
            animator.animator.SetBool("moving", true);
            agent.speed = speed;
        }

        PlayerGUI.instance.UpdateBigfootHealth(maxHealth, currentHealth);
    }

    void Update()
    {
        if (rage)
        {
            if (playerTarget == null)
            {
                Flee();
            }

            agent.SetDestination(playerTarget.position);

            print(agent.remainingDistance);
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



    [Command(requiresAuthority = false)]
    public void TakeDamage(int dmg)
    {
        currentHealth = Mathf.Clamp(currentHealth - dmg, 0, maxHealth);
        RpcClient_UpdateHealth(currentHealth, maxHealth);

        if (currentHealth == 0)
        {
            // Die Sequence
            agent.isStopped = false;
            animator.SetTrigger("dead");
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
