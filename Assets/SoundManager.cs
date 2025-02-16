using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;

    void Start()
    {

    }
    

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }
}
   