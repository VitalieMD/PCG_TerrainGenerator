using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour
{
    public static UI_Controller _instance;

    public int length, width, height;
    public GameObject settings, game;
    public Slider widthSlider, lenghtSlider, heightSlider;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

    }

    private void Update()
    {
        width = (int)widthSlider.value;
        length = (int)lenghtSlider.value;
        height = (int)heightSlider.value;
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            settings.SetActive(true);
            game.SetActive(false);
        }
        else
        {
            settings.SetActive(false);
            game.SetActive(true);
        }
    }

    public void ExitApp()
    {
        Application.Quit();
    }
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}