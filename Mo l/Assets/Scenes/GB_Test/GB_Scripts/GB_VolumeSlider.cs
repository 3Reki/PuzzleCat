using UnityEngine;
using UnityEngine.UI;

namespace PuzzleCat.Scenes.GB_Test.GB_Scripts
{
    public class GB_VolumeSlider : MonoBehaviour
    {

        Slider slider;

        void Start()
        {
            slider = GetComponent<Slider>();
            slider.value = GB_AudioManager.volumeSlider;
        }

        void Update()
        {
            GB_AudioManager.volumeSlider = slider.value;


        }
    }
}
