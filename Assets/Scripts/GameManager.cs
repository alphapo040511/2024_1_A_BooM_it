using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public NetworkPrefabRef playerRef;
    public AudioMixer audioMixer;

    public string mapIndex;

    public string[] weaponIndex = new string[3];
    public string itemIndex;

    public Dictionary<string, int> awardPoints = new Dictionary<string, int>();

    public CharacterType characterType = CharacterType.Girl;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        
    }
    private void Start()
    {
        StartCoroutine(LoadData());
    }

    private IEnumerator LoadData()
    {
        weaponIndex[0] = PlayerPrefs.GetString("weapon_0", "BasicBomb");
        weaponIndex[1] = PlayerPrefs.GetString("weapon_1", "CrossBomb");
        weaponIndex[2] = PlayerPrefs.GetString("weapon_2", "StraightBomb");

        itemIndex = PlayerPrefs.GetString("skill", "SpeedUp");

        characterType = (CharacterType)PlayerPrefs.GetInt("character", 0);

        audioMixer.SetFloat("Master", PlayerPrefs.GetFloat("Master", 0));
        audioMixer.SetFloat("BGM", PlayerPrefs.GetFloat("BGM", 0));
        audioMixer.SetFloat("SFX", PlayerPrefs.GetFloat("SFX", 0));

        SceneLoadManager.instance.LoadScene("LobbyScene");
        yield return null;
    }

    public void SaveData()
    {
        PlayerPrefs.SetString("weapon_0", weaponIndex[0]);
        PlayerPrefs.SetString("weapon_1", weaponIndex[1]);
        PlayerPrefs.SetString("weapon_2", weaponIndex[2]);

        PlayerPrefs.SetString("skill", itemIndex);

        PlayerPrefs.SetInt("character", (int)characterType);
    }

    public void NewMap(string mapName)
    {
        mapIndex = mapName;
    }    

    public void SetWeapon(string name, int index)
    {
        for(int i = 0; i < weaponIndex.Length; i++)
        {
            if (weaponIndex[i] == name)
            {
                weaponIndex[i] = "None";
                if(i == index)
                {
                    return;
                }
            }
        }

        weaponIndex[index] = name;
        SaveData();
    }

    public void SetItem(string name)
    {
        if(itemIndex == name)
        {
            itemIndex = "None";
        }
        else
        {
            itemIndex = name;
        }

        SaveData();
    }

    public void AwardAddValue(string name, int point)
    {
        if (awardPoints.ContainsKey(name))
        {
            awardPoints[name] += point;
        }
        else
        {
            awardPoints.Add(name, point);
        }
    }

    public void ClearAward()
    {
        awardPoints.Clear();
    }

    public void ChangeCharacter(CharacterType type)                       //캐릭터가 늘어날 수 있으니 enum으로 관리
    {
        characterType = type;
    }
}
