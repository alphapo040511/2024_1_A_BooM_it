using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialSceneButton : MonoBehaviour
{
    public void Click(string index)
    {
        string name = "Tutorial_" + index;
        SceneManager.LoadScene(name);
    }
}
