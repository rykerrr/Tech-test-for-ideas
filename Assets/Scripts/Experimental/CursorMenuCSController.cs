using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

#pragma warning disable 0649
public class CursorMenuCSController : MonoBehaviour
{
    [Header("UI References")] [SerializeField]
    private Image cursorPreview;

    [SerializeField] private RawImage cursorDot;
    [SerializeField] private Image cursorSlave;
    [SerializeField] private Transform crosshairContainer; // 0 - left 1 - right 2 - up 3 - down

    [SerializeField] private CsCustomCursorEntry properties;

    [Header("Saving settings")] [SerializeField]
    private string csCursorSavePath = "Resources/CustomCursorSaves/CS_Cursor";

    [SerializeField] private string saveFileName = "CS_Cursor1";

    private RectTransform[] crosshairLinesTransforms; // 0 - left 1 - right 2 - up 3 - down
    private RectTransform dotTransform;
    private Image[] crosshairLinesImages;
    private CursorSaveLoad saveLoadUtility;

    private void Awake() // Initialization
    {
        int childCount = crosshairContainer.childCount;

        crosshairLinesTransforms = new RectTransform[childCount];
        crosshairLinesImages = new Image[childCount];
        
        for (int i = 0; i < childCount; i++)
        {
            Transform child = crosshairContainer.GetChild(i);

            try
            {
                crosshairLinesImages[i] = child.GetComponent<Image>();
                crosshairLinesTransforms[i] = child.GetComponent<RectTransform>();
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.LogWarning("" + e + "\n| i: " + i);
                Debug.Break();
#endif
            }
        }


        dotTransform = cursorDot.GetComponent<RectTransform>();
        saveLoadUtility = new CursorSaveLoad();

        OnClick_ReloadProperties();
    }

