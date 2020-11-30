using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#pragma warning disable 0649
public class CursorIdentifyFriendOrFoe : MonoBehaviour // also the IFF
{
    private enum IFFCursorType
    {
        Neutral,
        Friendly,
        Enemy
    };
    
    // get custom cursor from here + position and shit
    [SerializeField] private CursorMenuController cursorMenu;
    [SerializeField] private Humanoid playerHumanoid;
    [SerializeField] private wAceCursor wAceCursor;
    [SerializeField] private Image customCursor;

    private bool customCursorBeingUsed = false;

    // This moves it slower since it moves it along the UI, and the UI completely depends on the resolution of the screen
        // Change its position via customCursor.transform.position whilst still doing the if checks on the anchored position
        // customCursorPos = customCursor.transform.position = wAceCursor.transform.position = customCursorPos + mouseMovement;
        //customCursorAnchoredPos = customCursor.rectTransform.anchoredPosition = wAceCursor.GetCursorRectTransform.anchoredPosition = customCursorAnchoredPos + mouseMovement;
        //customCursorPos = customCursor.transform.position;

        // deal with this

    private void Start()
    {
        Cursor.visible = false;
    }

    public void IdentifyFriendOrFoe(GameObject obj) // IFF
    {
        // raycast to target, get rigidbody or parent obj, find humanoid in them or children (the topmost humanoid)
        // compare their team to ours
        // The error could be in the raycastting method which would be annoying as fuck
        // or its just not being called when nothing is hit...
        
        Debug.Log(obj);
        
        if (cursorMenu.UseIFF && customCursorBeingUsed)
        {
            int cursSpriteHashCode = customCursor.sprite.GetHashCode();
            int normalCursSpriteHashCode = cursorMenu.NormalCursor.GetHashCode();

            if (obj)
            {
                IHumanoid hum = obj.GetComponent<IHumanoid>();
                
                if (hum != null)
                {
                    if (hum.TeamColor == playerHumanoid.TeamColor)
                    {
                        SetIFFCursorVisual(IFFCursorType.Friendly);
                    }
                    else if (hum.TeamColor != playerHumanoid.TeamColor)
                    {
                        SetIFFCursorVisual(IFFCursorType.Enemy);
                    }
                    else
                    {
                        SetIFFCursorVisual(IFFCursorType.Neutral);
                    }
                }
                else
                {
                    Debug.Log("Doesn't have a humanoid");
                    // whot this?
                    if (cursSpriteHashCode != normalCursSpriteHashCode)
                    {
                        Debug.Log("Should have been set to normal curs");
                        SetIFFCursorVisual(IFFCursorType.Neutral);
                    }
                    else
                    {
                        Debug.Log("Already is normal curs");
                    }
                }
            }
            else
            {
                Debug.Log("Didn't hit anything");
                if (cursSpriteHashCode != normalCursSpriteHashCode)
                {
                    Debug.Log("Should have been set to normal curs");
                    SetIFFCursorVisual(IFFCursorType.Neutral);
                }
                else
                {
                    Debug.Log("Already is normal curs");
                }
            }
        }
        else
        {
            Debug.Log("Ehhhh....");
        }
    }

    private void SetIFFCursorVisual(IFFCursorType cursorVisualType)
    {
        switch (cursorVisualType)
        {
            case IFFCursorType.Neutral:
                customCursor.sprite = cursorMenu.NormalCursor.GetCursorSprite;
                customCursor.color = cursorMenu.NormalCursor.GetCursorColor;
                break;
            case IFFCursorType.Friendly:
                customCursor.sprite = cursorMenu.FriendlyCursor.GetCursorSprite;
                customCursor.color = cursorMenu.FriendlyCursor.GetCursorColor;
                break;
            case IFFCursorType.Enemy:
                customCursor.sprite = cursorMenu.EnemyCursor.GetCursorSprite;
                customCursor.color = cursorMenu.EnemyCursor.GetCursorColor;
                break;
        }
    }
    
    public void CustomCursorUsage(bool use)
    {
        wAceCursor.gameObject.SetActive(!use);
        customCursor.gameObject.SetActive(use);

        customCursorBeingUsed = use;
        
        if (cursorMenu.NormalCursor)
        {
            customCursor.sprite = cursorMenu.NormalCursor.GetCursorSprite;
            customCursor.color = cursorMenu.NormalCursor.GetCursorColor; // have to deal with IFF as well
        }
    }
}
#pragma warning restore 0649