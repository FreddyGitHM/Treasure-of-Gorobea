using System.Collections;
using UnityEngine;
using Invector.vCharacterController;
using UnityEngine.UI;
using System;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using EventCodes;


public class Skills : MonoBehaviourPunCallbacks
{
    public enum Skill { Speed, SilentFootsteps }
    public Skill skill;
    public Texture2D[] skillIcons;
    [Range(0.1f, 10f)]
    public float chargingSpeed;

    [Header("Speed")]
    [Range(1f, 2f)]
    public float speedMultiplier;
    [Range(5, 20)]
    public int speedRunningTime;

    [Header("Silent Footsteps")]
    [Range(0f, 1f)]
    public float silentStepsVolume;
    [Range(5, 20)]
    public int silentFootstepsRunningTime;

    int chargingLevel;
    bool rechargeable;

    //GUI
    Texture2D selectedIcon;
    RawImage skillImage;
    Slider slider;
    Color grey;

    void Awake()
    {
        chargingLevel = 0;
        rechargeable = true;
    }

    void Start()
    {
        selectedIcon = skillIcons[Array.IndexOf(Enum.GetValues(skill.GetType()), skill)];

        skillImage = gameObject.transform.Find("Invector Components/UI/HUD/skill").gameObject.GetComponent<RawImage>();
        grey = skillImage.color;
        skillImage.texture = selectedIcon;
        slider = skillImage.gameObject.transform.Find("slider").GetComponent<Slider>();
        slider.value = chargingLevel;

        StartCoroutine(IncrCharging());
    }

    void Update()
    {
        if(chargingLevel > 100)
        {
            chargingLevel = 100;
        }
        slider.value = chargingLevel;
        if(chargingLevel == 100)
        {
            rechargeable = false;
            StopCoroutine(IncrCharging());

            skillImage.color = new Color(1f, 1f, 1f, 1f);
            slider.gameObject.SetActive(false);

            if (Input.GetKeyDown(KeyCode.N))
            {
                skillImage.color = new Color(1f, 0f, 0f, 1f);
                chargingLevel = 0;
                UseSkill();
            }
        }
    }

    void UseSkill()
    {
        switch(skill.ToString())
        {
            case "Speed":
                StartCoroutine(IncrSpeed(speedRunningTime));
                break;

            case "SilentFootsteps":
                StartCoroutine(ReduceFootstepNoise(silentFootstepsRunningTime));
                break;
        }
    }

    IEnumerator IncrCharging()
    {
        while (rechargeable && chargingLevel < 100)
        {
            yield return new WaitForSecondsRealtime(1 / chargingSpeed);
            chargingLevel++;
        }
    }

    IEnumerator IncrSpeed(int runningTime)
    {
        vThirdPersonController thirdPersonController = gameObject.GetComponent<vThirdPersonController>();
        float normalSpeed = thirdPersonController.speedMultiplier;
        thirdPersonController.speedMultiplier = speedMultiplier * normalSpeed;
        yield return new WaitForSecondsRealtime(runningTime);
        thirdPersonController.speedMultiplier = normalSpeed;
        skillImage.color = grey;
    }

    IEnumerator ReduceFootstepNoise(int runningTime)
    {
        //send message to the others to reduce my footstep sound
        object[] data = new object[] { gameObject.GetComponent<PhotonView>().ViewID, silentStepsVolume, runningTime };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
        raiseEventOptions.Receivers = ReceiverGroup.Others;
        raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

        SendOptions sendOptions = new SendOptions();
        sendOptions.Reliability = true;

        PhotonNetwork.RaiseEvent(Codes.SILENT_FOOTSTEPS, data, raiseEventOptions, sendOptions);

        FootStepVolumes footStepVolumes = gameObject.GetComponent<FootStepVolumes>();
        footStepVolumes.SetSilentStepsVolume(silentStepsVolume);
        footStepVolumes.SetSilentStepsActive(true);
        yield return new WaitForSecondsRealtime(runningTime);
        footStepVolumes.SetSilentStepsActive(false);
        skillImage.color = grey;
    }

    public void IncrChargingLevel(int n)
    {
        chargingLevel += n;
    }

}
