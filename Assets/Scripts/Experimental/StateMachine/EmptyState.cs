using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EmptyState : State
{
    protected EmptyState(StateMachine owner) : base(owner)
    {
    }

    public override void Tick()
    {
        base.Tick();
    }

    public override void Enter(Action onEntryAction)
    {
        base.Enter(onEntryAction);
    }

    public override void Exit(Action onExitAction)
    {
        base.Exit(onExitAction);
    }
}