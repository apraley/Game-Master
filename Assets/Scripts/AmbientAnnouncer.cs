using UnityEngine;

public class AmbientAnnouncer : MonoBehaviour
{
    [SerializeField] private AudioSource paSource;
    [SerializeField] private AudioClip[] announcementClips;
    [SerializeField] private Vector2 intervalRange = new Vector2(18f, 35f);

    private float timer;
    private float nextInterval;

    private void Start()
    {
        RollNextInterval();
    }

    private void Update()
    {
        if (paSource == null || announcementClips == null || announcementClips.Length == 0)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer < nextInterval || paSource.isPlaying)
        {
            return;
        }

        timer = 0f;
        paSource.PlayOneShot(announcementClips[Random.Range(0, announcementClips.Length)]);
        RollNextInterval();
    }

    private void RollNextInterval()
    {
        nextInterval = Random.Range(intervalRange.x, intervalRange.y);
    }
}
