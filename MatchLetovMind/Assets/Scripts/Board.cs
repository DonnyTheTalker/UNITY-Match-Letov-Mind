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
    public GameObject ColorBombPrefab;

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

    private IEnumerator DestroyBomb(GameObject bomb, List<Tuple<int, int>> whereToDestroy)
    {
        float scaleOffeset = 0.3f;

        for (int i = 0; i < 10; i++) {
            bomb.transform.localScale = new Vector3(bomb.transform.localScale.x + scaleOffeset,
                                                    bomb.transform.localScale.y + scaleOffeset,
                                                    bomb.transform.localScale.z + scaleOffeset);
            yield return new WaitForSeconds(0.01f);
        }

        int[] dX = { 0, 0, 1, 1, 1, -1, -1, -1 };
        int[] dY = { 1, -1, 0, 1, -1, 0, 1, -1 };

        var bombTile = bomb.GetComponent<Tile>();

        int x = bombTile.Column, y = bombTile.Row;

        for (int i = 0; i < 8; i++) {

            int x1 = x + dX[i], y1 = y + dY[i];

            if (x1 < Width && y1 < Height && x1 >= 0 && y1 >= 0)
                if (Indexes[x1, y1] != -1)
                    whereToDestroy.Add(new Tuple<int, int>(x1, y1));

        }
    }

    private IEnumerator DestroyColorBomb(GameObject bomb, int index, List<Tuple<int, int>> whereToDestroy)
    {
        float rotationOffset = 30f;
        Debug.Log("HELP");

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                if (Indexes[x, y] == index)
                    whereToDestroy.Add(new Tuple<int, int>(x, y));

        for (int i = 0; i < 20; i++) {
            if (bomb != null) {
                bomb.transform.Rotate(new Vector3(0f, 0f, rotationOffset));
                bomb.transform.localScale = new Vector3(bomb.transform.localScale.x + 0.05f,
                                                    bomb.transform.localScale.y + 0.05f,
                                                    bomb.transform.localScale.z);
            } else
                break;
            yield return new WaitForSeconds(0.005f);
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

    public void StartExplosion(int x, int y, int index = 0)
    {
        if (index < 0) index = 0;
        StartCoroutine(MakeExplosion(x, y, index));
    }

    public IEnumerator MakeExplosion(int x, int y, int index)
    {
        List<Tuple<int, int>> tilesToDestroy = new List<Tuple<int, int>>();
        tilesToDestroy.Add(new Tuple<int, int>(x, y));

        while (tilesToDestroy.Count > 0) {
            List<Tuple<int, int>> nextWave = new List<Tuple<int, int>>();

            for (int i = 0; i < tilesToDestroy.Count; i++) {
                int x1 = tilesToDestroy[i].Item1, y1 = tilesToDestroy[i].Item2;

                if (Indexes[x1, y1] != -1) {
                    if (Indexes[x1, y1] == -2)
                        StartCoroutine(DestroyBomb(Tiles[x1, y1], nextWave));
                    else if (Indexes[x1, y1] == -3) {
                        StartCoroutine(DestroyColorBomb(Tiles[x1, y1], index, nextWave));
                        index = (index + 1) % TilesPrefabs.Length;
                    } else
                        StartCoroutine(DestroyTile(Tiles[x1, y1]));

                    Indexes[x1, y1] = -1;
                    Destroy(Tiles[x1, y1], 0.3f);
                    Tiles[x1, y1] = null;
                }

            }

            tilesToDestroy = nextWave;
            yield return new WaitForSeconds(0.3f);
        }

        StartCoroutine(RefillBoard());

    }

    public void StartRefill()
    {
        StartCoroutine(RefillBoard());
    }

    public IEnumerator RefillBoard()
    {
        CurrectState = GameState.Wait;
        do {
            var bombs = new List<Tuple<int, int, GameObject>>();
            GetAllBombs(4, ref bombs);
            DestroyAllMatches();
            yield return new WaitForSeconds(0.3f);
            SpawnBombs(bombs);
            StartCoroutine(LowerTiles());
            yield return new WaitForSeconds(0.3f);
            FillEmptyTiles();
            yield return new WaitForSeconds(0.3f);
        } while (CanFindMatch());
        CurrectState = GameState.Move;
    }

    private void SpawnBombs(List<Tuple<int, int, GameObject>> bombs)
    {
        for (int i = 0; i < bombs.Count; i++) {

            SpawnBomb(bombs[i].Item1, bombs[i].Item2, bombs[i].Item3, bombs[i].Item3 == ColorBombPrefab ? -3 : -2);

        }
    }

    private void SpawnBomb(int x, int y, GameObject prefab, int index)
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

    private void GetAllBombs(int requiredRange, ref List<Tuple<int, int, GameObject>> regularBombs)
    {

        bool[,] used = new bool[Width, Height];

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                used[x, y] = false;

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                if (!used[x, y] && Indexes[x, y] != -1) {
                    int nTiles = GetGroupSizeAtPoint(x, y);
                    if (nTiles >= 5) {
                        regularBombs.Add(new Tuple<int, int, GameObject>(x, y, ColorBombPrefab));
                        DisableSameGroup(x, y, ref used);
                    } else if (nTiles >= 4) {
                        regularBombs.Add(new Tuple<int, int, GameObject>(x, y, SplashBombPrefab));
                        DisableSameGroup(x, y, ref used);
                    }
                }

    }

    private int GetGroupSizeAtPoint(int startX, int startY)
    {
        int maxVertival = 0;
        int maxHorizontal = 0;

        int tempX = startX;
        int tempY = startY;

        while (tempX < Width && Indexes[tempX, startY] == Indexes[startX, startY]) {
            maxHorizontal++;
            tempX++;
        }


        while (tempY < Height && Indexes[startX, tempY] == Indexes[startX, startY]) {
            maxVertival++;
            tempY++;
        }

        if (maxVertival > 2 && maxHorizontal > 2)
            return maxVertival + maxHorizontal - 1;

        return Math.Max(maxVertival, maxHorizontal);

    }

    private void DisableSameGroup(int startX, int startY, ref bool[,] used)
    {
        int maxVertival = 0;
        int maxHorizontal = 0;

        int tempX = startX;
        int tempY = startY;

        while (tempX < Width && Indexes[tempX, startY] == Indexes[startX, startY]) {
            maxHorizontal++;
            tempX++;
        }


        while (tempY < Height && Indexes[startX, tempY] == Indexes[startX, startY]) {
            maxVertival++;
            tempY++;
        }

        if (maxHorizontal > 2)
            for (int x = startX; x < tempX; x++)
                used[x, startY] = true;


        if (maxVertival > 2)
            for (int y = startY; y < tempY; y++)
                used[startX, y] = true;

    }
}
