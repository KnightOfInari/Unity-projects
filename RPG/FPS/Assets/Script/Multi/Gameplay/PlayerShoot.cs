using UnityEngine;
using UnityEngine.Networking;


//THIS CLASS CONTROLS HOW THE PLAYERS SHOOT
[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

    private void Start()
    {
        if (cam == null)
        {
            Debug.LogError("PlayerShoot: No camera referenced");
            this.enabled = false;
        }
        weaponManager = GetComponent<WeaponManager>();
    }



    // UPDATE CHECKS EVERY FRAME IF THE PLAYER INPUT IS THE FIRE BUTTON
    // IF SO IT SHOOTS
    private void Update()
    {
        if (PauseMenu.IsOn)
        {
            return;
        }
        currentWeapon = weaponManager.GetCurrentWeapon();
        //If fire rate is eqals to 0 or inferior, make the weapon semi automatic and shoot only once
        if (currentWeapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            //if weapon has fire rate > 0 weapon is automatic and shoots while fire button is pressed with a fire rate equals to the weapons fire rate
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1 / currentWeapon.fireRate); //3rd arguments is 1/ fire rate because it normally is a delay and not a rate.
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
    }

    [Command] // commands are executed only by the server
    void CmdOnShoot()//is called when player shoots
    {
        RpcDoShootEffect();
    }

    [ClientRpc]//calls method on all clients
    void RpcDoShootEffect()//show shoot effects on all client
    {
        weaponManager.GetCurrentGraphics().muzzelFlash.Play();
    }

    [Command]//Is called on the server when we hit something, takes the hit point and normal of the surface
    void CmdOnHit(Vector3 _pos, Vector3 _normal)
    {
        RpcDoImpactEffect(_pos, _normal);
    }

    [ClientRpc]//Is called on all clients. We spawn effects
    void RpcDoImpactEffect(Vector3 _pos, Vector3 _normal)//show impact effects on all client
    {
        GameObject hitEffect = (GameObject)Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
        Destroy(hitEffect, 1);
    }
    // SHOOT IS ONLY CALLED ON THE CLIENT SIDE
    [Client]
    private void Shoot()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        //We are shooting, call the method on server
        CmdOnShoot();
        Debug.Log("Test");
        // LAUNCHES A RAY FROM THE CAMERA TO SIMULATE A SHOT AND SEE IF IT HIT SOMETHING
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask))
        {
            //We hit something
            if (hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(hit.collider.name, currentWeapon.damage);
            }

            // Call the onHit method on the server when we hit somthing
            CmdOnHit(hit.point, hit.normal);
        }
    }

    //  CmdPlayerShot IS CALLED ON THE SERVER SIDE. IT CHECKS WHICH PLAYER HAS BEEN SHOT AND THE PLAYERMANAGER OF THAT PLAYER APPLIES DAMAGES
    [Command]
    void CmdPlayerShot(string _playerID, int _damage)
    {
        Debug.Log(_playerID + "has been shot.");

        PlayerManager _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage);
    }
}
