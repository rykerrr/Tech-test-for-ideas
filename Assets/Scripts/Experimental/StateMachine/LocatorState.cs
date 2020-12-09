using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LocatorState : EnemyState
{
    public LocatorState(StateMachine owner) : base(owner)
    {
    }

    public override void Tick()
    {
        // base.Tick(); // throws NotImplementedException

        IHumanoid human = TryLocateEnemy();

        // now the transitioning
        if (human != null)
        {
            Owner.ChangeStateByKey(typeof(AIAttackState));
        }
    }

    public IHumanoid TryLocateEnemy()
    {
        // human find logic boohoo
        if (Owner.StateMachineOwner.ThisStats.target == null)
        {    // do circlecast and shit to try and find the target
            Collider[] hits = Physics.OverlapSphere(Owner.StateMachineOwner.transform.position,
                Owner.StateMachineOwner.ThisStats.GetLocatorRange,
                Owner.StateMachineOwner.ThisStats.WhatIsTarget);

            foreach (Collider hit in hits)
            {
                if (hit.GetComponent<IHumanoid>() != null)
                {
                    Owner.StateMachineOwner.ThisStats.target = hit.transform;
                    return TryLocateEnemy();
                }
            }
        }
        else
        {
            // Debug.Log("Hello");
            IHumanoid humanoid;

            bool isTargetInRange =
                (Owner.StateMachineOwner.ThisStats.target.transform.position -
                 Owner.StateMachineOwner.transform.position).magnitude <
                Owner.StateMachineOwner.ThisStats.GetLocatorRange;

            // Debug.Log(isTargetInRange + " | " + (Owner.StateMachineOwner.ThisStats.target.transform.position -
                                                 // Owner.StateMachineOwner.transform.position).magnitude);

            if (isTargetInRange)
            {
                if ((humanoid = Owner.StateMachineOwner.ThisStats.target.GetComponent<IHumanoid>()) != null)
                {
                    return humanoid;
                }
            }

            DrawLocatorLines();
        }

        return null;
    }

    private void DrawLocatorLines()
    {
        Vector3 ownerPos = Owner.StateMachineOwner.transform.position;
        Vector3 targPos = Owner.StateMachineOwner.ThisStats.target.position;
        Vector3 locatorRangePos =
            (targPos - ownerPos).normalized * Owner.StateMachineOwner.ThisStats.GetLocatorRange;

        Debug.DrawLine(ownerPos, targPos, Color.yellow);
        Debug.DrawRay(ownerPos, locatorRangePos, Color.green);
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
