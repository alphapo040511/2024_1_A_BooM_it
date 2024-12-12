using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementList : MonoBehaviour
{
    public GameObject textPregabs;
    private Dictionary<string, GameObject> awards = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        LoadAwardList();
    }

    public void LoadAwardList()
    {
        foreach(var data in AwardList.awardList)
        {
            GameObject awardList = Instantiate(textPregabs, transform);
            GameObject temp = UpdateList(data.Value, awardList);
            awards.Add(data.Key, temp);
        }
    }

    public void ReloadAllAwardList()
    {
        foreach (var award in awards)
        {
            if (AwardList.awardList.ContainsKey(award.Key))
            {
                UpdateList(AwardList.awardList[award.Key], award.Value);
            }
        }
    }


    public void ReloadAwardList(string key)
    {
        if (AwardList.awardList.ContainsKey(key) && awards.ContainsKey(key))
        {
            UpdateList(AwardList.awardList[key], awards[key]);
        }
    }

    private GameObject UpdateList(AwardData data, GameObject awardList)
    {
        // RectTransform ¼³Á¤
        RectTransform rectTransform = awardList.GetComponent<RectTransform>();

        TextMeshProUGUI awardName = awardList.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI description = awardList.transform.Find("Description").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI value = awardList.transform.Find("Value").GetComponent<TextMeshProUGUI>();
        RectTransform sliderValue = awardList.transform.Find("SliderValue").GetComponent<RectTransform>();
        Image achieved = awardList.transform.Find("Achieved").GetComponent<Image>();

        data.currentValue = PlayerPrefs.GetInt(data.awardName, data.currentValue);

        awardName.text = data.awardName;
        description.text = data.awardDescription;
        value.text = $"{PlayerPrefs.GetInt(data.awardName, data.currentValue)} / {data.goalValue}";

        float size = (data.currentValue / (float)data.goalValue);
        size = Mathf.Clamp01(size);
        sliderValue.localScale = new Vector3(size, 1, 1);
        if (data.currentValue >= data.goalValue || data.isAchieved)
        {
            data.isAchieved = true;
            achieved.enabled = true;
        }
        else
        {
            achieved.enabled = false;
        }

        return awardList;
    }
}
