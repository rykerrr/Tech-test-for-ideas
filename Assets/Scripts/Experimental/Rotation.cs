using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using EventSystem = UnityEngine.EventSystems.EventSystem;

// overheat system time :D
#pragma warning disable 0649
public class Rotation : MonoBehaviour // Rotation should be handled here
{
    // but where the fuck is rotation
    // also pretty sure i need ray casting
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private Transform gun; // what is le gun!!!!!!!!
    [SerializeField] private float rotationDelta = 30f;

    private GameSceneSettings settings;

    // private void Awake()
    // {
    //     mainCam = Camera.main;
    //     thisRb = GetComponent<Rigidbody>();
    //     
    //     indicator = Instantiate(rayPrefab, Vector3.zero, Quaternion.identity);
    //     Vector3 localScale = indicator.localScale;
    //     
    //     indicator.localScale = new Vector3(localScale.x * 0.5f, localScale.y * 0.5f,
    //         localScale.z);
    //     indicator.parent = rayContainer;
    //     
    //     overheatLight.enabled = false;
    //     indicator.GetComponentInChildren<MeshRenderer>().material = indicatorMaterial;
    // }
    
    private void Start()
    {
        settings = GameSceneSettings.Instance;
    }

    void Update()
    {
        RotationMovement();
        
        
        // if (Time.time > heatCoolTimer)
        // {
        //     if (heatPercentage <= 0f)
        //     {
        //         heatPercentage = 0f;
        //         hasOverheated = false;
        //     }
        //     else
        //     {
        //         if (hasOverheated)
        //         {
        //             heatPercentage -= 0.5f * overheatMultiplier;
        //         }
        //         else
        //         {
        //             heatPercentage -= 0.5f;
        //         }
        //     }
        // }
        // else
        // {
        //     //Debug.Log("" + (heatCoolTimer - Time.time));
        // }
        //
        // if (Keyboard.current.qKey.wasPressedThisFrame)
        // {
        //     indicator.gameObject.SetActive(!indicator.gameObject.activeSelf);
        // }
    }

    private void RotationMovement()
    {
        Vector3 hitPos = Vector3.zero; // Set it to Vector3.zero or move it to an object-specific variable
        bool pointerIsOverUi = EventSystem.current.IsPointerOverGameObject(0);
        // is it over an event system (ui) object

        if (!pointerIsOverUi)
        {
            hitPos = settings.RaycastFunctions.FromCameraToMouseRaycast(gameObject, whatIsTarget, out var objHit); // where would fullraycast go to? what script?
        }

        // this handles the rotation
        Vector3 dir = hitPos - transform.position;
        Vector3 originDir = hitPos - gun.position;

        transform.forward = Vector3.MoveTowards(transform.forward, new Vector3(dir.x, 0f, dir.z),
            rotationDelta * Time.deltaTime);
    }
}
#pragma warning restore 0649