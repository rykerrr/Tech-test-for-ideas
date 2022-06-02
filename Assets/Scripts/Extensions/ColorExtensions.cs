using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtensions
{
    public static Color32 HexToColor32(this Color32 color, string hex)
    {
        if (hex.Contains("0x"))
        {
            hex = hex.Remove(hex.IndexOf("0x"), 2);
        }
        else if (hex.Contains("#"))
        {
            hex = hex.Remove(hex.IndexOf("#"), 1);
        }
        else if (hex.Contains("x") || hex.Contains("X"))
        {
            hex = hex.Remove(hex.ToLower().IndexOf("x"), 1);
        }

        string[] hexNumbers = new string[3];
        hexNumbers[0] = hex[0].ToString() + hex[1].ToString();
        hexNumbers[1] = hex[2].ToString() + hex[3].ToString();
        hexNumbers[2] = hex[4].ToString() + hex[5].ToString();

        byte[] numbers = new byte[3];
        numbers[0] = byte.Parse(hexNumbers[0], System.Globalization.NumberStyles.HexNumber); // r
        numbers[1] = byte.Parse(hexNumbers[1], System.Globalization.NumberStyles.HexNumber); // g
        numbers[2] = byte.Parse(hexNumbers[2], System.Globalization.NumberStyles.HexNumber); // b

        return new Color32(numbers[0], numbers[1], numbers[2], 255);
    }

    public static string Color32ToHex(this Color32 color)
    {
        string hex = "0x";
        string r = Convert.ToString(color.r, 16);
        string g = Convert.ToString(color.g, 16);
        string b = Convert.ToString(color.b, 16);

        hex += r + g + b;
        
        return hex;
    }

    public static bool CompareColor32SWithAlpha(this Color32 a, Color32 b)
    {
        if (a.r == b.r && a.b == b.b && a.g == b.g && a.a == b.a)
        {
            return true;
        }

        return false;
    }
    
    public static bool CompareColor32SWithoutAlpha(Color32 a, Color32 b)
    {
        if (a.r == b.r && a.b == b.b && a.g == b.g)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
