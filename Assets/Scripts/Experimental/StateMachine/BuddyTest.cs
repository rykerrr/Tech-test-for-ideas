using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class BuddyTest : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform cannonPivot;

    private void Update()
    {
        // RotateViaQuat();
        RotateViaDir();
    }

    private void RotateViaDir()
    {
        Vector3 turretDir = target.position - transform.position;
        Vector3 cannonDir = target.position - cannonPivot.position;

        Vector3 newTurretForward = new Vector3(turretDir.x, 0f, turretDir.z);
        Vector3 newCannonRight = new Vector3(cannonDir.x, cannonDir.y, cannonDir.z);

        Vector3 turretSmoothing = Vector3.MoveTowards(transform.right, newTurretForward, 2f * Time.deltaTime);
        Vector3 cannonSmoothing = Vector3.MoveTowards(cannonPivot.right, newCannonRight, 2f * Time.deltaTime);
        // float cannonSmoothing = Mathf.MoveTowards(cannonPivot.right.y, newCannonRight.y, 2f * Time.deltaTime);

        DrawDebugRays();

        transform.right = turretSmoothing;
        cannonPivot.right = new Vector3(cannonSmoothing.x, cannonSmoothing.y, turretSmoothing.z);
    }

    private void DrawDebugRays()
    {
        Debug.DrawRay(cannonPivot.position, cannonPivot.right.normalized * 5f, Color.green);
        Debug.DrawLine(cannonPivot.position, target.position, Color.red);
    }

    private void RotateViaQuat()
    {
        Vector3 dir = target.position - transform.position;
        Vector3 cannonDir = target.position - cannonPivot.position;

        Quaternion lookAngle = Quaternion.LookRotation(dir);
        Quaternion cannonLookAngle = Quaternion.LookRotation(cannonDir);

        Vector3 dirY = new Vector3(0f, lookAngle.eulerAngles.y, 0f);
        Vector3 dirZ = new Vector3(0f, 0f, -cannonLookAngle.eulerAngles.x);

        Quaternion lookAngleY = Quaternion.Euler(dirY);
        Quaternion lookAngleZ = Quaternion.Euler(dirZ);

        transform.rotation = lookAngleY;
        cannonPivot.rotation = lookAngleZ;

        cannonPivot.localEulerAngles = new Vector3(cannonPivot.localEulerAngles.x, 0f, cannonPivot.localEulerAngles.z);
    }
}
#pragma warning restore 0649