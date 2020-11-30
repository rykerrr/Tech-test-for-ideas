using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using Object = System.Object;

#pragma warning disable 0649
public class TextureMergerTest : MonoBehaviour
{
    [SerializeField] private Image cursorSlave;
    [SerializeField] private Texture2D[] availableTextures;
    [SerializeField] private RawImage[] crosshairImages;
    [SerializeField] private int[] texInd = new int[2];
    [SerializeField] private int cursWidth = 250;
    [SerializeField] private int cursHeight = 250;

    /*
     * width = (0 + right.localposition + right.width / 2) * 2
     * height = (0 + up.localposition + up.height / 2) * 2
     * x = 0 - width/2
     * y = 0 - height/2
     * ta-dah
     */

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Texture2D[] grayTextures = new Texture2D[crosshairImages.Length]; // off the start its used as storage

            Rect crosshairRect = GetCrosshairRect();
            Rect[] lineRects = new Rect[crosshairImages.Length];

            for (int i = 0; i < lineRects.Length; i++)
            {
                lineRects[i] = crosshairImages[i].GetPixelAdjustedRect();
            }

            // for this loop so we don't need to waste memory on another texture array
            for (int i = 0; i < grayTextures.Length; i++)
            {
                Texture2D tex = crosshairImages[i].texture as Texture2D;
                int width = 3, height = 3;

                grayTextures[i] = new Texture2D(width, height);

                if (tex == null)
                {
                    Color[] pixels = new Color[width * height];

                    for (int j = 0; j < pixels.Length; j++)
                    {
                        pixels[i] = Color.white;
                    }

                    grayTextures[i].SetPixels(pixels); // Yeee?
                }
                else
                {
                    grayTextures[i].SetPixels(tex.GetPixels());
                }

                grayTextures[i].Apply();
            }

            // then again here, it gets grayified
            for (int i = 0; i < grayTextures.Length; i++)
            {
                var grayscale = CreateGrayscaleTexture(grayTextures[i]);
                TextureScale.Point(grayscale.Item1, cursWidth, cursHeight);

                grayTextures[i] = grayscale.Item1;
            }

            TryMergeTextures(grayTextures);
        }
    }

    private Rect GetCrosshairRect() // Now how the fuck do i get the pixels...
    {
        RectTransform right = crosshairImages[2].rectTransform;
        RectTransform up = crosshairImages[3].rectTransform;

        int width = Mathf.FloorToInt((0 + Mathf.Abs(right.localPosition.x) + right.rect.width / 2f) * 2f);
        int height = Mathf.FloorToInt((0 + Mathf.Abs(up.localPosition.y) + up.rect.height / 2f) * 2f);

        int x = 0 - width;
        int y = 0 - height;

        return new Rect(x, y, width, height);
    }

    private (Texture2D, Color[]) CreateGrayscaleTexture(Texture2D inputTexture) // tuples pepe hype
    {
        Color[] pixels = inputTexture.GetPixels();
        Color[] grayPixels = pixels;

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].r = pixels[i].g = pixels[i].b;
        }

        Texture2D grayscale = new Texture2D(inputTexture.width, inputTexture.height);
        grayscale.SetPixels(grayPixels);
        grayscale.filterMode = FilterMode.Point;
        grayscale.wrapMode = TextureWrapMode.Clamp;
        grayscale.Apply();

        return (grayscale, pixels);
    }

    private Texture2D TryMergeTextures(params Texture2D[] textures)
    {
        // making a texture to smack pixels on, creating a 2d array of pixels for each given texture
        // Texture2D.Resize() can also be used btw, create a 250-250 texture each time with it?
        Texture2D newTex = new Texture2D(textures[0].width, textures[0].height);
        Color[] pixelColorArray = new Color[newTex.width * newTex.height];

        Color[][] texArray = new Color[textures.Length][];

        for (int i = 0; i < textures.Length; i++)
        {
            texArray[i] = textures[i].GetPixels();
        }

        for (int x = 0; x < newTex.width; x++)
        {
            for (int y = 0; y < newTex.height; y++)
            {
                int pixIndex = x + (y * newTex.width);

                for (int i = 0; i < textures.Length; i++)
                {
                    Color endPixel = texArray[i][pixIndex]; // each pixel in each texture yee
                    // Can throw an out of bounds exception if the source texture is smaller than the second
                    // one, e.g 2 1, 1 3

                    if (endPixel.a == 1)
                    {
                        // if its not completely transparent
                        pixelColorArray[pixIndex] = endPixel; // caching it to apply at the end
                    }
                    else if (endPixel.a > 0)
                    {
                        // Normal blending based on alpha
                        pixelColorArray[pixIndex] = NormalBlend(pixelColorArray[pixIndex], endPixel);
                    }
                    else // between 0 and 1 (explicitly)
                    {
                    }
                }
            }
        }

        // creating a sprite using the texture

        newTex.SetPixels(pixelColorArray);
        newTex.wrapMode = TextureWrapMode.Clamp;
        newTex.filterMode = FilterMode.Point;
        newTex.Apply();

        MakeSprite(newTex);

        return newTex;
    }

    private Color NormalBlend(Color dest, Color src) // destination, source
    {
        float srcAlpha = src.a;
        float destAlpha = (1 - srcAlpha) * dest.a;

        return dest * destAlpha + src * srcAlpha;
    }

    private void MakeSprite(Texture2D newTex)
    {
        Sprite newSprite = Sprite.Create(newTex, new Rect(0f, 0f, newTex.width, newTex.height), Vector2.one / 2f);
        cursorSlave.sprite = newSprite;
    }
}
#pragma warning restore 0649