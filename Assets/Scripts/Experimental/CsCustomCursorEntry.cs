using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CsCustomCursorEntry : MonoBehaviour
{
    [SerializeField] private string cursorName = "bob";
    [SerializeField] private Color cursorColorMultiplier = Color.black;

    [SerializeField] private float crossPivotRotation = 50f; // Only around the z axis

    // Line specific properties
    [SerializeField] private Color lineColor = Color.blue;
    [SerializeField] private float lineLength = 5f;
    [SerializeField] private float lineWidth = 2f;
    [SerializeField] private float lineDistanceFromCenter = 20f;

    [SerializeField] private float lineLocalRotation = 15f;

    // Dot properties
    [SerializeField] private Color dotColor = Color.cyan;
    [SerializeField] private float radius = 5f;
    [SerializeField] private float dotLocalRotation = 45f;

    public void LoadPropsFromSave(CsCursorSaveEntry saveEntry)
    {
        CursorName = saveEntry.cursorName;
        CursorColorMultiplier = saveEntry.cursorColorMultiplier;
        CrossPivotRotation = saveEntry.crossPivotRotation;
        LineColor = saveEntry.lineColor;
        LineLength = saveEntry.lineLength;
        LineWidth = saveEntry.lineWidth;
        LineWidth = saveEntry.lineDistanceFromCenter;
        LineLocalRotation = saveEntry.lineLocalRotation;
        DotColor = saveEntry.dotColor;
        Radius = saveEntry.radius;
        DotLocalRotation = saveEntry.dotLocalRotation;
    }
    
    public string CursorName
    {
        get => cursorName;
        private set => cursorName = value;
    }

    public Color CursorColorMultiplier
    {
        get => cursorColorMultiplier;
        private set => cursorColorMultiplier = value;
    }

    public float CrossPivotRotation
    {
        get => crossPivotRotation;
        private set => crossPivotRotation = value;
    }

    public Color LineColor
    {
        get => lineColor;
        private set => lineColor = value;
    }

    public float LineLength
    {
        get => lineLength;
        private set => lineLength = value;
    }

    public float LineWidth
    {
        get => lineWidth;
        private set => lineWidth = value;
    }

    public float LineDistanceFromCenter
    {
        get => lineDistanceFromCenter;
        private set => lineDistanceFromCenter = value;
    }

    public float LineLocalRotation
    {
        get => lineLocalRotation;
        private set => lineLocalRotation = value;
    }

    public Color DotColor
    {
        get => dotColor;
        private set => dotColor = value;
    }

    public float Radius
    {
        get => radius;
        private set => radius = value;
    }

    public float DotLocalRotation
    {
        get => dotLocalRotation;
        private set => dotLocalRotation = value;
    }
}