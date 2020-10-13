using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#pragma warning disable 0649
public class CustomCursorController : MonoBehaviour
{
    // get custom cursor from here + position and shit
    [SerializeField] private CursorMenuController cursorMenu;
    [SerializeField] private wAceCursor wAceCursor;
    [SerializeField] private Image customCursor;
    [SerializeField] private RectTransform cursorCanvas;
    [SerializeField] private float minOffset;
    [SerializeField] private float maxOffset;

    public Vector2 CustomCursorPos => customCursorPos;
    public Vector2 CustomCursorAnchoredPos => customCursorAnchoredPos;

    private Camera mainCam;
    private Vector2 customCursorPos;
    private Vector2 customCursorAnchoredPos;
    private Vector2 cursorOffset;
    private Vector2 mousePos;
    [SerializeField] private Vector2 mouseMovement;

    public static bool canCustomCursorMove = true;

    private void Awake()
    {
        mainCam = Camera.main;

        Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;

        cursorOffset = new Vector2(-Random.Range(minOffset, maxOffset), Random.Range(minOffset, maxOffset));
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject(-1))
        {
            // turned off raycast target on the custom cursor to make sure it doesnt ever get selected
            Cursor.visible = true;
           //  Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            if (Cursor.visible == true)
            {
                Cursor.visible = false;
                // Cursor.lockState = CursorLockMode.Locked;
            }
        }

        if (canCustomCursorMove)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                cursorOffset = new Vector2(-Random.Range(minOffset, maxOffset), Random.Range(minOffset, maxOffset));
            }

            //mousePos = Input.mousePosition;

            MoveCustomCursor();
        }
        else
        {

        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            customCursorPos = customCursor.transform.position = wAceCursor.transform.position = Input.mousePosition;
        }
    }

    /* Idea:
     * Move the custom cursor while it's located within the bounds of the screen, while keeping the normal cursor locked
     * Unlock the normal cursor in menu while locking the custom cursor in place
     * When not in menu lock the normal cursor and unlock custom cursor
     * For rotation lock the custom cursor
     */

    private void MoveCustomCursor()
    {
        //Debug.Log("Moving");
        // customCursorPos = customCursor.transform.position = wAceCursor.transform.position = pos + cursorOffset;
        mouseMovement = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        if (Mathf.Abs(customCursor.rectTransform.anchoredPosition.x) + Mathf.Abs(mouseMovement.x) >= cursorCanvas.rect.width / 2f)
        {
            if(customCursor.rectTransform.anchoredPosition.x >= 0)
            {
                if(mouseMovement.x >= 0)
                {
                    mouseMovement.x = 0;
                }
                else
                {

                }
            }
            else
            {
                if(mouseMovement.x >= 0)
                {

                }
                else
                {
                    mouseMovement.x = 0;
                }
            }
        }
        if (Mathf.Abs(customCursor.rectTransform.anchoredPosition.y) + Mathf.Abs(mouseMovement.y) >= cursorCanvas.rect.height / 2f)
        {
            // mouseMovement.y = cursorCanvas.rect.height / 2f - customCursorPos.y;

            if (customCursor.rectTransform.anchoredPosition.y >= 0)
            {
                if (mouseMovement.y >= 0)
                {
                    mouseMovement.y = 0;
                }
                else
                {

                }
            }
            else
            {
                if (mouseMovement.y >= 0)
                {

                }
                else
                {
                    mouseMovement.y = 0;
                }
            }
        }

        mouseMovement = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        customCursorPos = customCursorPos + (mouseMovement * cursorCanvas.localScale.x);

        customCursor.transform.position = wAceCursor.transform.position = customCursorPos;

        // This moves it slower since it moves it along the UI, and the UI completely depends on the resolution of the screen
        // Change its position via customCursor.transform.position whilst still doing the if checks on the anchored position
        // customCursorPos = customCursor.transform.position = wAceCursor.transform.position = customCursorPos + mouseMovement;
        //customCursorAnchoredPos = customCursor.rectTransform.anchoredPosition = wAceCursor.GetCursorRectTransform.anchoredPosition = customCursorAnchoredPos + mouseMovement;
        //customCursorPos = customCursor.transform.position;

        // deal with this
    }

    public void CustomCursorUsage(bool use)
    {
        wAceCursor.gameObject.SetActive(!use);
        customCursor.gameObject.SetActive(use);

        if (cursorMenu.NormalCursor)
        {
            customCursor.sprite = cursorMenu.NormalCursor.GetCursorSprite;
            customCursor.color = cursorMenu.NormalCursor.GetCursorColor; // have to deal with IFF as well
        }
    }
}
#pragma warning restore 0649