using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelector : MonoBehaviour
{
    public Image currentMapImage;
    public Image nextMapImage;
    public Image previousMapImage;

    private int index = 0;
    private MapData[] _maps;


    // Start is called before the first frame update
    void Start()
    {
        _maps = Resources.LoadAll<MapData>("CompletedData");
        UpdateMapData();
    }

    private void UpdateMapData()
    {
        currentMapImage.sprite = _maps[(int)Mathf.Repeat(index, _maps.Length)].MapImage;
        nextMapImage.sprite = _maps[(int)Mathf.Repeat(index + 1, _maps.Length)].MapImage;
        previousMapImage.sprite = _maps[(int)Mathf.Repeat(index - 1, _maps.Length)].MapImage;

        GameManager.instance.NewMap(_maps[(int)Mathf.Repeat(index, _maps.Length)].name);
    }

    public void ClickedNextMapButton()      //무언가 연출이 있을수 있으므로 분리
    {
        index = (int)Mathf.Repeat(index + 1, _maps.Length);
        UpdateMapData();
    }

    public void ClickedPreviousMapButton()
    {
        index = (int)Mathf.Repeat(index - 1, _maps.Length);
        UpdateMapData();
    }
}
