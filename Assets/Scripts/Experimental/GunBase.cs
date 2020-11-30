using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using EventSystem = UnityEngine.EventSystems.EventSystem;

public abstract class GunBase : MonoBehaviour
{
    [Header("Set on initialize")] [SerializeField]
    private wAceCursor aceCursor;

    [SerializeField] private CustomCursorInputController customCursorInputStuff;
    [SerializeField] private CursorIdentifyFriendOrFoe cursorVisualStuff;
    [SerializeField] private Transform rayContainer;
    [SerializeField] private RayType currentRayType;

    [FormerlySerializedAs("whatIsTargettable")] [Header("Options")] [SerializeField]
    private LayerMask whatIsTarget;

    [SerializeField] private MeshRenderer ammoOverheatIndicator;
    [SerializeField] private MeshRenderer sightFireDelayIndicator;
    [SerializeField] private Material indicatorMaterial;
    [SerializeField] private Light overheatLight;
    [SerializeField] private Transform gun;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform rayPrefab;
    [SerializeField] private Transform rayGunFireEffectPrefab;
    [SerializeField] private float rayLifeTime = 0.06f;
    [SerializeField] private float rayHoleLifetime = 0.1f;
    [SerializeField] private float fireDelay = 0.1f;
    [SerializeField] private float fancyRaySpeedDelta = 30f;
    [SerializeField] private float overheatMultiplier = 0.9f;
    [SerializeField] private float heatCoolDelay = 1f;

    public LayerMask WhatIsTarget => whatIsTarget;

    private bool[] indicatorCoroutine = new bool[2]; // 0 for mag, 1 for sight fire delay
    private Transform indicator;
    private GameSceneSettings settings;
    private Camera mainCam;
    private float fireTimer;
    private float heatCoolTimer;
    private float heatPercentage = 0;
    private bool isFiring = false;
    private bool hasOverheated = false;

    private void Start()
    {
        mainCam = Camera.main;
        settings = GameSceneSettings.Instance;

        indicator = Instantiate(rayPrefab, Vector3.zero, Quaternion.identity);
        CreateIndicator();
    }

    private void CreateIndicator()
    {
        Vector3 localScale = indicator.localScale;

        indicator.localScale = new Vector3(localScale.x * 0.5f, localScale.y * 0.5f,
            localScale.z);
        indicator.parent = rayContainer;

        overheatLight.enabled = false;
        indicator.GetComponentInChildren<MeshRenderer>().material = indicatorMaterial;
    }

    protected void Update()
    {
        CheckOverheat();
        Indicate(); // indicate expensive method invocation?
    }

    private void CheckOverheat()
    {
        if (Time.time > heatCoolTimer)
        {
            if (heatPercentage <= 0f)
            {
                heatPercentage = 0f;
                hasOverheated = false;
            }
            else
            {
                if (hasOverheated)
                {
                    heatPercentage -= 0.5f * overheatMultiplier;
                }
                else
                {
                    heatPercentage -= 0.5f;
                }
            }
        }

        // else
        // {
        //     Debug.Log("" + (heatCoolTimer - Time.time));
        // }
    }

    public void InvertIndicatorState()
    {
        indicator.gameObject.SetActive(!indicator.gameObject.activeSelf);
    }

