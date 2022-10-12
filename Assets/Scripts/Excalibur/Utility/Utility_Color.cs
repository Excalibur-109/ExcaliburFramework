using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Excalibur
{
    public static partial class Utility
    {
        public static Color GetColor(float r, float g, float b)
        {
            r = Clamp(r, 0f, 255f);
            g = Clamp(g, 0f, 255f);
            b = Clamp(b, 0f, 255f);
            return new Color(r / 255f, g / 255f, b / 255f);
        }
    }
}
