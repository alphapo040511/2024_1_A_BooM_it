using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class TutorialDescription : MonoBehaviour
{
    public TutorialMapGenerator generator;
    public GameObject popupObj;
    public VideoPlayer videoPlayer;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI anykey;
    public AudioClip audioClip;
    public AudioSource audioSource;

    public List<TutorialVideoData> videoDatas = new List<TutorialVideoData>();

    private int index = 0;
    private bool isContinuable = false;

    void Start()
    {
        if (videoDatas.Count > 0)
        {
            ShowDescription(index);
        }
        else
        {
            popupObj.SetActive(false);
            generator.player.playerState = PlayerState.Playing;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown && popupObj.activeSelf && !Input.GetKeyDown(KeyCode.Escape))
        {
            if (isContinuable)
            {
                audioSource.PlayOneShot(audioClip);
                isContinuable = false;
                index++;
                anykey.enabled = false;
                //StopCoroutine(TextBlink());
                if (index >= videoDatas.Count)
                {
                    Invoke("PlayerStateChange", 0.3f);
                    popupObj.SetActive(false);
                    return;
                }
                ShowDescription(index);
            }
            else
            {
                isContinuable = true;
                //StopCoroutine(TextBlink());
                //StartCoroutine(TextBlink());
                anykey.enabled = true;
            }
        }
        else if(Input.GetKeyDown(KeyCode.F1) && !popupObj.activeSelf && videoDatas.Count > 0)
        {
            index = 0;
            popupObj.SetActive(true);
            ShowDescription(index);
            generator.player.playerState |= PlayerState.Ready;
        }
    }

    private void PlayerStateChange()
    {
        generator.player.playerState = PlayerState.Playing;
    }

    private void ShowDescription(int index)
    {
        videoPlayer.clip = videoDatas[index].videoClip;
        videoPlayer.Play();
        descriptionText.text = videoDatas[index].description;
        popupObj.SetActive(true);
        StartCoroutine(CountinueTimer(videoDatas[index].description.Length));
    }

    private IEnumerator CountinueTimer(int length)
    {
        yield return new WaitForSeconds(length * 0.1f);
        isContinuable = true;
        //StopCoroutine(TextBlink());
        //StartCoroutine(TextBlink());
        anykey.enabled = true;
    }

    private IEnumerator TextBlink()
    {
        Color textColor = anykey.color;
        textColor.a = 0;
        anykey.color = textColor;
        anykey.enabled = true;

        while(true)
        {
            while (textColor.a < 1)
            {
            textColor.a += Time.deltaTime * 2;
            anykey.color = textColor;
            yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            while (textColor.a > 0)
            {
                textColor.a -= Time.deltaTime * 2;
                anykey.color = textColor;
                yield return null;
            }
        }
    }
}
