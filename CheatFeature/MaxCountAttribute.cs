using System;
using UnityEditor;
using UnityEngine;

public class MaxCountAttribute : PropertyAttribute
{
    public int MaxCount { get; private set; }

    public MaxCountAttribute(int maxCount)
    {
        MaxCount = maxCount;
    }
}
