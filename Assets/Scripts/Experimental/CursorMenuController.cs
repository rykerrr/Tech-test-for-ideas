using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = System.Object;

#pragma warning disable 0649
// selection on press, multiple selection when holding ctrl // Should be working
// save works with select thing if one is selected only // Should be working
// auto load/read all saved cursors on start // done
// deletes the .txt file if it can find it when deleting cursor entry
// figure out how to fukin set the saved variable in newcursorentry 
// add a motherfukin toggle that resets when u change cursor each time and
// when its toggled and u press save cursor/save as new it saves it
// cursor needs diff name to be saved as a different one though
// that will require a slight change in the system
// overload the == operator in NewCursorEntry
public class CursorMenuController : InheritableSingleton<CursorMenuController>
{
    [SerializeField] private string textureFolderPathName = "Custom_Cursors";
    [SerializeField] private string cursorSaveFolderPathName = "CustomCursorSaves";
    [SerializeField] private int pixelsPerUnit = 100;
    [SerializeField] private Color32 selectedColor = new Color32(100, 200, 100, 200);
    [SerializeField] private Color32 deselectedColor = new Color32(255, 255, 255, 200);
    [SerializeField] private bool deleteFiles = false;

    [FormerlySerializedAs("customCursorVisualStuff")] [Header("References")] [SerializeField]
    private CursorIdentifyFriendOrFoe cursorVisualStuff;

    [SerializeField] private Transform menuCanvas;
    [SerializeField] private Transform cursorListContent;
    [SerializeField] private Transform textureListContent;
    [SerializeField] private Transform savedCursorListContent;
    [SerializeField] private RawImage selectedTextureRawImage;
    [SerializeField] private FlexibleColorPicker cursorColorPicker;
    [SerializeField] private Image cursorPreview;
    [SerializeField] private Image[] selectedCursorPreviewImages; // 1 - normal, 2 - friendly, 3 - enemy
    [SerializeField] private InputField cursorNameInputField;
    [SerializeField] private Button saveCursorButton;
    [SerializeField] private Button saveCursorAsNewButton;
    [SerializeField] private Toggle useCustomCursorToggle;
    [SerializeField] private Text selectedTextureName;
    [SerializeField] private Text selectedCursorName;
    [SerializeField] private string cursorToLoadName;

    [Header("Prefabs")] [SerializeField] private CursorTextureEntry cursorTextureListingPrefab;
    [SerializeField] private NewCursorEntry cursorListingPrefab;
    [SerializeField] private CursorSaveDeletePrompt promptPrefab;

    [Header("Shown for debug")] [SerializeField]
    private List<CursorTextureEntry> textureEntries = new List<CursorTextureEntry>();

    [SerializeField] private List<NewCursorEntry> savedCursors = new List<NewCursorEntry>();
    [SerializeField] private List<NewCursorEntry> cursorEntries = new List<NewCursorEntry>();
    [SerializeField] private List<NewCursorEntry> selectedCursorEntryList = new List<NewCursorEntry>();

    private CursorSaveLoad cursorSaveLoadUtility;
    private NewCursorEntry curSelectedCursorEntry;
    private CursorTextureEntry curSelectedTextureEntry;

    private Texture2D curSelectedTexture;

    // private Color curSelectedColor; // this is cursorColorPicker.color
    private string curCursorName = "";
    private bool useCustomCursor;
    private bool useIFF = true;

    private NewCursorEntry neutralCursor;
    private NewCursorEntry enemyCursor;
    private NewCursorEntry friendlyCursor;

    public NewCursorEntry NeutralCursor => neutralCursor;
    public NewCursorEntry EnemyCursor => enemyCursor;
    public NewCursorEntry FriendlyCursor => friendlyCursor;

    public bool UseIFF => useIFF;

    private void Awake()
    {
        LoadTextures();
        cursorSaveLoadUtility = new CursorSaveLoad();
    }

    private void Start()
    {
        CursorSaveEntry[] getSaveEntries = cursorSaveLoadUtility.DeserializeAllCursorEntries();

        foreach (CursorSaveEntry entry in getSaveEntries) // load them into saved cursors
        {
            LoadCursor(entry); // select doesnt even run for them as onclick
        }

        ReloadCursorsFromSavedCursors();
        LoadPlayerPrefsForCustomCursorUsage();
    }

