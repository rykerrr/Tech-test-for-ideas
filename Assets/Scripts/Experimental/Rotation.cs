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
    [SerializeField] private Transform[] legPivots;
    [SerializeField] private Transform objToTilt;
    [SerializeField] private float rotationDelta = 30f;
    [SerializeField] private float tiltDelta = 45f;
    [SerializeField] private float tiltMultiplier = 7.5f;
    [SerializeField] private float legRotationDelta = 10f;

    private GameSceneSettings settings;
    private Rigidbody thisRb;

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

    private void Awake()
    {
        thisRb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        settings = GameSceneSettings.Instance;
    }

    void Update()
    {
        RotationMovement();
        Tilt();

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
            hitPos = settings.RaycastFunctions.FromCameraToMouseRaycast(gameObject, whatIsTarget,
                out var objHit); // where would fullraycast go to? what script?
            // Debug.Log(objHit);
        }

        // this handles the rotation
        Vector3 dir = hitPos - transform.position;

        transform.forward = Vector3.MoveTowards(transform.forward, new Vector3(dir.x, 0f, dir.z),
            rotationDelta * Time.deltaTime);
    }

    private void Tilt()
    {
        // But also, i could just tilt it towards its velocity
        // Problem is, the velocity needs to be local, if it's local to the body it tilts in the forwards direction
        // of the body
        // If its local to the camera, it tilts towards the direction of the input as the body could be
        // looking left, but you're moving forwards on the world, instead of tilting the player it's local forward,
        // it tilts it local right
        // Is this what i want? Or?
        // Also clampAngle doesn't function as it should here, probably because the tilt gets to a deadlock where it can't
        // return to 0f anymore
        Vector3 velLocal = transform.InverseTransformDirection(thisRb.velocity);
        Vector3 velLocalNormalized = velLocal.normalized;

        float
            tiltX = velLocalNormalized.z *
                    tiltMultiplier; //Mathf.MoveTowards(objToTilt.localEulerAngles.x, velLocal.z, 0.5f).ClampAngle(-30f, 30f);
        float
            tiltZ = -1 * velLocalNormalized.x *
                    tiltMultiplier; //Mathf.MoveTowards(objToTilt.localEulerAngles.z, velLocal.x, 0.5f).ClampAngle(-30f, 30f);
        // objToTilt.localEulerAngles = new Vector3(tiltX, objToTilt.localEulerAngles.y, tiltZ);

        if (thisRb.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 vel = thisRb.velocity.normalized;
            float heading = Mathf.Atan2(vel.x, vel.z);

            foreach (var legPivot in legPivots)
            {
                Quaternion rotToLeg = Quaternion.Euler(0f, heading * Mathf.Rad2Deg, 0f);
                legPivot.rotation =
                    Quaternion.RotateTowards(legPivot.rotation, rotToLeg, Time.deltaTime * legRotationDelta);
            }
        }
        else
        {
            foreach (var legPivot in legPivots)
            {
                legPivot.rotation = legPivot.rotation;
            }
        }

        // this literally fixes all rotation problems in the world :DDDDDDDD
        Quaternion rotToTilt = Quaternion.Euler(tiltX, objToTilt.localEulerAngles.y, tiltZ);
        objToTilt.localRotation =
            Quaternion.RotateTowards(objToTilt.localRotation, rotToTilt, tiltDelta * Time.deltaTime);
    }
}
#pragma warning restore 0649