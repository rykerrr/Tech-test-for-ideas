using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CsCursorSaveEntry
{
    public CsCursorSaveEntry(CsCustomCursorEntry cursorEntryEntry)
    {
        cursorName = cursorEntryEntry.CursorName;
        cursorColorMultiplier = cursorEntryEntry.CursorColorMultiplier;
        crossPivotRotation = cursorEntryEntry.CrossPivotRotation;
        lineColor = cursorEntryEntry.LineColor;
        lineLength = cursorEntryEntry.LineLength;
        lineWidth = cursorEntryEntry.LineWidth;
        lineDistanceFromCenter = cursorEntryEntry.LineWidth;
        lineLocalRotation = cursorEntryEntry.LineLocalRotation;
        dotColor = cursorEntryEntry.DotColor;
        radius = cursorEntryEntry.Radius;
        dotLocalRotation = cursorEntryEntry.DotLocalRotation;
    }

    #region Properties
    // General properties
    public string cursorName;
    public Color cursorColorMultiplier;
    public float crossPivotRotation; // Only around the z axis
    // Line specific properties
    public Color lineColor;
    public float lineLength;
    public float lineWidth;
    public float lineDistanceFromCenter;
    public float lineLocalRotation;
    // Dot properties
    public Color dotColor;
    public float radius;
    public float dotLocalRotation;
    #endregion
}
