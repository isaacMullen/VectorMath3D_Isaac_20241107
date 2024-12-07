using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishDetector : MonoBehaviour
{
    public DetectionManager detectionManager;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (detectionManager.detectionBar.value == detectionManager.detectionBar.maxValue)
        {
            LoadBySceneName("EndScreenLost");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            Debug.Log("EndScreen To Load");

            if (detectionManager.detectionBar.value != detectionManager.detectionBar.maxValue)
            {
                LoadBySceneName("EndScreen");
            }            
        }
    }

    public void LoadBySceneName(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.Log("No Such Scene");
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
}
