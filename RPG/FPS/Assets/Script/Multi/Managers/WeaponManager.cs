using UnityEngine;
using UnityEngine.Networking;

// Class managing weapons as well as the weapon holder
public class WeaponManager : NetworkBehaviour
{

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Transform weaponHolder;

    [SerializeField]
    private PlayerWeapon primaryWeapon;

    private PlayerWeapon currentWeapon;
    private WeaponGraphics currentGraphics;

    private void Start()
    {
        //Equips primary weapon
        EquipWeapon(primaryWeapon);
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }

    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    // Equips the weapon given in parameters
    void EquipWeapon(PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;

        //creation and Instantiation of the weapon at the position and rotation of the weapon holder
        GameObject _weaponIns = (GameObject)Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        _weaponIns.transform.SetParent(weaponHolder);

        //setting the graphic effects of the equiped weapon
        currentGraphics = _weaponIns.GetComponent<WeaponGraphics>();
        if (currentGraphics == null)
            Debug.LogError("No WeaponGraphics component on the weapon object: " + _weaponIns.name);

        //Check if you are local player and change your weapons layer and all the children to the weapon layer's name so it is seen only by the weapon camera
        if (isLocalPlayer)
            Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));
    }
}
