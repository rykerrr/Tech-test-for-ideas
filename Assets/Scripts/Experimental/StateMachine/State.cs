using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Serializable = System.SerializableAttribute;
using NonSerialized = System.NonSerializedAttribute;
using NotImplementedException = System.NotImplementedException;
using Action = System.Action;

[Serializable]
public class State
{
    [SerializeField] protected string name;
    [NonSerialized] private StateMachine owner;

    protected StateMachine Owner => owner;

    public State(StateMachine owner)
    {
        this.owner = owner;
        name = "" + GetType();
    }
    
    public virtual void Tick()
    {
        throw new NotImplementedException();
    }

    public virtual void Enter(Action onEntryAction = null)
    {
        onEntryAction?.Invoke();
    }

    public virtual void Exit(Action onExitAction = null)
    {
        onExitAction?.Invoke();
    }
}
