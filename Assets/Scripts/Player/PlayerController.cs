// Чтобы скрипт заработал, я его закинул в самого игрока (Player)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private BlockType selectedBlock = BlockType.Stone; // Текущий выбранный блок
    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu"); // Загрузка сцены меню
        }
        else
        {
            // Получаем код нажатой клавиши
            KeyCode keyPressed = KeyCode.None;

            // Проверяем нажатие клавиш от 1 до 9
            for (KeyCode keyCode = KeyCode.Alpha0; keyCode <= KeyCode.Alpha9; keyCode++)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    keyPressed = keyCode;
                    break;
                }
            }

            // Переключаемся в зависимости от нажатой клавиши
            switch (keyPressed)
            {
                case KeyCode.Alpha1:
                    selectedBlock = BlockType.Stone;
                    break;
                case KeyCode.Alpha2:
                    selectedBlock = BlockType.Dirt;
                    break;
                case KeyCode.Alpha3:
                    selectedBlock = BlockType.Grass;
                    break;
                case KeyCode.Alpha4:
                    selectedBlock = BlockType.Wood;
                    break;
                case KeyCode.Alpha5:
                    selectedBlock = BlockType.Plank;
                    break;
                case KeyCode.Alpha6:
                    selectedBlock = BlockType.Leaves;
                    break;
                case KeyCode.Alpha7:
                    selectedBlock = BlockType.Cobblestone;
                    break;
                case KeyCode.Alpha8:
                    selectedBlock = BlockType.Sand;
                    break;
                case KeyCode.Alpha9:
                    selectedBlock = BlockType.Bricks;
                    break;
                case KeyCode.Alpha0:
                    selectedBlock = BlockType.Water;
                    break;
                default:
                    // Если нажата клавиша, которая не соответствует выбору блока, не делаем ничего
                    break;
            }
        }
    }
    public BlockType GetSelectedBlock()
    {
        return selectedBlock;
    }
}