    private void SavePlayerPrefsForCustomCursorUsage()
    {
        PlayerPrefs.SetInt("CustomCursorUsage", useCustomCursor ? 1 : 0);

        if (useCustomCursor)
        {
            PlayerPrefs.SetString("NeutralCursor", neutralCursor.GetCursorName);
            PlayerPrefs.SetString("FriendlyCursor", friendlyCursor.GetCursorName);
            PlayerPrefs.SetString("EnemyCursor", enemyCursor.GetCursorName);
        }
        
#if UNITY_EDITOR
        Debug.Log("Custom cursors saved!");
#endif
    }
    
    private void LoadPlayerPrefsForCustomCursorUsage()
    {
        if (cursorEntries.Count > 0)
        {
            if (PlayerPrefs.HasKey("CustomCursorUsage"))
            {
                bool usedCustomCursorLastTime = PlayerPrefs.GetInt("CustomCursorUsage") >= 1;
                useCustomCursor = usedCustomCursorLastTime;
                
                if (usedCustomCursorLastTime)
                {
                    string neutralCursorName = "";
                    string enemyCursorName = "";
                    string friendlyCursorName = "";

                    if (PlayerPrefs.HasKey("NeutralCursor"))
                    {
                        neutralCursorName = PlayerPrefs.GetString("NeutralCursor");
                        curSelectedCursorEntry = this.neutralCursor = cursorEntries.Find(x => x.GetCursorName == neutralCursorName);
                        OnClick_SelectCursorAs(0);
                    }

                    if (PlayerPrefs.HasKey("FriendlyCursor"))
                    {
                        friendlyCursorName = PlayerPrefs.GetString("FriendlyCursor");
                        curSelectedCursorEntry = this.friendlyCursor = cursorEntries.Find(x => x.GetCursorName == friendlyCursorName);
                        OnClick_SelectCursorAs(1);
                    }

                    if (PlayerPrefs.HasKey("EnemyCursor"))
                    {
                        enemyCursorName = PlayerPrefs.GetString("EnemyCursor");
                        curSelectedCursorEntry = this.enemyCursor = cursorEntries.Find(x => x.GetCursorName == enemyCursorName);
                        OnClick_SelectCursorAs(2);
                    }

                    Debug.Log(friendlyCursorName + " | " + enemyCursorName + " | " + neutralCursorName);
                    Debug.Log("e" + this.neutralCursor + " | " + this.friendlyCursor + " | " + this.enemyCursor);
                    Toggle_CustomCursor(useCustomCursor);
                }
            }
        }
    }

    private void Update()
    {
        // unless the person is writing
        // Debug.Log(EventSystem.current.currentSelectedGameObject?.GetComponent<InputField>());
        // design a deselect method for eachtime the mouse is pressed unless its over a specific button
        // that works with either saving or selection
        
        bool isReceivingUiInput = false;
        GameObject curSelectedUiObj = EventSystem.current.currentSelectedGameObject;

        if (curSelectedUiObj != null) // should def work
        {
            if (curSelectedUiObj.GetComponent<InputField>())
            {
                isReceivingUiInput = true;
            }
            else
            {
                isReceivingUiInput = false;
            }
        }
        else
        {
            isReceivingUiInput = false;
        }

        if (isReceivingUiInput) // ignore keypresses, probably should do in movement too
        {
        }
        else // isnt receiving input
        {
            if (Keyboard.current.mKey.wasPressedThisFrame)
            {
                menuCanvas.gameObject.SetActive(!menuCanvas.gameObject.activeSelf);
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                ClearCursorSelectedEntries();
            }

            if (Keyboard.current.deleteKey.wasPressedThisFrame)
            {
                DeleteCursorEntry();
                //if (selectedCursorEntryList.Count > 0)
                //{
                //    selectedCursorEntryList.ForEach(x =>
                //    {
                //        if (x.GetSaveState == SaveState.EditedSinceLastSave || x.GetSaveState == SaveState.Saved)
                //        {
                //            if (File.Exists(folderPathName + x.GetCursorName))
                //            {
                //                File.Delete(folderPathName + x.GetCursorName);
                //            }
                //        }

                //        Destroy(x.gameObject);
                //    });
                //    selectedCursorEntryList.Clear();
                //}
            }

            if (Keyboard.current.leftCtrlKey.isPressed)
            {
                if (Keyboard.current.leftShiftKey.isPressed)
                {
                    if (Keyboard.current.sKey.wasPressedThisFrame)
                    {
                        if (selectedCursorEntryList.Count > 0) // actually would this work?
                        {
                            foreach (NewCursorEntry selectEntry in selectedCursorEntryList)
                            {
                                SaveCursorEntryToFile(selectEntry);
                            }
                            // occasion: select 4 files, change name to bob
                            // save files
                            // do they all become named bob?
                            // that would be a decent outcome
                            // would require name fix tho
                            // well technically no because it saves them to the file
                            // meanwhile save saves their preferences
                        }
                
                        if (curSelectedCursorEntry != null) // Saves last selected entry or loops?
                        {
                            SaveCursorEntryToFile(curSelectedCursorEntry);
                        }
                
                        ReloadSavedList();
                    }
                }
                else if (Keyboard.current.sKey.wasPressedThisFrame)
                {
                    // save preferences otherwise
                    if (selectedCursorEntryList.Count > 0)
                    {
                        foreach (NewCursorEntry selectedEntry in selectedCursorEntryList)
                        {
                            OnClick_SaveCursorPreferences();
                        }
                    }
                    else
                    {
                        if (curSelectedCursorEntry != null)
                        {
                            OnClick_SaveCursorPreferences();
                        }
                    }
                }
                // for saving
            }

            if (Keyboard.current.f11Key.wasPressedThisFrame)
            {
                CursorSaveEntry saveEntry = cursorSaveLoadUtility.JsonDeserialize(cursorToLoadName);
                LoadCursor(saveEntry);
            }
        }

        cursorPreview.color = cursorColorPicker.color;
    }

