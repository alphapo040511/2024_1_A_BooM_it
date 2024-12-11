using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    public TextMeshProUGUI errorMessege;
    public GameObject popup;
    public Image image;
    public Sprite[] icons = new Sprite[] { };

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        StartCoroutine(LoadingIcons());
        string name = SceneLoadManager.instance.targetSceneName;

        StartCoroutine (LoadingIcons());

        if(SceneManager.GetSceneByName(name) != null)
        {
            if (name == "ServerConnecting" || name == "DataLoading") return;

            StartCoroutine(LoadingAsync(name));
        }
        else
        {
            StartCoroutine(LoadingAsync("LobbyScene"));
        }    
    }

    private IEnumerator Timer()
    {
        float timer = 0;
        timer += Time.deltaTime;
        if (timer >= 15)
        {
            string reason = "";
            switch (SceneLoadManager.instance.targetSceneName)
            {
                case "DataLoading":
                    reason = "데이터를 불러올 수 없습니다.";
                    break;
                case "ServerConnecting":
                    reason = "서버에 연결할 수 없습니다.";
                    break;
                default:
                    reason = "데이터 처리 제한 시간이 초과되었습니다.";
                    break;
            }
            errorMessege.text = reason;
            popup.SetActive(true);
            yield break;
        }
    }

    private IEnumerator LoadingAsync(string name)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            if(asyncOperation.progress >= 0.9f)
            {
                break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        asyncOperation.allowSceneActivation = true;

        yield break;
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

    public void ClickExit()
    {
        Application.Quit();
    }
}
