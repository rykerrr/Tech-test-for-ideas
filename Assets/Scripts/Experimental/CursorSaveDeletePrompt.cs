using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class CursorSaveDeletePrompt : MonoBehaviour // polymorphism for deleteprompts later on
{
    [SerializeField] private NewCursorEntry cursorEntry;

    private NewCursorEntry cursorEntryToDelete;

    // Create an instance via prefab in cursormenucontroller and it initializes it with the instance id of the cursor

    public void Initialize(NewCursorEntry newCursor)
    {
        cursorEntry.SetCursorPreferences(newCursor);
        cursorEntryToDelete = newCursor;
    }

    public void OnClick_DeleteEntry()
    {
        // deletion logic
        CursorMenuController.Instance.DeleteSavedCursor(cursorEntryToDelete); // Perhaps using CursorSaveEntry thing or smth
    }

    public void OnClick_ClosePrompt()
    {
        Destroy(gameObject);
    }
}
#pragma warning restore 0649