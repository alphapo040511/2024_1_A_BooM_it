using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialButton : MonoBehaviour
{
    public GameObject button;

    public TextMeshProUGUI title;
    public TextMeshProUGUI stageName;
    public TextMeshProUGUI description;

    public Image image;

    private string sceneName;

    public void NewData(TutorialData data)
    {
        sceneName = data.SceneName;
        title.text = data.Title;
        stageName.text = data.StageName;
        description.text = data.Description;

        image.sprite = data.stageImage;

        button.gameObject.SetActive(true);
        title.enabled = true;
    }

    public void DeleteData()
    {
        button.gameObject.SetActive(false);
        title.enabled = false;
    }

    public void LoadTutorial()
    {
        if (sceneName != null)
        {
            SceneLoadManager.instance.LoadScene(sceneName);
        }
    }
}