    private void SaveCursorEntryToFile(NewCursorEntry entry)
    {
        cursorSaveLoadUtility.JsonSerialize(entry);
        entry.SetSaveState(SaveState.Saved);
        entry.SavedAsFileName = entry.GetCursorName;
    }

    private NewCursorEntry CreateCursor(bool isSaveEntry = false)
    {
        NewCursorEntry newCursorEntry;
        Button cursorEntryButton;

        if (isSaveEntry)
        {
            newCursorEntry = Instantiate(cursorListingPrefab, cursorListContent);
        }
        else
        {
            cursorEntries.Add(newCursorEntry = Instantiate(cursorListingPrefab, cursorListContent));
        }

        newCursorEntry.isSaveEntry = isSaveEntry;

        cursorEntryButton = newCursorEntry.GetComponent<Button>();
        
        cursorEntryButton.onClick.AddListener(() => OnClick_SelectCursor(newCursorEntry));

        Input_CursorNameInput(TryFixName(curCursorName, isSaveEntry));

        newCursorEntry.SetCursorPreferences(curCursorName, cursorColorPicker.color, curSelectedTexture,
            cursorPreview.sprite);

        return newCursorEntry;
    }

    private string TryFixName(string cursName, bool isSaveFile = false) // Should work properly now
    {
        bool exists;
        
        if (isSaveFile)
        {
            if ((exists = savedCursors.Exists(x => x.GetCursorName == cursName)) == true)
            {
                cursName += " (1)";
                cursName = TryFixName(cursName); 
            } // otherwise cursname is cursname
        }
        else
        {
            if ((exists = cursorEntries.Exists(x => x.GetCursorName == cursName)) == true)
            {
                cursName += " (1)";
                cursName = TryFixName(cursName);
            } // otherwise cursname is cursname
        }

        return cursName;
    }

    private void ReloadCursorsFromSavedCursors()
    {
        foreach (NewCursorEntry savedEntry in savedCursors)
        {
            NewCursorEntry entry;
            
            if ((entry = cursorEntries.Find(x => x.SavedAsFileName == savedEntry.SavedAsFileName)) != null) // == precedes &&
            {
                // overload the == operator in NewCursorEntry instead
                // just reload it
                entry.SetCursorPreferences(savedEntry);
                entry.SetSaveState(SaveState.Saved);
            }
            else // re-create it i guess
            {
                // Debug.Log("Recreating");
                // Debug.Log(savedEntry.GetCursorName + " | " + savedEntry.GetCursorColor + " | " + savedEntry.GetCursorSprite.name);
                // Debug.Log(cursorEntries.Find(x => x.GetCursorName == savedEntry.GetCursorName && x.SavedAsFileName == savedEntry.SavedAsFileName));

                // Debug.Log(curCursorName + " | " + savedEntry.isSaveEntry + " | " + savedEntry.GetCursorName + " | " + savedEntry.SavedAsFileName);
                // 850 true 850 850
                NewCursorEntry
                    reloadedCursor =
                        CreateCursor(); // instantiates it, adds it to created cursors list, sets preferences and
                // on click but the sprite is created beforehand so this method isn't as resource heavy
                // check if a cursor with the same shit already exists
                // which is technically being done above?

                
                reloadedCursor.SavedAsFileName = savedEntry.SavedAsFileName;
                reloadedCursor.SetCursorPreferences(savedEntry);
                reloadedCursor.SetSaveState(SaveState.Saved);
            }
        }
    }

