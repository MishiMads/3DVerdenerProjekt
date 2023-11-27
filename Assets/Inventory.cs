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

    public void Interact()
    {
        Physics.Raycast(MainCamera.transform.position, MainCamera.transform.forward, out RaycastHit hit, 1f);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Key"))
            {
                Destroy(hit.collider.gameObject);
                keys++;
                keyCounter.text = keys.ToString();
                
            }
            if (hit.collider.CompareTag("Door") && keys == 4)
            {
                print("Door Open");
                winScreen.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

    }
}