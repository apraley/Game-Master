using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroScreenController : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text subtitleText;
    [SerializeField] private string startSceneName = "ShadyPines_EastWing";

    private const string Title = "ESCAPE FROM SHADY PINES";
    private const string Subtitle = "there's only one way folks leave shady pines… body bags.";

    private void Start()
    {
        if (titleText != null)
        {
            titleText.text = Title;
        }

        if (subtitleText != null)
        {
            subtitleText.text = Subtitle;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(startSceneName);
        }
    }
}
