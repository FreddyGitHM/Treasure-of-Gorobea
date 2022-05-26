using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Invector;
using Photon.Pun;

public class HeroManager : MonoBehaviour
{
    public int previousIndex;
    public int currentIndex;
    public Heroes[] heroes;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;
    public GameObject AbilitySprite;
    public TextMeshProUGUI AbilityTitle;
    public TextMeshProUGUI AbilityText;
    public GameObject SpawnPosition;
    public static string selectedHero;
    private GameObject Button;
    private GameStatus gameStatus;

    void Start()
    {
        gameStatus = GameObject.FindWithTag("GameController").GetComponent<GameStatus>();
        currentIndex = gameStatus.HeroSelected;
        changeCharacter(gameStatus.HeroSelected);
        SetSelectedHero();
    }

    public void changeCharacter(int index)
    {
        previousIndex = currentIndex;

        DeletePreviousModel();

        GameObject hero = Instantiate(heroes[index].Hero, SpawnPosition.transform.position + Vector3.back * 3, Quaternion.AngleAxis(180f, Vector3.up));
        DisableComponent(hero);
        hero.transform.localScale = new Vector3(4, 4, 4);

        Name.text = heroes[index].HeroName;
        Description.text = heroes[index].HeroDescription;

        AbilitySprite.GetComponent<Image>().sprite = heroes[index].AbilitySprite;
        AbilityTitle.text = heroes[index].AbilityName;
        AbilityText.text = heroes[index].AbilityDescription;

        currentIndex = index;
    }

    public void SetSelectedHero()
    {
        // Find previous selected hero button
        GameObject PreviousButton = GameObject.Find(heroes[previousIndex].HeroName + "Button");
        PreviousButton.GetComponent<Image>().sprite = heroes[previousIndex].HeroSprite;

        selectedHero = heroes[currentIndex].HeroName;

        Button = GameObject.Find(heroes[currentIndex].HeroName + "Button");
        Button.GetComponent<Image>().sprite = heroes[currentIndex].HeroSpriteSelected;

        gameStatus.HeroSelected = currentIndex;
        SaveSystem.Save();
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
