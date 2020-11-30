using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class RaycastTest : MonoBehaviour
{
    [SerializeField] private Transform ignoreObject;
    [SerializeField] private LineRenderer rayLineRenderer;
    [SerializeField] private List<Transform> targets;

    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
        rayLineRenderer.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
    }

    private void Update()
    {
        Fire();
    }

    private void Fire()
    {
        Transform hitObj = null;
        Vector3 hitPos = new Vector3();
        Vector2 curMousePos = Input.mousePosition;
        Ray ray = mainCam.ScreenPointToRay(curMousePos);
        RaycastHit[] hits = Physics.RaycastAll(ray, float.MaxValue);

        targets.Clear();
        foreach(RaycastHit hit in hits)
        {
            targets.Add(hit.transform);
        }

        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider)
                {
                    if (hits[i].collider.transform == transform || hits[i].collider.transform == ignoreObject.transform) // this check never runs for some reason
                    { // check if object itself is ignored or this
                        Debug.Log("hit ignroed obj or itself");
                        if(i == hits.Length - 1)
                        {
                            hitPos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCam.farClipPlane));
                        }

                        continue;
                    }
                    else // didnt directly hit itself or ignored obj
                    { // if it isnt perform other checks
                        if(hits[i].collider.attachedRigidbody)
                        { // checks the main rigidbody (if the object has child colliders, then this is set to the parent rb
                            if(hits[i].collider.attachedRigidbody.gameObject == gameObject || hits[i].rigidbody.gameObject == ignoreObject)
                            { // if the rigidbody's gameobject is the gameobject or ignored one
                                if (i == hits.Length - 1)
                                {
                                    hitPos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCam.farClipPlane));
                                }

                                continue;
                            }
                            else
                            { // rigidbody isnt gameobject or ignored one
                                Debug.Log("set here 1");

                                hitPos = hits[i].rigidbody.transform.position;
                                hitObj = hits[i].transform;
                                break;
                            }
                        } // doesnt have an rb attached 
                        else
                        {
                            Debug.Log("set here 2");

                            hitPos = hits[i].point;
                            hitObj = hits[i].transform;
                            break;
                        }
                    }
                }

                if (i == hits.Length - 1)
                {
                    hitPos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCam.farClipPlane));
                }
            }
        }
        else
        {
            hitPos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCam.farClipPlane));
        }

        Debug.Log("" + hitObj);
        rayLineRenderer.SetPositions(new Vector3[] { transform.position, hitPos });
    }
}
#pragma warning restore 0649