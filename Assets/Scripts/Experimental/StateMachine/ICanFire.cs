using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

public interface ICanFire
{
    float Fire(Vector3 hitPos,Transform firePoint, LayerMask whatIsTarget, float fireDelay, float fireTimer);
    IEnumerator MoveRay(Transform ray, Vector3 destination,
        float delta);
}
