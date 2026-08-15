// Данный скрипт нужен для того чтобы генерировать ландшафт


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LandscapeGenerator
{
    public static BlockType[,,] GenerateLandscape(int xOffset, int zOffset, int dirtLayerHeight)
    {
        var result = new BlockType[ChunkRenderer.ChunkWidth, ChunkRenderer.ChunkHeight, ChunkRenderer.ChunkWidth];

        // Получаем имя текущей загруженной сцены
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        for (int x = 0; x < ChunkRenderer.ChunkWidth; x++)
        {
            for (int z = 0; z < ChunkRenderer.ChunkWidth; z++)
            {
                float height = Mathf.PerlinNoise((x / 8f + xOffset) * .2f, (z / 8f + zOffset) * .2f) * 6 + 6;

                // Устанавливаем тип блока в зависимости от сцены
                if (currentSceneName == "ClassicWorld")
                {
                    result[x, (int)height - 1, z] = BlockType.Grass;
                    for (int y = (int)height - 2; y >= 0; y--)
                    {
                        result[x, y, z] = BlockType.Dirt;
                    }
                }
                else if (currentSceneName == "SandWorld")
                {
                    result[x, (int)height - 1, z] = BlockType.Sand;
                    for (int y = (int)height - 2; y >= 0; y--)
                    {
                        result[x, y, z] = BlockType.Sand;
                    }
                }
                else if (currentSceneName == "StoneWorld")
                {
                    result[x, (int)height - 1, z] = BlockType.Stone;
                    for (int y = (int)height - 2; y >= 0; y--)
                    {
                        result[x, y, z] = BlockType.Stone;
                    }
                }

                for (int y = 0; y < (int)height - 2; y++)
                {
                    result[x, y, z] = BlockType.Stone;
                }

                result[x, 0, z] = BlockType.Bedrock;
            }
        }

        return result;
    }
}
