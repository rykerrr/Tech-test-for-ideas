using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.XR;

#pragma warning disable 0649
// literally dont know what else to call it
public class CursorSaveLoad
{
    public string folderPath = "Assets/Resources/CustomCursorSaves/";
    public string csCursorFolderPath = "Assets/Resources/CustomCursorSaves/CS_Cursor/";
    public string folderName = "CustomCursorSaves";

    public CursorSaveEntry[] DeserializeAllCursorEntries()
    {
        List<string>
            names = Directory.GetFiles(folderPath).Where(x => !Path.GetExtension(x).Contains(".meta"))
                .ToList(); // i assume i get their names here?

        //for (int i = 0; i < names.Count; i++)
        //{
        //    if (names[i] != null)
        //    {
        //        if (names[i].Contains(".meta"))
        //        {
        //            names.Remove(names[i]);
        //        }
        //    }
        //}

        CursorSaveEntry[] saveEntries = new CursorSaveEntry[names.Count];

        for (int i = 0; i < names.Count; i++)
        {
            saveEntries[i] = JsonDeserialize(null, names[i]);
        }

        return saveEntries;
    }

    public void CsCursorJsonSerialize(CsCustomCursorEntry entry)
    {
        CsCursorSaveEntry saveEntry = new CsCursorSaveEntry(entry);
        string json = JsonUtility.ToJson(saveEntry);
        string filePath = Path.Combine(csCursorFolderPath, entry.CursorName);

        if (File.Exists(csCursorFolderPath))
        {
            File.WriteAllText(filePath, json);
        }
        else
        {
            FileStream fileStream = File.Create(filePath);
            fileStream.Close();

            File.WriteAllText(filePath, json);
        }

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log(json + "\n------------------------------------------");
#endif
    }

    public CsCursorSaveEntry CsCursorJsonDeserialize(string cursorName = null, string path = null)
    {
        if (path != null) // if a path is given e.g by the start load thing
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path, System.Text.Encoding.ASCII);
                CsCursorSaveEntry returnEntry = null;
                
#if UNITY_EDITOR
                try
                {
#endif
                    
                    returnEntry = (CsCursorSaveEntry) JsonUtility.FromJson(json, typeof(CsCursorSaveEntry));

#if UNITY_EDITOR
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning(e);
                    Debug.Break();
                }

                if (returnEntry == null)
                {
                    Debug.Log(cursorName + " | " + path + " | " + File.Exists(path));
                    Debug.Break();
                }
#endif
                
