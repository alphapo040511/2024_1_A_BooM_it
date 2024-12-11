using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LoadingEffect : MonoBehaviour
{
    private float duration;

    private float startHeight;

    void Start()
    {
        startHeight = GetComponent<RectTransform>().localPosition.y;
        StartCoroutine(Movement());
    }

    private IEnumerator Movement()
    {
        while (true)
        {
            duration = Random.Range(0, 5f);
            yield return new WaitForSeconds(duration);
        
            while (GetComponent<RectTransform>().localPosition.y >= startHeight - 120)
            {
                GetComponent<RectTransform>().localPosition -= Vector3.up * 60 * Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(duration * 0.5f);

            while (GetComponent<RectTransform>().localPosition.y <= startHeight)
            {
                GetComponent<RectTransform>().localPosition += Vector3.up * 60 * Time.deltaTime;
                yield return null;
            }
        }
    }
}
