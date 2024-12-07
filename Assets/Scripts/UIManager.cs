using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject instructionPanel;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowInstructions()
    {
        startPanel.SetActive(false);
        instructionPanel.SetActive(true);
    }

    public void StartGame()
    {
        startPanel.SetActive(false);
        instructionPanel.SetActive(false);
        SceneManager.LoadScene("SampleScene");
    }
}
