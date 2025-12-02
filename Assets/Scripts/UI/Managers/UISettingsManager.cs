using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UISettingsManager
{
    public Toggle fullScreenToggle;
    public Toggle vsyncToggle;
    public TMP_Dropdown antiAliasingDropdown;
    public Toggle anisotropicFilteringToggle;
    public TMP_Dropdown textureQualityDropdown;
    public Toggle realtimeReflectionsToggle;

    public void Initialize()
    {
        fullScreenToggle.onValueChanged.AddListener(SetFullscreen);
        vsyncToggle.onValueChanged.AddListener(SetVsync);
        antiAliasingDropdown.onValueChanged.AddListener(SetAntiAliasing);
        anisotropicFilteringToggle.onValueChanged.AddListener(SetAnisotropicFiltering);
        textureQualityDropdown.onValueChanged.AddListener(SetTextureQuality);
        realtimeReflectionsToggle.onValueChanged.AddListener(SetRealtimeReflections);
    }
    //settings
    public void SetFullscreen(bool to)
    {
        Screen.fullScreen = to;
    }

    public void SetVsync(bool to)
    {
        QualitySettings.vSyncCount = to ? 1 : 0;
    }

    public void SetAntiAliasing(int level)
    {
        // Dropdown: 0=Off, 1=2x, 2=4x, 3=8x
        int[] aaLevels = { 0, 2, 4, 8 };
        QualitySettings.antiAliasing = aaLevels[level];
    }

    public void SetAnisotropicFiltering(bool enabled)
    {
        QualitySettings.anisotropicFiltering = enabled ? AnisotropicFiltering.Enable : AnisotropicFiltering.Disable;
    }

    public void SetTextureQuality(int quality)
    {
        // 0 = full res, 1 = half res, 2 = quarter res, 3 = eighth res
        QualitySettings.globalTextureMipmapLimit = quality;
    }

    public void SetRealtimeReflections(bool enabled)
    {
        QualitySettings.realtimeReflectionProbes = enabled;
    }
}