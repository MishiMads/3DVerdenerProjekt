using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MrX : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    public float viewDistance;
    public float updateInterval;
    public float fieldOfViewAngle;
    public LayerMask obstacleLayer; // Layer for obstacles
    private float lastUpdateTime;
    private Ray debugRay;
    public Animator Animator;
    public float caughtDistance;
    public float baseSpeed;
    public float chaseSpeed;
    public GameObject deathScreen;
    private Vector3 lastKnownPlayerPosition;
    private AudioSource scream;

    [SerializeField] private List<GameObject> punkter;
    private GameObject currentPoint;

    enum PossibleStates
    {
        Chasing,
        Patrolling,
        Searching,
        Idling
    }
    
    PossibleStates currentState;
    

    private void Start()
    {
        currentState = PossibleStates.Patrolling;
        agent = GetComponent<NavMeshAgent>();
        lastUpdateTime = Time.time;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentPoint = punkter[0];
        lastKnownPlayerPosition = player.position;
        scream = GetComponent<AudioSource>();
    }

    private void Update()
    {
        
        debugRay = new Ray(transform.position, player.position - transform.position);
        Debug.DrawRay(debugRay.origin, debugRay.direction * viewDistance, Color.red); // Draw a red ray in the scene view to show the view distance
        
        switch (currentState)
        {
            case PossibleStates.Patrolling:
                print("patrolling");
                patrol();
                checkForPlayer();
                break;
            case PossibleStates.Idling:
                print("idling");
                idle();
                break;
            case PossibleStates.Chasing:
                print("chasing");
                chase();
                break;
            case PossibleStates.Searching:
                print("searching");
                search();
                checkForPlayer();
                break;
        }
    }
    
    
    private void PlayMonsterScream()
    {
        if (!scream.isPlaying)
        {
            scream.Play();
        }
    }

    private void checkForPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= viewDistance && IsPlayerInFieldOfView())
        {
            currentState = PossibleStates.Chasing;
            lastKnownPlayerPosition = player.position;
            PlayMonsterScream();
        }
    }
    
    private void chase()
    {
        if (Time.time - lastUpdateTime > updateInterval)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            print("Stalking player");
            agent.SetDestination(player.position);
            Animator.SetBool("Walk", true);
            agent.speed = chaseSpeed;

            if (distanceToPlayer > viewDistance || !IsPlayerInFieldOfView())
            {
                // Player is either out of view distance or not in the field of view
                lastKnownPlayerPosition = player.position; // Store the last known position
                currentState = PossibleStates.Searching;
            }
            
            if (distanceToPlayer < caughtDistance)
            {
                //Debug.Log("Caught player");
                
                print("You died");
                deathScreen.SetActive(true);
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

            }

            lastUpdateTime = Time.time;
        }   
    }
    
    private IEnumerator idle()
    {
        Animator.SetBool("Walk", false);
        //Debug.Log("Idling");
        // After the delay, set the current waypoint to a random waypoint
        currentPoint = punkter[Random.Range(0, punkter.Count)];
        currentState = PossibleStates.Patrolling; // Set the state to Patrolling
        yield return new WaitForSeconds(2f);
    }

    private void patrol()
    {
        // Set the destination to the first waypoint
        Animator.SetBool("Walk", true);
        agent.SetDestination(currentPoint.transform.position);

        // Check if the agent is close to the current waypoint
        if (Vector3.Distance(transform.position, currentPoint.transform.position) < 2f)
        {
            StartCoroutine(idle()); // Start the idle coroutine
        }
    }

    private void search()
    {
        agent.speed = baseSpeed;

        if (lastKnownPlayerPosition != Vector3.zero)
        {
            // If there's a last known player position, go there
            agent.SetDestination(lastKnownPlayerPosition);
        }
        else
        {
            // If there's no last known position, go to a random location within a radius
            Vector3 randomDirection = Random.insideUnitSphere * 10f;
            agent.SetDestination(randomDirection + transform.position);
        }

        StartCoroutine(idle());
    }

    private void OnDrawGizmos()
    {
        RaycastHit hitInfo;
        Gizmos.color = Color.red;

        if (Physics.Raycast(debugRay, out hitInfo))
        {
            Gizmos.DrawSphere(hitInfo.point, 0.1f);
        }
    }

    private bool IsPlayerInFieldOfView()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle > fieldOfViewAngle * 0.5f)
        {
            return false;
        }
        
        RaycastHit hitinfo;
        if (!Physics.Raycast(transform.position, directionToPlayer, out hitinfo, viewDistance))
        {
            print("ikke inden for viewdistance"); // Debugging
            //print(hitinfo.transform.name); // Debugging
            return false;
            
        }

        if (hitinfo.transform.gameObject.CompareTag("obstacle"))
        {
            print("rammer obstacle tag"); // Debugging
            //print(hitinfo.transform.name); // Debugging
            // Player is obstructed by an obstacle, so MrX can't see the player
            return false;
        }

        Debug.Log("Hit object: " + hitinfo.transform.name);
        if (hitinfo.transform.gameObject.layer == obstacleLayer)
        {
            print("rammer obstacle layer"); // Debugging
            //print(hitinfo.transform.name); // Debugging
            // Player is obstructed by an obstacle, so MrX can't see the player
            return false;
        }
        

        //Spilleren er ikke bag nogle objekter og er inden for viewDistance
        //print("ser spiller"); // Debugging
        //print(hitinfo.transform.name); // Debugging
        return true;
    }
    // Lav måske:
    // if (isCrouching == true) viewDistance / 2
    // if (isRunning == true) viewDistance * 2
}
