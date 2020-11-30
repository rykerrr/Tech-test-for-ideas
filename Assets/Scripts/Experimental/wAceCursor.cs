using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0414
#pragma warning disable 0649
public class wAceCursor : MonoBehaviour
{
    [Header("Lines go as follows: 1 - left, 2 - right, 3 - up, 4 - down")]
    [SerializeField] private RectTransform[] lineLocalPos = new RectTransform[4]; // 4 lines
    [SerializeField] private RectTransform circleCursor;
    [SerializeField] private RectTransform dotCursor;
    [SerializeField] private float lineMaxDistanceFromOrigin;
    [SerializeField] private float tweenTime = 2f;

    public RectTransform GetCursorRectTransform => cursorRectTransform;

    private RectTransform cursorRectTransform;
    private Quaternion originalCustomCursorRotation = new Quaternion();
    private Vector2[] originalLineLocalPos = new Vector2[4];
    private Vector2[] distantLineLocalPos = new Vector2[4];
    private Vector2 originalLocalCircleCursorScale = new Vector2();

    private bool isFiring = false;

    [SerializeField] private float magn;
    [SerializeField] private float mult;
    private float originMagn;


    private void Awake()
    {
        cursorRectTransform = GetComponent<RectTransform>();
        CalculatePositions();

        originMagn = (distantLineLocalPos[0] - originalLineLocalPos[0]).magnitude;
    }

    private void CalculatePositions()
    {
        for (int i = 0; i < lineLocalPos.Length; i++)
        {
            originalLineLocalPos[i] = lineLocalPos[i].localPosition;
        }

        distantLineLocalPos[0] = new Vector2(originalLineLocalPos[0].x - lineMaxDistanceFromOrigin, originalLineLocalPos[0].y); // left
        distantLineLocalPos[1] = new Vector2(originalLineLocalPos[1].x + lineMaxDistanceFromOrigin, originalLineLocalPos[1].y); // right
        distantLineLocalPos[2] = new Vector2(originalLineLocalPos[2].x, originalLineLocalPos[2].y + lineMaxDistanceFromOrigin); // up
        distantLineLocalPos[3] = new Vector2(originalLineLocalPos[3].x, originalLineLocalPos[3].y - lineMaxDistanceFromOrigin); // down
    }

    private void Update()
    {
        magn = (distantLineLocalPos[0] - (Vector2)lineLocalPos[0].position).magnitude;
        mult = Mathf.Clamp(originMagn / magn, 1f, 2f);
    }

    // works fine as long as the fire delay isnt 0, because then it calls expand line way too much

    private void ExpandLine(GameObject line, Vector3 targetPos, Vector3 returnOriginPos)
    {
        // Debug.Log("expando");
        // Debug.Log("calling...");
        iTween.MoveTo(line, iTween.Hash("name", "expandcursortween", "position", targetPos, "time", tweenTime / 2f , "easeType", iTween.EaseType.linear, "oncomplete", "ShrinkLine", "onCompleteTarget", gameObject, "onCompleteParams", new object[] { line, returnOriginPos }, "islocal", true));
    }

    private void ShrinkLine(object[] values)
    {
        // Debug.Log("shrinkendo");
        // Debug.Log("callign back..." + " | " + (GameObject)values[0] + " | " + (Vector3)values[1]);
        iTween.MoveTo((GameObject) values[0], iTween.Hash("name", "shrinkcursortween", "position", (Vector3) values[1], "time", tweenTime / 2f, "easetype", iTween.EaseType.linear, "islocal", true));
    }

    public void Fire()
    {
        for(int i = 0; i < lineLocalPos.Length; i++)
        {
            ExpandLine(lineLocalPos[i].gameObject, distantLineLocalPos[i], originalLineLocalPos[i]);
        }
    }
}
#pragma warning restore 0649
#pragma warning restore 0414