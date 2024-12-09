using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    public RectTransform finishImage;
    public TutorialInfomation info;
    public AudioClip audioClip;
    public AudioSource audioSource;
    private bool isClear = false;
    private TutorialType type;
    private int goalValue;
    private int currentValue;

    private void Awake()
    {
        instance = this;
    }

    public void Init(TutorialType type, int value)
    {
        this.type = type;
        goalValue = value;
        info.UpdateData(0, goalValue);
    }

    public void AddPoint(TutorialType type)
    {
        if (this.type != type) return;

        currentValue++;

        info.UpdateData(currentValue, goalValue);

        if(currentValue >= goalValue)
        {
            StageClear();
        }
    }

    public void ResetPoint()
    {
        currentValue = 0;
        info.UpdateData(currentValue, goalValue);
    }

    public void StageClear()
    {
        if (isClear) return;
        isClear = true;
        audioSource.PlayOneShot(audioClip);
        StartCoroutine(Finish());
        Debug.Log("스테이지 클리어");
    }

    private IEnumerator Finish()
    {
        finishImage.DOLocalMove(Vector2.zero, 1f);
        yield return new WaitForSeconds(4);
        SceneLoadManager.instance.LoadScene("TutorialLobbyScene");
    }
}
