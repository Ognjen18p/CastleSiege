using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour {
    public AudioSource audioSource;
    public AudioClip buttonClickSound;

    public AudioMixer audioMixer;
    public Scrollbar volumeScrollbar;

    void Start() {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        volumeScrollbar.value = savedVolume;
        ChangeVolume();
    }

    public void ChangeVolume() {
        audioSource.PlayOneShot(buttonClickSound);
        float volume = volumeScrollbar.value;

        float volumeDB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;

        audioMixer.SetFloat("MasterVolume", volumeDB);

        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }
}