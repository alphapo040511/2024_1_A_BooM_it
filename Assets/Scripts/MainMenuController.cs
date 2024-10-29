using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenu;

    public GameObject nowTab;
    public Stack<GameObject> tabStack = new Stack<GameObject>();

    void Start()
    {
        nowTab = mainMenu;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Back();
        }
    }

    public void NewTab(GameObject newTab)
    {
        tabStack.Push(nowTab);
        TabMove(nowTab, newTab);
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
        }
        TabMove(nowTab, targetTab);
    }

    private void TabMove(GameObject nowTab, GameObject newTab)
    {
        this.nowTab = newTab;

        if (nowTab == newTab) return;

        nowTab.GetComponent<RectTransform>().transform.DOMoveX(1920, 0.5f).SetEase(Ease.OutCubic);
        newTab.GetComponent<RectTransform>().transform.DOMoveX(960, 0.5f).SetEase(Ease.OutCubic);
    }
}
