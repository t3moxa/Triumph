using UnityEngine;

public class CameraController : MonoBehaviour
{
    float moveSpeed = 10f; // Скорость перемещения камеры
    float edgeThickness = 10f; // Толщина области, при наведении на которую камера будет двигаться

    float zoomSpeed = 2f; // Скорость изменения масштаба
    float minZoom = 2f; // Минимальное значение масштаба
    float maxZoom = 20f; // Максимальное значение масштаба

    Vector2 minBounds = new Vector2(-20, -20);
    Vector2 maxBounds = new Vector2(60, 60);

    private Camera cam; // Ссылка на компонент камеры
    void Start()
    {
        cam = GetComponent<Camera>(); // Получаем компонент камеры
    }

    void Update()
    {
        Vector3 move = Vector3.zero;

        // Получаем позицию курсора мыши
        Vector3 mousePosition = Input.mousePosition;

        // Проверяем, находится ли курсор в области левой границы
        if (mousePosition.x < edgeThickness)
        {
            move.x = -1; // Двигаем камеру влево
        }
        // Проверяем, находится ли курсор в области правой границы
        else if (mousePosition.x > Screen.width - edgeThickness)
        {
            move.x = 1; // Двигаем камеру вправо
        }

        // Проверяем, находится ли курсор в области нижней границы
        if (mousePosition.y < edgeThickness)
        {
            move.y = -1; // Двигаем камеру вниз
        }
        // Проверяем, находится ли курсор в области верхней границы
        else if (mousePosition.y > Screen.height - edgeThickness)
        {
            move.y = 1; // Двигаем камеру вверх
        }

        // Добавляем управление с помощью стрелок и WASD
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            move.x = -1; // Двигаем камеру влево
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            move.x = 1; // Двигаем камеру вправо
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            move.y = -1; // Двигаем камеру вниз
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            move.y = 1; // Двигаем камеру вверх
        }

        // Перемещаем камеру
        var boundedMove = transform.position + moveSpeed * Time.deltaTime * move;
        boundedMove.x = Mathf.Clamp(boundedMove.x, minBounds.x, maxBounds.x);
        boundedMove.y = Mathf.Clamp(boundedMove.y, minBounds.y, maxBounds.y);

        transform.position = boundedMove;

        // Обработка изменения масштаба при вращении колёсика мыши
        float scrollData = Input.GetAxis("Mouse ScrollWheel");
        if (scrollData != 0f)
        {
            cam.orthographicSize -= scrollData * zoomSpeed; // Изменяем размер ортографической камеры
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom); // Ограничиваем масштаб
        }
    }
}
