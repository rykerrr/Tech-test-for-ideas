using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serializable = System.SerializableAttribute;
using Action = System.Action;

[Serializable]
public class DroneAttackState : AIAttackState
{
    private LayerMask whatIsTarget;
    private Coroutine stopMovementCoroutine = null;
    private Transform firePoint;

    private bool isFiring;
    private float fireTimer;
    private float fireDelay = 0.2f;

    public DroneAttackState(StateMachine owner, DroneStats stats) : base(owner)
    {
        this.whatIsTarget = stats.whatIsTarget;
        this.firePoint = stats.firePoint;
    }

    public override void Tick()
    {
        if (Owner.StateMachineOwner.ThisStats.target)
        {
            Vector3 targPos = Owner.StateMachineOwner.ThisStats.target.position;
            Vector3 ownerPos = Owner.StateMachineOwner.transform.position;

            bool isTargetInRange = (targPos - ownerPos).magnitude < Owner.StateMachineOwner.ThisStats.GetLocatorRange;
            bool isTargetTooClose = (targPos - ownerPos).magnitude < Owner.StateMachineOwner.ThisStats.GetTooCloseRange;

            if (isTargetInRange)
            {
                // deal damage yeet?
            }
            else
            {
                // switch state
                Owner.ChangeStateByKey(typeof(LocatorState));
            }

            Owner.StateMachineOwner.ThisStats.LastTargetPosition = targPos;

            // DrawLinesTowardsTarget(targPos);

            #region pls format better #1

            if (isTargetTooClose) // yes, that means target isn't far enough away
            {   // since that comparison for some reason confuses me
                Vector3 upVector = Owner.StateMachineOwner.transform.up;
                upVector = RotateTowardsTarget(targPos, upVector);

                if (stopMovementCoroutine == null)
                {
                    stopMovementCoroutine = Owner.StateMachineOwner.StartCoroutine(Coroutine_StopMovement());
                }
            }
            else
            {
                MoveTowardsTarget(targPos);

                if (stopMovementCoroutine != null)
                {
                    Owner.StateMachineOwner.StopCoroutine(Coroutine_StopMovement());
                    stopMovementCoroutine = null;
                }
            }

            #endregion

            fireTimer = Fire(firePoint, whatIsTarget, fireDelay, fireTimer);
        }
        else
        {
            // switch state
            Owner.ChangeStateByKey(typeof(LocatorState));
        }
    }

    private void MoveTowardsTarget(Vector3 targPos)
    {
        Vector3 upVector = Owner.StateMachineOwner.transform.up;
        upVector = RotateTowardsTarget(targPos, upVector);

        Owner.StateMachineOwner.ThisStats.GetRigidbody.velocity =
            upVector * Owner.StateMachineOwner.ThisStats.MovementSpeed;
    }

    private Vector3 RotateTowardsTarget(Vector3 targPos, Vector3 upVector)
    {
        Vector3 dir = (targPos - Owner.StateMachineOwner.transform.position)
            .normalized;

        return Owner.StateMachineOwner.transform.up = Vector3.MoveTowards(upVector, dir,
            Owner.StateMachineOwner.ThisStats.GetRotationSpeed * Time.deltaTime);
    }


    private IEnumerator Coroutine_MoveToLastTargPos()
    {
        Vector3 targPos = Owner.StateMachineOwner.ThisStats.LastTargetPosition;

        while ((targPos - Owner.StateMachineOwner.transform.position).magnitude > 1f)
        {
            MoveTowardsTarget(targPos);
            yield return new WaitForEndOfFrame();
        }

        Owner.StateMachineOwner.StartCoroutine(Coroutine_StopMovement());
        Owner.StateMachineOwner.StopCoroutine(Coroutine_MoveToLastTargPos());
        yield break;
    }

    private IEnumerator Coroutine_StopMovement()
    {
        while (Owner.StateMachineOwner.ThisStats.GetRigidbody.velocity.magnitude > 0.05f)
        {
            Owner.StateMachineOwner.ThisStats.GetRigidbody.velocity = Vector3.MoveTowards(
                Owner.StateMachineOwner.ThisStats.GetRigidbody.velocity,
                Vector3.zero, Owner.StateMachineOwner.ThisStats.MovementSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        Owner.StateMachineOwner.ThisStats.GetRigidbody.velocity = Vector3.zero;
        yield break;
    }

    public override void Enter(Action preEnterAction = null)
    {
        Owner.StateMachineOwner.StopAllCoroutines();

        // set it in enter each time as the value may change
        fireDelay = Owner.StateMachineOwner.ThisStats.GetAttackDelay;

        base.Enter(preEnterAction);
    }

    public override void Exit(Action postExitAction = null)
    {
        Owner.StateMachineOwner.StartCoroutine(Coroutine_MoveToLastTargPos());

        base.Exit(postExitAction);
    }
}