using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RaycastHelper
{
    private Camera mainCam;
    private CustomCursorInputController customCursorInputStuff;

    public RaycastHelper(CustomCursorInputController inputStuff, Camera mainCamera)
    {
        mainCam = mainCamera;
        customCursorInputStuff = inputStuff;
    }

    public Vector3 FromAToBRaycast(GameObject caster, Vector3 dir, LayerMask whatIsTarget, out GameObject objHit)
    {
        Vector3 hitPos = new Vector3();
        objHit = null;

        Ray ray = new Ray(caster.transform.position, dir);
        RaycastHit[] hits = Physics.RaycastAll(ray, 5000f, whatIsTarget);

        if (hits.Length > 0)
        {
            hits = hits.OrderBy(x => x.distance).ToArray();

            for (int i = 0; i < hits.Length; i++)
            {
                if (i == (hits.Length - 1))
                {
                    hitPos = dir * 5000f;
                }
                
                if (hits[i].collider)
                {
                    if (hits[i].collider.attachedRigidbody)
                    {
                        if (hits[i].collider.attachedRigidbody.gameObject != caster)
                        {
                            hitPos = hits[i].point;
                            objHit = hits[i].rigidbody.gameObject;

                            break;
                        }
                    }
                    else
                    {
                        if (hits[i].collider.gameObject != caster)
                        {
                            hitPos = hits[i].point;
                            objHit = hits[i].collider.gameObject;

                            break;
                        }
                    }
                }
            }
        }
        else
        {
            hitPos = dir * 5000f;
        }
        
        // if (hits.Length > 0)
        // {
        //     hits = hits.OrderBy(x => x.distance).ToArray();
        //     
        //     for (int i = 0; i < hits.Length; i++)
        //     {
        //         if (hits[i].collider)
        //         {
        //             if (hits[i].collider.transform == caster.transform) // this check never runs for some reason
        //             {
        //                 // check if object itself is ignored or this
        //                 if (i == hits.Length - 1)
        //                 {
        //                     hitPos = dir * 5000f;
        //                 }
        //
        //                 continue;
        //             }
        //             else // didnt directly hit itself or ignored obj
        //             {
        //                 // if it isnt perform other checks
        //                 if (hits[i].collider.attachedRigidbody)
        //                 {
        //                     // checks the main rigidbody (if the object has child colliders, then this is set to the parent rb
        //                     if (hits[i].collider.attachedRigidbody.gameObject == caster)
        //                     {
        //                         // if the rigidbody's gameobject is the gameobject or ignored one
        //                         if (i == hits.Length - 1)
        //                         {
        //                             hitPos = dir * 5000f;
        //                         }
        //
        //                         continue;
        //                     }
        //                     else
        //                     {
        //                         // rigidbody isnt gameobject or ignored one
        //                         //Debug.Log("set here 1");
        //
        //                         hitPos = hits[i].rigidbody.transform.position;
        //                         objHit = hits[i].rigidbody.gameObject;
        //
        //                         break;
        //                     }
        //                 } // doesnt have an rb attached 
        //                 else
        //                 {
        //                     //Debug.Log("set here 2");
        //
        //                     hitPos = hits[i].point;
        //                     objHit = hits[i].collider.gameObject;
        //
        //                     break;
        //                 }
        //             }
        //         }
        //         else
        //         {
        //             Debug.Log("Has no collider at all, how'd we even hit it?");
        //         }
        //
        //         if (i == hits.Length - 1)
        //         {
        //             hitPos = dir * 5000f;
        //         }
        //     }
        // }
        // else
        // {
        //     hitPos = dir * 5000f;
        // }

        // Debug.DrawLine(ray.origin, hitPos, Color.yellow);
        return hitPos;
    }

    public Vector3
        FromCameraToMouseRaycast(GameObject caster, LayerMask whatIsTarget,
            out GameObject objHit) // this is used by other functions too
    {
        Vector3 hitPos = new Vector3();
        Vector2 screenCursorPos = customCursorInputStuff.CustomCursorPos; // need this
        objHit = null; // perhaps make customCursorInputStuff.CustomCursorPos static since there's only one? per player

        Ray ray = mainCam.ScreenPointToRay(screenCursorPos);
        // Debug.DrawLine(ray.origin, ray.direction.normalized * 300f);
        // the ray goes through the enemy
        // Debug.Log(whatIsTarget);
        RaycastHit[] hits = Physics.RaycastAll(ray, 5000f, whatIsTarget);
        // RaycastAll or RaycastNonAlloc?

        if (hits.Length > 0)
        {
            hits = hits.OrderBy(x => x.distance).ToArray();

            // Debug.Log("-------------------------------------------");
            // foreach (var hit in hits)
            // {
            //     if (hit.collider)
            //     {
            //         if (hit.collider.attachedRigidbody != null)
            //         {
            //             Debug.Log(hit.collider.attachedRigidbody.gameObject.name);
            //         }
            //         else
            //         {
            //             Debug.Log(hit.collider.gameObject);
            //         }
            //     }
            // }
            // Debug.Log("-------------------------------------------");
            
            for (int i = 0; i < hits.Length; i++)
            {
                // Debug.Log(hits[i].collider.name);
                
                if (i == hits.Length - 1) // is this check required x1
                {
                    // Debug.Log("Or perhaps this?");
                    hitPos = mainCam.ScreenToWorldPoint(new Vector3(screenCursorPos.x, screenCursorPos.y,
                        mainCam.farClipPlane));
                }

                // if (hits[i].collider)
                // {
                //     if (hits[i].collider.attachedRigidbody != null)
                //     {
                //         if (hits[i].collider.attachedRigidbody.gameObject != null)
                //         {
                //             if (hits[i].collider.attachedRigidbody.gameObject != caster)
                //             {
                //                 Debug.Log(hits[i].collider.attachedRigidbody.gameObject.name);
                //                 break;
                //             }
                //         }
                //     }
                //     else
                //     {
                //         if (hits[i].collider.gameObject != caster)
                //         {
                //             Debug.Log(hits[i].collider.gameObject.name);
                //             break;
                //         }
                //     }
                // }

                if (hits[i].collider)
                {
                    if (hits[i].collider.attachedRigidbody != null)
                    {
                        if (hits[i].collider.attachedRigidbody.gameObject != null)
                        {
                            if (hits[i].collider.attachedRigidbody.gameObject != caster)
                            {
                                hitPos = hits[i].point;
                                objHit = hits[i].rigidbody.gameObject;
                                // Debug.Log(objHit);

                                break;
                            }
                        }
                    }
                    else
                    {
                        if (hits[i].collider.gameObject != caster)
                        {
                            hitPos = hits[i].point;
                            objHit = hits[i].collider.gameObject;
                            // Debug.Log(objHit);

                            break;
                        }
                    }

                    // if (hits[i].collider.gameObject == caster) // this check never runs for some reason
                    // {
                    //     // check if object itself is ignored or this
                    //     //Debug.Log("hit ignored obj or itself");
                    //
                    //     continue;
                    // }
                    // else // didnt directly hit itself or ignored obj
                    // {
                    //     // if it isnt perform other checks
                    //     if (hits[i].collider.attachedRigidbody != null)
                    //     {
                    //         if (hits[i].collider.attachedRigidbody.gameObject != null)
                    //         {
                    //             // checks the main rigidbody (if the object has child colliders, then this is set to the parent rb
                    //             if (hits[i].collider.attachedRigidbody.gameObject == caster)
                    //             {
                    //                 // if the rigidbody's gameobject is the gameobject or ignored one
                    //
                    //                 continue;
                    //             }
                    //             else
                    //             {
                    //                 // rigidbody isnt gameobject or ignored one
                    //                 //Debug.Log("set here 1");
                    //
                    //                 // hitPos = hits[i].rigidbody.transform.position;
                    //                 hitPos = hits[i].point;
                    //                 objHit = hits[i].rigidbody.gameObject;
                    //
                    //                 break;
                    //             }
                    //         }
                    //     }
                    //     else
                    //     {
                    //         //Debug.Log("set here 2");
                    //
                    //         hitPos = hits[i].point;
                    //         objHit = hits[i].collider.gameObject;
                    //
                    //         break;
                    //     }
                    // }
                }

                // Debug.Log(objHit);
            }
        }
        else
        {
            // Debug.Log("Is it this?");
            hitPos = mainCam.ScreenToWorldPoint(new Vector3(screenCursorPos.x, screenCursorPos.y,
                mainCam.farClipPlane));
        }
        
        return hitPos;
    }
}