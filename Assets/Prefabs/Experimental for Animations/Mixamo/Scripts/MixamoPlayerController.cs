using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class MixamoPlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator thisAnim;
    private Rigidbody thisRb;

    private Vector3 movementVector = Vector3.zero;
    private float movMult = 1f;

    private void Awake()
    {
        thisAnim = GetComponent<Animator>();
        thisRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        Keyboard kCurrent = Keyboard.current;
        
        float right = kCurrent.dKey.isPressed ? 1 : kCurrent.aKey.isPressed ? -1 : 0;
        float forward = kCurrent.wKey.isPressed ? 1 : kCurrent.sKey.isPressed ? -1 : 0;
        
        if (kCurrent.leftShiftKey.isPressed)
        {
            movMult = 2.5f;
        }
        else
        {
            movMult = 1f;
        }

        movementVector = new Vector3(right, 0f, forward);
        
        float animMoveSpeed = movementVector.sqrMagnitude * movMult;
        
        // Moving
        if (movementVector.magnitude > 0)
        {
            movementVector.Normalize();
            movementVector *= movMult * Time.deltaTime;
            transform.Translate(movementVector, Space.World);
        }

        // Animating
        float velocityZ = Vector3.Dot(movementVector.normalized, transform.forward);
        float velocityX = Vector3.Dot(movementVector.normalized, transform.right);
        
        thisAnim.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);
        thisAnim.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
    }

    // private void FixedUpdate()
    // {
    //     Vector3 movVector = movementVector * movMult * Time.fixedTime;
    //     
    //     thisRb.velocity = Vector3.MoveTowards(thisRb.velocity, movVector, 1f);
    // }
}
