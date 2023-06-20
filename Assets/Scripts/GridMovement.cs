using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridMovement : MonoBehaviour
{
    public float speed = 10f; // Скорость перемещения камеры
    public float dragSpeed = 2f; // Скорость перетаскивания сетки
    public float minX = -10f; // Минимальное значение координаты X
    public float maxX = 10f; // Максимальное значение координаты X
    public float minY = -10f; // Минимальное значение координаты Y
    public float maxY = 10f; // Максимальное значение координаты Y
    public static bool isTouchEnabled = false; // Флаг, разрешающий передвижение с помощью сенсора телефона
    private Vector2 touchStartPos;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {

        if (isTouchEnabled && Input.touchCount == 1)
        {
            // Перемещение сетки путем перетаскивания одним пальцем
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 deltaPosition = touch.deltaPosition;
                transform.Translate(-deltaPosition.x * dragSpeed * Time.deltaTime, -deltaPosition.y * dragSpeed * Time.deltaTime, 0f);
            }
        }

        // Ограничение перемещения сетки в заданных пределах
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
        transform.position = position;
    }

    public static void ToggleTouchMovement()
    {
        Game.DrawEnable = false;
        isTouchEnabled = !isTouchEnabled;
    }
}