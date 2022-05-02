using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
