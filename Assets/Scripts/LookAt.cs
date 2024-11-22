using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class LookAt : MonoBehaviour
{        
    public Animator animator;
    private Quaternion targetRotation;
    
    public Transform player;    
    Vector3 directionToPlayer;
    Vector3 lastRecordedPlayerPosition;

    public TextMeshProUGUI dotProductText;
    public TextMeshProUGUI isInViewText;

    //Materials to switch between when in view
    public Material nonAggroMaterial;
    public Material aggroMaterial;
   
    Vector3 edgeDirection;
    Vector3 negativeEdgeDirection;
            
    public float moveSpeed;
    public float rotationSpeed;   

    public float viewAngle = 60f;
    public float aggroRange;
    
    bool enemyAggrod;
    bool shouldChasePlayer;
    bool goToLastPosition;
    
    bool visionCheckOnCooldown;
    bool visionCheckRunning;
    bool checkingVision;

    float dotProduct;


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, lastRecordedPlayerPosition);
        
        //Drawing a line to the player we are looking at
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, player.position);
        //Getting the direction from the enemy (looking object) to the player (object to look at)
        Vector3 directionToObject = (player.position - transform.position).normalized;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.forward * 2);

        //Determining the dot product
        dotProduct = Vector3.Dot(transform.forward, directionToObject);

        dotProductText.text = $"Dot Product: {dotProduct}";             

        //Getting the directions to the edges of the viewpoint
        edgeDirection = Quaternion.Euler(0, (viewAngle / 2), 0) * transform.forward;
        negativeEdgeDirection = Quaternion.Euler(0, (-viewAngle / 2), 0) * transform.forward;

        //Drawing the lines on the edge of the viewing frustum
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + edgeDirection * 10);
        Gizmos.DrawLine(transform.position, transform.position + negativeEdgeDirection * 10);

        //Comparing the dot product to the angle of view on each side of the forward direction. Hence dividing the viewangle by 2.
        //If i didn't divide it by 2, the 45 degree angle would be used on each side, resulting in a 90 degree total viewing angle.
        if (dotProduct > Mathf.Cos((viewAngle * .5f) * Mathf.Deg2Rad) && Vector3.Distance(transform.position, player.position) < aggroRange)
        {
            isInViewText.text = $"In View";
            GetComponent<Renderer>().material = aggroMaterial;
        }
        
        else
        {
            isInViewText.text = $"Out of View";
            GetComponent<Renderer>().material = nonAggroMaterial;
        }
    }
    // Start is called before the first frame update
            
    // Update is called once per frame
    void Update()
    {
        EnemyAggro(); //RETURNING A FLAG TO CALL THE CHASE METHOD IN FIXED UPDATE       
    }
   
    private void FixedUpdate()
    {
        //Debug.Log(shouldChasePlayer);
        if (enemyAggrod)
        {
            StartCoroutine(DirectVisionCheck());            
        } 
        if(shouldChasePlayer || goToLastPosition)
        {
            ChasePlayer();
        }
        
    }
    bool EnemyAggro()
    {                                        
        //GETTING DIFFERENCE BETWEEN PLAYER POSITION AND CURRENT POSITION (NORMALIZED)
        directionToPlayer = (player.position - transform.position).normalized;

        //Determining the dot product
        dotProduct = Vector3.Dot(transform.forward, directionToPlayer);

        dotProductText.text = $"Dot Product: {dotProduct}";

        //IF THE PLAYER IS INSIDE THE VIEWING ANGLE AND IN RANGE
        if (dotProduct > Mathf.Cos((viewAngle * .5f) * Mathf.Deg2Rad) && Vector3.Distance(transform.position, player.position) < aggroRange)
        {
            
            isInViewText.text = $"In View";            
            GetComponent<Renderer>().material = aggroMaterial;
            return enemyAggrod = true;
        }
        else
        {
            isInViewText.text = $"Out of View";            
            GetComponent<Renderer>().material = nonAggroMaterial;
            return enemyAggrod = false;
            
        }        
    }

    //CHECKS FOR THE SHADER
    

    void ChasePlayer()
    {
        var step = rotationSpeed * Time.deltaTime;
                
        if (shouldChasePlayer)
        {
            //SETTING THE TARGET ROTATION TO ROTATE TOWARDS USING THE NORMALIZED DIFFERENCE OF PLAYER AND CURRENT POSITION 
            directionToPlayer = (player.position - transform.position).normalized;
            targetRotation = Quaternion.LookRotation(directionToPlayer);
            
            //MOVE TOWARDS PLAYER
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);


        }
        else if(goToLastPosition)
        {
            //SETTING THE TARGET DIRECTION TO ROTATE TOWARDS BASED ON THE NOMRALIZED DIFFERENE OF LAST RECORDED AND CURRENT POSITION
            Vector3 directionToLastKnownPosition = (lastRecordedPlayerPosition - transform.position).normalized;
            targetRotation = Quaternion.LookRotation(directionToLastKnownPosition);
            
            //MOVE TOWARDS PLAYERS LAST POSITION
            transform.position = Vector3.MoveTowards(transform.position, lastRecordedPlayerPosition, moveSpeed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
            Debug.Log("Going To Last Location");
            

            if (transform.position == lastRecordedPlayerPosition)
            {
                //PLAYING THE ANIMATION WHEN THE ENEMY REACHES PLAYERS LAST KNOW LOCATION
                animator.enabled = true;

                //STOPPING THE MOVEMENT ONCE THE ENEMY GETS TO THE PLAYERS LAST RECORD POSITION. LATER HE WILL LOOK AROUND FOR THEM.
                goToLastPosition = false;
                shouldChasePlayer = false;
            }
        }
        //ENEMY WILL CONTINUE TO RAYCAST AS LONG AS THE PLAYER IS DETECTED. OTHERWISE ENEMY JUST CONTINUES TO MOVE TO HIS LAST POSITION ('checkingVision' IS TOGGLED IN THE RAYCAST COROUTINE)       
        if(checkingVision)
        {
            StartCoroutine(DirectVisionCheck());
        }                                                         
    }

    IEnumerator DirectVisionCheck()
    {
        //IF THE COROUTINE ISNT ALREADY RUNNING
        if(visionCheckRunning) yield break;
        
        //SET THE COROUTINE FLAG TO RUNNING
        visionCheckRunning = true;
        
        //SETTING A TOGGLE, INSTEAD OF A TYPICAL RETURN VALIE OF TRUE OR FALSE, SO THAT THE ENEMY WILL CONTINUE TO RAYCAST ANYTIME IT IS MOVING
        //AS WELL AS ANYTIME IT IS AGGROD BY SEEING THE PLAYER ('ENEMYAGGRO' STARTS THIS COROUTINE)
        checkingVision = true;

        //COOLDOWN FOR THE RAYCAST
        yield return new WaitForSeconds(.01f);
        
        //SELECTING MASKS TO INTERACT WITH
        LayerMask layerMask = LayerMask.GetMask("Walls", "Player");
        string objectHit = null;

        //SENDING OUT A RAY TOWARDS THE PLAYER, RETURNING THE LAYERMASK THAT IT DETECTS, EITHER 'PLAYER' OR 'WALLS'
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            //CHECKING WHICH OBJECT WAS HIT
            objectHit = hit.collider.gameObject.name;
            Debug.Log($"Object Hit By Cast: {objectHit}");
            
            //Debug.Log($"Recorded Position: {lastRecordedPlayerPosition} | {objectHit}");
        }
        
        if(objectHit == "Player")
        {
            //STOPPING ANIMATION WHEN THE ENEMY CAN SEE THE PLAYER
            animator.enabled = false;

            //STORING PLAYERS POSITION ON RAYCASTHIT
            lastRecordedPlayerPosition = player.position;
            
            //START B-LINING PLAYER
            goToLastPosition = false;
            shouldChasePlayer = true;            
        }
        else
        {
            //STOP RAYCASTING TO THE PLAYERONCE THE HE IS HIDDEN.
            checkingVision = false;
            
            //START B-LINING PLAYERS LAST SEEN POSITION
            shouldChasePlayer = false;
            goToLastPosition = true;            
        }
                
        //RESETTING THE COROUTINE FLAG SO IT CAN BE RUN AGAIN.
        visionCheckRunning = false;               
    }
}
