using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public enum GameState
{
    Move, Wait
}

public class Board : MonoBehaviour
{
    [Header("Board Settings")]
    public int Width = 16;
    public int Height = 8;

    public GameState CurrectState = GameState.Move;
    public GameObject[,] Tiles;
    public int[,] Indexes;
    
    public GameObject[] TilesPrefabs;
    public GameObject SplashBombPrefab;
    
    public Color DeathColor;

    private void Start()
    {
        Tiles = new GameObject[Width, Height];
        Indexes = new int[Width, Height];
        Setup();
        RemoveAllMatches();
        CurrectState = GameState.Move;
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
        bool hasMatch = true;
        while (hasMatch) {
            hasMatch = false;
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    while (IsMatch(x, y)) {
                        Destroy(Tiles[x, y].gameObject);
                        SpawnTile(x, y);
                        hasMatch = true;
                    }
                }
            }
        }
    }

    private void SpawnTile(int x, int y)
    {
        Vector2 tilePosition = new Vector2(x, y);
        int iTile = UnityEngine.Random.Range(0, TilesPrefabs.Length);
        GameObject tile = Instantiate(TilesPrefabs[iTile], tilePosition, Quaternion.identity) as GameObject;

        tile.GetComponent<Tile>().SetPos(y, x);
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

    // define if we have a match of 3 (or more - doesn't matter)
    // where tile with coordinates [x, y] is a middle element
    public bool IsMatch(int x, int y, bool horizontal = true, bool vertical = true)
    {
        if (x > 0 && x < Width - 1 && horizontal) {
            if (Indexes[x, y] == Indexes[x - 1, y] && Indexes[x, y] != -1 && Indexes[x + 1, y] == Indexes[x, y])
                return true;
        }

        if (y > 0 && y < Height - 1 && vertical) {
            if (Indexes[x, y] == Indexes[x, y - 1] && Indexes[x, y] != -1 && Indexes[x, y + 1] == Indexes[x, y])
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

            if (Indexes[x, y] != -1) {
                Indexes[x, y] = -1;
                StartCoroutine(DestroyTile(Tiles[x, y]));
                Destroy(Tiles[x, y], 0.3f);
                Tiles[x, y] = null;
            }

        }

    }

    private IEnumerator DestroyTile(GameObject tile)
    {  
        var spriteRenderer = tile.GetComponent<SpriteRenderer>(); 

        float redOffset = (spriteRenderer.color.r - DeathColor.r) / 10f;
        float greenOffset = (spriteRenderer.color.g - DeathColor.g) / 10f;
        float blueOffset = (spriteRenderer.color.b - DeathColor.b) / 10f;
        float alphaOffset = (spriteRenderer.color.a - DeathColor.a) / 10f;

        for (int i = 0; i < 10; i++) {

            float red = spriteRenderer.color.r;
            float green = spriteRenderer.color.g;
            float blue = spriteRenderer.color.b;
            float alpha = spriteRenderer.color.a;

            spriteRenderer.color = new Color(red - redOffset, green - greenOffset,
                blue - blueOffset, alpha - alphaOffset);
            yield return null;
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

    public void StartRefill()
    {
        StartCoroutine(RefillBoard());
    }

    public IEnumerator RefillBoard()
    {
        CurrectState = GameState.Wait;
        while (CanFindMatch()) { 
            var bombs = GetAllBombs(4);
            DestroyAllMatches();
            yield return new WaitForSeconds(0.3f);
            SpawnBombs(bombs, SplashBombPrefab);
            StartCoroutine(LowerTiles());
            yield return new WaitForSeconds(0.3f);
            FillEmptyTiles();
            yield return new WaitForSeconds(0.3f);
        }
        CurrectState = GameState.Move;
    }

    private void SpawnBombs(List<Tuple<int, int>> bombs, GameObject BombPrefab, int index = -2)
    {
        for (int i = 0; i < bombs.Count; i++) {

            SpawnBomb(bombs[i].Item1, bombs[i].Item2, BombPrefab, index);

        }
    }

    private void SpawnBomb(int x, int y, GameObject prefab, int index = -2)
    {
        Vector2 tilePosition = new Vector2(x, y); 
        GameObject tile = Instantiate(prefab, tilePosition, Quaternion.identity) as GameObject;

        tile.GetComponent<Tile>().SetPos(y, x);
        tile.transform.parent = this.transform;
        tile.name = "( " + x + ", " + y + ")";
        Tiles[x, y] = tile;
        Indexes[x, y] = index;
    }

    public IEnumerator LowerTiles()
    { 
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                if (Indexes[x, y] != -1) {

                    // counting empty spaces under non-empty tile
                    int tempRow = y - 1;

                    while (tempRow >= 0 && Indexes[x, tempRow] == -1)
                        tempRow--;
                    tempRow++;

                    if (tempRow != y) {
                        int temp = Indexes[x, y];
                        Indexes[x, tempRow] = temp;
                        Indexes[x, y] = -1;
                        Tiles[x, tempRow] = Tiles[x, y];
                        Tiles[x, y] = null;
                        Tiles[x, tempRow].GetComponent<Tile>().SetPos(tempRow, x);
                    }

                }
            }
        }
        yield return new WaitForSeconds(0.4f);
    }

    public void FillEmptyTiles()
    { 
        for (int x = 0; x < Width; x++) 
            for (int y = 0; y < Height; y++) 
                if (Indexes[x, y] == -1) {

                    int tempRow = Height;

                    while (y < Height) { 
                        SpawnTile(x, y);
                        var tile = Tiles[x, y].GetComponent<Tile>();
                        tile.transform.position = new Vector3(x, tempRow, 0f);
                        tile.SetPos(y, x);

                        y++;
                        tempRow++;
                    }

                    break;
                }
    }

    private List<Tuple<int, int>> GetAllBombs(int requiredRange)
    {
        List<Tuple<int, int>> bombs = new List<Tuple<int, int>>();

        bool[,] used = new bool[Width, Height];

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                used[x, y] = false;

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                if (!used[x, y] && GetGroupSizeAtPoint(x, y, used) >= requiredRange) {
                    bombs.Add(new Tuple<int, int>(x, y));
                }
 

        return bombs;
    }

    private int GetGroupSizeAtPoint(int startX, int startY, bool[,] used)
    {
        int maxVertival = 0;
        int maxHorizontal = 0;

        int tempX = startX;
        int tempY = startY;

        while (tempX < Width && !used[tempX, startY] && Indexes[tempX, startY] == Indexes[startX, startY]) {
            maxHorizontal++;
            used[tempX, startY] = true;
            tempX++;
        }

        used[startX, startY] = false;

        while (tempY < Height && !used[startX, tempY] && Indexes[startX, tempY] == Indexes[startX, startY]) {
            maxVertival++; 
            used[startX, tempY] = true;
            tempY++;
        } 

        if (maxVertival > 2 && maxHorizontal > 2) 
            return maxVertival + maxHorizontal - 1;
        return Math.Max(maxVertival, maxHorizontal);
        
    }

}
