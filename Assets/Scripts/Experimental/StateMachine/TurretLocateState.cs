﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TurretLocateState : LocatorState
{
    public TurretLocateState(StateMachine owner) : base(owner)
    {
    }

    public override void Tick()
    {
        base.Tick();
    }

    public override void Enter(Action onEntryAction = null)
    {
        base.Enter(onEntryAction);
    }

    public override void Exit(Action onExitAction = null)
    {
        base.Exit(onExitAction);
    }
}
