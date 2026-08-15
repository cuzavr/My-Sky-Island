/*
Данный скрипт нужен для того чтобы создавать чанки мира ну и сам блок из треугольников

Как я сделал скрипт рабочим
1. Создал пустой объект Empty, назвал Chunk
2. Закинул этот скрипт в этот объект
3. Добавил в этот Chunk в компонентах Mesh Collider
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))] // Обычный меш рендер юнити для рендера чанков, но с meshfilter и meshrenderer чтобы автоматом вешались эти 2 компонента

public class ChunkRenderer : MonoBehaviour
{
    /* Создаем массив в котором будем хранить данные о блоках, где установлен камень, земля и т.д
    Параметры выносим в константы, чтобы их можно было быстро менять 
    Пока-что будет так, что если 0 - воздух, если 1 - то туда спавним какой-нибудь блок */
    public const int ChunkWidth = 75; // Ширина
    public const int ChunkHeight = 75; // Высота
    public static float BlockScale = 0.5f; // Чтобы блоки могли быть не 1 на 1 а любое другое число (было const сделал static)

    public ChunkData ChunkData;
    public GameWorld ParentWorld;

    // Перегенирируем меш при каждой установке блока
    private Mesh chunkMesh;

    // Для генерации меша мы генерируем все вертексы, а потом делаем треугольник из этих вертиксов
    private List<Vector3> verticies = new List<Vector3>(); // Массив для вершин
    private List<Vector2> uvs = new List<Vector2>(); // Для передачи массивов uvs
    private List<int> triangles = new List<int>(); // Массив для треугольников

    void Start()
    {
        chunkMesh = new Mesh();
        // Mesh chunkMesh = new Mesh(); // Создаём объект типо меша
        GetComponent<MeshFilter>().mesh = chunkMesh; // Закидываем меш в этот компонент, задаём его тут
        RegenerateMesh();
    }

    private void RegenerateMesh()
    {
        // Очистка списков
        verticies.Clear();
        uvs.Clear();
        triangles.Clear();

        // Цикл который будет идти по местам, где может быть установлен блок в чанке
        for (int y = 0; y < ChunkHeight; y++)
        {
            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int z = 0; z < ChunkWidth; z++)
                {
                    GenerateBlock(x, y, z);
                }
            }
        }

        // Закинем в меш как массив
        chunkMesh.triangles = Array.Empty<int>();
        chunkMesh.vertices = verticies.ToArray();
        chunkMesh.uv = uvs.ToArray();
        chunkMesh.triangles = triangles.ToArray();

        // Функция с которой меши будут более оптимально образом расположены вертексы, игра будет быстрее но грузиться дольше
        chunkMesh.Optimize();

        // Чтобы меш нормально взаимодействовал с освещением и проще было вешать коллайдеры
        chunkMesh.RecalculateNormals(); // Для освещения
        chunkMesh.RecalculateBounds(); // Для коллайдеров и всяких юнити штук

        GetComponent<MeshCollider>().sharedMesh = chunkMesh; // Для Mesh Collider
    }

    // Функция по указанным координатам будет устанавливать блок
    public void SpawnBlock(Vector3Int blockPosition, BlockType blockType)
    {
        ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = blockType;
        RegenerateMesh();
    }
    // Функция по указанным координатам будет удалять блок
    public void DestroyBlock(Vector3Int blockPosition)
    {
        if (ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] == BlockType.Bedrock) return; // Чтобы игрок не мог ломать бедрок
        ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockType.Air;
        RegenerateMesh();

    }

    // Функция генерации блоков
    private void GenerateBlock(int x, int y, int z)
    {
        Vector3Int blockPosition = new Vector3Int(x, y, z);

        BlockType blockType = GetBlockAtPosition(blockPosition);
        if (blockType == BlockType.Air) return;

        if (GetBlockAtPosition(blockPosition + Vector3Int.right) == 0)
        {
            GenerateRightSide(blockPosition);
            AddUvs(blockType, Vector3Int.right);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.left) == 0)
        {
            GenerateLeftSide(blockPosition);
            AddUvs(blockType, Vector3Int.left);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.forward) == 0)
        {
            GenerateFrontSide(blockPosition);
            AddUvs(blockType, Vector3Int.forward);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.back) == 0)
        {
            GenerateBackSide(blockPosition);
            AddUvs(blockType, Vector3Int.back);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.up) == 0)
        {
            GenerateTopSide(blockPosition);
            AddUvs(blockType, Vector3Int.up);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.down) == 0)
        {
            GenerateBottomSide(blockPosition);
            AddUvs(blockType, Vector3Int.down);
        }
    }

    // Функция которая будет возвращать блок по положению
    private BlockType GetBlockAtPosition(Vector3Int blockPosition)
    {
        if (blockPosition.x >= 0 && blockPosition.x < ChunkWidth &&
           blockPosition.y >= 0 && blockPosition.y < ChunkHeight &&
           blockPosition.z >= 0 && blockPosition.z < ChunkWidth)
        {
            return ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
        }
        else
        {
            if (blockPosition.y < 0 || blockPosition.y >= ChunkHeight) return BlockType.Air;

            Vector2Int adjacentChunkPosition = ChunkData.ChunkPosition; // Посчитаем координаты чанков
            if (blockPosition.x < 0) // Если вышли по x вниз
            {
                adjacentChunkPosition.x--;
                blockPosition.x += ChunkWidth;
            }
            else if (blockPosition.x >= ChunkWidth) // Если вышли по x вверх
            {
                adjacentChunkPosition.x++;
                blockPosition.x -= ChunkWidth;
            }
            if (blockPosition.z < 0) // Если вышли по z вниз
            {
                adjacentChunkPosition.y--;
                blockPosition.z += ChunkWidth;
            }
            else if (blockPosition.z >= ChunkWidth) // Если вышли по z вверх
            {
                adjacentChunkPosition.y++;
                blockPosition.z -= ChunkWidth;
            }
            // Достаем что за блок в чанке
            if (ParentWorld.ChunkDatas.TryGetValue(adjacentChunkPosition, out ChunkData adjacentChunk))
            {
                return adjacentChunk.Blocks[blockPosition.x, blockPosition.y, blockPosition.z]; // Если нашла то переходим по blockPosition
            }


            return BlockType.Air;
        }
    }

    // Для всех блоков которые установлены в массиве, будут рисоваться правые стороны
    private void GenerateRightSide(Vector3Int blockPosition)
    {
        // Чтобы создать один треугольник, нужны вертексы
        verticies.Add((new Vector3(x: 1, y: 0, z: 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(x: 1, y: 1, z: 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(x: 1, y: 0, z: 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(x: 1, y: 1, z: 1) + blockPosition) * BlockScale);
        AddLastVerticiesSquare();
    }

    // Для всех блоков которые установлены в массиве, будут рисоваться левые стороны
    private void GenerateLeftSide(Vector3Int blockPosition)
    {
        // Чтобы создать один треугольник, нужны вертексы
        verticies.Add((new Vector3(x: 0, y: 0, z: 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(x: 0, y: 0, z: 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(x: 0, y: 1, z: 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(x: 0, y: 1, z: 1) + blockPosition) * BlockScale);
        AddLastVerticiesSquare();
    }

    // Для всех блоков которые установлены в массиве, будут рисоваться спереди
    private void GenerateFrontSide(Vector3Int blockPosition)
    {
        // Чтобы создать один треугольник, нужны вертексы
        verticies.Add((new Vector3(x: 0, y: 0, z: 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(x: 1, y: 0, z: 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(x: 0, y: 1, z: 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(x: 1, y: 1, z: 1) + blockPosition) * BlockScale);
        AddLastVerticiesSquare();
    }

    // Для всех блоков которые установлены в массиве, будут рисоваться сзади
    private void GenerateBackSide(Vector3Int blockPosition)
    {
        // Чтобы создать один треугольник, нужны вертексы
        verticies.Add((new Vector3(x: 0, y: 0, z: 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(x: 0, y: 1, z: 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(x: 1, y: 0, z: 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(x: 1, y: 1, z: 0) + blockPosition) * BlockScale);
        AddLastVerticiesSquare();
    }

    // Для всех блоков которые установлены в массиве, будут рисоваться сверху
    private void GenerateTopSide(Vector3Int blockPosition)
    {
        // Чтобы создать один треугольник, нужны вертексы
        verticies.Add((new Vector3(x: 0, y: 1, z: 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(x: 0, y: 1, z: 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(x: 1, y: 1, z: 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(x: 1, y: 1, z: 1) + blockPosition) * BlockScale);
        AddLastVerticiesSquare();
    }

    // Для всех блоков которые установлены в массиве, будут рисоваться снизу
    private void GenerateBottomSide(Vector3Int blockPosition)
    {
        // Чтобы создать один треугольник, нужны вертексы
        verticies.Add((new Vector3(x: 0, y: 0, z: 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(x: 1, y: 0, z: 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(x: 0, y: 0, z: 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(x: 1, y: 0, z: 1) + blockPosition) * BlockScale);
        AddLastVerticiesSquare();
    }

    // Функция для создания треугольников
    private void AddLastVerticiesSquare()
    {
        // Теперь создаём из них треугольник
        triangles.Add(verticies.Count - 4);
        triangles.Add(verticies.Count - 3);
        triangles.Add(verticies.Count - 2);
        // Второй треугольник
        triangles.Add(verticies.Count - 3);
        triangles.Add(verticies.Count - 1);
        triangles.Add(verticies.Count - 2);
    }
    // Чтобы спавнились разные блоки, текстуры берутся из материала, там эти текстуры есть
    private void AddUvs(BlockType blockType, Vector3Int normal)
    {
        Vector2 uv;
        /* 
        первое значение к примеру 16f/256 это место в ряду, а вот второе значение к примеру 240f/256 это ряд
        как работают координаты?
            вся текстура это координаты от 0 до еденички, левый нижний угол 0.0 а правый верхний 1.1
            сам код он высчитывает, он делит чтобы получить нужный результат, чтобы самому не считать а считала программа
            в общем 16 строк текстур и на каждой строке по 16 текстур 
        */
        switch (blockType)
        {
            // ПЕРВЫЙ РЯД
            case BlockType.Stone: // Камень
                uv = new Vector2(16f / 256, 240f / 256);
                break;
            case BlockType.Dirt: // Грязь
                uv = new Vector2(32f / 256, 240f / 256);
                break;
            case BlockType.Grass: // Трава с землёй
                uv = normal == Vector3Int.up ? new Vector2(0, 240f / 256) :
                     normal == Vector3Int.down ? new Vector2(32f / 256, 240f / 256) :
                     new Vector2(48f / 256, 240f / 256);
                break;
            case BlockType.Plank: // Доски
                uv = new Vector2(64f / 256, 240f / 256);
                break;
            case BlockType.Sand: // Песок
                uv = new Vector2(80f / 256, 240f / 256);
                break;
            case BlockType.Bricks: // Кирпичи
                uv = new Vector2(96f / 256, 240f / 256);
                break;
            case BlockType.Bedrock: // Бедрок
                uv = new Vector2(112f / 256, 240f / 256);
                break;
            case BlockType.Water: // Вода
                uv = new Vector2(128f / 256, 240f / 256);
                break;
            // ВТОРОЙ РЯД
            case BlockType.Cobblestone: // Булыжник
                uv = new Vector2(0f / 256, 224f / 256);
                break;
            case BlockType.Wood: // Дерево
                uv = normal == Vector3Int.up ? new Vector2(80f / 256, 224f / 256) :
                     normal == Vector3Int.down ? new Vector2(80f / 256, 224f / 256) :
                     new Vector2(64f / 256, 224f / 256);
                break;
            // ЧЁТВЕРТЫЙ РЯД
            case BlockType.Leaves: // Листва
                uv = new Vector2(80f / 256, 192f / 256);
                break;
            // ПРОЧЕЕ
            default: // Неизвестный блок (Блок без текстуры)
                uv = new Vector2(160f / 256, 224f / 256);
                break;
        }
        for (int i = 0; i < 4; i++) { uvs.Add(uv); }
    }
}
