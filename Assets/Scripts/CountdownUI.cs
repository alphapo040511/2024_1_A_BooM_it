using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CountdownUI : MonoBehaviour
{
    public RectTransform rect;
    public Image uiImage;
    public bool isGoImage;

    private const float fadeInTime = 0.5f;

    private Sequence sizeDown;
    private Sequence positionDown;

    private Vector2 initialPosition;
    private Vector2 initialSize;

    private void Awake()
    {
        initialPosition = rect.position;
        initialSize = rect.sizeDelta;

        sizeDown = DOTween.Sequence().SetAutoKill(false).Pause(); ;

        sizeDown.Append(rect.DOSizeDelta(initialSize * 1.3f, 0));
        sizeDown.Join(uiImage.DOFade(1, 0.01f));

        sizeDown.Append(rect.DOSizeDelta(initialSize, fadeInTime)).SetEase(Ease.Linear);
        sizeDown.Join(uiImage.DOFade(1, fadeInTime));
        sizeDown.Append(uiImage.DOFade(0, 0.2f));

        sizeDown.OnComplete(() => gameObject.SetActive(false));



        positionDown = DOTween.Sequence().SetAutoKill(false).Pause(); ;

        positionDown.Append(rect.DOMove(initialPosition + Vector2.up * 100, 0));
        positionDown.Join(uiImage.DOFade(1, 0.01f));

        positionDown.Append(rect.DOMove(initialPosition, fadeInTime)).SetEase(Ease.Linear);
        positionDown.Append(uiImage.DOFade(0, 0.2f));

        positionDown.OnComplete(() => gameObject.SetActive(false));
    }

    void OnEnable()
    {
        if (isGoImage)
        {
            sizeDown.Restart();
        }
        else
        {
            positionDown.Restart();
        }
    }
}
