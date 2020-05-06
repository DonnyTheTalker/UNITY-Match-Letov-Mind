using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Board Settings")]
    public int Width = 16;
    public int Height = 8;

    [Header("Tile Setup")] 
    public GameObject[,] Tiles;
    public GameObject[] TilesPrefabs;

    private void Start()
    {
        Tiles = new GameObject[Width, Height];
        Setup();
    }

    private void Setup()
    {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {

                Vector2 tilePosition = new Vector2(x, y);
                int iTile = Random.Range(0, TilesPrefabs.Length);
                GameObject tile = Instantiate(TilesPrefabs[iTile], tilePosition, Quaternion.identity) as GameObject;

                tile.transform.parent = this.transform;
                tile.name = "( " + x + ", " + y + ")";
                Tiles[x, y] = tile;

            }
        }
    }

}
