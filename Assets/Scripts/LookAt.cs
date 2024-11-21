using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class LookAt : MonoBehaviour
{
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

    public float viewAngle = 60f;
    public float aggroRange;
    
    bool enemyAggrod;
    bool shouldChasePlayer;
    bool goToLastPosition;
    bool visionCheckOnCooldown;
    bool visionCheckRunning;

    float dotProduct;


    private void OnDrawGizmos()
    {
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
        if (dotProduct > Mathf.Cos((viewAngle * .5f) * Mathf.Deg2Rad))
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
        Debug.Log(shouldChasePlayer);
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

    void ChasePlayer()
    {        
        
        if(shouldChasePlayer)
        {
            //MOVE TOWARDS PLAYER
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed);
        }
        else if(goToLastPosition)
        {
            //MOVE TOWARDS PLAYERS LAST POSITION
            transform.position = Vector3.MoveTowards(transform.position, lastRecordedPlayerPosition, moveSpeed);            
            
            if (transform.position == lastRecordedPlayerPosition)
            {                
                //STOPPING THE MOVEMENT ONCE THE ENEMY GETS TO THE PLAYERS LAST RECORD POSITION. LATER HE WILL LOOK AROUND FOR THEM.
                goToLastPosition = false;
                shouldChasePlayer = false;
            }
        }

                                 
    }

    IEnumerator DirectVisionCheck()
    {
        //IF THE COROUTINE ISNT ALREADY RUNNING
        if(visionCheckRunning) yield break;
        
        //SET THE COROUTINE FLAG TO RUNNING
        visionCheckRunning = true;
        
        //COOLDOWN FOR THE RAYCAST
        yield return new WaitForSeconds(.1f);
        
        //SELECTING MASKS TO INTERACT WITH
        LayerMask layerMask = LayerMask.GetMask("Walls", "Player");
        string objectHit = null;

        //SENDING OUT A RAY TOWARDS THE PLAYER, RETURNING THE LAYERMASK THAT IT DETECTS, EITHER 'PLAYER' OR 'WALLS'
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            //CHECKING WHICH OBJECT WAS HIT
            objectHit = hit.collider.gameObject.name;
            
            //Debug.Log($"Recorded Position: {lastRecordedPlayerPosition} | {objectHit}");
        }
        
        if(objectHit == "Player")
        {            
            //STORING PLAYERS POSITION ON RAYCASTHIT
            lastRecordedPlayerPosition = player.position;
            
            //STARTING B-LINING PLAYER
            goToLastPosition = false;
            shouldChasePlayer = true;
        }
        else
        {
            //START B-LINING PLAYERS LAST SEEN POSITION
            shouldChasePlayer = false;
            goToLastPosition = true;
        }
        
        //Debug.Log(objectHit);
        //RESETTING THE COROUTINE FLAG SO IT CAN CONTINUE TO RUN.
        visionCheckRunning = false;               
    }
}
