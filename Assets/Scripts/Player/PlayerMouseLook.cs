/*
Как я сделал скрипт рабочим в Unity 3d
1. Закинул скрипт в самого игрока (Player)
2. Закинул скрипт в Main Camera (который тоже был уже в Player)
3. В Axes в Player выбрал X, а вот в Axes в Main Camera я выбрал Y
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouseLook : MonoBehaviour
{
    // Создаём паблик, благодаря которому игрок сможет поворачивать мышкой
    public enum RorationAxes
    {
        // Возможные варианты вращения оси игроком
        XandY,
        X,
        Y
    }
    // Переменная для Unity для выбора относительно какой оси игрок будет вращаться
    public RorationAxes _axes = RorationAxes.XandY;

    // Перемененная для управления скоростью
    public float _rotationSpeedHor = 3.0f; // вращение по горизонтали
    public float _rotationSpeedVer = 3.0f; // вращение по вертикали

    // Ограничиваем управление по вертикали относительно разных углов
    public float maxVert = 85.0f;
    public float minVert = -85.0f;

    // Переменная для угла поворота по вертикали
    public float _rotationX = 0;

    public void Start()
    {
        // Фикс мышки при просмотре игры через программу самого движка
        Cursor.lockState = CursorLockMode.Locked;
        // Делаем так чтобы с Rigidbody (физика) можно было спокойно вращаться
        Rigidbody body = GetComponent<Rigidbody>();
        // Проверка, получили этот компонент или нет
        if (body != null)
        {
            body.freezeRotation = true; // если получили, то можем спокойно вращаться
        }
    }


    // Работаем в каждом кадре
    private void Update()
    {
        // Проверка оси движения
        if (_axes == RorationAxes.XandY)
        {
            _rotationX -= Input.GetAxis("Mouse Y") * _rotationSpeedVer; // получаем данные вводимые с помощью мыши Y
            _rotationX = Mathf.Clamp(_rotationX, minVert, maxVert);  // ограничиваем углы

            // Учитываем изменение угла поворота, вычисляем угол поворота через дельту
            float delta = Input.GetAxis("Mouse X") * _rotationSpeedHor;
            float _rotationY = transform.localEulerAngles.y + delta;

            // Применяем значения, которые вычислили
            transform.localEulerAngles = new Vector3(_rotationX, _rotationY, 0);
        }
        else if (_axes == RorationAxes.X)
        {
            // Получаем данные вводимые с помощью мыши X
            transform.Rotate(0, Input.GetAxis("Mouse X") * _rotationSpeedHor, 0);
        }
        else if (_axes == RorationAxes.Y)
        {
            _rotationX -= Input.GetAxis("Mouse Y") * _rotationSpeedVer;  // получаем данные вводимые с помощью мыши Y
            _rotationX = Mathf.Clamp(_rotationX, minVert, maxVert); // ограничиваем углы
            float _rotationY = transform.localEulerAngles.y; // сохраняем одинаковый угол поворота вокруг оси Y
            // Применяем значения, которые вычислили
            transform.localEulerAngles = new Vector3(_rotationX, _rotationY, 0); // создаем новый вектор и добавляем значения в скобках
        }
    }
}