using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
public class CursorTextureEntry : MonoBehaviour
{
    [SerializeField] private Text textureNameText;
    [SerializeField] private RawImage textureImage;
    [SerializeField] private Image backgroundForSelection;

    public Image GetBackgroundImageForSelection => backgroundForSelection;
    public Texture2D GetTexture => thisTexture;
    public string GetTextureName => textureName;

    private string textureName;
    private Texture2D thisTexture;

    public void SetTextureListingPreferences(string textureName, Texture2D texture, bool updateListing = true)
    {
        this.textureName = textureName;
        thisTexture = texture;
        
        if(updateListing)
        {
            textureImage.texture = thisTexture;
            textureNameText.text = textureName;
        }
    }
}
#pragma warning restore 0649