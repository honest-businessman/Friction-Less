using UnityEngine;

public static class UtilityScript
{
    public enum BrightnessMode { Add, Multiply, HSV, HDR }

    public static Color AdjustBrightness(Color c, float amount, BrightnessMode mode)
    {
        switch (mode)
        {
            case BrightnessMode.Add:
                return new Color(
                    Mathf.Clamp01(c.r + amount),
                    Mathf.Clamp01(c.g + amount),
                    Mathf.Clamp01(c.b + amount),
                    c.a
                );

            case BrightnessMode.Multiply:
                return new Color(
                    Mathf.Clamp01(c.r * amount),
                    Mathf.Clamp01(c.g * amount),
                    Mathf.Clamp01(c.b * amount),
                    c.a
                );

            case BrightnessMode.HSV:
                Color.RGBToHSV(c, out float h, out float s, out float v);
                v = Mathf.Clamp01(v + amount);
                return Color.HSVToRGB(h, s, v);

            case BrightnessMode.HDR:
                return c * amount; // no clamp
        }

        return c;
    }
}
