using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    public TutorialData[] TutorialDatas = new TutorialData[6];

    public TutorialButton current;
    public TutorialButton next;
    public TutorialButton prev;

    public AudioSource audioSource;
    public AudioClip audioClip;

    private int index = 0;

    public float scroll = 0;

    private void Start()
    {
        UpdateButtons();
    }

    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel") *600 * Time.deltaTime;

        if((scroll > 0 && scrollInput < 0) || (scroll < 0 && scrollInput > 0))
        {
            scroll = 0;
        }

        scroll += scrollInput;
        
        if(scroll >= 1)
        {
            scroll = 0;
            PrevTap();
        }
        else if (scroll <= -1)
        {
            scroll = 0;
            NextTap();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            current.LoadTutorial();
        }

    }


    public void NextTap()
    {
        if (index >= TutorialDatas.Length - 1) return;
        audioSource.PlayOneShot(audioClip);
        index = Mathf.Clamp(index + 1, 0, TutorialDatas.Length - 1);
        UpdateButtons();
    }

    public void PrevTap()
    {
        if (index <= 0) return;
        audioSource.PlayOneShot(audioClip);
        index = Mathf.Clamp(index - 1, 0, TutorialDatas.Length - 1);
        UpdateButtons();
    }

    private void UpdateButtons()
    {

        SendData(current, index);
        SendData(next, index + 1);
        SendData(prev, index - 1);
    }

    private void SendData(TutorialButton button, int index)
    {
        if(index >= 0 && index < TutorialDatas.Length)
        {
            button.NewData(TutorialDatas[index]);
        }
        else
        {
            button.DeleteData();
        }
    }
}
