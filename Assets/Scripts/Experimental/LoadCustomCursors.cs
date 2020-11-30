using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0414
#pragma warning disable 0649
public class LoadCustomCursors : MonoBehaviour
{
    [SerializeField] private Transform cursorUiListContent;
    [SerializeField] private CursorListingEntry cursorEntryPrefab;
    [SerializeField] private string folderPathName = "Custom_Cursors";
    [SerializeField] private int pixelsPerUnit = 100;

    [Header("Shown for debug")]
    [SerializeField] private List<Sprite> cursorImages = new List<Sprite>(); // name, images

    private void OnValidate()
    {
        cursorImages.Clear();

        LoadImages();
    }

    private void Awake()
    {
        foreach(Sprite cursor in cursorImages)
        {
            CursorListingEntry cursorEntryClone = Instantiate(cursorEntryPrefab, cursorUiListContent);
            cursorEntryClone.SetCursorSpecifics(cursor.name, cursor);
        }
    }

    private void LoadImages()
    {
        Texture2D[] loadedTextures = Resources.LoadAll<Texture2D>(folderPathName);

        List<Sprite> tempList = new List<Sprite>();
        Sprite temp;

        foreach (Texture2D cursorTexture in loadedTextures)
        {
            cursorImages.Add(temp = Sprite.Create(cursorTexture, new Rect(0, 0, cursorTexture.width, cursorTexture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit));
            temp.name = cursorTexture.name;

            Debug.Log(cursorTexture.name);
            Debug.Log(temp.name);
        }
    }
}
#pragma warning restore 0649
#pragma warning restore 0414