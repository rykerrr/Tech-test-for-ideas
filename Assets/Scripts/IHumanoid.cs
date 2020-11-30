using UnityEngine;

public interface IHumanoid
{
    string HumanoidName { get; }
    int Health { get; }
    int MaxHealth { get; }
    Color TeamColor { get; }
}