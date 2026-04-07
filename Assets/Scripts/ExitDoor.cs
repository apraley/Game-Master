using UnityEngine;

public class ExitDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private AudioSource lockedAudio;
    [SerializeField] private AudioSource openAudio;

    private bool opened;

    public void Interact(PlayerController player)
    {
        if (opened)
        {
            return;
        }

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            return;
        }

        if (!gameManager.HasKeycard)
        {
            if (lockedAudio != null)
            {
                lockedAudio.Play();
            }

            return;
        }

        opened = true;

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open");
        }

        if (openAudio != null)
        {
            openAudio.Play();
        }

        gameManager.TriggerWin();
    }
}
