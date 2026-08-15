// Чтобы скрипт заработал, я его закинул в Canvas сцены с меню.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void GameMenu()
    {
        GameObject startButton = GameObject.FindWithTag("StartGame"); // Находим кнопку начала игры по тегу
        GameObject exitButton = GameObject.FindWithTag("QuitGame"); // Находим кнопку выхода с игры по тегу
        if (startButton != null)
        {
            // Добавляем обработчик нажатия на кнопку начала игры
            startButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(LoadMainScene);
            exitButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ExitGame);
        }
        else
        {
            Debug.LogError("Кнопка не найдена");
        }
    }
    // Метод загрузки основной игровой сцены
    void LoadMainScene()
    {
        SceneManager.LoadScene("ChooseWorld"); // Загрузка сцены игрового мира
    }
    void ExitGame()
    {
        Application.Quit(); // Закрытие приложения
    }
    void Start()
    {
        GameMenu();
        Cursor.visible = true; // Показать курсор мыши
        Cursor.lockState = CursorLockMode.None; // Разблокировать курсор мыши
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit(); // Закрытие приложения
        }
    }
}