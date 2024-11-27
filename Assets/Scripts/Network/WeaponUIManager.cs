using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUIManager : MonoBehaviour
{
    public static WeaponUIManager instance;

    public Image currentWeapon;
    public Image previousWeapon;
    public Image nextWeapon;
    public Image itemImage;

    public AudioSource AudioSource;
    public AudioClip AudioClip;

    private void Awake()
    {
        instance = this;
    }

    public void SetWeapons(Sprite current, Sprite previous, Sprite next, Sprite item)
    {
        currentWeapon.sprite = current;
        previousWeapon.sprite = previous;
        nextWeapon.sprite = next;
        itemImage.sprite = item;
    }

    public void ChangeWeapons(Sprite current, Sprite previous, Sprite next)
    {
        currentWeapon.sprite = current;
        previousWeapon.sprite = previous;
        nextWeapon.sprite = next;
        AudioSource.PlayOneShot(AudioClip);
    }

    public void UseItem(float timer)
    {
        StopCoroutine(ItemTimer(timer));
        StartCoroutine(ItemTimer(timer));
    }

    private IEnumerator ItemTimer(float timer)
    {
        float time = 0;
        while(time < timer)
        {
            yield return null;
            time += Time.deltaTime;
            itemImage.fillAmount = Mathf.Clamp01(time / timer);
        }

        yield break;
    }
}
