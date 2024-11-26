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

    [SerializeField] private AudioMixer audioMixer;                 //private ������ �͵��� �ν����� â���� ��������

    [SerializeField] private Slider MasterSlider;              //UI Slider
    [SerializeField] private Slider BGMSlider;                 //UI Slider
    [SerializeField] private Slider SFXSlider;                 //UI Slider

    private CursorLockMode cursorState = CursorLockMode.None;

    //�����̴� Minvalue�� 0.001

    private void Awake()
    {
        MasterSlider.onValueChanged.AddListener(SetMasterVolume);                  //UI Slider�� ���� ���� �Ǿ��� ��� SetMasterVolume �Լ��� ȣ�� �Ѵ�.
        BGMSlider.onValueChanged.AddListener(SetBGMVolume);                        //UI Slider�� ���� ���� �Ǿ��� ��� SetBGMVolume �Լ��� ȣ�� �Ѵ�.
        SFXSlider.onValueChanged.AddListener(SetSFXVolume);                        //UI Slider�� ���� ���� �Ǿ��� ��� SetSFXVolume �Լ��� ȣ�� �Ѵ�.
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
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);                //���������� 0 ~ 1 <- Mathf.Log10(volume) * 20
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);                //���������� 0 ~ 1 <- Mathf.Log10(volume) * 20
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);                //���������� 0 ~ 1 <- Mathf.Log10(volume) * 20
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

    public void OpenTab()   //Ȥ�ó� ������ �߰� �� �� �����Ƿ� �и�
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
