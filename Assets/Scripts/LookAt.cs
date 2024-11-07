using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class LookAt : MonoBehaviour
{
    public Transform toLookAt;
    public TextMeshProUGUI dotProductText;
    public TextMeshProUGUI isInViewText;

    //Materials to switch between when in view
    public Material nonAggroMaterial;
    public Material aggroMaterial;
   
    Vector3 edgeDirection;
    Vector3 negativeEdgeDirection;
        
    bool chasingPlayer;
    public float speed;

    public float viewAngle = 60f;

    float dotProduct;


    private void OnDrawGizmos()
    {
        //Drawing a line to the player we are looking at
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, toLookAt.position);

        //Getting the direction from the enemy (looking object) to the player (object to look at)
        Vector3 directionToObject = (toLookAt.position - transform.position).normalized;

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
        Vector3 directionToObject = (toLookAt.position - transform.position).normalized;

        //Determining the dot product
        dotProduct = Vector3.Dot(transform.forward, directionToObject);

        dotProductText.text = $"Dot Product: {dotProduct}";

        //Total Viewing Angle

        //Getting the directions to the edges of the viewpoint
        Vector3 edgeDirection = Quaternion.Euler(0, (viewAngle / 2), 0) * transform.forward;
        Vector3 negativeEdgeDirection = Quaternion.Euler(0, (-viewAngle / 2), 0) * transform.forward;
        
        if (dotProduct > Mathf.Cos((viewAngle * .5f) * Mathf.Deg2Rad))
        {
            isInViewText.text = $"In View";
            chasingPlayer = true;
            GetComponent<Renderer>().material = aggroMaterial;
        }
        else
        {
            isInViewText.text = $"Out of View";
            chasingPlayer = false;  
            GetComponent<Renderer>().material = nonAggroMaterial;
        }

        if(chasingPlayer)
        {
            transform.position = Vector3.MoveTowards(transform.position, toLookAt.position, speed);
        }
        
    }
}
