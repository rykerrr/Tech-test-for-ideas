using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Serializable = System.SerializableAttribute;

[Serializable]
public abstract class EnemyAI : MonoBehaviour // EnemyAI baseclass, not sure if it should be abstract because of
{    // serialization problems
    [SerializeField] protected EnemyStats stats;
    [SerializeField] protected StateMachine stateMachine;

    public EnemyStats ThisStats => stats;

    protected virtual void Awake()
    {
        stateMachine = new StateMachine(this);
        
        State initialState = CreateStatesAndReturnInitial();
        
        stateMachine.StartFrom(initialState, AIGameSceneSettings.Instance.HistoryDepth);
    }

    protected virtual void Update()
    {
        stateMachine.Tick();
    }

    protected abstract State CreateStatesAndReturnInitial();
}
