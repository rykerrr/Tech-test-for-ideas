using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
public class NewCursorEntry : MonoBehaviour
{
    [SerializeField] private Text cursorNameText;
    [SerializeField] private RawImage cursorTexturePreviewRawImage;
    [SerializeField] private Image backgroundForSelection;
    [SerializeField] private Image colorPreviewImage;
    [SerializeField] private Image cursorImagePreviewImage;
    [SerializeField] private Image savedToFileIndicator; // green if saved, yellow if edited, red if not saved

    private SaveState saveState = SaveState.NotSaved;
    public SaveState GetSaveState => saveState;

    public Image GetBackgroundImageForSelection => backgroundForSelection;
    
    public Sprite GetCursorSprite => cursorSprite;                    
    public Texture2D GetCursorTexture => cursorTexture;
    public Color GetCursorColor => cursorColor;
    public string GetCursorName => cursorName;
    public string SavedAsFileName
    {
        get => savedAsFileName;
        set => savedAsFileName = value;
    }

    private Sprite cursorSprite;
    private Texture2D cursorTexture;
    private Color cursorColor;
    private string cursorName;
    [SerializeField] private string savedAsFileName;

    public bool isSaveEntry = false;
    
    public override bool Equals(object other)
    {
        NewCursorEntry b = (NewCursorEntry)other;

        return (this.cursorColor == b.cursorColor && this.cursorName == b.cursorName && this.cursorTexture == b.cursorTexture);
    }

    public override int GetHashCode()
    {
        // this works, it has to be overloaded because of the following rules:
        // if two objects are equal, then they must both have the same hash code
        // if two objects have different has codes, then they must be unequal
        // if two objects have the same hash code, then they can be both equal and unequal
        return base.GetHashCode();
    }

    private void Awake()
    {
        RefreshSaveIndicator();
    }

    private void RefreshSaveIndicator()
    {
        if (savedToFileIndicator)
        {
            switch (saveState)
            {
                case SaveState.NotSaved:
                    savedToFileIndicator.color = Color.red;
                    break;
                case SaveState.EditedSinceLastSave:
                    savedToFileIndicator.color = Color.yellow;
                    break;
                case SaveState.Saved:
                    savedToFileIndicator.color = Color.green;
                    break;
            }
        }
    }

    public void RefreshListing()
    {
        cursorNameText.text = cursorName;
        cursorTexturePreviewRawImage.texture = cursorTexture;
        colorPreviewImage.color = cursorColor;
        cursorImagePreviewImage.color = cursorColor;
        cursorImagePreviewImage.sprite = cursorSprite;
    }

    public void SetSaveState(SaveState newState)
    {
        // can change states,
        // if it's not saved it can only be saved
        // if it's saved it can be edited only (notsaved would mean its deleted)
        // if editedSinceLastSave it can only be saved

        switch (saveState) // shitty fsm
        {
            case SaveState.NotSaved:

                if (newState == SaveState.Saved)
                {
                    saveState = newState;
                }
                else
                {
                    // Debug.LogWarning(newState);
                }

                break;

            case SaveState.EditedSinceLastSave:

                if (newState == SaveState.Saved)
                {
                    saveState = newState;
                }
                else if (newState == SaveState.NotSaved)
                {
                    // save file probably deleted
                    saveState = SaveState.NotSaved;
                }

                break;
            case SaveState.Saved:

                if (newState == SaveState.EditedSinceLastSave)
                {
                    saveState = newState;
                }
                else if (newState == SaveState.NotSaved)
                {
                    // save file probably deleted
                    saveState = SaveState.NotSaved;
                }

                break;
        }

        RefreshSaveIndicator();
    }

    public void SetCursorPreferences(string cursorName, Color cursorColor, Texture2D cursorTexture, Sprite cursorSprite, bool updateListing = true)
    {
        //bool areTexturesSame = false;

        //if (this.cursorTexture == cursorTexture)
        //{
        //    areTexturesSame = true;
        //}

        this.cursorTexture = cursorTexture;
        this.cursorName = cursorName;
        this.cursorColor = cursorColor;
        this.cursorSprite = cursorSprite;
        SetSaveState(SaveState.EditedSinceLastSave);

        //if (cursorSprite == null || !areTexturesSame)
        //{
        //    cursorSprite = Sprite.Create(cursorTexture, new Rect(0f, 0f, cursorTexture.width, cursorTexture.height), new Vector2(0.5f, 0.5f), ppu);
        //}

        if (updateListing)
        {
            RefreshListing();
        }
    }

    public void SetCursorPreferences(NewCursorEntry cursorToStealPrefFrom, bool updateListing = true)
    {
        this.cursorTexture = cursorToStealPrefFrom.cursorTexture;
        this.cursorName = cursorToStealPrefFrom.cursorName;
        this.cursorColor = cursorToStealPrefFrom.cursorColor;
        this.cursorSprite = cursorToStealPrefFrom.cursorSprite;
        SetSaveState(SaveState.EditedSinceLastSave);

        if (updateListing)
        {
            RefreshListing();
        }
    }
}
#pragma warning restore 0649

public enum SaveState { NotSaved, EditedSinceLastSave, Saved }