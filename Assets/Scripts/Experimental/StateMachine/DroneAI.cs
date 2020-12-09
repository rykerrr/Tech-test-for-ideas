using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Serializable = System.SerializableAttribute;

#pragma warning disable 0649
[Serializable]
public class DroneAI : EnemyAI
{
    [SerializeField] private Transform firePoint;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }

    private DroneStats CreateStats()
    {
        DroneStats newStats = new DroneStats();
        newStats.firePoint = firePoint;
        newStats.whatIsTarget = stats.WhatIsTarget;

        return newStats;
    }
    
    protected override State CreateStatesAndReturnInitial() // returns initial state
    {
        IdleState initialState = new IdleState(stateMachine);
        DroneAttackState attackState = new DroneAttackState(stateMachine, CreateStats());
        DroneLocateState locatingState = new DroneLocateState(stateMachine);

        stateMachine.AddStates((typeof(IdleState), initialState), (typeof(LocatorState), locatingState)
            , (typeof(AIAttackState), attackState));

        return initialState;
    }
}
#pragma warning restore 0649