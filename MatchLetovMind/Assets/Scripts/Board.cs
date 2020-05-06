using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Board Settings")]
    public int Width;
    public int Height;

    [Header("Tile Setup")] 
    private GameObject[,] _tiles;
    public GameObject[] TilesPrefabs;

    private void Start()
    {
        _tiles = new GameObject[Width, Height];
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
                _tiles[x, y] = tile;

            }
        }
    }

}
