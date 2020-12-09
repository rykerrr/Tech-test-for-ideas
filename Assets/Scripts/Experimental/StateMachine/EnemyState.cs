using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Serializable = System.SerializableAttribute;

[Serializable]
public class EnemyState : State
{
    public EnemyState(StateMachine owner) : base(owner)
    {
    }
}
