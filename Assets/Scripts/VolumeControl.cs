using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

/// Based on the assumption Unity's AudioMixer is being used
public class VolumeControl : MonoBehaviour
{
    [Header("Configuration")]
    
    [Tooltip("The text to display above the slider (e.g. 'Main Audio')")]
    [SerializeField] private string labelText = "Volume";
    [Tooltip("The exposed Parameter name in AudioMixer")]
    [SerializeField] private string mixerParameter = "MasterVolume";
    [Tooltip("Unique ID for saving to PlayerPrefs (e.g. 'Vol_Master')")]
    [SerializeField] private string saveKey = "volume_master";

    [Header("References")]
    
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Text labelComponent;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (labelComponent != null)
        {
            labelComponent.text = labelText;
        }
        
        float savedValue = PlayerPrefs.GetFloat(saveKey, 1.0f); // if volume not set, default to max (1.0f)
        volumeSlider.value = savedValue;

        volumeSlider.onValueChanged.AddListener(HandleSliderChange);
        HandleSliderChange(savedValue);
    }

    private void HandleSliderChange(float value)
    {
        PlayerPrefs.SetFloat(saveKey, value);

        if (audioMixer != null)
        {
            // AudioMixer expects volume in decibels, takes linear slider value and turns it into logarithmic one
            // Electronic audio is flipped, so the range here is from -80dB to 0dB
            // Less negative dB means more volume is "let through", thus louder sound
            float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1.0f)) * 20;
            audioMixer.SetFloat(mixerParameter, dB);
        }
    }
}
