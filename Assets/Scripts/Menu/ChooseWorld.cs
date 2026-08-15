using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseWorld : MonoBehaviour
{
    public void GameChooseWorld()
    {
        GameObject World1Button = GameObject.FindWithTag("World1");
        GameObject World2Button = GameObject.FindWithTag("World2");
        GameObject World3Button = GameObject.FindWithTag("World3");
        GameObject BackMenuButton = GameObject.FindWithTag("BackMenu");
        if (World1Button != null)
        {
            // Добавляем обработчик нажатия на кнопку начала игры
            World1Button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(LoadWorld1Scene);
            World2Button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(LoadWorld2Scene);
            World3Button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(LoadWorld3Scene);
            BackMenuButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(BackMenuScene);
        }
        else
        {
            Debug.LogError("Кнопка не найдена");
        }
    }
    void LoadWorld1Scene()
    {
        SceneManager.LoadScene("ClassicWorld"); // Загрузка сцены игрового мира
    }
    void LoadWorld2Scene()
    {
        SceneManager.LoadScene("SandWorld"); // Загрузка сцены игрового мира
    }
    void LoadWorld3Scene()
    {
        SceneManager.LoadScene("StoneWorld"); // Загрузка сцены игрового мира
    }
    void BackMenuScene()
    {
        SceneManager.LoadScene("MainMenu"); // Загрузка сцены меню
    }
    void Start()
    {
        GameChooseWorld();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu"); // Загрузка сцены меню
        }
    }
}
