using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public bool isTitleScene = false;
    public GameObject menuUI;

    [SerializeField] private AudioMixer audioMixer;                 //private 선언한 것들을 인스팩터 창에서 보여지게

    [SerializeField] private Slider MasterSlider;              //UI Slider
    [SerializeField] private Slider BGMSlider;                 //UI Slider
    [SerializeField] private Slider SFXSlider;                 //UI Slider

    private CursorLockMode cursorState = CursorLockMode.None;

    //슬라이더 Minvalue을 0.001

    private void Awake()
    {
        MasterSlider.onValueChanged.AddListener(SetMasterVolume);                  //UI Slider의 값이 변경 되었을 경우 SetMasterVolume 함수를 호출 한다.
        BGMSlider.onValueChanged.AddListener(SetBGMVolume);                        //UI Slider의 값이 변경 되었을 경우 SetBGMVolume 함수를 호출 한다.
        SFXSlider.onValueChanged.AddListener(SetSFXVolume);                        //UI Slider의 값이 변경 되었을 경우 SetSFXVolume 함수를 호출 한다.
    }

    public void Update()
    {
        if (!isTitleScene && Input.GetKeyDown(KeyCode.Escape))
        {
            TurnTab();
        }
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);                //볼륨에서의 0 ~ 1 <- Mathf.Log10(volume) * 20
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);                //볼륨에서의 0 ~ 1 <- Mathf.Log10(volume) * 20
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);                //볼륨에서의 0 ~ 1 <- Mathf.Log10(volume) * 20
    }

    public void TurnTab()
    {
        if(menuUI.activeSelf)
        {
            CloseTab();
        }
        else
        {
            OpenTab();
        }
    }

    public void OpenTab()   //혹시나 연출을 추가 할 수 있으므로 분리
    {
        menuUI.SetActive(true);
        ChangeVolumeValue();
        cursorState = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseTab()
    {
        menuUI.SetActive(false);
        Cursor.lockState = cursorState;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Disconnect()
    {
        NetworkManager.Instance.DisconnectClicked();
    }

    private void ChangeVolumeValue()
    {
        float master = 0;
        audioMixer.GetFloat("Master", out master);
        MasterSlider.value = Mathf.Pow(10, master * 0.05f);
        float BGM = 0;
        audioMixer.GetFloat("BGM", out BGM);
        BGMSlider.value = Mathf.Pow(10, BGM * 0.05f);
        float SFX = 0;
        audioMixer.GetFloat("SFX", out SFX);
        SFXSlider.value = Mathf.Pow(10, SFX * 0.05f);
    }

}
