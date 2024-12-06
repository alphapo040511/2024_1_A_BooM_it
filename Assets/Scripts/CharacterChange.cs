using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    Girl,
    Boy
}

public class CharacterChange : MonoBehaviour
{
    public GameObject girl;
    public GameObject boy;

    public void Start()
    {
        CharacterType type = GameManager.instance.characterType;

        if (type == CharacterType.Girl)
        {
            girl.SetActive(true);
            boy.SetActive(false);

        }
        else if (type == CharacterType.Boy)
        {
            girl.SetActive(false);
            boy.SetActive(true);
        }
    }

    public void Change()
    {
        CharacterType type = GameManager.instance.characterType;

        if (type == CharacterType.Girl)
        {
            type = CharacterType.Boy;
        }
        else if (type == CharacterType.Boy)
        {
            type = CharacterType.Girl;
        }

        if (type == CharacterType.Girl)
        {
            girl.SetActive(true);
            boy.SetActive(false);

        }
        else if (type == CharacterType.Boy)
        {
            girl.SetActive(false);
            boy.SetActive(true);
        }

        GameManager.instance.ChangeCharacter(type);
    }
}
