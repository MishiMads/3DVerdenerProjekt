using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;


public class MainMenuScript : MonoBehaviour
{
    public void SinglePlayer()
    {
        SceneManager.LoadScene("Scenes/SampleScene");
    }
    
    public void Multiplayer()
    {
        SceneManager.LoadScene("Scenes/MultiPlayer");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
 }