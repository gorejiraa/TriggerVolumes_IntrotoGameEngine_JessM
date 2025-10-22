using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Tooltip("Scene to load when Play is pressed")]
    public string nextSceneName = "TriggerVolumes";

    public void PlayGame()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}