using System.Collections.Generic;
using UnityEngine;

using Serializable = System.SerializableAttribute;

[Serializable]
public class DropOutStack<T> where T: class
{
    [SerializeField] private List<T> elements = new List<T>();
    [SerializeField] private int maxDepth;

    public DropOutStack(int maxDepth)
    {
        this.maxDepth = maxDepth;
    }
    
    public T Pop()
    {
        if (elements.Count > 0)
        {
            return elements[elements.Count - 1];
        }

        return null;
    }

    public void Push(T item)
    {
        if (elements.Count > maxDepth)
        {
            elements.RemoveAt(0);
        }
        
        elements.Add(item);
    }
}
