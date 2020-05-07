using System;
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
    public int[,] Indexes;
    public GameObject[] TilesPrefabs;

    private void Start()
    {
        Tiles = new GameObject[Width, Height];
        Indexes = new int[Width, Height];
        Setup();
        RemoveAllMatches();
    }

    private void Setup()
    {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {

                SpawnTile(x, y);

            }
        }
    } 

    public void RemoveAllMatches()
    {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                while (IsMatch(x, y)) {
                    Destroy(Tiles[x, y].gameObject);
                    SpawnTile(x, y);
                }
            }
        }
    }

    private void SpawnTile(int x, int y)
    {
        Vector2 tilePosition = new Vector2(x, y);
        int iTile = UnityEngine.Random.Range(0, TilesPrefabs.Length);
        GameObject tile = Instantiate(TilesPrefabs[iTile], tilePosition, Quaternion.identity) as GameObject;

        tile.transform.parent = this.transform;
        tile.name = "( " + x + ", " + y + ")";
        Tiles[x, y] = tile;
        Indexes[x, y] = iTile;
    }

    public bool CanFindMatch()
    { 
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                if (IsMatch(x, y))
                    return true;

        return false;
    }

    // define if we have a match of 3 (o more - doesn't matter)
    // where tile with coordinates [x, y] is a middle element
    public bool IsMatch(int x, int y, bool horizontal = true, bool vertical = true)
    {
        if (x > 0 && x < Width - 1 && horizontal) {
            if (Indexes[x, y] == Indexes[x - 1, y] && Indexes[x, y] >= 0 && Indexes[x + 1, y] == Indexes[x, y])
                return true;
        }

        if (y > 0 && y < Height - 1 && vertical) {
            if (Indexes[x, y] == Indexes[x, y - 1] && Indexes[x, y] >= 0 && Indexes[x, y + 1] == Indexes[x, y])
                return true;
        }

        return false;
    }

    public void DestroyAllMatches()
    {
        var matches = GetAllMaches();

        for (int i = 0; i < matches.Count; i++) {

            int x = matches[i].Item1;
            int y = matches[i].Item2;

            if (Indexes[x, y] >= 0) {
                Indexes[x, y] = -1;
                Destroy(Tiles[x, y].gameObject);
                Tiles[x, y] = null;
            }

        }

    }

    public List<Tuple<int, int>> GetAllMaches()
    {
        List<Tuple<int, int>> matches = new List<Tuple<int, int>>();

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                
                if (IsMatch(x, y, true, false)) {
                    matches.Add(new Tuple<int, int>(x, y));
                    matches.Add(new Tuple<int, int>(x + 1, y));
                    matches.Add(new Tuple<int, int>(x - 1, y));
                }

                if (IsMatch(x, y, false, true)) {
                    matches.Add(new Tuple<int, int>(x, y));
                    matches.Add(new Tuple<int, int>(x, y + 1));
                    matches.Add(new Tuple<int, int>(x, y - 1));
                }

            }
        }

        return matches;
    }

    public IEnumerator FillEmptyTiles()
    {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                if (Indexes[x, y] >= 0) {

                    // counting empty spaces under non-empty tile
                    int tempRow = y - 1;

                    while (tempRow >= 0 && Indexes[x, tempRow] == -1)
                        tempRow--;
                    tempRow++;

                    if (tempRow != y) {
                        Indexes[x, tempRow] = Indexes[x, y];
                        Indexes[x, y] = -1;
                        Tiles[x, tempRow] = Tiles[x, y];
                        Tiles[x, y] = null;
                        Tiles[x, tempRow].GetComponent<Tile>().SetPos(tempRow, x);
                    }

                }
            }
        }

        yield return new WaitForSeconds(0.4f);

        if (CanFindMatch()) {
            DestroyAllMatches();
            StartCoroutine(FillEmptyTiles());
        }
    }

}
