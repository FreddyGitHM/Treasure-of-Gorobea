using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Invector;
using Photon.Pun;

public class HeroManager : MonoBehaviour
{
    public int currentIndex;
    public Heroes[] heroes;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;
    public GameObject AbilitySprite;
    public TextMeshProUGUI AbilityTitle;
    public TextMeshProUGUI AbilityText;
    public GameObject SpawnPosition;
    public static string selectedHero;

    void Start()
    {
        changeCharacter(0);
        selectedHero = heroes[currentIndex].HeroName;
    }

    public void changeCharacter(int index)
    {
        DeletePreviousModel();

        currentIndex = index;

        GameObject hero = Instantiate(heroes[index].Hero, SpawnPosition.transform.position + Vector3.back * 3, Quaternion.AngleAxis(180f, Vector3.up));
        DisableComponent(hero);
        hero.transform.localScale = new Vector3(4, 4, 4);

        Name.text = heroes[index].HeroName;
        Description.text = heroes[index].HeroDescription;

        AbilitySprite.GetComponent<Image>().sprite = heroes[index].AbilitySprite;
        AbilityTitle.text = heroes[index].AbilityName;
        AbilityText.text = heroes[index].AbilityDescription;
    }

    public void SetSelectedHero()
    {
        selectedHero = heroes[currentIndex].HeroName;
    }

    private void DeletePreviousModel()
    {
        foreach(Heroes hero in heroes)
        {
            GameObject heroGO = GameObject.Find(hero.HeroName + "(Clone)");
            if(heroGO != null)
            {
                DestroyImmediate(heroGO);
                break;
            }
        }
    }

    private void DisableComponent(GameObject hero)
    {
        hero.GetComponent<vFootStep>().enabled = false;
        hero.GetComponent<FootStepVolumes>().enabled = false;
        hero.GetComponent<PhotonTransformView>().enabled = false;
        hero.GetComponent<PhotonAnimatorView>().enabled = false;
        hero.GetComponent<EventsCall>().enabled = false;
        hero.GetComponent<AnimationSync>().enabled = false;

        hero.transform.Find("Minimap").gameObject.SetActive(false);
        hero.transform.Find("Invector Components/AimCanvas").gameObject.SetActive(false);
    }
}
