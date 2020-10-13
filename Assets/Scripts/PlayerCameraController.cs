using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0414
#pragma warning disable 0649
public class PlayerCameraController : MonoBehaviour
{
    private const float LOW_LIMIT = 0.0f;
    private const float HIGH_LIMIT = 85.0f;

    [SerializeField] private Transform cameraPosObj;
    [SerializeField] private Transform target;
    [SerializeField] private Transform obj;
    [SerializeField] private Transform rotor;
    [SerializeField] private float dist = 3f;
    [SerializeField] private float mouseSensitivityX = 4.0f;
    [SerializeField] private float mouseSensitivityY = 2.0f;

    [HideInInspector] public Vector2 actualCursorPos;

    private Camera mainCam;
    private Vector3 offset;
    private Vector2 lastCursorPos;
    private Vector2 lastRotationCursorPos;

    [SerializeField] private float x;
    [SerializeField] private float y;

    private int selectedShoulder = 0;

    private void Awake()
    {
        mainCam = Camera.main;
        offset = obj.position;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                lastCursorPos = Input.mousePosition;
            }

            RotateCamera();
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                // would set the mousepos back but CANT!
                offset = obj.position;
            }

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }

            obj.position = target.position + offset;
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            actualCursorPos = lastCursorPos;
        }
        else
        {
            actualCursorPos = Input.mousePosition;
        }
    }

    private void LateUpdate()
    {
        //mainCam.transform.position = target.position;
    }

    private void RotateCamera()
    {
        // pause mouse, stop it from moving
        // rotate camera in direction where mouse would be moving towards
        Cursor.lockState = CursorLockMode.Locked;

        x = Input.GetAxis("Mouse X");
        y = Input.GetAxis("Mouse Y");

        Vector2 mousePos = new Vector2(x, y);
        lastRotationCursorPos = mousePos;

        obj.RotateAround(target.position, new Vector3(mousePos.y, 0f, mousePos.x), mousePos.magnitude);
        obj.position = (obj.position - target.position).normalized * 5f + target.position;
    }
}
#pragma warning restore 0649
#pragma warning restore 0414