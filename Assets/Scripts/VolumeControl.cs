using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeControl : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string labelText = "Volume";
    [SerializeField] private string mixerParameter = "MasterVolume";
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
        
        float savedValue = PlayerPrefs.GetFloat(saveKey, 1.0f);
        volumeSlider.value = savedValue;

        volumeSlider.onValueChanged.AddListener(HandleSliderChange);
        HandleSliderChange(savedValue);
    }

    private void HandleSliderChange(float value)
    {
        PlayerPrefs.SetFloat(saveKey, value);

        if (audioMixer != null)
        {
            float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1.0f)) * 20;
            audioMixer.SetFloat(mixerParameter, dB);
        }
    }
}
