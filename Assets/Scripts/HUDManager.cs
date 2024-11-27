using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HUDManager : MonoBehaviour
{
    public BattleManager battleManager;
    public RectTransform[] blue_Stars = new RectTransform[3];
    public RectTransform[] red_Stars = new RectTransform[3];

    public RectTransform finish;
    public RectTransform win;
    public RectTransform lose;

    public AudioSource audioSurce;
    public AudioClip audioClip;

    private bool isOver;
    private bool isWin;

    //private Sequence pointUpdate;

    void Start()
    {
        //pointUpdate = DOTween.Sequence().SetAutoKill(false).Pause();
        //pointUpdate.Append(finish.DOMove(new Vector3(960, 540), 1));
    }

    public void AddPoint(bool isMyPoint, int point)
    {
        if(point >= 3) 
        { 
            isOver = true;
            if (isMyPoint)
            {
                isWin = true;
                GameManager.instance.AwardAddValue("Win", 1);
            }
            else
            {
                isWin = false;
            }
        }

        Debug.Log(isMyPoint);
        if(isMyPoint)
        {
            StartCoroutine(StarTurnOn(blue_Stars[point - 1], point));
            GameManager.instance.AwardAddValue("Kill", 1);
        }
        else
        {
            StartCoroutine(StarTurnOn(red_Stars[point - 1], point));
            GameManager.instance.AwardAddValue("Die", 1);
        }
    }

    private IEnumerator StarTurnOn(RectTransform target, int point)
    {
        audioSurce.PlayOneShot(audioClip);

        if (isOver)
        {
            if (isWin)
            {
                win.gameObject.SetActive(true);
            }
            else
            {
                lose.gameObject.SetActive(true);
            }

        }

        finish.DOMove(new Vector3(960, 540, 0), 1f);
        yield return new WaitForSeconds(1);

        float size = 0;
        target.localScale = Vector2.one * size;       //?????? ?????? ?????? ???? ????
        target.gameObject.SetActive(true);
        while (size < 1)
        {
            size += Time.deltaTime * 4f;
            target.localScale = Vector2.one * size;
            yield return null;
        }

        yield return new WaitForSeconds(isOver ? 3 : 1);
        battleManager.UpdatePointComplete();
        yield break;
    }
}
