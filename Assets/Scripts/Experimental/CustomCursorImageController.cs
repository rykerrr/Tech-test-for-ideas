using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomCursorImageController : MonoBehaviour
{
    [SerializeField] private wAceCursor wAceCursor;
    [SerializeField] private CursorMenuController cursorMenu; // will need an IFF for this 

    [SerializeField] private Image customCursorImage;
    // Above would be best changed in the IFF

    private CustomCursorType curCursorType; // reference might come in useful...

    public void SetCursorActive(CustomCursorType newCursorType)
    {
        if (newCursorType != CustomCursorType.CsCursor || curCursorType == newCursorType)
        {
            return;
        }

        curCursorType = newCursorType;

        switch (newCursorType)
        {
            case CustomCursorType.wAceCursor:
                customCursorImage.gameObject.SetActive(false);
                wAceCursor.gameObject.SetActive(true);
                break;
            case CustomCursorType.CustomImageCursor:
                customCursorImage.gameObject.SetActive(true);
                wAceCursor.gameObject.SetActive(false);
                break;
        }
    }

    public void ReloadCursorImage()
    {
        NewCursorEntry entry = cursorMenu.NormalCursor;

        customCursorImage.sprite = entry.GetCursorSprite;
        customCursorImage.color = entry.GetCursorColor;
        customCursorImage.name = entry.GetCursorName;
    }
}

public enum CustomCursorType
{
    wAceCursor,
    CustomImageCursor,
    CsCursor
}