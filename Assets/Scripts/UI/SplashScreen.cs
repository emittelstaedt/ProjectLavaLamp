using UnityEngine;
using UnityEngine.EventSystems;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject mainMenuCamera;

    public void LoadMainMenu()
    {
        loadingScreen.SetActive(true);
        mainMenuCamera.SetActive(true);
        mainMenu.gameObject.SetActive(true);
    }
}