    private void ClearCursorSelectedEntries()
    {
        // Why was this > 1? Was i just retarded?
        if (selectedCursorEntryList.Count > 0) 
        {
            selectedCursorEntryList.ForEach(x =>
            {
                if (x != null) x.GetComponent<Image>().color = Color.white;
            });
            selectedCursorEntryList.Clear();
            OnClick_DeselectCursor();
        }
    }

    public void DeleteSavedCursor(NewCursorEntry cursorToDelete)
    {
        string path = Path.Combine(Application.dataPath, "Resources", cursorSaveFolderPathName,
            cursorToDelete.GetCursorName);

        if (deleteFiles)
        {
            NewCursorEntry removeEntry;

            if (savedCursors.Find(x => x.Equals(cursorToDelete))) // OVERLOAD == AT NEWCURSORENTRY NOOB
            {
                savedCursors.Remove(cursorToDelete);
            }

            if ((removeEntry = cursorEntries.Find(x => x.Equals(cursorToDelete))) != null)
            {
                removeEntry.SetSaveState(SaveState.NotSaved);
            }

            Destroy(cursorToDelete.gameObject);
            File.Delete(path);
            Debug.Log("Deleting file at path: " + path);
        }
        else
        {
            Debug.Log(File.Exists(path) + " | " + path + " | delete files: " + deleteFiles);
        }
    }

    private bool DeleteCursorEntry() //Entry/s
    {
        bool retValue = false;

        // now how to delete dis
        if (selectedCursorEntryList.Count > 0)
        {
            foreach (NewCursorEntry entry in selectedCursorEntryList)
            {
                if (entry.isSaveEntry /*savedCursors.Contains(entry)*/
                ) // or do .Find(x => x.GetInstanceID() == entry.GetInstanceID())
                {
                    CreateFilePromptInstance(curSelectedCursorEntry);
                    continue;
                }

                // do them in a separate function that gets called by the file delete button ( could just use loaded cursors in another list )
                // 
                // selectedCursorEntryList.Remove(entry); no can do as it modifies the collection
                // create a copy of the collection, then iterate over that while removing from the other collection
                // or clear entire selection list which would be technically viable
                // but is it viable? what if we select saved entries? or smth else? im 80% sure something will cause an exception
                cursorEntries.Remove(entry);
                Destroy(entry.gameObject);
            }

            // BUT AM I NOT CLEARING IT HERE????
            ClearCursorSelectedEntries(); // Clears selection after it opens prompt if it opens prompt, which technically should and should
            // not happen, design choice i guess

            retValue = true;
        }
        else if (curSelectedCursorEntry != null) // maybe not maybe yes
        {
            if (curSelectedCursorEntry.isSaveEntry /*savedCursors.Contains(curSelectedCursorEntry)*/
            ) // or do .Find(x => x.GetInstanceID() == curSelectedCursorEntry.GetInstanceID())
            {
                // now to delete it here!!!!
                CreateFilePromptInstance(curSelectedCursorEntry);
                return false;
            }

            if (selectedCursorEntryList.Contains(curSelectedCursorEntry))
            {
                selectedCursorEntryList.Remove(curSelectedCursorEntry);
            }

            selectedCursorEntryList.Remove(curSelectedCursorEntry);
            cursorEntries.Remove(curSelectedCursorEntry);
            Destroy(curSelectedCursorEntry.gameObject);

            retValue = true;
        }

        return retValue;
    }

