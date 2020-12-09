using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Serializable = System.SerializableAttribute;

#pragma warning disable 0649
[Serializable]
public class TurretAI : EnemyAI
{
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private Transform firePoint; // where projectiles originate from
    [SerializeField] private Transform turretPivot; // the pivot attachment part
    [SerializeField] private Transform cannonPivot; // the cannon part
    [SerializeField] private Transform indic;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }

    private TurretStats CreateStats()
    {
        TurretStats newStats = new TurretStats();
        newStats.indic = indic;
        newStats.cannonPivot = cannonPivot;
        newStats.firePoint = firePoint;
        newStats.turretPivot = turretPivot;
        newStats.whatIsTarget = whatIsTarget;

        return newStats;
    }

    protected override State CreateStatesAndReturnInitial()
    {
        IdleState initialState = new IdleState(stateMachine);
        TurretAttackState attackState = new TurretAttackState(stateMachine, CreateStats());
        TurretLocateState locatingState = new TurretLocateState(stateMachine);

        stateMachine.AddStates((typeof(IdleState), initialState), (typeof(LocatorState), locatingState)
            , (typeof(AIAttackState), attackState));

        return initialState;
    }
}
#pragma warning restore 0649