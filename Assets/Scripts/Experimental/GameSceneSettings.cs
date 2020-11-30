using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneSettings : InheritableSingleton<GameSceneSettings>
{
    public RayType rayType = RayType.BasicRays;

    [SerializeField] private CustomCursorInputController customCursorInputController;
    [SerializeField] private CursorIdentifyFriendOrFoe customCursorIFF;

    public CustomCursorInputController CustomCursorInputController => customCursorInputController;
    public CursorIdentifyFriendOrFoe CustomCursorIFF => customCursorIFF;
    public RaycastHelper RaycastFunctions => raycasts;
    
    private RaycastHelper raycasts;

    private void Start()
    {
        raycasts = new RaycastHelper(customCursorInputController, Camera.main);
    }
}

public enum RayType
{
    BasicRays,
    FancyRays
}