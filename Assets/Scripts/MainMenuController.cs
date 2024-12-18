using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenu;

    public Menu Menu;

    public RectTransform selectBar;

    public AudioSource audioSource;
    public AudioClip selectSound;

    private GameObject nowTab;
    private Stack<GameObject> tabStack = new Stack<GameObject>();

    void Start()
    {
        nowTab = mainMenu;
        tabStack = new Stack<GameObject>();
        DOTween.Init();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Back();
        }
    }

    public void SelectButton(RectTransform newTransform)
    {
        selectBar.gameObject.SetActive(true);
        selectBar.anchoredPosition = newTransform.anchoredPosition - Vector2.right * 10;
        selectBar.sizeDelta = newTransform.sizeDelta + Vector2.right * 30;

        audioSource.PlayOneShot(selectSound);
    }

    public void NewTab(GameObject newTab)
    {
        tabStack.Push(nowTab);
        TabMove(nowTab, newTab);
    }

    public void TutorialScene()
    {
        SceneLoadManager.instance.LoadScene("TutorialLobbyScene");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Back()
    {
        GameObject targetTab;
        if(tabStack.Count > 0)
        {
            targetTab = tabStack.Pop();
            if(targetTab == mainMenu)
            {
                tabStack = new Stack<GameObject>();
            }
        }
        else
        {
            targetTab = mainMenu;
            tabStack = new Stack<GameObject>();
            Menu.TurnTab();
        }
        TabMove(nowTab, targetTab);
    }

    private void TabMove(GameObject nowTab, GameObject newTab)
    {
        this.nowTab = newTab;

        if (nowTab == newTab) return;

        nowTab.GetComponent<RectTransform>().DOLocalMoveX(1920, 0.5f).SetEase(Ease.OutCubic);
        newTab.GetComponent<RectTransform>().DOLocalMoveX(0, 0.5f).SetEase(Ease.OutCubic);
    }
}
