using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridMovement : MonoBehaviour
{
    public float speed = 10f; // �������� ����������� ������
    public float dragSpeed = 2f; // �������� �������������� �����
    public float minX = -10f; // ����������� �������� ���������� X
    public float maxX = 10f; // ������������ �������� ���������� X
    public float minY = -10f; // ����������� �������� ���������� Y
    public float maxY = 10f; // ������������ �������� ���������� Y
    public static bool isTouchEnabled = false; // ����, ����������� ������������ � ������� ������� ��������
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
            // ����������� ����� ����� �������������� ����� �������
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 deltaPosition = touch.deltaPosition;
                transform.Translate(-deltaPosition.x * dragSpeed * Time.deltaTime, -deltaPosition.y * dragSpeed * Time.deltaTime, 0f);
            }
        }

        // ����������� ����������� ����� � �������� ��������
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