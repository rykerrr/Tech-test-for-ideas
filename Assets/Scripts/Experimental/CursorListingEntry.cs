using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
public class CursorListingEntry : MonoBehaviour
{
    [SerializeField] private Text cursorName;
    [SerializeField] private Image cursorSpriteImage;

    public Sprite GetCursorSprite => spriteOfCursor;
    public string GetCursorName => nameOfCursor;

    private Sprite spriteOfCursor;
    private string nameOfCursor;

    public void SetCursorSpecifics(string name, Sprite sprite)
    {
        spriteOfCursor = sprite;
        nameOfCursor = name;

        cursorName.text = nameOfCursor;
        cursorSpriteImage.sprite = spriteOfCursor;
    }
}
#pragma warning restore 0649