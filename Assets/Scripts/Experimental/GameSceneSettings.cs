using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class GameSceneSettings : InheritableSingleton<GameSceneSettings>
{
    [SerializeField] private Transform rayContainer;
    [SerializeField] private Transform rayPrefab; // might make it local to weapons/ai
    [SerializeField] private Transform rayGunFireEffectPrefabPrefab; // also might make it local to weapons/ai
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
    public Transform GetRayHoleEffectPrefab => rayGunFireEffectPrefabPrefab;
    public float GetRayLifeTime => rayLifeTime;
    public float GetRayHoleLifeTime => rayHoleLifeTime;
    public float GetFancyRaySpeedDelta => fancyRaySpeedDelta;
    
    private RaycastHelper raycasts;

    private void Start()
    {
        raycasts = new RaycastHelper(customCursorInputController, Camera.main);
    }
}
#pragma warning restore 0649

public enum RayType
{
    BasicRays,
    FancyRays
}