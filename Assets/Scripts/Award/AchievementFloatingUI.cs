using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AchievementFloatingUI : MonoBehaviour
{
    public static AchievementFloatingUI instance;
    public GameObject textPregabs;

    private Queue<GameObject> popupObject = new Queue<GameObject>();

    private bool isWork = false;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if(popupObject.Count > 0 && isWork == false)
        {
            StartCoroutine(AnimateText(popupObject.Dequeue()));
        }
    }

    public void ShowAchievementPopup(AwardData data)
    {
        GameObject popup = Instantiate(textPregabs, transform);

        // RectTransform ¼³Á¤
        RectTransform rectTransform = popup.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 1f); // ÇÏ´Ü Áß¾Ó ¾ÞÄ¿
        rectTransform.anchorMax = new Vector2(0.5f, 1f); // ÇÏ´Ü Áß¾Ó ¾ÞÄ¿
        rectTransform.pivot = new Vector2(0.5f, 1f);     // Pivot ¼³Á¤
        rectTransform.anchoredPosition = new Vector2(0f, 160f);

        TextMeshProUGUI awardName = popup.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI description = popup.transform.Find("Description").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI value = popup.transform.Find("Value").GetComponent<TextMeshProUGUI>();
        RectTransform sliderValue = popup.transform.Find("SliderValue").GetComponent<RectTransform>();
        Image achieved = popup.transform.Find("Achieved").GetComponent<Image>();

        awardName.text = data.awardName;
        description.text = data.awardDescription;
        value.text = $"{data.currentValue} / {data.goalValue}";

        float size = ((float)data.currentValue / (float)data.goalValue);
        size = Mathf.Clamp01(size);
        sliderValue.localScale = new Vector3(size, 1, 1);
        if(data.currentValue >= data.goalValue || data.isAchieved)
        {
            data.isAchieved = true;
            achieved.enabled = true;
        }
        else
        {
            achieved.enabled = false;
        }


        popupObject.Enqueue(popup);
    }

    private IEnumerator AnimateText(GameObject target)
    {
        isWork = true;

        RectTransform rectTransform = target.GetComponent<RectTransform>();
        while (rectTransform.anchoredPosition.y >= 0)
        {
            rectTransform.anchoredPosition += Vector2.up * -200 * Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.75f);

        while (rectTransform.anchoredPosition.y <= 160)
        {
            rectTransform.anchoredPosition += Vector2.up * 200 * Time.deltaTime;
            yield return null;
        }

        Destroy(target);

        isWork = false;
    }
}
