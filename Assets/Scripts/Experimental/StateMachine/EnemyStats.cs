using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Serializable = System.SerializableAttribute;
using Type = System.Type;

#pragma warning disable 0649
[Serializable]
public class EnemyStats : HumanoidStats, ICanMove, IDamager // why is this not being serialized?
{
    public Transform target;

    [SerializeField] private Rigidbody thisRb;
    [SerializeField] private List<Type> stateKeys = new List<Type>(); // will house specific types, though we could mitigate having
    // to do this by implementing Transitions properly
    // unserializable here?
    [SerializeField] private DamageType damageType;
    [SerializeField] private LayerMask whatIsTarget;

    [SerializeField] private float locatorRange; // how far can it be located
    [SerializeField] private float attackDelay; // delay between each attack
    [SerializeField] private float attackRange; // how far the attack goes
    [SerializeField] private float tooCloseRange;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private int movementSpeed;
    [SerializeField] private int damage;

    [SerializeField] private float projectileSpeed; // everything will be a projectile, so essentially the speed of
    // the projectile

    private Vector3 lastTargetPosition = Vector3.zero;
    
    public int MovementSpeed
    {
        get { return movementSpeed; }
        protected set { movementSpeed = value; }
    }
    public Vector3 LastTargetPosition
    {
        get => lastTargetPosition;
        set => lastTargetPosition = value;
    }

    public List<Type> GetStateKeys => stateKeys;
    public Rigidbody GetRigidbody => thisRb;
    public float GetLocatorRange => locatorRange;
    public float GetTooCloseRange => tooCloseRange;
    public float GetRotationSpeed => rotationSpeed;
    public float GetAttackDelay => attackDelay;
    public float GetAttackRange => attackRange;
    public float GetProjectileSpeed => projectileSpeed;

    public int Damage => damage;
    public DamageType TypeOfDamage => damageType;
    public LayerMask WhatIsTarget => whatIsTarget;
    
    public virtual void ExecuteMovement()
    {
        throw new System.NotImplementedException();
    }
}
#pragma warning restore 0649