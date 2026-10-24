using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Menu
{
    public class AudioOptions : MonoBehaviour
    {
    
        [SerializeField] private Slider voiceChatVolumeSlider, masterVolumeSlider, musicVolumeSlider, SFXVolumeSlider;
        [SerializeField] private AudioMixer masterMixer;
        private const string MASTER_VOLUME_GROUP = "Mastervolume", MUSIC_VOLUME_GROUP = "Musicvolume", SFX_VOLUME_GROUP = "SFXvolume", VOICE_CHAT_VOLUME_GROUP = "VoiceChatVolume";
    
        private 
            //TODO: Experiment with music playing
            void Start()
        {
            SetupListeners();
        }
        private void SetupListeners()
        {
            voiceChatVolumeSlider.onValueChanged.AddListener(OnVoiceChatVolumeChanged);
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            SFXVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
    
    
    
    

        private void OnVoiceChatVolumeChanged(float value)
        {
            Debug.Log("VOICE CHAT VOLUME: " + value);
            masterMixer.SetFloat(VOICE_CHAT_VOLUME_GROUP, masterVolumeSlider.value);
        }
        private void OnMasterVolumeChanged(float value)
        {
            Debug.Log("MASTER VOLUME: " + value);
            masterMixer.SetFloat(MASTER_VOLUME_GROUP, masterVolumeSlider.value);
        }
        private void OnMusicVolumeChanged(float value)
        {
        
            Debug.Log("MUSIC VOLUME: " + value);
            masterMixer.SetFloat(MUSIC_VOLUME_GROUP, masterVolumeSlider.value);
        }
        private void OnSFXVolumeChanged(float value)
        {
            Debug.Log("SFX VOLUME: " + value);
            masterMixer.SetFloat(SFX_VOLUME_GROUP, masterVolumeSlider.value);
        }
    }
}
