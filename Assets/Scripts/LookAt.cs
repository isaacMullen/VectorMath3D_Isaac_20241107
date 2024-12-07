using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    public Material agrroMat;
    public Material nonAggroMat;
    new Renderer renderer;

    public DetectionManager detectionManager;

    public Animator animator;
    private Quaternion targetRotation;

    public Transform player; // Reference to the player
    public List<Transform> balls; // List of balls or other targets

    public float moveSpeed;
    public float rotationSpeed;

    public float viewAngle = 60f;
    public float aggroRange;

    public LayerMask detectionLayers;
    public LayerMask obstacleLayers;

    private Transform currentTarget;
    private Vector3 lastKnownPosition;
    private bool isChasingLastPosition;

    private void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        DetectTargets();
    }

    private void FixedUpdate()
    {
        MoveToTarget();
    }

    void DetectTargets()
    {
        Transform bestTarget = null;
        float closestDistance = Mathf.Infinity;

        // Check if the player is visible
        if (CanSeeTarget(player))
        {
            if(renderer != null)
            {
                renderer.material = agrroMat;
            }
            Debug.Log("CAN SEE PLAYER");

            // Optionally, add a delay or cooldown here before resetting detection value
            detectionManager.currentDetectionValue += Time.deltaTime * detectionManager.detectionSpeed;
            detectionManager.currentDetectionValue = Mathf.Clamp(detectionManager.currentDetectionValue, 0, detectionManager.maxDetectionValue);

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer < closestDistance)
            {
                closestDistance = distanceToPlayer;
                bestTarget = player;
            }
        }
        else if(renderer != null)
        {
            renderer.material = nonAggroMat;
        }

        // Check if any ball is visible
        

        // Update the current target
        if (bestTarget != null)
        {
            currentTarget = bestTarget;
            lastKnownPosition = bestTarget.position;
            isChasingLastPosition = false;
        }
        else if (currentTarget != null)
        {
            // If we lose sight of the target, move to the last known position
            currentTarget = null;
            isChasingLastPosition = true;
        }
    }


    bool CanSeeTarget(Transform target)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, directionToTarget);

        // Check if the target is within view angle
        if (dotProduct > Mathf.Cos((viewAngle * 0.5f) * Mathf.Deg2Rad))
        {
            // Perform a raycast to check for obstacles
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToTarget, out hit, aggroRange, detectionLayers | obstacleLayers))
            {
                Debug.Log("Raycast hit: " + hit.transform.name);
                if (hit.transform == target)
                {
                    return true; // Target is visible
                }
            }
            else
            {
                Debug.Log("Raycast did not hit anything.");
            }
        }
        return false; // Target is not visible
    }

    void MoveToTarget()
    {
        Vector3 targetPosition;

        if (currentTarget != null)
        {            

            // Move towards the current target
            targetPosition = currentTarget.position;
        }
        else if (isChasingLastPosition)
        {
            // Move towards the last known position
            targetPosition = lastKnownPosition;

            // Stop chasing if we've reached the last known position
            if (Vector3.Distance(transform.position, lastKnownPosition) < 0.5f)
            {
                isChasingLastPosition = false;
                return;
            }
        }
        else
        {
            // No target and not chasing last position
            return;
        }

        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        targetRotation = Quaternion.LookRotation(directionToTarget);

        // Rotate toward the target
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Move toward the target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
}
