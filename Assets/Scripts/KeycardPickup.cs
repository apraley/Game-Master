using UnityEngine;

public class KeycardPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioSource pickupAudio;

    public void Interact(PlayerController player)
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            return;
        }

        gameManager.CollectKeycard();

        if (pickupAudio != null)
        {
            pickupAudio.transform.parent = null;
            pickupAudio.Play();
            Destroy(pickupAudio.gameObject, pickupAudio.clip.length + 0.1f);
        }

        Destroy(gameObject);
    }
}
