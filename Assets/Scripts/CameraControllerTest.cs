using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private CustomCursorController customCursorStuff;
    [SerializeField] private float followDistance = 5.0f;
    [SerializeField] private float mouseSensitivityX = 4.0f;
    [SerializeField] private float mouseSensitivityY = 2.0f;
    [SerializeField] private float scrollSensitivity = 2f;
    [SerializeField] private float heightOffset = 0.5f;

    //private variables are hidden in editor
    private bool isPaused = false;
    private float zoom = (MAX_ZOOM_LIMIT - MIN_ZOOM_LIMIT) / 2f;
    private Vector3 offsetVector;


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
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            //if we are not paused, get the mouse movement and adjust the camera
            //position and rotation to reflect this movement around player
            CustomCursorController.canCustomCursorMove = false;
            Vector2 cameraMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            //first we place the camera at the position of the player + height offset
            theCamera.transform.position = gameObject.transform.position + new Vector3(0, heightOffset, 0);

            //next we adjust the rotation based on the captured mouse movement
            //we clamp the pitch (X angle) of the camera to avoid flipping
            //we also adjust the values to account for mouse sensitivity settings
            theCamera.transform.eulerAngles = new Vector3(
                ClampAngle(theCamera.transform.eulerAngles.x + cameraMovement.y * mouseSensitivityY, LOW_LIMIT, HIGH_LIMIT),
                theCamera.transform.eulerAngles.y + cameraMovement.x * mouseSensitivityX, 0); // figure out a way to clamp angles yet again

            //then we move out to the desired follow distance
            theCamera.transform.position -= theCamera.transform.forward * zoom;
        }
        else if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
        {
            CustomCursorController.canCustomCursorMove = true;
            offsetVector = theCamera.transform.position - transform.position;
        }
        else
        {
            if (CustomCursorController.canCustomCursorMove == false)
            {
                CustomCursorController.canCustomCursorMove = true;
            }

            theCamera.transform.position = transform.position + offsetVector;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            //if (Cursor.lockState == CursorLockMode.Locked)
            //{
            //    Cursor.lockState = CursorLockMode.None;
            //}
            //else if (Cursor.lockState == CursorLockMode.None)
            //{
            //    Cursor.lockState = CursorLockMode.Locked;
            //}
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            zoom = Mathf.Clamp(zoom + Input.mouseScrollDelta.y * scrollSensitivity, MIN_ZOOM_LIMIT, MAX_ZOOM_LIMIT);
            offsetVector = offsetVector.normalized * zoom;
        }
    }

    private float ClampAngle(float angle, float min, float max)
    {
        // method 2

        angle = Mathf.Repeat(angle, 360);
        min = Mathf.Repeat(min, 360);
        max = Mathf.Repeat(max, 360);
        bool inverse = false;
        var tmin = min;
        var tangle = angle;
        if (min > 180)
        {
            inverse = !inverse;
            tmin -= 180;
        }
        if (angle > 180)
        {
            inverse = !inverse;
            tangle -= 180;
        }
        var result = !inverse ? tangle > tmin : tangle < tmin;
        if (!result)
            angle = min;

        inverse = false;
        tangle = angle;
        var tmax = max;
        if (angle > 180)
        {
            inverse = !inverse;
            tangle -= 180;
        }
        if (max > 180)
        {
            inverse = !inverse;
            tmax -= 180;
        }

        result = !inverse ? tangle < tmax : tangle > tmax;
        if (!result)
            angle = max;
        return angle;

        // method 1

        //if (angle > 180) angle = 360 - angle;
        //Debug.Log(angle + " | " + min + " | " + max);
        //angle = Mathf.Clamp(angle, min, max);
        //Debug.Log(angle + " | " + min + " | " + max);
        //if (angle < 0) angle = 360 + angle;
        //Debug.Log(angle + " | " + min + " | " + max);

        //Debug.Log("--------------------------------");

        // return angle;
    }
}
#pragma warning restore 0649
#pragma warning restore 0414