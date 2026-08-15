// ƒанный скрипт будет хранить в себе массивы блоков


using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class ChunkData
{
    public Vector2Int ChunkPosition; // „тобы чанк внутри себ€ знал в каких он координатах и мог получить блоки соседних чанков
    public ChunkRenderer Renderer;
    public BlockType[,,] Blocks;
}
