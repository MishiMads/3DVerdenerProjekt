using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MrX : MonoBehaviour
{
    // A bunch of variables
    public Transform player;
    private NavMeshAgent agent;
    public float viewDistance;
    public float updateInterval;
    public float fieldOfViewAngle;
    public LayerMask obstacleLayer;
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

    // possible enemy states
    enum PossibleStates
    {
        Chasing,
        Patrolling,
        Searching,
        Idling
    }
    // this variable is used to store the current state of the enemy
    PossibleStates currentState;
    

    private void Start()
    {
        currentState = PossibleStates.Patrolling; // Set the current state to Patrolling, since that's what we want the enemy to do at the start
        agent = GetComponent<NavMeshAgent>();
        lastUpdateTime = Time.time;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentPoint = punkter[0];
        lastKnownPlayerPosition = player.position;
        scream = GetComponent<AudioSource>();
    }

    private void Update()
    {
        
        // Raycast used for debugging
        debugRay = new Ray(transform.position, player.position - transform.position);
        Debug.DrawRay(debugRay.origin, debugRay.direction * viewDistance, Color.red); // Draw a red ray in the scene view to show the view distance
        
        // Finite state machine
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

    // this method checks if the player is within the view distance and field of view:
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
    
    // this method is called when the chase state is triggered and the enemy is chasing the player:
    private void chase()
    {
        if (Time.time - lastUpdateTime > updateInterval)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            print("Stalking player");
            agent.SetDestination(player.position);
            Animator.SetBool("Walk", true);
            agent.speed = chaseSpeed; // Here, I change the speed of the enemy when chasing the player
            

            // Check if the player is out of view distance or not in the field of view
            if (distanceToPlayer > viewDistance || !IsPlayerInFieldOfView())
            {
                // Player is either out of view distance or not in the field of view
                lastKnownPlayerPosition = player.position; // Store the last known position
                currentState = PossibleStates.Searching;
            }
            
            // Check if the player is close enough to be caught which triggers a game over
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
    
    // this method is called when the idle state is active which makes the enemy idle for a short period of time:
    private IEnumerator idle()
    {
        Animator.SetBool("Walk", false);
        //Debug.Log("Idling");
        currentPoint = punkter[Random.Range(0, punkter.Count)]; // This code selects a random point from the list of points, which the enemy will go to when the patrol state is activated.
        currentState = PossibleStates.Patrolling; // Set the state to Patrolling
        yield return new WaitForSeconds(2f);
    }

    
    // this method is called when the patrol state is active which makes the enemy patrol between the points:
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

    // this method is called when the search state is active which makes the enemy search for the player by going to the player's last known position:
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

    // this method was used for debugging:
    private void OnDrawGizmos()
    {
        RaycastHit hitInfo;
        Gizmos.color = Color.red;

        if (Physics.Raycast(debugRay, out hitInfo))
        {
            Gizmos.DrawSphere(hitInfo.point, 0.1f);
        }
    }

    // this method checks if the player is within a specified field of view:
    private bool IsPlayerInFieldOfView()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle > fieldOfViewAngle * 0.5f)
        {
            return false;
        }
        
        // This condition checks whether the player is within the view distance
        RaycastHit hitinfo;
        if (!Physics.Raycast(transform.position, directionToPlayer, out hitinfo, viewDistance))
        {
            print("ikke inden for viewdistance"); // Debugging
            //print(hitinfo.transform.name); // Debugging
            return false;
            
        }

        // the following two conditions are similar as they were both employed to check what the best way to check for obstacles was. The first one checks for tags and the second one checks for layers.
        
        // This condition checks whether the player is behind an obstacle with the tag "obstacle"
        if (hitinfo.transform.gameObject.CompareTag("obstacle"))
        {
            print("rammer obstacle tag"); // Debugging
            //print(hitinfo.transform.name); // Debugging
            // Player is obstructed by an obstacle, so MrX can't see the player
            return false;
        }

        // This condition checks whether the player is behind an obstacle with the layer "obstacle"
        Debug.Log("Hit object: " + hitinfo.transform.name);
        if (hitinfo.transform.gameObject.layer == obstacleLayer)
        {
            print("rammer obstacle layer"); // Debugging
            //print(hitinfo.transform.name); // Debugging
            // Player is obstructed by an obstacle, so MrX can't see the player
            return false;
        }
        

        // If none of the if statements are triggered, the player is within the field of view and can be seen by the enemy.
        return true;
    }
}
