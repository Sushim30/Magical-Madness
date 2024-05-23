using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using System.IO;

public class AttackManager : MonoBehaviourPunCallbacks, IDamagable
{
    [SerializeField] private AudioSource shootSound;
    [SerializeField] private AudioSource deathSound;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Camera cam;

    private PhotonView PV;
    public float spawnDistance = 1.4f;
    public float spawnHeight = 1f;
    public int maxHealth = 100;
    public int currentHealth;
    private Vector3 spawnPosition;
    private float speed = 40f;
    private bool ballPresence = false;
    public Animator anim;

    public PlayerManager playerManager;

    public void Awake()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        PV = GetComponent<PhotonView>();

        // Find and assign the PlayerManager dynamically
        playerManager = FindObjectOfType<PlayerManager>();
        if (playerManager == null)
        {
            Debug.LogError("PlayerManager not found on the same GameObject.");
        }
    }

    public void Attack()
    {
        if (!PV.IsMine) return;  // Only allow attack if this is the local player

        Debug.Log("Power attack initiated");

        anim.SetTrigger("shooting");
        Vector3 sphereSpawnPosition = transform.position + transform.forward * spawnDistance + Vector3.up * spawnHeight;

        GameObject chargeSphere = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PowerSphere"), sphereSpawnPosition, Quaternion.identity);
        if (chargeSphere == null)
        {
            return;
        }

        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 directionToHit = (hit.point - sphereSpawnPosition).normalized;
            chargeSphere.GetComponent<Rigidbody>().velocity = directionToHit * speed;
        }
        else
        {
            chargeSphere.GetComponent<Rigidbody>().velocity = cam.transform.forward * speed;
        }
        StartCoroutine(DestroyChargeSphereAfterDelay(chargeSphere, 3f));
        shootSound.Play();
    }

    private IEnumerator DestroyChargeSphereAfterDelay(GameObject chargeSphere, float delay)
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(chargeSphere);
        ballPresence = false;
    }

    public void Rotate()
    {
        if (!PV.IsMine) return;  // Only allow rotation if this is the local player

        transform.rotation = cam.transform.rotation;
    }

    public void TakeDamage(int damage)
    {
        PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(int damage, PhotonMessageInfo info)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            PlayerManager.Find(info.Sender).GetKill();
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("you died");
        if (playerManager != null)
        {
            playerManager.Die();
        }
        else
        {
            Debug.LogError("playerManager is null. Ensure it is assigned properly.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PV.IsMine) return;  // Only process collisions for the local player

        if (other.CompareTag("ball"))
        {
            TakeDamage(75);
            Debug.Log("took damage");
        }
    }

    private void FixedUpdate()
    {
        if (!PV.IsMine) return;  // Only process FixedUpdate for the local player

        if (!ballPresence)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame && !ballPresence)
            {
                Rotate();
                anim.SetTrigger("shooting");
                ballPresence = true;
                Invoke("Attack", 0.5f);
            }
        }
    }
}
