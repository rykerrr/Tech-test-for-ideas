using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastHelper
{
    private Camera mainCam;
    private CustomCursorInputController customCursorInputStuff;
    
    public RaycastHelper(CustomCursorInputController inputStuff, Camera mainCamera)
    {
        mainCam = mainCamera;
        customCursorInputStuff = inputStuff;
    }
    
    public Vector3 FullRaycast(GameObject caster, LayerMask whatIsTarget, out GameObject objHit) // this is used by other functions too
    {
        Vector3 hitPos = new Vector3();
        Vector2 screenCursorPos = customCursorInputStuff.CustomCursorPos; // need this
        objHit = null; // perhaps make customCursorInputStuff.CustomCursorPos static since there's only one? per player

        Ray ray = mainCam.ScreenPointToRay(screenCursorPos);
        RaycastHit[] hits = Physics.RaycastAll(ray, float.MaxValue, whatIsTarget);
        // RaycastAll or RaycastNonAlloc?
        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider)
                {
                    if (hits[i].collider.transform == caster.transform) // this check never runs for some reason
                    {
                        // check if object itself is ignored or this
                        //Debug.Log("hit ignroed obj or itself");
                        if (i == hits.Length - 1)
                        {
                            hitPos = mainCam.ScreenToWorldPoint(new Vector3(screenCursorPos.x, screenCursorPos.y,
                                mainCam.farClipPlane));
                        }

                        continue;
                    }
                    else // didnt directly hit itself or ignored obj
                    {
                        // if it isnt perform other checks
                        if (hits[i].collider.attachedRigidbody)
                        {
                            // checks the main rigidbody (if the object has child colliders, then this is set to the parent rb
                            if (hits[i].collider.attachedRigidbody.gameObject == caster)
                            {
                                // if the rigidbody's gameobject is the gameobject or ignored one
                                if (i == hits.Length - 1)
                                {
                                    hitPos = mainCam.ScreenToWorldPoint(new Vector3(screenCursorPos.x,
                                        screenCursorPos.y, mainCam.farClipPlane));
                                }

                                continue;
                            }
                            else
                            {
                                // rigidbody isnt gameobject or ignored one
                                //Debug.Log("set here 1");

                                hitPos = hits[i].rigidbody.transform.position;
                                objHit = hits[i].rigidbody.gameObject;

                                break;
                            }
                        } // doesnt have an rb attached 
                        else
                        {
                            //Debug.Log("set here 2");

                            hitPos = hits[i].point;
                            objHit = hits[i].collider.gameObject;

                            break;
                        }
                    }
                }

                if (i == hits.Length - 1)
                {
                    hitPos = mainCam.ScreenToWorldPoint(new Vector3(screenCursorPos.x, screenCursorPos.y,
                        mainCam.farClipPlane));
                }
            }
        }
        else
        {
            hitPos = mainCam.ScreenToWorldPoint(new Vector3(screenCursorPos.x, screenCursorPos.y,
                mainCam.farClipPlane));
        }

        return hitPos;
    }
}
