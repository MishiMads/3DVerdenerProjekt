using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Inventory : MonoBehaviour
{
    private InputAction interact;
    [SerializeField] private int keys;
    [SerializeField] private GameObject MainCamera;
    public TMPro.TextMeshProUGUI keyCounter;
    public GameObject winScreen;

    public void Start()
    {
        keyCounter.text = keys.ToString();
    }

    // this method is called when the player presses the interact button and allows them to pick up keys and open the door when they have all 4 keys.
    public void Interact()
    {
        // instead of using colliders and triggers, I used raycasting to detect when the player is looking at an object.
        Physics.Raycast(MainCamera.transform.position, MainCamera.transform.forward, out RaycastHit hit, 1f);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Key"))
            {
                Destroy(hit.collider.gameObject);
                keys++;
                keyCounter.text = keys.ToString(); // this updates the key counter on the screen
                
            }
            if (hit.collider.CompareTag("Door") && keys == 4)
            {
                print("Door Open");
                winScreen.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                // it took me a while to figure out how to allow the player to click buttons in the pause menu, but unlocking the cursor and making it visible did the trick.
            }
        }

    }
}