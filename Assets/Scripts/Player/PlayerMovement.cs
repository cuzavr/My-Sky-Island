using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Для передвижения игрока
    public float _speed = 3.0f;   // Скорость с которой перемещается игрок
    public float _gravity = -9.8f; // Гравитация игрока

    // Для прыжков игрока
    public float _jumpHeight = 1.0f; // Высота прыжка
    private float _ySpeed; // Переменная для хранения текущей скорости прыжка

    // Для распознавания столкновений используем этот объект
    private CharacterController _characterController;

    // Позиция для телепортации если игрок упал под карту
    public Vector3 teleportPosition = new Vector3(8, 8, 8);

    private void Start()
    {
        _characterController = GetComponent<CharacterController>(); // Получаем CharacterController
        if (_characterController != null) { } // Проверяем, получили или нет
    }

    private void Update()
    {
        bool isGrounded = _characterController.isGrounded; // Проверяем, находится ли игрок на земле
        if (isGrounded) { _ySpeed = -0.8f; } // Если игрок на земле, сбрасываем вертикальную скорость для прыжков
        _ySpeed += _gravity * Time.deltaTime; // Применяем гравитацию

        // Если игрок нажимает прыжок и он на земле, применяем силу прыжка
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            _ySpeed = Mathf.Sqrt(_jumpHeight * -0.3f * _gravity);
        }

        // Чтобы игрок ходил исключительно на WASD
        float deltaX = 0f;
        float deltaZ = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            deltaX = -_speed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            deltaX = _speed;
        }

        if (Input.GetKey(KeyCode.W))
        {
            deltaZ = _speed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            deltaZ = -_speed;
        }

        Vector3 movement = new Vector3(deltaX, _ySpeed, deltaZ); // Делаем чтобы можно было двигаться относительно characterController

        // Ограничиваем значение относительно нашей скорости
        movement = Vector3.ClampMagnitude(movement, _speed);
        movement *= Time.deltaTime;

        // Преобразуем вектор движения от локальных к глобальным координатам
        movement = transform.TransformDirection(movement);

        // Сделаем чтобы передвигались благодаря CharacterController
        _characterController.Move(movement);

        // Получаем текущую позицию игрока
        Vector3 playerPosition = transform.position;

        // Проверяем, находится ли игрок ниже -1 по оси Y
        if (playerPosition.y < -1)
        {
            // Телепортируем игрока на указанную позицию
            transform.position = teleportPosition;
        }
    }
}
