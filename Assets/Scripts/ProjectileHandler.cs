using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    public LayerMask layerMask;   

    public float shotSpeed;
    bool shotStopped;
    private Vector3 velocity;

    public float shotTime;
    public float dampingFactor = 0.95f;
    private float timer;

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        // Reset timer and initialize direction
        timer = 0f;
        velocity = transform.forward * shotSpeed;
    }

    // Update is called once per frame     
    private void Update()
    {
        if(!shotStopped)
        {
            MoveProjectile();
        }
                               
        // Increment timer and disable object if its lifetime expires
        timer += Time.deltaTime;
        if (timer > shotTime)
        {
            gameObject.SetActive(false);
        }        
    }

    void MoveProjectile()
    {
        Ray ray = new Ray(transform.position, velocity);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, shotSpeed * Time.deltaTime, layerMask))
        {
            velocity = Vector3.Reflect(velocity, hit.normal);

            transform.position = hit.point + velocity.normalized * 0.1f;
        }
        else
        {           
            transform.position += velocity * Time.deltaTime;

            velocity *= dampingFactor; 
            if (velocity.magnitude <= 0.1f)  
            {
                shotStopped = true;
            }
        }
    }
    
}
