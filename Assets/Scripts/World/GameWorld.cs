/*
Данный скрипт нужен для генерации мира используя функции со скриптов LandscapeGenerator.cs, ChunkRender.cs

Как я сделал скрипт рабочим
1. Cоздал Empty - Game World, и его туда закинул как компонент
2. Префаб Chunk закинул в Chunk Prefab
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    public int dirtLayerHeight = 3;
    public Dictionary<Vector2Int, ChunkData> ChunkDatas = new Dictionary<Vector2Int, ChunkData>();  // Словарь когда GameWorld будет-что загружать
    public ChunkRenderer ChunkPrefab;  // Создадим сам объект на сцене
    private Camera mainCamera;
    private void Start()
    {
        mainCamera = Camera.main;

        // Цикл который генерирует чанк (1 на 1)
        for (int x = 0; x < 1; x++)
        {
            for (int y = 0; y < 1; y++)
            {
                float xPos = x * ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;
                float zPos = y * ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;

                int xIntPos = Mathf.FloorToInt(xPos);
                int zIntPos = Mathf.FloorToInt(zPos);

                // Спавним чанки
                ChunkData chunkData = new ChunkData(); // Массив блоков который храниться в ChunkData
                chunkData.ChunkPosition = new Vector2Int(x, y);

                // Передача dirtLayerHeight при вызове GenerateLandscape
                chunkData.Blocks = LandscapeGenerator.GenerateLandscape(xIntPos, zIntPos, dirtLayerHeight);


                ChunkDatas.Add(new Vector2Int(x, y), chunkData);

                var chunk = Instantiate(ChunkPrefab, new Vector3(xPos, 0, zPos), Quaternion.identity, transform);
                chunk.ChunkData = chunkData; // Чтобы передать в ChunkRender массив блоков
                chunk.ParentWorld = this;
                chunkData.Renderer = chunk;
            }
        }

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) // Если игрок нажал на лкм либо пкм
        {
            bool isDestroying = Input.GetMouseButton(0); // True удаляем блок, False добавляем

            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(ray, out var hitInfo)) // Если луч попал в чанк, то в hitInfo будет содержаться информация о попадании
            {
                // Проверка расстояния между точкой, куда игрок хочет поставить блок, и позицией игрока
                Vector3 playerPos = mainCamera.transform.position;
                Vector3 targetPos = hitInfo.point + hitInfo.normal * ChunkRenderer.BlockScale / 2;
                float distance = Vector3.Distance(playerPos, targetPos);

                // Установка максимального расстояния
                float maxDistance = 5f; // Максимальное допустимое расстояние

                // Проверка, находится ли расстояние в пределах допустимого
                if (distance <= maxDistance)
                {
                    Vector3 blockCenter;
                    if (isDestroying)
                    {
                        blockCenter = hitInfo.point - hitInfo.normal * ChunkRenderer.BlockScale / 2; // Получим точку в центре блока для удаления
                    }
                    else
                    {
                        blockCenter = hitInfo.point + hitInfo.normal * ChunkRenderer.BlockScale / 2; // Получим точку в центре нового блока для установки
                    }
                    Vector3Int blockWorldPos = Vector3Int.FloorToInt(blockCenter / ChunkRenderer.BlockScale); // Получим мировые координаты целочисленного блока
                    Vector2Int chunkPos = GetChunkContainingBlock(blockWorldPos); // Вызываем функцию GetChunkContainingBlock и получаем позицию чанков, в котором находится блок

                    // Проверка после которой блок сможет спавниться по клику на ПКМ
                    if (ChunkDatas.TryGetValue(chunkPos, out ChunkData chunkData))
                    {
                        Vector3Int chunkOrigin = new Vector3Int(chunkPos.x, chunkPos.y) * ChunkRenderer.ChunkWidth;
                        if (isDestroying)
                        {
                            chunkData.Renderer.DestroyBlock(blockWorldPos - chunkOrigin); // Удаление блока
                        }
                        else
                        {
                            BlockType selectedBlock = FindObjectOfType<PlayerController>().GetSelectedBlock();
                            chunkData.Renderer.SpawnBlock(blockWorldPos - chunkOrigin, selectedBlock);
                        }
                    }
                }
            }
        }
    }

    // Функция которая даст понять в каком чанке находится блок и будет возвращать координаты чанка в котором блок находиться
    public Vector2Int GetChunkContainingBlock(Vector3Int blockWorldPos)
    {
        return new Vector2Int(blockWorldPos.x / ChunkRenderer.ChunkWidth, blockWorldPos.z / ChunkRenderer.ChunkHeight);
    }

}
