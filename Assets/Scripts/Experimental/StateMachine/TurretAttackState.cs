using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

using Serializable = System.SerializableAttribute;
using Action = System.Action;

[Serializable]
public class TurretAttackState : AIAttackState
{
    private LayerMask whatIsTarget;
    private Transform firePoint; // where projectiles originate from
    private Transform turretPivot; // the pivot attachment part
    private Transform cannonPivot; // the cannon part
    private Transform indic;

    private float fireDelay = 0.2f;
    
    private bool isFiring;
    private float fireTimer;
    
    public TurretAttackState(StateMachine owner, TurretStats stats) :
        base(owner)
    {
        // Pass these in a struct or class or smth
        this.firePoint = stats.firePoint;
        this.turretPivot = stats.turretPivot;
        this.cannonPivot = stats.cannonPivot;
        this.indic = stats.indic;
        this.whatIsTarget = stats.whatIsTarget;
    }

    public override void Tick()
    {
        if (Owner.StateMachineOwner.ThisStats.target == null)
        {
            Owner.ChangeStateByKey(typeof(LocatorState));
        }
        else
        {
            bool isTargetInRange =
                (Owner.StateMachineOwner.transform.position -
                 Owner.StateMachineOwner.ThisStats.target.transform.position).magnitude <
                Owner.StateMachineOwner.ThisStats.GetLocatorRange;

            if (isTargetInRange)
            {
                // deal damage yeet?
                Vector3 targPos = Owner.StateMachineOwner.ThisStats.target.position;

                DrawLinesTowardsTarget(targPos);
                AimAtTarget(targPos);
            }
            else
            {
                // switch state
                Owner.ChangeStateByKey(typeof(LocatorState));
            }
        }
    }

    private void AimAtTarget(Vector3 targPos)
    {
        // RotateViaQuat(targPos);
        RotateViaDir(targPos);

        // turretPivot.right =
        //     Vector3.MoveTowards(turretPivot.right, newPivot,
        //         Owner.StateMachineOwner.ThisStats.GetRotationSpeed * Time.deltaTime);
        // turretCannon.right =
        //     Vector3.MoveTowards(turretCannon.right, newCannon,
        //         Owner.StateMachineOwner.ThisStats.GetRotationSpeed * Time.deltaTime);

        fireTimer = Fire(firePoint, whatIsTarget, fireDelay, fireTimer);
    }

    private void RotateViaDir(Vector3 targPos)
    {
        float rotSpeed = Owner.StateMachineOwner.ThisStats.GetRotationSpeed;
        float rotSpeedWithTimeDelta = rotSpeed * Time.deltaTime;

        Vector3 turretDir = targPos - turretPivot.position;
        Vector3 cannonDir = targPos - cannonPivot.position;

        Vector3 newTurretForward = new Vector3(turretDir.x, 0f, turretDir.z);
        Vector3 newCannonRight = new Vector3(cannonDir.x, cannonDir.y, cannonDir.z);

        Vector3 turretSmoothing = Vector3.MoveTowards(turretPivot.right, newTurretForward, rotSpeedWithTimeDelta * 2f);
        // DrawDebugRays(targPos);

        turretPivot.right = turretSmoothing;

        Vector3 cannonSmoothing = Vector3.MoveTowards(cannonPivot.right, newCannonRight, rotSpeedWithTimeDelta * 0.5f);

        cannonPivot.right = cannonSmoothing;
    }

    private void DrawDebugRays(Vector3 targPos)
    {
        Debug.DrawLine(indic.position, targPos, Color.red);
    }

    private void RotateViaQuat(Vector3 targPos)
    {
        Vector3 turretPivotDir = targPos - turretPivot.position;
        Vector3 cannonPivotDir = targPos - cannonPivot.position;

        Quaternion turretPivotLookRot = Quaternion.LookRotation(turretPivotDir);
        Quaternion cannonPivotLookRot = Quaternion.LookRotation(cannonPivotDir);

        Vector3 turretDirY = new Vector3(0f, turretPivotLookRot.eulerAngles.y - 90f, 0f);
        Vector3 cannonDirY = new Vector3(0f, 0f, -cannonPivotLookRot.eulerAngles.x);

        Vector3 turretDirSmoothing = new Vector3(0f,
            Mathf.MoveTowards(turretPivot.eulerAngles.y, turretDirY.y,
                Owner.StateMachineOwner.ThisStats.GetRotationSpeed), 0f);
        Vector3 cannonDirSmoothing = new Vector3(0f,
            0f, Mathf.MoveTowards(cannonPivot.eulerAngles.z, cannonDirY.z,
                Owner.StateMachineOwner.ThisStats.GetRotationSpeed));

        Quaternion turretYRot = Quaternion.Euler(turretDirSmoothing);
        Quaternion cannonYRot = Quaternion.Euler(cannonDirSmoothing);

        turretPivot.rotation = turretYRot;
        cannonPivot.rotation = cannonYRot;

        // Not sure why this is happening, try and figure it out eventually i guess, just fixes the child rotation back
        // to 0

        Vector3 cannonPivotLocalEuler = cannonPivot.localEulerAngles;
        cannonPivot.localEulerAngles = new Vector3(0f, 0f, cannonPivotLocalEuler.z);
    }

    public override void Enter(Action onEntryAction = null)
    {
        // set it in enter each time as the value may change
        fireDelay = Owner.StateMachineOwner.ThisStats.GetAttackDelay;

        base.Enter(onEntryAction);
    }

    public override void Exit(Action onExitAction = null)
    {
        base.Exit(onExitAction);
    }
}