    private void LoadCursorSave(CursorSaveEntry saveEntry) // Load it into save only
    {
        CursorTextureEntry cursorTexture = textureEntries.Find(x => x.GetTextureName == saveEntry.textureName);

        if (cursorTexture == null)
        {
            Debug.LogError("Can not find texture associated with save entry, texture name: " + saveEntry.textureName);
        }
        else
        {
            Input_CursorNameInput(saveEntry.cursorName);

            curSelectedTexture = cursorTexture.GetTexture;

            Sprite loadedCursorSprite = Sprite.Create(cursorTexture.GetTexture,
                new Rect(0f, 0f, curSelectedTexture.width, curSelectedTexture.height), new Vector2(0.5f, 0.5f),
                pixelsPerUnit);

            // cursorColorPicker.color is throwing an error
            // how fuckin fun
            cursorColorPicker.color = saveEntry.cursorColor;
            cursorPreview.sprite = loadedCursorSprite;

            OnClick_PreviewCursor(); // should work? no?

            NewCursorEntry savedCursor = CreateCursor(true);
            savedCursor.transform.SetParent(savedCursorListContent);
            savedCursor.SetCursorPreferences(savedCursor);
            savedCursor.SetSaveState(SaveState.Saved);
            savedCursor.SavedAsFileName = saveEntry.cursorName;
            savedCursor.isSaveEntry = true; // THIS IS THE FUCKING SAVED CURSOR

            Button loadedEntryButton = savedCursor.GetComponent<Button>();
            loadedEntryButton.onClick.AddListener(() => OnClick_SelectCursor(savedCursor));
            
            if (!savedCursors.Contains(savedCursor)) // do this beforef
            {
                savedCursors.Add(savedCursor);
            }
        }
    }

    private void LoadCursor(CursorSaveEntry saveEntry) // Load them into saved stuff
    {
        CursorTextureEntry cursorTexture = textureEntries.Find(x => x.GetTextureName == saveEntry.textureName);

        if (cursorTexture == null)
        {
            Debug.LogError("Can not find texture associated with save entry, texture name: " + saveEntry.textureName);
        }
        else
        {
            Input_CursorNameInput(saveEntry.cursorName);

            curSelectedTexture = cursorTexture.GetTexture;

            Sprite loadedCursorSprite = Sprite.Create(cursorTexture.GetTexture,
                new Rect(0f, 0f, curSelectedTexture.width, curSelectedTexture.height), new Vector2(0.5f, 0.5f),
                pixelsPerUnit);

            // cursorColorPicker.color is throwing an error
            // how fuckin fun
            cursorColorPicker.color = saveEntry.cursorColor;
            cursorPreview.sprite = loadedCursorSprite;

            OnClick_PreviewCursor();

            NewCursorEntry loadedCursor = CreateCursor(false); // Change this to a create method instead?
            loadedCursor.SetSaveState(SaveState.Saved);
            loadedCursor.SavedAsFileName = saveEntry.cursorName;
            loadedCursor.isSaveEntry = false; // THIS IS THE NORMAL FUCKING CURSOR

            #region Creating cursorEntry for savedList, perhaps add an overload for CreateCursor instead?

            NewCursorEntry savedCursor = Instantiate(loadedCursor, savedCursorListContent);
            savedCursor.SetCursorPreferences(loadedCursor);
            savedCursor.SetSaveState(SaveState.Saved);
            savedCursor.isSaveEntry = true; // THIS IS THE FUCKING SAVED CURSOR

            Button loadedEntryButton = savedCursor.GetComponent<Button>();
            loadedEntryButton.onClick.AddListener(() => OnClick_SelectCursor(savedCursor));
            
            #endregion

            if (!savedCursors.Contains(savedCursor))
            {
                savedCursors.Add(savedCursor);
            }
        }
    }

    private void LoadTextures()
    {
        Texture2D[] loadedTextures = Resources.LoadAll<Texture2D>(textureFolderPathName);

        foreach (Texture2D texture in loadedTextures)
        {
            CursorTextureEntry textureListingClone = Instantiate(cursorTextureListingPrefab, textureListContent);
            Button textureButton = textureListingClone.GetComponent<Button>();

            textureEntries.Add(textureListingClone);
            textureListingClone.SetTextureListingPreferences(texture.name, texture);
            textureButton.onClick.AddListener(() => OnClick_SelectTextureListing(textureListingClone));
        }
    }

