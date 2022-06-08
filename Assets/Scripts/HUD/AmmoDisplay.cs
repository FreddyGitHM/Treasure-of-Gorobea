using UnityEngine;
using UnityEngine.UI;
using Invector.vItemManager;
using Invector.vShooter;


public class AmmoDisplay : MonoBehaviour
{
    GameObject player;
    vShooterWeapon vShooterWeapon;
    Text currentAmmo;
    Text totalAmmo;
    int totalAmmoLeft;
    int shottedAmmo;
    bool init;
    bool visible;

    void Start()
    {
        player = GameObject.FindWithTag("NetworkManager").GetComponent<NetworkManager>().GetPlayer();
        vShooterWeapon = player.GetComponent<EventsCall>().weapon.GetComponent<vShooterWeapon>();
        currentAmmo = gameObject.transform.Find("primaryText").GetComponent<Text>();
        totalAmmo = gameObject.transform.Find("secundaryText").GetComponent<Text>();
        totalAmmoLeft = 0;
        shottedAmmo = 0;
        init = false;
        visible = false;

        currentAmmo.text = vShooterWeapon.ammoCount.ToString();
    }

    void Update()
    {
        if(init == false)
        {
            vAmmo vAmmo = player.GetComponent<vAmmoManager>().GetAmmo(vShooterWeapon.ammoID);
            if(vAmmo != null)
            {
                totalAmmoLeft = vAmmo.count;
                totalAmmo.text = totalAmmoLeft.ToString();
                init = true;
            }
        }

        if(init && visible == false)
        {
            currentAmmo.enabled = true;
            totalAmmo.enabled = true;
            totalAmmo.gameObject.transform.GetChild(0).GetComponent<Image>().enabled = true;
            visible = true;
        }

        if(visible)
        {
            currentAmmo.text = vShooterWeapon.ammoCount.ToString();
            totalAmmo.text = totalAmmoLeft.ToString();
        }
    }

    public void OnReload()
    {
        shottedAmmo = vShooterWeapon.clipSize - vShooterWeapon.ammoCount;
    }

    public void OnFinishReload()
    {
        totalAmmoLeft -= shottedAmmo;
        if(totalAmmoLeft < 0)
        {
            totalAmmoLeft = 0;
        }
        shottedAmmo = 0;
    }

}
