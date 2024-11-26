using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;

    public float mouseSensitivity;

    public Transform player;
    public Transform launchPoint;
    public CharacterController controller;

    private float xRotation;

    public ObjectPooler pooler;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
        
        //MOVEMENT LOGIC
        float movementX = Input.GetAxis("Horizontal");
        float movementY = Input.GetAxis("Vertical");

        Vector3 move = transform.TransformDirection(movementX, 0, movementY);

        controller.SimpleMove(move * speed);

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); ;

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.Rotate(Vector3.up, mouseX);
    }

    void Shoot()
    {
        GameObject projectile = pooler.GetPooledObject();

        projectile.transform.position = launchPoint.position;
        projectile.transform.rotation = launchPoint.rotation;

        projectile.SetActive(true);
    }
}
