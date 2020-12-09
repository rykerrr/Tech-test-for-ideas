using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretStats : EnemyStats
{
    public LayerMask whatIsTarget;
    public Transform firePoint; // where projectiles originate from
    public Transform turretPivot; // the pivot attachment part
    public Transform cannonPivot; // the cannon part
    public Transform indic;
}
