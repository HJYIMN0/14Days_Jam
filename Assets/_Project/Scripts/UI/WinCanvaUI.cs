using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCanvaUI : MonoBehaviour
{

    public string mainMenuLvlName = "MainMenu";
    public void Quit() => SceneManager.LoadScene(mainMenuLvlName);

    private GameObject player;

    private void Update()
    {
        if (this.gameObject.activeSelf)
        {
            if (player == null) player = GameObject.FindGameObjectWithTag("Player");
            player.SetActive(false);
        }
    }
}
