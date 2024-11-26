using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    public float shotSpeed;        
      
    public float shotTime;
    private float timer;

    private void OnEnable()
    {
        timer = 0f;
    }

    // Update is called once per frame     
    private void Update()
    {
        timer += Time.deltaTime;

        if( timer > shotTime )
        {
            gameObject.SetActive( false );
        }
                
        transform.Translate(shotSpeed * Time.deltaTime * Vector3.forward);

    }    
}
