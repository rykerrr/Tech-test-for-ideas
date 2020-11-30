using UnityEngine;

#pragma warning disable 0649
public class CursorSaveEntry
{
    public CursorSaveEntry(NewCursorEntry cursorEntry)
    {
        cursorColor = cursorEntry.GetCursorColor;
        cursorName = cursorEntry.GetCursorName;
        textureName = cursorEntry.GetCursorTexture.name;
        textureItself = cursorEntry.GetCursorTexture;
        spriteItself = cursorEntry.GetCursorSprite;
    }

    public CursorSaveEntry(string cursorName, string textureName, Color cursorColor)
    {
        this.cursorColor = cursorColor;
        this.cursorName = cursorName;
        this.textureName = textureName;
    }

    public string cursorName;
    public string textureName;
    public Texture2D textureItself; // checking if these can be serialized, textureName will be used otherwise
    public Sprite spriteItself; // checking if these can be serialized, textureName will be used otherwise
    public Color cursorColor;
}
#pragma warning restore 0649