    private void UpdateIFFCursorPreview(int cursor)
    {
        // check whether selectedcursor exists already happens in the OnClick mnethod for it

        switch (cursor)
        {
            case 0:
                selectedCursorPreviewImages[0].sprite = neutralCursor.GetCursorSprite;
                selectedCursorPreviewImages[0].color = neutralCursor.GetCursorColor;
                break;
            case 1:
                selectedCursorPreviewImages[1].sprite = friendlyCursor.GetCursorSprite;
                selectedCursorPreviewImages[1].color = friendlyCursor.GetCursorColor;
                break;
            case 2:
                selectedCursorPreviewImages[2].sprite = enemyCursor.GetCursorSprite;
                selectedCursorPreviewImages[2].color = enemyCursor.GetCursorColor;
                break;
        }
    }

    public void CreateFilePromptInstance(NewCursorEntry entryForPrompt) // for deleting saved cursors
    {
        CursorSaveDeletePrompt promptClone = Instantiate(promptPrefab, transform) as CursorSaveDeletePrompt;
        promptClone.Initialize(
            entryForPrompt); // Passes a monobehaviour which is a component on a gameobject, allows us to get the InstanceID
    }

    private void ReloadSavedList()
    {
        // clear then reload
        
        foreach (NewCursorEntry entry in savedCursors)
        {
            // had a tiny headache over being unable to delete this
            // BECAUSE IT DOESNT WORK ON NON GAMEOBJECTS RETARD
            Destroy(entry.gameObject);
        }
        
        savedCursors.Clear();
        
        CursorSaveEntry[] getSaveEntries = cursorSaveLoadUtility.DeserializeAllCursorEntries(); // one gets added each time for some reason
        
        foreach (CursorSaveEntry entry in getSaveEntries) // load them into saved cursors
        {
            LoadCursorSave(entry); // select doesnt even run for them as onclick
        }
    }

    public void Input_CursorNameInput(string input)
    {
        curCursorName = input;
    }

    public void OnClick_SwitchCursorListWindow() // content.Viewport.Scrollview
    {
        // teleport them for now, tween later on

        RectTransform t1 = cursorListContent.parent.parent.GetComponent<RectTransform>();
        RectTransform t2 = savedCursorListContent.parent.parent.GetComponent<RectTransform>();

        float x1 = t1.position.x;
        float x2 = t2.position.x;

        t2.position = new Vector3(x1, t2.position.y);
        t1.position = new Vector3(x2, t1.position.y);
    }

    public void OnClick_ReloadSavedCursorsAsUsable()
    {
        ReloadCursorsFromSavedCursors();
    }

    public void OnClick_DeleteCursorEntry()
    {
        // figure out how to add a delete feature
        // maybe just by clicking an x on a cursor then it shows a prompt asking you if you really want to delete it
        // add another event to onclick which allows u to select multiple cursors
        DeleteCursorEntry();
    }

    public void OnClick_SelectCursor(NewCursorEntry entry)
    {
        if (!Input.GetKey(KeyCode.LeftControl) || entry.isSaveEntry) // perhaps be able to select multiple entries?
        {
            if (curSelectedCursorEntry)
            {
                curSelectedCursorEntry.GetComponent<Image>().color = Color.white;
            }

            ClearCursorSelectedEntries();
        }

        curSelectedCursorEntry = entry;
        curSelectedCursorEntry.GetComponent<Image>().color = Color.green;

        if (!selectedCursorEntryList.Contains(curSelectedCursorEntry))
        {
            if (!Input.GetKey(KeyCode.LeftControl))
            {
                if (selectedCursorEntryList.Count > 0)
                {
                    ClearCursorSelectedEntries();
                }
            }

            selectedCursorEntryList.Add(entry);
        }

        selectedTextureRawImage.texture = entry.GetCursorTexture;
        selectedTextureName.text = entry.GetCursorTexture.name;
        cursorColorPicker.color = entry.GetCursorColor;
        selectedCursorName.text = entry.GetCursorName;

        curCursorName = entry.GetCursorName;
        cursorColorPicker.color = entry.GetCursorColor;
        curSelectedTexture = entry.GetCursorTexture;

        saveCursorButton.gameObject.SetActive(true);

        //if (Input.GetKey(KeyCode.LeftControl))
        //{
        //    if (selectedCursorEntryList.Contains(entry))
        //    {
        //        entry.GetComponent<Image>().color = Color.white;
        //        Debug.Log("Deslected " + entry.name);
        //        selectedCursorEntryList.Remove(entry);
        //    }
        //    else // Not found in queue
        //    {
        //        entry.GetComponent<Image>().color = Color.green;
        //        Debug.Log("Selected " + entry.name);
        //        selectedCursorEntryList.Add(entry);
        //    }
        //}
        //else
        //{
        //    ClearCursorSelectedEntries();
        //}

        // Debug.Log("selecto");

        OnClick_PreviewCursor();
    }

