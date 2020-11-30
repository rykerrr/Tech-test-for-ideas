using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.EventSystems;

#pragma warning disable 0649
public class CustomCursorInputController : MonoBehaviour
{
    [SerializeField] private Canvas cursorCanvas;
    [SerializeField] private RectTransform virtualCursor;
    [SerializeField] private RectTransform vCursorConfine;
    [SerializeField] private Vector2 virtualCursorOffset = new Vector2(4, 4);
    
    private float canWidth;
    private float canHeight;

    private Vector2 customCursorPos;

    [HideInInspector] public bool canCustomCursorMove;

    public Vector2 CustomCursorPos
    {
        get => customCursorPos;
        set => customCursorPos = value;
    }

    private void Start()
    {
        // .position is the global world position
        // .anchoredPosition seems to be the position relative to the anchor point (if it's centered it's pretty much the .localPosition)
        RectTransform canvTransform = cursorCanvas.GetComponent<RectTransform>();
        Rect canvRect = canvTransform.rect;
        canWidth = canvRect.width;
        canHeight = canvRect.height;
    }

    private void Update()
    {
        if (Application.isFocused)
        {
            bool pointerOverUi = EventSystem.current.IsPointerOverGameObject(0);

            if (Mouse.current.middleButton.isPressed || Mouse.current.rightButton.isPressed)
            {
                canCustomCursorMove = false;
            }
            else
            {
                if (canCustomCursorMove == false)
                {
                    canCustomCursorMove = true;
                }
            }

            if (pointerOverUi)
            {
                Cursor.visible = true;
            }
            else
            {
                Cursor.visible = false;
            }
            
            // debug cursor visibility
            // Debug.Log(pointerOverUi + " | " + canCustomCursorMove + " | " + Cursor.visible);
            
            if (canCustomCursorMove)
            {
                MoveCustomCursor(pointerOverUi);
            }
        }
    }

    private void MoveCustomCursor(bool pointerOverUi)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        if (Mouse.current.rightButton.isPressed && !pointerOverUi)
        {
            if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            Vector3 virtualMouseDeltaMovement = new Vector3(mouseDelta.x, mouseDelta.y, 0f) * 0.5f;

            if (mouseDelta.magnitude != 0)
            {
                #region Cursor confinement

                vCursorConfine.position = (Vector2) (virtualCursor.position + virtualMouseDeltaMovement) -
                                          (virtualCursorOffset * 1.05f);

                if (vCursorConfine.anchoredPosition.x >= canWidth / 2f ||
                    vCursorConfine.anchoredPosition.x <= -(canWidth / 2f))
                {
                    if (vCursorConfine.anchoredPosition.x > 0) // Means its hitting or past the right border
                    {
                        // cursorConfine is currently past the right border, that means deltaMovement should be 0 if deltaMovement is positive
                        if (virtualMouseDeltaMovement.x > 0)
                        {
                            virtualMouseDeltaMovement.x =
                                0; // Perhaps this shouldnt be 0 but Clamped canWidth / 2f - Mathf.Abs(vCursorConfine.anchoredPosition.x)
                            // which would give the max distance to the screen from there
                        }
                    }
                    else // Means its hitting or past the left
                    {
                        // cursorConfine is currently past the left border, that means deltaMovement should be 0 if deltaMovement is negative
                        if (virtualMouseDeltaMovement.x < 0)
                        {
                            virtualMouseDeltaMovement.x = 0; // Same here in regards to the clamp?
                        }
                    }
                }

                if (vCursorConfine.anchoredPosition.y >= canHeight / 2f ||
                    vCursorConfine.anchoredPosition.y <= -(canHeight / 2f))
                {
                    if (vCursorConfine.anchoredPosition.y > 0) // Means it's hitting or past the top
                    {
                        // cursorConfine is currently past the upper border, that means deltaMovement should be 0 if deltaMovement is positive
                        if (virtualMouseDeltaMovement.y > 0)
                        {
                            virtualMouseDeltaMovement.y = 0;
                        }
                    }
                    else // Means its hitting or past the bottom
                    {
                        // cursorConfine is currently past the bottom border, that means deltaMovement should be 0 if deltaMovement is positive
                        if (virtualMouseDeltaMovement.y < 0)
                        {
                            virtualMouseDeltaMovement.y = 0;
                        }
                    }
                }

                #endregion // Probably could've been done a lot more elegantly

                // above is needed because of the cursor warping

                virtualCursor.position += virtualMouseDeltaMovement;
            }
        }
        else
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            
            if (Keyboard.current.qKey.isPressed)
            {
                virtualCursor.position = mousePos + virtualCursorOffset;
            }
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            Vector2 warpPosition = (Vector2) virtualCursor.position - virtualCursorOffset;
            Mouse.current.WarpCursorPosition(warpPosition);
            InputState.Change(Mouse.current.position, warpPosition);
        }
        else
        {
            if (!Mouse.current.rightButton.isPressed)
            {
                Vector2 virtCursorPos = mousePos + virtualCursorOffset;

                virtualCursor.position = virtCursorPos; // Checking here doesn't even need to happen quite literally
                // Input won't be recorded if the app isn't in focus
            }
        }

        //#region UI raycastting for custom cursor
        // Was just something fun to try, not really planning on implementing this
        //PointerEventData data = new PointerEventData(EventSystem.current)
        //{
        //    position = virtualCursor.position,
        //    pressPosition = virtualCursor.position,
        //    button = PointerEventData.InputButton.Left
        //};

        //List<RaycastResult> uiRaycastResults = new List<RaycastResult>();
        //EventSystem.current.RaycastAll(data, uiRaycastResults);

        //if (uiRaycastResults.Count > 0)
        //{
        //    uiRaycastResults.OrderByDescending(x => x.depth);
        //    // Depth
        //    if (Mouse.current.leftButton.wasPressedThisFrame)
        //    {
        //        ExecuteEvents.ExecuteHierarchy(uiRaycastResults[0].gameObject, data, ExecuteEvents.pointerClickHandler);
        //    }
        //}
        //#endregion

        customCursorPos = virtualCursor.position;
    }
}
#pragma warning restore 0649