using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

#pragma warning disable 0649
public class CustomCursorRaycastTest : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform originObject;
    [SerializeField] private Transform indicatorPrefab;
    [SerializeField] private Transform indicator;
    [SerializeField] private CursorMenuController cursorMenu;
    [SerializeField] private wAceCursor wAceCursor;
    [SerializeField] private CustomCursorInputController cursorController;
    [SerializeField] private RectTransform cursorParentToMove;
    [SerializeField] private Image customCursor;
    [SerializeField] private Canvas cursorCanvas;
    [SerializeField] private float fireDelay = 0.1f;
    
    private Camera mainCam;
    private Vector2 customCursorPos;
    private Vector2 cursorOffset;
    private Vector2 mousePos;

    private float fireTimer;

    private bool moveCursor = true;

    private void Awake()
    {
        mainCam = Camera.main;
        
        indicator = Instantiate(indicatorPrefab, firePoint.position, Quaternion.identity);
    }

    private void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            RaycastTowardsCustomCursor();
        }
    }

	
	// used in firing, dont need this normally
    private void RaycastTowardsCustomCursor()
    {
        Vector3 hitPos = new Vector3();
        Vector2 screenCursorPos = cursorController.CustomCursorPos;

        Ray ray = mainCam.ScreenPointToRay(screenCursorPos);
        //Physics.Raycast(ray, out RaycastHit hit); // change to raycastall
        RaycastHit[] hits = Physics.RaycastAll(ray, float.MaxValue);

        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider)
                {
                    if (hits[i].collider.transform == transform) // this check never runs for some reason
                    { // check if object itself is ignored or this
                        //Debug.Log("hit ignroed obj or itself");
                        if (i == hits.Length - 1)
                        {
                            hitPos = mainCam.ScreenToWorldPoint(new Vector3(screenCursorPos.x, screenCursorPos.y, mainCam.farClipPlane));
                        }

                        continue;
                    }
                    else // didnt directly hit itself or ignored obj
                    { // if it isnt perform other checks
                        if (hits[i].collider.attachedRigidbody)
                        { // checks the main rigidbody (if the object has child colliders, then this is set to the parent rb
                            if (hits[i].collider.attachedRigidbody.gameObject == gameObject)
                            { // if the rigidbody's gameobject is the gameobject or ignored one
                                if (i == hits.Length - 1)
                                {
                                    hitPos = mainCam.ScreenToWorldPoint(new Vector3(screenCursorPos.x, screenCursorPos.y, mainCam.farClipPlane));
                                }

                                continue;
                            }
                            else
                            { // rigidbody isnt gameobject or ignored one
                                //Debug.Log("set here 1");

                                hitPos = hits[i].rigidbody.transform.position;
                                break;
                            }
                        } // doesnt have an rb attached 
                        else
                        {
                            //Debug.Log("set here 2");

                            hitPos = hits[i].point;
                            break;
                        }
                    }
                }

                if (i == hits.Length - 1)
                {
                    hitPos = mainCam.ScreenToWorldPoint(new Vector3(screenCursorPos.x, screenCursorPos.y, mainCam.farClipPlane));
                }
            }
        }
        else
        {
            hitPos = mainCam.ScreenToWorldPoint(new Vector3(screenCursorPos.x, screenCursorPos.y, mainCam.farClipPlane));
        }

        // Raycasting from target to where the camera hit now to check if there's obstruction between the target and origin
        Physics.Raycast(new Ray(originObject.position, (hitPos - originObject.position).normalized), out RaycastHit hit, float.MaxValue);
        Debug.DrawLine(originObject.position, hitPos);

        if(hit.collider)
        { // kinda fucked in case it hits itself or anything around it...should use raycast all
            hitPos = hit.point;
        }

        Indicate(hitPos);
    }

    private void Indicate(Vector3 hitPos)
    {
        if (indicator.gameObject.activeSelf)
        {
            indicator.position = firePoint.position;
            indicator.forward = (hitPos - firePoint.position).normalized;
            indicator.localScale = new Vector3(indicator.localScale.x, indicator.localScale.y, (hitPos - firePoint.position).magnitude);

            if (Time.time > fireTimer)
            {
                if(wAceCursor.gameObject.activeSelf)
                {
                    wAceCursor.Fire();
                }

                fireTimer = Time.time + fireDelay;
            }
        }
    }

    public void CustomCursorUsage(bool use)
    {
        wAceCursor.gameObject.SetActive(!use);
        customCursor.gameObject.SetActive(use);

        if(cursorMenu.NeutralCursor)
        {
            customCursor.sprite = cursorMenu.NeutralCursor.GetCursorSprite;
            customCursor.color = cursorMenu.NeutralCursor.GetCursorColor; // have to deal with IFF as well
        }
    }
}
#pragma warning restore 0649