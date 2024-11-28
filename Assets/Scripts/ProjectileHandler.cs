using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    public LayerMask layerMask;

    private Rigidbody rb;

    public float shotSpeed;
    private Vector3 currentDirection;

    public float shotTime;
    private float timer;

    private void Start()
    {
        // Initialize Rigidbody and set collision detection mode to avoid missed collisions
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void OnEnable()
    {
        // Reset timer and initialize direction
        timer = 0f;
        currentDirection = transform.forward.normalized;
    }

    // Update is called once per frame     
    private void Update()
    {
        // Increment timer and disable object if its lifetime expires
        timer += Time.deltaTime;
        if (timer > shotTime)
        {
            gameObject.SetActive(false);
        }

        // Move projectile while respecting Unity's physics system
        rb.MovePosition(transform.position + shotSpeed * Time.deltaTime * currentDirection);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Log collision events for debugging
        Debug.Log($"Collision with: {collision.gameObject.name}");

        // Reflect direction if collision occurs with an object tagged as "Wall"
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Wall Collision Detected");

            // Get the collision's contact point and surface normal
            ContactPoint contact = collision.contacts[0];
            Vector3 surfaceNormal = contact.normal;

            // Reflect the current direction based on the surface normal
            currentDirection = Vector3.Reflect(currentDirection, surfaceNormal).normalized;
        }
    }
}
