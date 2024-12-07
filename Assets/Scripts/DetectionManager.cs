using UnityEngine;
using UnityEngine.UI;

public class DetectionManager : MonoBehaviour
{
    public Slider detectionBar;   // Reference to the UI slider
    public float maxDetectionValue = 100f;  // Maximum detection value
    public float currentDetectionValue = 0f; // Current detection value
    public float detectionSpeed = 5f; // Speed at which detection increases/decreases

    private void Start()
    {
        detectionBar.maxValue = maxDetectionValue;
    }

    private void Update()
    {
        // You can increase or decrease detection based on conditions here
        // For example, let's increase detection based on time for testing
        //currentDetectionValue += Time.deltaTime * detectionSpeed;
        currentDetectionValue = Mathf.Clamp(currentDetectionValue, 0, maxDetectionValue);

        // Update the UI slider
        detectionBar.value = currentDetectionValue;
    }

    // Method to reset detection value when needed
    public void ResetDetection()
    {
        currentDetectionValue = 0f;
        detectionBar.value = currentDetectionValue;
    }
}
