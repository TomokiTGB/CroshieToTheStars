using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class AudioManager : MonoBehaviour
{
    //public static SoundManager instance { get; private set; }

    public AudioMixer masterMixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    private float musicVolume;
    private float sfxVolume;
    private void Awake()
    {
        LoadVolumeToSliders();
    }
    private void OnEnable()
    {
        LoadVolumeToSliders();
    }
    // Start is called before the first frame update
    void Start()
    {
        //thanks to {} for the help
        //https://docs.unity3d.com/540/Documentation/ScriptReference/UI.Slider-onValueChanged.html
        musicSlider.onValueChanged.AddListener(delegate { MasterValueChange(); });
        sfxSlider.onValueChanged.AddListener(delegate { SFXValueChange(); });
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void LoadVolumeToSliders()
    {
        masterMixer.GetFloat("MusicVolume", out musicVolume); // gets the "Music"(in Mixer)'s value and gives it to "musicVolume"
        musicSlider.value = musicVolume; // sets the value of "musicSlider" to "musicVolume"

        masterMixer.GetFloat("SfxVolume", out sfxVolume);
        sfxSlider.value = sfxVolume;
    }
    public void MasterValueChange()
    {
        //Debug.Log(musicSlider.value);
        masterMixer.SetFloat("MusicVolume", musicSlider.value);
    }
    public void SFXValueChange()
    {
        //Debug.Log(sfxSlider.value);
        masterMixer.SetFloat("SfxVolume", sfxSlider.value);
    }
}
