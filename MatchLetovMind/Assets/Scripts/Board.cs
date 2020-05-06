using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Board Settings")]
    public int Width;
    public int Height;

    [Header("Tile Setup")]
    public GameObject TilePrefab;
    private TileBackground[,] _tiles;

    private void Start()
    {
        _tiles = new TileBackground[Width, Height];
        Setup();
    }

    private void Setup()
    {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {

                Vector2 tilePosition = new Vector2(x, y);
                Instantiate(TilePrefab, tilePosition, Quaternion.identity);

            }
        }
    }

}
