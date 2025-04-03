using UnityEngine;

public class CameraController : MonoBehaviour
{
    float moveSpeed = 10f; // �������� ����������� ������
    float edgeThickness = 10f; // ������� �������, ��� ��������� �� ������� ������ ����� ���������

    float zoomSpeed = 2f; // �������� ��������� ��������
    float minZoom = 2f; // ����������� �������� ��������
    float maxZoom = 20f; // ������������ �������� ��������

    Vector2 minBounds = new Vector2(-20, -20);
    Vector2 maxBounds = new Vector2(60, 60);

    private Camera cam; // ������ �� ��������� ������
    void Start()
    {
        cam = GetComponent<Camera>(); // �������� ��������� ������
    }

    void Update()
    {
        Vector3 move = Vector3.zero;

        // �������� ������� ������� ����
        Vector3 mousePosition = Input.mousePosition;

        // ���������, ��������� �� ������ � ������� ����� �������
        if (mousePosition.x < edgeThickness)
        {
            move.x = -1; // ������� ������ �����
        }
        // ���������, ��������� �� ������ � ������� ������ �������
        else if (mousePosition.x > Screen.width - edgeThickness)
        {
            move.x = 1; // ������� ������ ������
        }

        // ���������, ��������� �� ������ � ������� ������ �������
        if (mousePosition.y < edgeThickness)
        {
            move.y = -1; // ������� ������ ����
        }
        // ���������, ��������� �� ������ � ������� ������� �������
        else if (mousePosition.y > Screen.height - edgeThickness)
        {
            move.y = 1; // ������� ������ �����
        }

        // ��������� ���������� � ������� ������� � WASD
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            move.x = -1; // ������� ������ �����
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            move.x = 1; // ������� ������ ������
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            move.y = -1; // ������� ������ ����
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            move.y = 1; // ������� ������ �����
        }

        // ���������� ������
        var boundedMove = transform.position + moveSpeed * Time.deltaTime * move;
        boundedMove.x = Mathf.Clamp(boundedMove.x, minBounds.x, maxBounds.x);
        boundedMove.y = Mathf.Clamp(boundedMove.y, minBounds.y, maxBounds.y);

        transform.position = boundedMove;

        // ��������� ��������� �������� ��� �������� ������� ����
        float scrollData = Input.GetAxis("Mouse ScrollWheel");
        if (scrollData != 0f)
        {
            cam.orthographicSize -= scrollData * zoomSpeed; // �������� ������ ��������������� ������
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom); // ������������ �������
        }
    }
}
