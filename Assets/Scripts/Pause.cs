using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine.SceneManagement;

// This script allows the player to pause the game, and restart or quit the game from the pause menu.

public class Pause : MonoBehaviour
{
    public bool gameIsPaused = false;
    public GameObject pauseScreen;

    public void RestartGame()
     {
         SceneManager.LoadScene(SceneManager.GetActiveScene().name); // loads current scene
         Time.timeScale = 1.0f;
         Cursor.visible = false;
         Cursor.lockState = CursorLockMode.Locked;
     } 
     
    public void QuitGame()
    {
        Application.Quit();
    }

   public void MainMenu()
    {
        SceneManager.LoadScene("Scenes/StartScreen");
        Time.timeScale = 1.0f;
    }
   
   public void PauseGame()
   {
       if (gameIsPaused)
       {
           pauseScreen.SetActive(true);
           Time.timeScale = 0f;
           Cursor.visible = true;
           Cursor.lockState = CursorLockMode.None;
       }
       else
       {
           pauseScreen.SetActive(false);
           Time.timeScale = 1.0f;
           Cursor.visible = false;
           Cursor.lockState = CursorLockMode.Locked;
       }
           
   }
}