                return returnEntry;
            }
        }

        if (cursorName != null)
        {
            Debug.Log(cursorName);
            string filePath = Path.Combine(csCursorFolderPath, cursorName);

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath, System.Text.Encoding.ASCII);
                CsCursorSaveEntry returnEntry =
                    (CsCursorSaveEntry) JsonUtility.FromJson(json, typeof(CsCursorSaveEntry));
                return returnEntry;
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning("File with name: " + cursorName + "  does not exist. Perhaps save first?");
#endif
                return null;
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning("Cursor name is null, path: " + path + "\nReturning null...");
#endif
            return null;
        }
    }

    public CsCursorSaveEntry CsCursorJsonDeserialize(string fullPath)
    {
        if (fullPath != null)
        {
            string json = File.ReadAllText(fullPath, Encoding.ASCII);
            CsCursorSaveEntry returnEntry = null;

            try
            {
                returnEntry = (CsCursorSaveEntry) JsonUtility.FromJson(json, typeof(CsCursorSaveEntry));
            }
            catch (System.Exception e)
            {
#if UNITY_EDITOR
                Debug.LogWarning(e);
                Debug.Break();
#endif
            }

            if (returnEntry == null)
            {
#if UNITY_EDITOR
                Debug.Log("File exists: " + File.Exists(fullPath) + " Path: " + fullPath);
                Debug.Break();
#endif
            }

            return returnEntry;
        }

        return null; // the path was not passed
    }

    public void JsonSerialize(NewCursorEntry entry) // perhaps rename to savetofile or smth since it doesn't just serialize it but 
    {    // saves it completely? maybe factorize the method too?
        // Check if it has a previous file name, if it does rename that file to the new name
        string fileName = entry.GetCursorName;
        string saveFileName = entry.SavedAsFileName == null ? "" : entry.SavedAsFileName;
        string newFileName = entry.GetCursorName == null ? "" : entry.GetCursorName;
        
        if(entry.SavedAsFileName != "")
        {
            if (entry.SavedAsFileName != entry.GetCursorName)
            {
                // if the file name isnt the same as the cursor name we have to
                // rename the file to the new cursor name

                // argumentNullException ???
                string oldFilePath = Path.Combine(folderPath, saveFileName); // this returns a null exception
                // because savedAsFileName is empty
                // WHY DOES THIS THROW AN EXCEPTION LOL
                string newFilePath = Path.Combine(folderPath, newFileName);
                
                if (File.Exists(oldFilePath)) // if it exists rename it, otherwise who cares since
                {   // it'll just be created again
                    File.Move(oldFilePath, newFilePath);
                }
            }
        }
        
        CursorSaveEntry saveEntry = new CursorSaveEntry(entry);
        string json = JsonUtility.ToJson(saveEntry);
        string filePath = Path.Combine(folderPath, entry.GetCursorName);

        if (File.Exists(filePath))
        {
            File.WriteAllText(filePath, json);
        }
        else
        {
            FileStream fileStream = File.Create(filePath);
            fileStream.Close();

            File.WriteAllText(filePath, json);
        }

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log(json + "\n------------------------------------------");
#endif
    }

    public CursorSaveEntry JsonDeserialize(string cursorName = null, string path = null) // THATS THE FUCKIN CURSOR NAME RETARD
    {
        if (path != null) // if a path is given e.g by the start load thing
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path, System.Text.Encoding.ASCII);
                CursorSaveEntry returnEntry = null;

#if UNITY_EDITOR
                try
                {
#endif
                    
                    returnEntry = (CursorSaveEntry) JsonUtility.FromJson(json, typeof(CursorSaveEntry));
                    
#if UNITY_EDITOR
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning(e);
                    Debug.Break();
                }
                
                if (returnEntry == null)
                {
                    Debug.Log(cursorName + " | " + path + " | " + File.Exists(path));
                    Debug.Break();
                }
#endif
                
                return returnEntry;
            }
        }

        if (cursorName != null)
        {
            string filePath = Path.Combine(folderPath + cursorName);

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath, System.Text.Encoding.ASCII);
                CursorSaveEntry returnEntry = (CursorSaveEntry) JsonUtility.FromJson(json, typeof(CursorSaveEntry));
                return returnEntry;
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning("File with name: " + cursorName + "  does not exist. Perhaps save first?");
#endif
                return null;
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning("Cursor name is null, path: " + path + "\nReturning null...");
#endif
            return null;
        }
    }

    public CursorSaveEntry JsonDeserialize(string fullPath)
    {
        if (fullPath != null)
        {
            string json = File.ReadAllText(fullPath, Encoding.ASCII);
            CursorSaveEntry returnEntry = null;

            try
            {
                returnEntry = (CursorSaveEntry) JsonUtility.FromJson(json, typeof(CursorSaveEntry));
            }
            catch (System.Exception e)
            {
#if UNITY_EDITOR
                Debug.LogWarning(e);
                Debug.Break();
#endif
            }

            if (returnEntry == null)
            {
#if UNITY_EDITOR
                Debug.Log("File exists: " + File.Exists(fullPath) + " Path: " + fullPath);
                Debug.Break();
#endif
            }

            return returnEntry;
        }

        return null; // the path was not passed
    }
}
#pragma warning restore 0649