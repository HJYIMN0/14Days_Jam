using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuCanva : MonoBehaviour
{
    public string lvl01name = "Lvl01";
    public void Play()
    {
        SceneManager.LoadScene(lvl01name);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        Debug.Log("Can't quit while on editor!");
#endif
        Application.Quit();
    }
}
