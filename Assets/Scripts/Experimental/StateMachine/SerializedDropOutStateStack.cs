using UnityEngine;
using Serializable = System.SerializableAttribute;

[Serializable]
public class SerializedDropOutStateStack : DropOutStack<State>
{
    public SerializedDropOutStateStack(int maxDepth) : base(maxDepth)
    {
        
    }
}
