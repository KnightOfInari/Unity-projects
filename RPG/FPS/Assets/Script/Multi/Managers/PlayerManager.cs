using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


//PLAYER MANAGER CLASS HANDLES THE HEALTH AND DEATH OF THE PLAYER
[RequireComponent(typeof(PlayerSetup))]
public class PlayerManager : NetworkBehaviour
{
    //SyncVar make it so the variable is always up to date between the server and all the clients
    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;

    //Initialises player settings to default and enables controls.
    public void SetupPlayer()
    {
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }

        CmdBroadCastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
            firstSetup = false;
        }
        SetDefaults();
    }
    //private void Update()
    //{
    //    if (!isLocalPlayer)
    //        return;

    //    if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        RpcTakeDamage(6000);
    //    }
    //}


    //Client calculates the damages taken by the player if currentHealth <= 0 player dies
    [ClientRpc]
    public void RpcTakeDamage(int _amount)
    {
        if (isDead)
            return;
        currentHealth -= _amount;

        Debug.Log(transform.name + " now has " + currentHealth + " health.");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Disable player controlls while player is dead. calls coroutine Respawn.
    private void Die()
    {
        isDead = true;


        //Disable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        //Disable gameObjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        //Disable Collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = false;
            //Spawn death effect
            GameObject _gfxIns = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(_gfxIns, 3f);

            //Switch cameras
            if (isLocalPlayer)
            {
                GameManager.instance.SetSceneCameraActive(true);
                GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
            }

            Debug.Log(transform.name + " is dead!");

            //CALL RESPAWN METHOD

            StartCoroutine(Respawn());

        }
    }


    // Coroutine waits for respawnTime set in GameManager script before respawning player on one of the availlable spawnpoint
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f); // Small delay to make sure the positions are set before we set the player up

        SetupPlayer();

        Debug.Log(transform.name + " respawned.");
    }

    // Default settings after a player is spawned. Not dead, max health and controlls enabled back
    public void SetDefaults()
    {
        isDead = false;

        currentHealth = maxHealth;
        //enable the components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }
        //enable the gameobjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }
        //Enable the collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = true;
        }


        //create spawn effect
        GameObject _gfxIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);
    }
}