    private void Update()
    {
        UpdateCrosshairLineRectangles();
        UpdateDotRectangle();
        UpdateCrosshairDistances();
        UpdateCrosshairLocalRotation();
        UpdateCrosshairRotationAroundCursor();
        UpdateDotLocalRotation();
        UpdateColors();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            // SaveCursorImage();
            TrySaveCursorImage();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(crosshairContainer);

            foreach (var obj in crosshairLinesImages)
            {
                Debug.Log("" + obj.name);
            }
            
        }
    }

    private void LoadSavedCursor()
    {
        string fullPath = Path.Combine(Application.dataPath, csCursorSavePath, saveFileName);

        if (File.Exists(fullPath))
        {
        }
        else
        {
            Debug.LogWarning("File doesn't exist...full path: " + fullPath);
        }
    }

    private void UpdateColors()
    {
        // cursorPreview.color = new Color(cursorColor.r, cursorColor.g, cursorColor.b, 0f);
        //
        // Color newDotColor = dotColor * cursorColor;
        // newDotColor = new Color(newDotColor.r, newDotColor.g, newDotColor.b, dotColor.a);
        // Color newCrosshairColor = lineColor * cursorColor;
        // newCrosshairColor = new Color(newCrosshairColor.r, newCrosshairColor.g, newCrosshairColor.b, lineColor.a);
        //
        // cursorDot.color = newDotColor;
        //
        // foreach (Image line in crosshairLinesImages)
        // {
        //     line.color = newCrosshairColor;
        // }
    }


    private void UpdateDotRectangle()
    {
        dotTransform.sizeDelta = new Vector2(properties.Radius, properties.Radius);
    }

    private void UpdateCrosshairLineRectangles()
    {   // left right up down
        crosshairLinesTransforms[0].sizeDelta = new Vector2(properties.LineLength, properties.LineWidth);
        crosshairLinesTransforms[1].sizeDelta = new Vector2(properties.LineLength, properties.LineWidth);
        crosshairLinesTransforms[2].sizeDelta = new Vector2(properties.LineWidth, properties.LineLength);
        crosshairLinesTransforms[3].sizeDelta = new Vector2(properties.LineWidth, properties.LineLength);
    }

    private void UpdateDotLocalRotation()
    {
        dotTransform.localEulerAngles = new Vector3(0f, 0f, properties.DotLocalRotation);
    }

    private void UpdateCrosshairLocalRotation()
    {
        crosshairLinesTransforms[0].localEulerAngles = new Vector3(0f, 0f, properties.LineLocalRotation);
        crosshairLinesTransforms[1].localEulerAngles = new Vector3(0f, 0f, properties.LineLocalRotation);
        crosshairLinesTransforms[2].localEulerAngles = new Vector3(0f, 0f, properties.LineLocalRotation);
        crosshairLinesTransforms[3].localEulerAngles = new Vector3(0f, 0f, properties.LineLocalRotation);
    }

    private void UpdateCrosshairRotationAroundCursor()
    {
        crosshairContainer.localEulerAngles = new Vector3(0f, 0f, properties.CrossPivotRotation);
    }

    private void UpdateCrosshairDistances()
    {
        crosshairLinesTransforms[0].localPosition = new Vector3(-properties.LineDistanceFromCenter, 0f, 0f);
        crosshairLinesTransforms[1].localPosition = new Vector3(properties.LineDistanceFromCenter, 0f, 0f);
        crosshairLinesTransforms[2].localPosition = new Vector3(0f, properties.LineDistanceFromCenter, 0f);
        crosshairLinesTransforms[3].localPosition = new Vector3(0f, -properties.LineDistanceFromCenter, 0f);
    }

    private Texture2D CombineTextures(Texture2D aBaseTexture, Texture2D aToCopyTexture) // combines them in a nutshell
    {
        // idk if this works but the merge one is probably better lol
        // if it merges it probably merges around the center point

        int aWidth = aBaseTexture.width;
        int aHeight = aBaseTexture.height;

        Texture2D aReturnTexture = new Texture2D(aWidth, aHeight, TextureFormat.RGBA32, false);

        Color[] aBaseTexturePixels = aBaseTexture.GetPixels();
        Color[] aCopyTexturePixels = aToCopyTexture.GetPixels();
        Color[] aColorList = new Color[aBaseTexturePixels.Length];
        int aPixelLength = aBaseTexturePixels.Length;

        for (int p = 0; p < aPixelLength; p++)
        {
            aColorList[p] = Color.Lerp(aBaseTexturePixels[p], aCopyTexturePixels[p], aCopyTexturePixels[p].a);
        }

        aReturnTexture.SetPixels(aColorList);
        aReturnTexture.Apply(false);

        return aReturnTexture;
    }

    private void TrySaveCursorImage() // Putting on hold temporarily til i figure out what the heckarooni to do
    {
        // textureTest.texture = crosshairLinesImages[0].mainTexture; // this is not the way
        Texture2D rightLeft = CombineTextures(crosshairLinesImages[0].mainTexture as Texture2D,
            crosshairLinesImages[1].mainTexture as Texture2D);
        Texture2D upDown = CombineTextures(crosshairLinesImages[2].mainTexture as Texture2D,
            crosshairLinesImages[3].mainTexture as Texture2D);
        Texture2D crosshair = CombineTextures(rightLeft, upDown);
        Texture2D entireCursor = CombineTextures(crosshair, cursorDot.mainTexture as Texture2D);

        // textureTest.texture = entireCursor;
        Sprite sprite = Sprite.Create(entireCursor, new Rect(0f, 0f, entireCursor.width, entireCursor.height),
            new Vector2(0.5f, 0.5f));

#if UNITY_EDITOR
        Debug.Log("b");
#endif
    }

    public void OnClick_ReloadProperties()
    {
        CsCursorSaveEntry entry =
            saveLoadUtility.CsCursorJsonDeserialize();

        if (entry == null)
        {
            Debug.LogWarning("Entry is: " + entry + ", save file probably doesn't exist?");
        }
        else
        {
            properties.LoadPropsFromSave(entry);
        }
    }

    public void OnClick_SaveProperties()
    {
        saveLoadUtility.CsCursorJsonSerialize(properties);
    }

    /*private void SaveCursorImage()
    {
        // 0 - left // 1 - right // 2 - up // 3 down
        // idk if this'll work because it's not actually a sprite but it is a drawn rectangle...maybe add a random sprite and see how it goes?

        Texture2D[] crosshairTextures = new Texture2D[crosshairLinesTransforms.Length];
        Color[] pixels = new Color[512 * 512];
        Texture2D textureTempVar;

        // Create a texture the size of the lines combined
        // from pixel left x, bot y, to top y, right x
        float distY = Vector2.Distance(new Vector2(0f, crosshairLinesTransforms[2].position.y + crosshairLinesTransforms[2].lossyScale.y / 2f),
                                       new Vector2(0f, crosshairLinesTransforms[3].position.y - crosshairLinesTransforms[3].lossyScale.y / 2f));
        float distX = Vector2.Distance(new Vector2(crosshairLinesTransforms[0].position.x - crosshairLinesTransforms[0].lossyScale.x / 2f, 0f),
                                       new Vector2(crosshairLinesTransforms[1].position.x + crosshairLinesTransforms[1].lossyScale.x / 2f, 0f));
        Texture2D resultingTexture = new Texture2D((int)distX, (int)distY);

        for (int i = 0; i < crosshairTextures.Length; i++)
        {
            Rect lineRect = crosshairLinesImages[i].GetPixelAdjustedRect();

            crosshairTextures[i] = new Texture2D((int)lineRect.width, (int)lineRect.height);
            crosshairLinesImages[i].material.mainTexture = crosshairTextures[i];

            textureTempVar = crosshairLinesImages[i].mainTexture as Texture2D;
            pixels = textureTempVar.GetPixels();

            // pixels = crosshairLinesImages[i].material.mainTexture.

            //pixels = lineSprite.texture.GetPixels((int)lineSprite.textureRect.x, (int)lineSprite.textureRect.y,
            //                                      (int)lineSprite.textureRect.width, (int)lineSprite.textureRect.height);
            crosshairTextures[i].SetPixels(pixels);
            crosshairTextures[i].Apply();
        }

        Sprite dotSprite = cursorDot.sprite;
        Rect dotRect = cursorDot.GetPixelAdjustedRect();
        Texture2D dotTexture = new Texture2D((int)dotRect.width, (int)dotRect.height);

        textureTempVar = cursorDot.mainTexture as Texture2D;
        pixels = textureTempVar.GetPixels();

        //pixels = dotSprite.texture.GetPixels((int)dotSprite.textureRect.x, (int)dotSprite.textureRect.y,
        //                                     (int)dotSprite.textureRect.width, (int)dotSprite.textureRect.height);

        dotTexture.SetPixels(pixels);
        dotTexture.Apply();

        // Debug.Log(resultingTexture.GetPixels().Length); // 4624 LOL
        foreach (Texture2D crosshairLine in crosshairTextures)
        {
            pixels = crosshairLine.GetPixels(); // we get 200 pixels here
            resultingTexture.SetPixels(pixels); // but we need more to fill this up
            resultingTexture.Apply();
        }

        pixels = dotTexture.GetPixels();
        resultingTexture.SetPixels(pixels);
        resultingTexture.Apply();

        textureTest.texture = resultingTexture;

        // Now we have both the dot and crosshair textures, all that's left is to combine them



        // Get the textures for each and combine them

    }*/
}
#pragma warning restore 0649