    public virtual void Fire(Vector3 mouseHit)
    {
        if (!hasOverheated)
        {
            if (Time.time > fireTimer)
            {
                Transform rayClone = Instantiate(rayPrefab, Vector3.zero, Quaternion.identity);
                Transform rayGunFireClone = Instantiate(rayGunFireEffectPrefab, Vector3.zero, Quaternion.identity);

                RaycastHit[] hits = Physics.RaycastAll(firePoint.position, (mouseHit - firePoint.position).normalized,
                    float.MaxValue);
                Vector3 hitPos = new Vector3((int) 69, (int) 420, (int) 1111337);

                if (hits.Length > 0)
                {
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].collider)
                        {
                            if (hits[i].collider.attachedRigidbody)
                            {
                                if (hits[i].collider.attachedRigidbody.gameObject == gameObject)
                                {
                                    if (i == hits.Length - 1)
                                    {
                                        hitPos = mouseHit;
                                    }

                                    continue;
                                }
                                else
                                {
                                    hitPos = hits[i].point;
                                    break;
                                }
                            }
                            else
                            {
                                if (hits[i].collider.gameObject == gameObject)
                                {
                                    if (i == hits.Length - 1)
                                    {
                                        hitPos = mouseHit;
                                    }

                                    continue;
                                }
                                else
                                {
                                    hitPos = hits[i].point;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    hitPos = mouseHit;
                }

                if (hitPos == new Vector3((int) 69, (int) 420, (int) 1111337))
                {
                    Debug.Log("Uhhhhhhhhhh" + " | " + mouseHit + " | " + firePoint.position);
                    Debug.Break();
                }

                rayGunFireClone.position = firePoint.position;
                rayGunFireClone.localScale *= 0.5f;
                rayGunFireClone.parent = gun;
                rayClone.position = firePoint.position;
                rayClone.forward = (hitPos - firePoint.position).normalized;

                Vector3 rayCloneLocalScale = rayClone.localScale;

                if (currentRayType == RayType.BasicRays)
                {
                    Transform rayPointFireClone =
                        Instantiate(rayGunFireEffectPrefab, Vector3.zero, Quaternion.identity);

                    rayPointFireClone.position = hitPos;
                    rayPointFireClone.localScale *= 0.2f;
                    rayPointFireClone.parent = rayContainer;

                    rayClone.localScale = new Vector3(rayCloneLocalScale.x, rayCloneLocalScale.y,
                        (hitPos - firePoint.position).magnitude); // now how to move the ray...
                    rayClone.parent = rayContainer;

                    Destroy(rayClone.gameObject, rayLifeTime);
                }
                else if (currentRayType == RayType.FancyRays)
                {
                    float dist = (hitPos - firePoint.position).magnitude;
                    float distMultiplier = Mathf.Clamp(dist * 0.1f, 0.1f, 10f);

                    float raySize = Mathf.Clamp(dist / 3f, 1f, 10f);

                    rayClone.localScale = new Vector3(rayClone.localScale.x, rayCloneLocalScale.y, raySize);
                    rayClone.parent = rayContainer;

                    StartCoroutine(MoveRay(rayClone, hitPos, fancyRaySpeedDelta * distMultiplier));
                }


                Destroy(rayGunFireClone.gameObject, rayLifeTime);
                fireTimer = Time.time + fireDelay;
                heatPercentage += 3f;

                aceCursor.Fire();
                if (heatPercentage >= 100f)
                {
                    // Debug.Log("Overheated !");
                    hasOverheated = true;
                    heatCoolTimer = fireTimer + heatCoolDelay * 1.2f;
                }
                else
                {
                    heatCoolTimer = fireTimer + heatCoolDelay;
                }
            }
        }
    }

    private void Indicate() // Set it to Vector3.zero or move it to an object-specific variable
    {
        // why is this an expensive method invocation
        // is it because of FullRaycast
        
        GameObject objHit = null;
        Vector3 hitPos = Vector3.zero;
        bool pointerIsOverUi = EventSystem.current.IsPointerOverGameObject(0);
        // is it over an event system (ui) object

        if (!pointerIsOverUi)
        {
            hitPos = settings.RaycastFunctions.FullRaycast(gameObject, whatIsTarget,
                out objHit); // where would fullraycast go to? what script?
        }

        if (indicator.gameObject.activeSelf) // Indicator needs to be instantiated somewhere
        {
            indicator.position = firePoint.position;
            indicator.forward = (hitPos - firePoint.position).normalized;
            indicator.localScale = new Vector3(indicator.localScale.x, indicator.localScale.y,
                (hitPos - firePoint.position).magnitude);
        }

        cursorVisualStuff.IdentifyFriendOrFoe(objHit);
    }

    protected IEnumerator FireNotReadyIndicatorCoroutine(MeshRenderer indicator, int index, float waitTime)
    {
        indicatorCoroutine[index] = true;

        Material
            baseMat = indicator
                .material; // possibly find a more efficient way to do this instead of creating a new mat each time...
        Material tempNewMat = new Material(baseMat);
        tempNewMat.color = Color.red;

        indicator.material = tempNewMat;

        yield return new WaitForSeconds(waitTime);

        indicator.material = baseMat;

        indicatorCoroutine[index] = false;
    }

    protected IEnumerator OverheatedIndicatorCoroutine(MeshRenderer indicator, int index)
    {
        indicatorCoroutine[index] = true;
        overheatLight.enabled = true;

        Material
            baseMat = indicator
                .material; // possibly find a more efficient way to do this instead of creating a new mat each time...
        Material tempNewMat = new Material(baseMat);
        tempNewMat.color = Color.red;

        indicator.material = tempNewMat;

        while (hasOverheated == true)
        {
            yield return new WaitForEndOfFrame();
        }

        indicator.material = baseMat;
        overheatLight.enabled = false;

        indicatorCoroutine[index] = false;
    }

    protected IEnumerator MoveRay(Transform ray, Vector3 destination,
        float delta) // do lerp instead with a for loop if u cant find a way to make this work nicely
    {
        Vector3 origin = ray.position;

        while ((destination - ray.position).magnitude >= 0.1f)
        {
            ray.position = Vector3.MoveTowards(ray.position, destination, delta * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        Transform rayPointFireClone = Instantiate(rayGunFireEffectPrefab, Vector3.zero, Quaternion.identity);

        rayPointFireClone.position = destination;
        rayPointFireClone.localScale *= 0.2f;
        rayPointFireClone.parent = rayContainer;

        Destroy(rayPointFireClone.gameObject, rayHoleLifetime);
        Destroy(ray.gameObject);
    }
}