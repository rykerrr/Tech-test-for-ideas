using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using EventSystem = UnityEngine.EventSystems.EventSystem;

#pragma warning disable 0649
public abstract class GunBase : MonoBehaviour, IDamager
{
    [Header("Set on initialize")] [SerializeField]
    private wAceCursor aceCursor;

    [SerializeField] private CustomCursorInputController customCursorInputStuff;
    [SerializeField] private CursorIdentifyFriendOrFoe cursorVisualStuff;
    [SerializeField] private RayType currentRayType;

    [FormerlySerializedAs("whatIsTargettable")] [Header("Options")] [SerializeField]
    private LayerMask whatIsTarget;

    [SerializeField] private MeshRenderer ammoOverheatIndicator;
    [SerializeField] private MeshRenderer sightFireDelayIndicator;
    [SerializeField] private Material indicatorMaterial;
    [SerializeField] private Light overheatLight;
    [SerializeField] private Transform gun;
    [SerializeField] private Transform firePoint;
    [SerializeField] private DamageType typeOfDamage;
    [SerializeField] private float fireDelay = 0.1f;
    [SerializeField] private float overheatMultiplier = 0.9f;
    [SerializeField] private float heatCoolDelay = 1f;
    [SerializeField] private int damage;

    public LayerMask WhatIsTarget => whatIsTarget;
    public bool CanFire => (Time.time > fireTimer);
    public bool IsFiring => isFiring;
    public int Damage => damage;
    public DamageType TypeOfDamage => typeOfDamage;

    private bool[] indicatorCoroutine = new bool[2]; // 0 for mag, 1 for sight fire delay
    private GameSceneSettings settings;
    private Transform indicator;
    private float fireTimer;
    private float heatCoolTimer;
    private float heatPercentage = 0;
    private bool hasOverheated = false;
    private bool isFiring = false;

    private void Start()
    {
        settings = GameSceneSettings.Instance;

        indicator = Instantiate(GameSceneSettings.Instance.GetRayPrefab, Vector3.zero, Quaternion.identity);
        indicator.name = "Fire Indicator";
        // Debug.Log(indicator + " | " + indicatorMeshRenderer + " | " + indicator.GetComponent<MeshRenderer>()
        //  + " | " + indicator.GetComponent<UnityEngine.Renderer>());


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
        Indicate();
        RefreshIff();
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

                Transform rayClone = Instantiate(GameSceneSettings.Instance.GetRayPrefab, Vector3.zero,
                    Quaternion.identity);
                Transform rayGunFireClone = Instantiate(GameSceneSettings.Instance.GetRayHoleEffectPrefab, Vector3.zero,
                    Quaternion.identity);
                Transform objHit = null;

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
                                    objHit = hits[i].collider.transform;
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
                                    objHit = hits[i].collider.transform;
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

                if (objHit)
                {
                    IHumanoid hitHumanoid = null;

                    if ((hitHumanoid = objHit.GetComponent<IHumanoid>()) != null)
                    {
                        hitHumanoid.TakeDamage(this);
                    }
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
                        Instantiate(GameSceneSettings.Instance.GetRayHoleEffectPrefab, Vector3.zero,
                            Quaternion.identity);

                    rayPointFireClone.position = hitPos;
                    rayPointFireClone.localScale *= 0.2f;
                    rayPointFireClone.parent = GameSceneSettings.Instance.GetRayContainer;

                    rayClone.localScale = new Vector3(rayCloneLocalScale.x, rayCloneLocalScale.y,
                        (hitPos - firePoint.position).magnitude); // now how to move the ray...
                    rayClone.parent = GameSceneSettings.Instance.GetRayContainer;

                    Destroy(rayClone.gameObject, GameSceneSettings.Instance.GetRayLifeTime);
                }
                else if (currentRayType == RayType.FancyRays)
                {
                    float dist = (hitPos - firePoint.position).magnitude;
                    float distMultiplier = Mathf.Clamp(dist * 0.1f, 0.1f, 10f);

                    float raySize = Mathf.Clamp(dist / 3f, 1f, 10f);

                    rayClone.localScale = new Vector3(rayClone.localScale.x, rayCloneLocalScale.y, raySize);
                    rayClone.parent = GameSceneSettings.Instance.GetRayContainer;

                    StartCoroutine(MoveRay(rayClone, hitPos,
                        GameSceneSettings.Instance.GetFancyRaySpeedDelta * distMultiplier));
                }


                Destroy(rayGunFireClone.gameObject, GameSceneSettings.Instance.GetRayLifeTime);
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

            isFiring = false;
        }
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

    public virtual IEnumerator MoveRay(Transform ray, Vector3 destination,
        float delta) // do lerp instead with a for loop if u cant find a way to make this work nicely
    {
        Vector3 origin = ray.position;

        while ((destination - ray.position).magnitude >= 0.1f)
        {
            ray.position = Vector3.MoveTowards(ray.position, destination, delta * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        Transform rayPointFireClone =
            Instantiate(GameSceneSettings.Instance.GetRayHoleEffectPrefab, Vector3.zero, Quaternion.identity);

        rayPointFireClone.position = destination;
        rayPointFireClone.localScale *= 0.2f;
        rayPointFireClone.parent = GameSceneSettings.Instance.GetRayContainer;

        Destroy(rayPointFireClone.gameObject, GameSceneSettings.Instance.GetRayHoleLifeTime);
        Destroy(ray.gameObject);
    }
}
#pragma warning restore 0649