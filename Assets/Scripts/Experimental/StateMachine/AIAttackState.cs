using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serializable = System.SerializableAttribute;

[Serializable]
public class AIAttackState : EnemyState
{
    public AIAttackState(StateMachine owner) : base(owner)
    {
    }

    protected void DrawLinesTowardsTarget(Vector3 targPos)
    {
        Vector3 ownerPos = Owner.StateMachineOwner.transform.position;

        Debug.DrawLine(ownerPos, targPos, Color.red);
    }

    public virtual float Fire(Transform firePoint, LayerMask whatIsTarget, float fireDelay, float fireTimer)
    {
        bool canFire = Time.time > fireTimer;

        #region fire part

        if (canFire)
        {
            RaycastHelper raycasts = GameSceneSettings.Instance.RaycastFunctions;
            Vector3 hitPos = raycasts.FromAToBRaycast(firePoint.gameObject, firePoint.forward, (LayerMask)~0,
                out GameObject objHit);

            if (objHit != null)
            {
                IHumanoid hitHumanoid = null;
                if ((hitHumanoid = objHit.GetComponent<IHumanoid>()) != null)
                {
                    hitHumanoid.TakeDamage(Owner.StateMachineOwner.ThisStats as IDamager);
                }
            }

            // Transform rayClone =
            //     Object.Instantiate(GameSceneSettings.Instance.GetRayPrefab, Vector3.zero, Quaternion.identity) as
            //         Transform;
            Transform rayGunFireClone =
                Object.Instantiate(GameSceneSettings.Instance.GetFireEffectPrefab, Vector3.zero,
                    Quaternion.identity) as Transform;

            Vector3 firePos = firePoint.position;

            // rayGunFireClone.position = firePos;
            // rayGunFireClone.localScale *= 0.5f;
            // rayGunFireClone.parent = Owner.StateMachineOwner.transform;
            //
            // rayClone.position = firePos;
            // rayClone.forward = (hitPos - firePos).normalized;
            //
            // Vector3 rayCloneLocalScale = rayClone.localScale;

            // if (GameSceneSettings.Instance.rayType == RayType.BasicRays)
            // {
            //     Transform rayPointFireClone =
            //         Object.Instantiate(GameSceneSettings.Instance.GetOnHitEffect, Vector3.zero,
            //             Quaternion.identity) as Transform;
            //
            //     rayPointFireClone.position = hitPos;
            //     rayPointFireClone.localScale *= 0.2f;
            //
            //     if (objHit == null)
            //     {
            //         rayPointFireClone.parent = GameSceneSettings.Instance.GetRayContainer;
            //     }
            //     else
            //     {
            //         rayPointFireClone.parent = objHit.transform;
            //     }
            //
            //     rayClone.localScale = new Vector3(rayCloneLocalScale.x, rayCloneLocalScale.y,
            //         (hitPos - firePoint.position).magnitude); // now how to move the ray...
            //     rayClone.parent = GameSceneSettings.Instance.GetRayContainer;
            //
            //     Object.Destroy(rayClone.gameObject, GameSceneSettings.Instance.GetRayLifeTime);
            // }
            // else if (GameSceneSettings.Instance.rayType == RayType.FancyRays)
            // {
            //     float dist = (hitPos - firePoint.position).magnitude;
            //     float distMultiplier = Mathf.Clamp(dist * 0.1f, 0.1f, 10f);
            //
            //     float raySize = Mathf.Clamp(dist / 3f, 1f, 10f);
            //
            //     rayClone.localScale = new Vector3(rayClone.localScale.x, rayCloneLocalScale.y, raySize);
            //     rayClone.parent = GameSceneSettings.Instance.GetRayContainer;
            //
            //     GameSceneSettings.Instance.StartCoroutine(GameSceneSettings.Instance.MoveRay(rayClone, hitPos,
            //         GameSceneSettings.Instance.GetFancyRaySpeedDelta * distMultiplier, objHit));
            // }


            Object.Destroy(rayGunFireClone.gameObject, GameSceneSettings.Instance.GetRayLifeTime);
            fireTimer = Time.time + fireDelay;
            // heatPercentage += 3f;
            //
            // if (heatPercentage >= 100f)
            // {
            //     // Debug.Log("Overheated !");
            //     hasOverheated = true;
            //     heatCoolTimer = fireTimer + heatCoolDelay * 1.2f;
            // }
            // else
            // {
            //     heatCoolTimer = fireTimer + heatCoolDelay;
            // }
        }

        #endregion

        return fireTimer;
    }
}