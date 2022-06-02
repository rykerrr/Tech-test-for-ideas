using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

#pragma warning disable 0649
#pragma warning disable 0414
public class CameraControllerTest : MonoBehaviour
{
    //define some constants
    private const float LOW_LIMIT = -60.0f;
    private const float HIGH_LIMIT = 60.0f;
    private const float MIN_ZOOM_LIMIT = 3f;
    private const float MAX_ZOOM_LIMIT = 10f;

    //these will be available in the editor
    [SerializeField] private GameObject theCamera;
    [SerializeField] private CustomCursorInputController customCursorInputStuff;
    [SerializeField] private float followDistance = 5.0f;
    [SerializeField] private float mouseSensitivityX = 4.0f;
    [SerializeField] private float mouseSensitivityY = 2.0f;
    [SerializeField] private float scrollSensitivity = 2f;
    [SerializeField] private float heightOffset = 0.5f;

    //private variables are hidden in editor
    private bool isPaused = false;
    private float zoom = (MAX_ZOOM_LIMIT - MIN_ZOOM_LIMIT) / 2f;
    private Vector3 offsetVector;
    
    private MouseHelperStuff mouseHelper;

    private void Awake()
    {
        mouseHelper = new MouseHelperStuff();
    }

    void Start()
    {
        //place the camera and set the forward vector to match player
        theCamera.transform.forward = gameObject.transform.forward;
        theCamera.transform.position = transform.position + new Vector3(0f, heightOffset, 0f);
        theCamera.transform.position -= theCamera.transform.forward * zoom;
        offsetVector = theCamera.transform.position - transform.position;
         // Cursor.lockState = CursorLockMode.Locked;

         //hide the cursor and lock the cursor to center
    }

    void Update()
    {
        if (Mouse.current.middleButton.isPressed || Mouse.current.rightButton.isPressed)
        {
            //if we are not paused, get the mouse movement and adjust the camera
            //position and rotation to reflect this movement around player
            Vector2 cameraMovement = Mouse.current.delta.ReadValue();

            //first we place the camera at the position of the player + height offset
            theCamera.transform.position = gameObject.transform.position + new Vector3(0, heightOffset, 0);

            //next we adjust the rotation based on the captured mouse movement
            //we clamp the pitch (X angle) of the camera to avoid flipping
            //we also adjust the values to account for mouse sensitivity settings
            float angleToClamp = theCamera.transform.eulerAngles.x + cameraMovement.y * mouseSensitivityY;
            theCamera.transform.eulerAngles = new Vector3(
                angleToClamp.ClampAngle(LOW_LIMIT, HIGH_LIMIT),
                theCamera.transform.eulerAngles.y + cameraMovement.x * mouseSensitivityX, 0); // figure out a way to clamp angles yet again

            //then we move out to the desired follow distance
            theCamera.transform.position -= theCamera.transform.forward * zoom;
        }
        else if (Mouse.current.middleButton.wasReleasedThisFrame || Mouse.current.rightButton.wasReleasedThisFrame)
        {
            customCursorInputStuff.canCustomCursorMove = true;
            offsetVector = theCamera.transform.position - transform.position;
        }
        else
        {
            theCamera.transform.position = transform.position + offsetVector;
        }

        float scrollDelta = Mouse.current.scroll.ReadValue().y;
        
        if (scrollDelta != 0f)
        {
            zoom = Mathf.Clamp(zoom + scrollDelta * scrollSensitivity * Time.deltaTime, MIN_ZOOM_LIMIT, MAX_ZOOM_LIMIT);
            offsetVector = offsetVector.normalized * zoom;
        }
    }

    private void OnEnable()
    {
        mouseHelper.Enable();
    }

    private void OnDisable()
    {
        mouseHelper.Disable();
    }
}
#pragma warning restore 0649
#pragma warning restore 0414