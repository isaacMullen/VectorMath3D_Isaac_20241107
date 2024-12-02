using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public float gravityForce;
    public float sphereRadius;
    public float maxCheckDistance;
    public LayerMask groundLayer;

    bool gravityEnabled;

    private Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        velocity = Vector3.zero;
        gravityEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(gravityEnabled)
        {
            velocity.y -= gravityForce * Time.deltaTime;
        }
        

        RaycastHit hit;

        if(Physics.SphereCast(transform.position, sphereRadius, Vector3.down, out hit, maxCheckDistance, groundLayer))
        {
            velocity.y = 0;
            transform.position = new Vector3(transform.position.x, hit.point.y + sphereRadius, transform.position.z);
            gravityEnabled = false;
        }        
        
        transform.position += velocity * Time.deltaTime;
    }
}
