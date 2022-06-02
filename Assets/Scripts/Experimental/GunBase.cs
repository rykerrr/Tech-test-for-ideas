using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using EventSystem = UnityEngine.EventSystems.EventSystem;

#pragma warning disable 0649
public abstract class GunBase : MonoBehaviour, IDamager
{
    [Header("Set on initialize")] [SerializeField]
    private wAceCursor aceCursor;

    [SerializeField] private CustomCursorInputController customCursorInputStuff;
    [SerializeField] private CursorIdentifyFriendOrFoe cursorVisualStuff;

    [FormerlySerializedAs("whatIsTargettable")] [Header("Options")] [SerializeField]
    private LayerMask whatIsTarget;

    [SerializeField] private MeshRenderer ammoOverheatIndicator;
    [SerializeField] private MeshRenderer sightFireDelayIndicator;
    [SerializeField] private Material indicatorMaterial;
    [SerializeField] private Light overheatLight;
    [SerializeField] private Transform projectilePrefab;
    [SerializeField] private Transform magPrefab;
    [SerializeField] private Transform magThrowObj;
    [SerializeField] private Transform gun;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Humanoid owner;
    [SerializeField] private DamageType typeOfDamage;
    [SerializeField] private float fireDelay = 0.1f;
    [SerializeField] private float overheatMultiplier = 0.9f;
    [SerializeField] private float heatCoolDelay = 1f;
    [SerializeField] private float coolingPerFrame = 2f;
    [SerializeField] private float maxHeat = 100f;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float reloadTime;
    [SerializeField] private int damage;

    public LayerMask WhatIsTarget => whatIsTarget;
    public Animator playerGunAnimator;
    public bool CanFire => (Time.time > fireTimer);
    public bool IsFiring => isFiring;
    public int Damage => damage;
    public DamageType TypeOfDamage => typeOfDamage;
    private RayType currentRayType;

    private bool[] indicatorCoroutine = new bool[2]; // 0 for mag, 1 for sight fire delay
    private GameSceneSettings settings;
    private Transform indicator;
    private float fireTimer;
    private float heatCoolTimer;
    private float heatPercentage = 0;
    private bool hasOverheated = false;
    private bool isFiring = false;

    private int gunReloadHash;
    
    private void Start()
    {
        settings = GameSceneSettings.Instance;

        indicator = Instantiate(GameSceneSettings.Instance.GetRayPrefab, Vector3.zero, Quaternion.identity);
        indicator.name = "Fire Indicator";
        // Debug.Log(indicator + " | " + indicatorMeshRenderer + " | " + indicator.GetComponent<MeshRenderer>()
        //  + " | " + indicator.GetComponent<UnityEngine.Renderer>());

        gunReloadHash = Animator.StringToHash("isReloading");
        
        CreateIndicator();
    }

    private void CreateIndicator()
    {
        Vector3 localScale = indicator.localScale;

        indicator.localScale = new Vector3(localScale.x * 0.5f, localScale.y * 0.5f,
            localScale.z);
        indicator.parent = GameSceneSettings.Instance.GetRayContainer;

        overheatLight.enabled = false;
        indicator.GetComponentInChildren<MeshRenderer>().material = indicatorMaterial;
    }

