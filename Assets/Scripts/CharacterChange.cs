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
            girl.transform.Rotate(Vector3.up * 300 * Time.deltaTime);
            boy.SetActive(false);
        }
        else if (type == CharacterType.Boy)
        {
            girl.SetActive(false);
            boy.SetActive(true);
        }
    }

    private void Update()
    {
        girl.transform.Rotate(Vector3.up * 30 * Time.deltaTime);
        boy.transform.Rotate(Vector3.up * 30 * Time.deltaTime);
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
            girl.transform.rotation = Quaternion.identity;
            boy.SetActive(false);
        }
        else if (type == CharacterType.Boy)
        {
            girl.SetActive(false);
            boy.SetActive(true);
            boy.transform.rotation = Quaternion.identity;
        }

        GameManager.instance.ChangeCharacter(type);
    }
}