    public void OnClick_SelectTextureListing(CursorTextureEntry textureEntry)
    {
        ClearCursorSelectedEntries();

        Debug.Log(textureEntry);

        cursorNameInputField.text = textureEntry.GetTextureName;
        curSelectedTextureEntry = textureEntry;
        curSelectedTexture = textureEntry.GetTexture;

        selectedTextureRawImage.texture = textureEntry.GetTexture;
        selectedTextureName.text = textureEntry.GetTextureName;

        cursorPreview.sprite = Sprite.Create(textureEntry.GetTexture,
            new Rect(0f, 0f, curSelectedTexture.width, curSelectedTexture.height), new Vector2(0.5f, 0.5f),
            pixelsPerUnit);
    }

    public void OnClick_ReloadSavedList()
    {
        ReloadSavedList();
    }

    public void OnClick_ReloadAllCursors()
    {
        foreach (NewCursorEntry entry in cursorEntries)
        {
            Destroy(entry.gameObject);
        }

        foreach (NewCursorEntry entry in savedCursors)
        {
            Destroy(entry.gameObject);
        }
        
        cursorEntries.Clear();
        savedCursors.Clear();
        
        // CursorSaveEntry[] getSaveEntries = cursorSaveLoadUtility.DeserializeAllCursorEntries();
        //
        // foreach (CursorSaveEntry entry in getSaveEntries) // load them into saved cursors
        // {
        //     LoadCursor(entry); // select doesnt even run for them as onclick
        // }

        ReloadSavedList();
        ReloadCursorsFromSavedCursors();
    }
    
    public void OnClick_DeselectCursor()
    {
        curSelectedTexture = null;
        curSelectedTextureEntry = null;
        curSelectedCursorEntry = null;
        cursorColorPicker.color = cursorColorPicker.startingColor;

        curCursorName = "";

        selectedCursorName.text = "N/A";
        selectedTextureName.text = "";
        selectedTextureRawImage.texture = null;
        saveCursorButton.gameObject.SetActive(false);
    }

    public void OnClick_SaveCursorPreferences()
    {
        if (curSelectedTexture == null)
        {
            return;
        }

        if (cursorPreview.sprite == null)
        {
            Debug.Log(curSelectedTexture + " | " + cursorPreview.sprite);
            return;
        }
        
        Input_CursorNameInput(TryFixName(curCursorName));
        // ReloadSavedList(); // uhhhhhh
        // ReloadCursorsFromSavedCursors();
        
        curSelectedCursorEntry.SetCursorPreferences(curCursorName, cursorColorPicker.color, curSelectedTexture,
            cursorPreview.sprite);
        // this saves the cursor not the file LOL
    }

    public void OnClick_PreviewCursor()
    {
        if (cursorPreview.sprite)
        {
            if (cursorPreview.sprite.texture != curSelectedTexture)
            {
                cursorPreview.sprite = Sprite.Create(curSelectedTexture,
                    new Rect(0f, 0f, curSelectedTexture.width, curSelectedTexture.height), new Vector2(0.5f, 0.5f),
                    pixelsPerUnit);
            }
        }

        cursorPreview.color = cursorColorPicker.color;
    }

    public void OnClick_SelectCursorAs(int selection)
    {
        // 1 - normal, 2 - friendly, 3 - enemy
        if (curSelectedCursorEntry == null)
        {
            return;
        }

        switch (selection)
        {
            case 0:
                neutralCursor = curSelectedCursorEntry;
                break;
            case 1:
                friendlyCursor = curSelectedCursorEntry;
                break;
            case 2:
                enemyCursor = curSelectedCursorEntry;
                break;
        }

        UpdateIFFCursorPreview(selection);
    }

    public void OnClick_SaveCursorPreferencesAsNew()
    {
        CreateCursor();
    }

    public void Toggle_CustomCursor(bool toggle)
    {
        useCustomCursor = toggle;
        useCustomCursorToggle.isOn = toggle;
        
        cursorVisualStuff.CustomCursorUsage(useCustomCursor);
    }

    private void OnEnable()
    {
        LoadPlayerPrefsForCustomCursorUsage();
    }
    
    private void OnDisable()
    {
        SavePlayerPrefsForCustomCursorUsage();
    }
}
#pragma warning restore 0649