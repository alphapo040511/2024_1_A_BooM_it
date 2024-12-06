using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowImage : MonoBehaviour
{
    public GameObject image;

    private RectTransform thisRect;
    private RectTransform target;

    private void Awake()
    {
        thisRect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if(target != null)
        {
            if(!image.activeSelf) image.SetActive(true);
            thisRect.position = target.position;
        }
        else
        {
            if (image.activeSelf) image.SetActive(false);
        }
    }

    public void Follow(RectTransform pos)
    {
        target = pos;
    }
}
