using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Action = System.Action;
using Type = System.Type;
using Serializable = System.SerializableAttribute;

[Serializable]
public class IdleState : State
{
    // needs to be set by the stateMachine in the constructor
    
    public IdleState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter(Action preEnterAction = null)
    {
        base.Enter(preEnterAction);
    }

    public override void Tick()
    {
        // base.Tick(); // throws NotImplementedException
        
        Owner.ChangeStateByKey(typeof(LocatorState));
    }

    public override void Exit(Action postExitAction = null)
    {
        base.Exit(postExitAction);
    }
}
