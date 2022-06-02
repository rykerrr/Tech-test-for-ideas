using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class GameSceneSettings : InheritableSingleton<GameSceneSettings>
{
    [SerializeField] private Transform rayContainer;
    [SerializeField] private Transform rayPrefab; // might make it local to weapons/ai
    [SerializeField] private Transform rayGunFireEffectPrefab; // also might make it local to weapons/ai
    [SerializeField] private Transform rayOnHitEffectPrefab;
    [SerializeField] private float rayLifeTime;
    [SerializeField] private float rayHoleLifeTime;
    [SerializeField] private float fancyRaySpeedDelta;
    
    public RayType rayType = RayType.BasicRays;

    [SerializeField] private CustomCursorInputController customCursorInputController;
    [SerializeField] private CursorIdentifyFriendOrFoe customCursorIFF;

    public CustomCursorInputController CustomCursorInputController => customCursorInputController;
    public CursorIdentifyFriendOrFoe CustomCursorIFF => customCursorIFF;
    public RaycastHelper RaycastFunctions => raycasts;
    public Transform GetRayContainer => rayContainer;
    public Transform GetRayPrefab => rayPrefab;
    public Transform GetFireEffectPrefab => rayGunFireEffectPrefab;
    public Transform GetOnHitEffect => rayOnHitEffectPrefab;
    public float GetRayLifeTime => rayLifeTime;
    public float GetRayHoleLifeTime => rayHoleLifeTime;
    public float GetFancyRaySpeedDelta => fancyRaySpeedDelta;
    
    private RaycastHelper raycasts;

    private void Start()
    {
        raycasts = new RaycastHelper(customCursorInputController, Camera.main);
    }
    
    // public IEnumerator MoveRay(Transform ray, Vector3 destination,
    //     float delta, GameObject objHit = null) // do lerp instead with a for loop if u cant find a way to make this work nicely
    // {
    //     Vector3 origin = ray.position;
    //     
    //     while ((destination - ray.position).magnitude >= 0.1f)
    //     {
    //         float dist = (destination - ray.position).magnitude;
    //         ray.position = Vector3.MoveTowards(ray.position, destination, delta * Time.deltaTime * Mathf.Clamp(1/dist, 0.05f, 10f));
    //         yield return new WaitForEndOfFrame();
    //     }
    //
    //     Transform rayOnHitEffect =
    //         Instantiate(GameSceneSettings.Instance.GetOnHitEffect, Vector3.zero, Quaternion.identity);
    //     
    //     rayOnHitEffect.parent = null;
    //     rayOnHitEffect.localScale = Vector3.one / 2f;
    //     rayOnHitEffect.position = destination;
    //
    //     if (objHit != null)
    //     {
    //         rayOnHitEffect.parent = objHit.transform;
    //     }
    //     else
    //     {
    //         rayOnHitEffect.parent = Instance.GetRayContainer;
    //     }
    //     
    //     Destroy(rayOnHitEffect.gameObject, GameSceneSettings.Instance.GetRayHoleLifeTime);
    //     Destroy(ray.gameObject);
    // }
}
#pragma warning restore 0649

public enum RayType
{
    BasicRays,
    FancyRays
}