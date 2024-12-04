using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Image image;
    public Sprite[] icons = new Sprite[] { };

    private void Start()
    {
        StartCoroutine(LoadingIcons());
        string name = SceneLoadManager.instance.targetSceneName;
        if(SceneManager.GetSceneByName(name) != null)
        {
            if (name == "ServerConnecting") return;

            StartCoroutine(LoadingAsync(name));
        }
        else
        {
            StartCoroutine(LoadingAsync("LobbyScene"));
        }    
    }

    private IEnumerator LoadingAsync(string name)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name);

        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        asyncOperation.allowSceneActivation = true;
    }

    private IEnumerator LoadingIcons()
    {
        int index = 0;
        while (true)
        {
            int random = Random.Range(0, icons.Length);
            if(random == index)
            {
                index = (int)Mathf.Repeat(random++, icons.Length - 1);
            }
            else
            {
                index = random;
            }

            Sprite targetSprite = icons[index];
            image.sprite = targetSprite;

            Color color = image.color;
            color.a = 0;

            while (color.a < 1)
            {
                color.a += Time.deltaTime;
                image.color = color;
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            while (color.a > 0)
            {
                color.a -= Time.deltaTime;
                image.color = color;
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
