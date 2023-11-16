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

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lastUpdateTime = Time.time;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        
        debugRay = new Ray(transform.position, player.position - transform.position);
        Debug.DrawRay(debugRay.origin, debugRay.direction * viewDistance, Color.red); // Draw a red ray in the scene view to show the view distance
        
        if (Time.time - lastUpdateTime > updateInterval)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= viewDistance && IsPlayerInFieldOfView())
            {
                Debug.Log("Stalking player");
                agent.SetDestination(player.position);
            }
            else
            {
                Debug.Log("Not stalking");
                agent.ResetPath(); // Stop moving towards the player when not stalking
            }

            lastUpdateTime = Time.time;
        }
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

        if (angle <= fieldOfViewAngle * 0.5f)
        {
            RaycastHit hitinfo;

            if (Physics.Raycast(transform.position, directionToPlayer, out hitinfo, viewDistance))
            {
                if (hitinfo.transform.gameObject.CompareTag("obstacle"))
                {
                    // Player is obstructed by an obstacle, so MrX can't see the player
                    return false;
                }
                {


                    Debug.Log("Hit object: " + hitinfo.transform.name);
                    Debug.Log("Hit layer: " + LayerMask.LayerToName(hitinfo.transform.gameObject.layer));

                    if (hitinfo.transform.gameObject.layer == obstacleLayer)
                    {
                        // Player is obstructed by an obstacle, so MrX can't see the player
                        return false;
                    }
                }

                // Player is in line of sight and not obstructed by obstacles, so MrX can see the player
                return true;
            }

            // Player is not within the field of view angle, so MrX can't see the player
            return false;
        }
        return false;
    }
    // if (isCrouching == true) viewDistance / 2
    // if (isRunning == true) viewDistance * 2
}
