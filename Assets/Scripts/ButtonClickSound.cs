using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSound : MonoBehaviour
{
    public bool isItem = false;
    void Start()
    {
        if (!isItem) GetComponent<Button>().onClick.AddListener(Click);
        else GetComponent<Button>().onClick.AddListener(ItemChange);

    }

    public void Click()
    {
        SoundManager.Instance.PlayClickSound();
    }

    public void ItemChange()
    {
        SoundManager.Instance.ItemChange();
    }
}
