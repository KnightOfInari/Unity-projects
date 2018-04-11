using UnityEngine;


// CLASS DEFINING THE CHARACTERISTICS OF A WEAPON
[System.Serializable]
public class PlayerWeapon
{

    public string weaponName = "Weapon";

    public int damage = 10;
    public float range = 200f;

    public float fireRate = 0;

    public GameObject graphics;
}
