using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] string BGMVolume = "BGMVolume";
    [SerializeField] string SFXVolume = "SFXVolume";
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Toggle BGMToggle;
    [SerializeField] Toggle SFXToggle;
    [SerializeField] float multiplier = 30f;
    [SerializeField] bool disableToogleEventBGM;
    [SerializeField] bool disableToogleEventSFX;

    private void Awake()
    {
        BGMSlider.onValueChanged.AddListener(BGMSliderValueChanged);
        SFXSlider.onValueChanged.AddListener(SFXSliderValueChanged);
        BGMToggle.onValueChanged.AddListener(BGMToogleValueChanged);
        SFXToggle.onValueChanged.AddListener(SFXToogleValueChanged);
    }

    private void Start()
    {
        BGMSlider.value = PlayerPrefs.GetFloat(BGMVolume, BGMSlider.value);
        SFXSlider.value = PlayerPrefs.GetFloat(SFXVolume, SFXSlider.value);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(BGMVolume, BGMSlider.value);
        PlayerPrefs.SetFloat(SFXVolume, SFXSlider.value);
    }

    private void BGMSliderValueChanged(float value)
    {
        mixer.SetFloat(BGMVolume, Mathf.Log10(value) * multiplier);
        disableToogleEventBGM = true;
        BGMToggle.isOn = BGMSlider.value > BGMSlider.minValue;
        disableToogleEventBGM = false;
    }

    private void SFXSliderValueChanged(float value)
    {
        mixer.SetFloat(SFXVolume, Mathf.Log10(value) * multiplier);
        disableToogleEventSFX = true;
        SFXToggle.isOn = SFXSlider.value > SFXSlider.minValue;
        disableToogleEventSFX = false;
    }

    private void BGMToogleValueChanged(bool enableBGM)
    {
        if (disableToogleEventBGM)
        {
            return;
        }

        if (enableBGM)
        {
            BGMSlider.value = BGMSlider.maxValue;
        }
        else
        {
            BGMSlider.value = BGMSlider.minValue;
        }
    }

    private void SFXToogleValueChanged(bool enableBGM)
    {
        if (disableToogleEventSFX)
        {
            return;
        }

        if (enableBGM)
        {
            SFXSlider.value = SFXSlider.maxValue;
        }
        else
        {
            SFXSlider.value = SFXSlider.minValue;
        }
    }
}