    protected void Update()
    {
        CheckOverheat();
        // Indicate();
        RefreshIff();

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (Time.time > fireTimer)
            {
                Reload();
            }
        }
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
                    heatPercentage -= coolingPerFrame * overheatMultiplier;
                }
                else
                {
                    heatPercentage -= coolingPerFrame;
                }
            }
        }

        // else
        // {
        //     Debug.Log("" + (heatCoolTimer - Time.time));
        // }
    }

    private void RefreshIff()
    {
        settings.RaycastFunctions.FromCameraToMouseRaycast(gameObject, whatIsTarget, out GameObject objHit);

        cursorVisualStuff.IdentifyFriendOrFoe(objHit);
    }

    public void InvertIndicatorState()
    {
        indicator.gameObject.SetActive(!indicator.gameObject.activeSelf);
    }

    public virtual void Fire(Vector3 mouseHit)
    {
        if (!hasOverheated)
        {
            if (CanFire)
            {
                isFiring = true;

                // Transform rayClone = Instantiate(GameSceneSettings.Instance.GetRayPrefab, Vector3.zero,
                //     Quaternion.identity);
                Bullet projClone = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity)
                    .GetComponent<Bullet>();
                Transform rayGunFireClone = Instantiate(GameSceneSettings.Instance.GetFireEffectPrefab, Vector3.zero,
                    Quaternion.identity);
                GameObject objHit = null;

                // Debug.DrawLine(transform.position, mouseHit);
                RaycastHit[] hits = Physics.RaycastAll(firePoint.position, (mouseHit - firePoint.position).normalized,
                    float.MaxValue, whatIsTarget);

                Vector3 hitPos = new Vector3((int) 69, (int) 420, (int) 1111337);

                #region possible problem with the raycast

                if (hits.Length > 0) // raycast from body
                {
                    hits = hits.OrderBy(x => x.distance).ToArray();

                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (i == hits.Length - 1)
                        {
                            hitPos = mouseHit;
                        }

                        if (hits[i].collider)
                        {
                            if (hits[i].collider.attachedRigidbody != null)
                            {
                                if (hits[i].collider.attachedRigidbody.gameObject != gameObject)
                                {
                                    hitPos = hits[i].point;
                                    objHit = hits[i].collider.attachedRigidbody.gameObject;
                                    // Debug.Log(hits[i].collider.gameObject.name + " | " + objHit.name + " | " +
                                              // hits[i].point + " | " + hitPos);
                                    break;
                                }
                            }
                            else
                            {
                                if (hits[i].collider.gameObject != gameObject)
                                {
                                    hitPos = hits[i].point;
                                    objHit = hits[i].collider.gameObject;
                                    // Debug.Log(hits[i].collider.gameObject.name + " | " + objHit.name + " | " + hits[i].point + " | " + hitPos);
                                    break;
                                }
                            }
                        }

                        // if (hits[i].collider)
                        // {
                        //     if (hits[i].collider.attachedRigidbody)
                        //     {
                        //         // Debug.Log("hit obj attached to a rigidbody");
                        //         if (hits[i].collider.attachedRigidbody.gameObject == gameObject)
                        //         {
                        //             // Debug.Log("hit ourselves");
                        //
                        //             continue;
                        //         }
                        //         else
                        //         {
                        //             // Debug.Log("hit someone else, settings the hitpos");
                        //             hitPos = hits[i].point;
                        //             objHit = hits[i].collider.transform;
                        //             break;
                        //         }
                        //     }
                        //     else
                        //     {
                        //         // Debug.Log("does not have an rb attached");
                        //         if (hits[i].collider.gameObject == gameObject)
                        //         {
                        //             // Debug.Log("hit ourselves without the attached rb check");
                        //             continue;
                        //         }
                        //         else
                        //         {
                        //             // Debug.Log("hit someone else without the attached rb check");
                        //             hitPos = hits[i].point;
                        //             objHit = hits[i].collider.transform;
                        //             break;
                        //         }
                        //     }
                        // }
                    }
                }
                else
                {
                    // Debug.Log("hit nothing, length is less than 1");
                    hitPos = mouseHit;
                }

                #endregion

                // if (objHit != null)
                // {
                //     // Debug.Log(objHit + " | " + objHit.transform?.parent + " | "+ objHit.transform?.parent?.parent + " | " + objHit.transform?.parent?.parent?.parent + " | "
                //     //  + objHit.GetComponent<Collider>()?.attachedRigidbody.gameObject);
                //     IHumanoid hitHumanoid = null;
                //
                //     if ((hitHumanoid = objHit.GetComponent<IHumanoid>()) != null)
                //     {
                //         // Debug.Log("Humanoid hit");
                //         hitHumanoid.TakeDamage(this);
                //     }
                // }


                if (hitPos == new Vector3((int) 69, (int) 420, (int) 1111337))
                {
                    Debug.Log("Uhhhhhhhhhh" + " | " + mouseHit + " | " + firePoint.position);
                    Debug.Break();
                }

                projClone.transform.position = firePoint.position;
                projClone.transform.parent = GameSceneSettings.Instance.GetRayContainer;
                projClone.transform.forward = (hitPos - firePoint.position);
                projClone.Init(projectileSpeed, owner.gameObject, this);

                rayGunFireClone.position = firePoint.position;
                rayGunFireClone.localScale *= 0.5f;
                rayGunFireClone.parent = gun;


                // rayClone.position = firePoint.position;
                // rayClone.forward = (hitPos - firePoint.position).normalized;
                //
                // Vector3 rayCloneLocalScale = rayClone.localScale;

                // Transform rayOnHitEffect =
                //     Instantiate(GameSceneSettings.Instance.GetOnHitEffect, Vector3.zero,
                //         Quaternion.identity);
                //
                // rayOnHitEffect.parent = null;
                // rayOnHitEffect.position = hitPos;
                // rayOnHitEffect.localScale = Vector3.one / 2f;
                //
                // if (objHit)
                // {
                //     rayOnHitEffect.parent = objHit;
                // }
                // else
                // {
                //     rayOnHitEffect.parent = GameSceneSettings.Instance.GetRayContainer;
                // }

                // if (currentRayType == RayType.BasicRays)
                // {
                //     rayClone.localScale = new Vector3(rayCloneLocalScale.x, rayCloneLocalScale.y,
                //         (hitPos - firePoint.position).magnitude); // now how to move the ray...
                //     rayClone.parent = GameSceneSettings.Instance.GetRayContainer;
                //
                //     Destroy(rayClone.gameObject, GameSceneSettings.Instance.GetRayLifeTime);
                // }
                // else if (currentRayType == RayType.FancyRays)
                // {
                //     float dist = (hitPos - firePoint.position).magnitude;
                //     float distMultiplier = Mathf.Clamp(dist * 0.1f, 0.1f, 10f);
                //
                //     float raySize = Mathf.Clamp(dist / 3f, 1f, 10f);
                //
                //     rayClone.localScale = new Vector3(rayClone.localScale.x, rayCloneLocalScale.y, raySize);
                //     rayClone.parent = GameSceneSettings.Instance.GetRayContainer;
                //
                //     GameSceneSettings.Instance.StartCoroutine(GameSceneSettings.Instance.MoveRay(rayClone, hitPos,
                //         GameSceneSettings.Instance.GetFancyRaySpeedDelta * distMultiplier, objHit));
                // }


                Destroy(rayGunFireClone.gameObject, GameSceneSettings.Instance.GetRayLifeTime);

                fireTimer = Time.time + fireDelay;
                heatPercentage += 3f;

                aceCursor.Fire();
                if (heatPercentage >= maxHeat)
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
            else // do smth with gun cant fire yet delay / sight fire delay
            {
                if (indicatorCoroutine[1] == false)
                {
                    indicatorCoroutine[1] = true;
                    // Debug.Log(fireTimer + " | " + Time.time + " | " + (fireTimer - Time.time) + " | " +
                    //           (Time.time - fireTimer));
                    StartCoroutine(
                        FireNotReadyIndicatorCoroutine(sightFireDelayIndicator,
                            (fireTimer - Time.time) / 2f)); // Time.time - fireTimer
                    // because it waits until the gun can fire again in a nutshell
                }
            }
        }
        else // do smth with overheat coroutine
        {
            if (indicatorCoroutine[0] == false)
            {
                indicatorCoroutine[0] = true;
                StartCoroutine(OverheatedIndicatorCoroutine(ammoOverheatIndicator));
            }

            if (indicatorCoroutine[1] == false)
            {
                indicatorCoroutine[1] = true;
                // Debug.Log(fireTimer + " | " + Time.time + " | " + (fireTimer - Time.time) + " | " +
                //           (Time.time - fireTimer));
                StartCoroutine(FireNotReadyIndicatorCoroutine(sightFireDelayIndicator,
                    (fireDelay) / 2f)); // Time.time - fireTimer
                // because it waits until the gun can fire again in a nutshell
            }
        }

        isFiring = false;
    }

    protected virtual void Reload()
    {
        playerGunAnimator.SetBool(gunReloadHash, true);

        fireTimer = Int32.MaxValue;

        StartCoroutine(FinishReloading());
    }

    private IEnumerator FinishReloading()
    {
        yield return new WaitForSeconds(reloadTime);
        
        playerGunAnimator.SetBool(gunReloadHash, false);

        fireTimer = Time.time;
    }
    
    private void Indicate() // Set it to Vector3.zero or move it to an object-specific variable
    {
        // why is this an expensive method invocation
        // is it because of FullRaycast

        Vector3 hitPos = Vector3.zero;
        bool pointerIsOverUi = EventSystem.current.IsPointerOverGameObject(0);
        // is it over an event system (ui) object

        if (!pointerIsOverUi)
        {
            hitPos = settings.RaycastFunctions.FromCameraToMouseRaycast(gameObject, whatIsTarget,
                out GameObject objHit); // where would fullraycast go to? what script?
        }

        if (indicator.gameObject.activeSelf) // Indicator needs to be instantiated somewhere
        {
            Vector3 firePointPos = firePoint.position;
            Vector3 indicLocalScale = indicator.localScale;

            indicator.position = firePointPos;
            indicator.forward = (hitPos - firePointPos).normalized;
            indicator.localScale = new Vector3(indicLocalScale.x, indicLocalScale.y,
                (hitPos - firePointPos).magnitude);
        }
    }

    protected IEnumerator FireNotReadyIndicatorCoroutine(MeshRenderer indicMr, float waitTime, int index = 1)
    {
        indicatorCoroutine[index] = true;

        Material
            baseMat = indicMr
                .material; // possibly find a more efficient way to do this instead of creating a new mat each time...
        Material tempNewMat = new Material(baseMat);
        tempNewMat.color = Color.red;

        indicMr.material = tempNewMat;

        yield return new WaitForSeconds(waitTime);

        indicMr.material = baseMat;

        indicatorCoroutine[index] = false;
    }

    protected IEnumerator OverheatedIndicatorCoroutine(MeshRenderer indicMr, int index = 0)
    {
        indicatorCoroutine[index] = true;
        overheatLight.enabled = true;

        Material
            baseMat = indicMr
                .material; // possibly find a more efficient way to do this instead of creating a new mat each time...
        Material tempNewMat = new Material(baseMat);
        tempNewMat.color = Color.red;

        indicMr.material = tempNewMat;

        while (hasOverheated == true)
        {
            yield return new WaitForEndOfFrame();
        }

        indicMr.material = baseMat;
        overheatLight.enabled = false;

        indicatorCoroutine[index] = false;
    }

    public void SpawnMag()
    {
        // Grab current position of mag casing and spawn mag there

        GameObject obj = Instantiate(magPrefab, magThrowObj.position + new Vector3(0f, 0.5f, 0f), magThrowObj.rotation).gameObject;
    }
}
#pragma warning restore 0649