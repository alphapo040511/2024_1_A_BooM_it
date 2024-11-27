using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowImage : MonoBehaviour
{
    public void Follow(RectTransform pos)
    {
        GetComponent<RectTransform>().position = pos.position;
    }
}
