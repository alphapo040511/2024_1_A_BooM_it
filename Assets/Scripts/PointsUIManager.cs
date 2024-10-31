using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsUIManager : MonoBehaviour
{
    public BattleManager battleManager;
    public RectTransform[] blue_Stars = new RectTransform[3];
    public RectTransform[] red_Stars = new RectTransform[3];

    public void AddPoint(bool isMyPoint, int point)
    {
        Debug.Log(isMyPoint);
        if(isMyPoint)
        {
            StartCoroutine(StarTurnOn(blue_Stars[point - 1]));
        }
        else
        {
            StartCoroutine(StarTurnOn(red_Stars[point - 1]));
        }
    }

    private IEnumerator StarTurnOn(RectTransform target)
    {
        float size = 0;
        target.localScale = Vector2.one * size;
        target.gameObject.SetActive(true);
        while (size < 1)
        {
            Debug.Log(size);
            size += Time.deltaTime * 3f;
            target.localScale = Vector2.one * size;
            yield return null;
        }

        battleManager.UpdatePointComplete();
    }
}
