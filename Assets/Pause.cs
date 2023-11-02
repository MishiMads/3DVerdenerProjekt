using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour

{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pauseButton;

     public void RestartGame()
     {
         SceneManager.LoadScene(SceneManager.GetActiveScene().name); // loads current scene
         Time.timeScale = 1.0f; 
     } 
     
    public void QuitGame()
    {
        Application.Quit();
    }

   public void MainMenu()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
        Time.timeScale = 1.0f;
    }
   
   public void PauseButton()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void ResumeButton()
    {
        Time.timeScale = 1.0f;
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
    }
}