using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaveFunction: MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase[] tiles;
    [SerializeField] private int n;

    private int[] dx = new int[] {0, 0, -1, 1};
    private int[] dy = new int[] {1, -1, 0, 0};
    private long[,] rules;

    public class WaveFunctionTile
    {
        private int length;
        private long entropy;
        private static long[,] rules;

        public bool ApplyRules(WaveFunctionTile other, int k, long[,] rules)
        {
            long otherEntropy = other.GetEntropy();
            long oldEntropy = entropy;

            long mask = 0;
            for(int idx = 0; idx < length; idx++)
            {
                if((otherEntropy & (1L << idx)) != 0)
                {
                    mask |= rules[idx, k];
                }
            }
            entropy &= mask;

            return entropy != oldEntropy;
        }

        public long GetEntropy() {
            return entropy;
        }

        public int GetIndex() {
            if(!hasCollapsed())
                return -1;

            for(int i = 0; i < length; i++) {
                if((entropy & (1L << i)) != 0) {
                    return i;
                }
            }

            return -1;
        }

        public void Collapse() {
            // select a random set bit and unset all other bits and return the index of the set bit
            int i = -1;
            while(!hasCollapsed()) {
                i = Random.Range(0, length);
                if((entropy & (1L << i)) != 0) {
                    entropy = 1L << i;
                    break;
                }
            }
        }

        public bool hasCollapsed() {
            // return true if entropy has only one bit set
            return (entropy & (entropy - 1)) == 0;
        }

        public WaveFunctionTile(TileBase[] tiles)
        {
            length = tiles.Length;
            for(int i = 0; i < tiles.Length; i++)
            {
                entropy |= (1L << i);
            }
        }
    }
    private WaveFunctionTile[,] waveFunctionGrid;

    public enum Direction
    {
        Top = 0,    
        Down = 1,  
        Left = 2, 
        Right = 3
    }

    public static float ColorDistance(Color color1, Color color2)
    {
        float rDiff = color2.r - color1.r;
        float gDiff = color2.g - color1.g;
        float bDiff = color2.b - color1.b;
        float aDiff = color2.a - color1.a; 

        return Mathf.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff + aDiff * aDiff);
    }

    private Color GetPixelColor(int tileIndex, Direction k) 
    {
        TileBase tile = tiles[tileIndex];
        Sprite sprite = (tile as RuleTile)?.m_DefaultSprite;
        Texture2D texture = sprite.texture;

        Rect rect = sprite.rect;
        int delta = 4;
        if(k == Direction.Top) {
            Vector2 topCenter = new Vector2(rect.xMin + rect.width / 2f, rect.yMax - delta);
            return texture.GetPixel((int)topCenter.x, (int)topCenter.y);
        }

        if(k == Direction.Down) {
            Vector2 bottomCenter = new Vector2(rect.xMin + rect.width / 2f, rect.yMin + delta);
            return texture.GetPixel((int)bottomCenter.x, (int)bottomCenter.y);
        }

        if(k == Direction.Right) {
            Vector2 rightCenter = new Vector2(rect.xMax - delta, rect.yMin + rect.height / 2f);
            return texture.GetPixel((int)rightCenter.x, (int)rightCenter.y);
        }

        if(k == Direction.Left) {
            Vector2 leftCenter = new Vector2(rect.xMin + delta, rect.yMin + rect.height / 2f);
            return texture.GetPixel((int)leftCenter.x, (int)leftCenter.y);
        }

        return Color.clear;
    }

    private void CreateRules()
    {
        rules = new long[tiles.Length, 4];
        for(int i = 0; i < tiles.Length; i++)
        {
            Color topColor = GetPixelColor(i, Direction.Top);
            Color downColor = GetPixelColor(i, Direction.Down);
            Color leftColor = GetPixelColor(i, Direction.Left);
            Color rightColor = GetPixelColor(i, Direction.Right);

            for(int j = 0; j < tiles.Length; j++)
            {
                Color otherTopColor = GetPixelColor(j, Direction.Top);
                Color otherDownColor = GetPixelColor(j, Direction.Down);
                Color otherLeftColor = GetPixelColor(j, Direction.Left);
                Color otherRightColor = GetPixelColor(j, Direction.Right);

                float delta = 0.3f;

                if(ColorDistance(topColor, otherDownColor) < delta) {
                    rules[i, (int)Direction.Top] |= (1L << j);
                }

                if(ColorDistance(downColor, otherTopColor) < delta) {
                    rules[i, (int)Direction.Down] |= (1L << j);
                }

                if(ColorDistance(leftColor, otherRightColor) < delta) {
                    rules[i, (int)Direction.Left] |= (1L << j);
                }

                if(ColorDistance(rightColor, otherLeftColor) < delta) {
                    rules[i, (int)Direction.Right] |= (1L << j);
                }
            }
        }
    }

    void ApplyRules(int x_s, int y_s)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(x_s, y_s));

        while(!(queue.Count == 0)) {
            Vector2Int current = queue.Dequeue();
            int x = current.x;
            int y = current.y;
            for(int k = 0; k < 4; k++)
            {
                int nx = x + dx[k];
                int ny = y + dy[k];
                if(nx >= 0 && nx < n && ny >= 0 && ny < n)
                {
                    if(waveFunctionGrid[nx, ny].ApplyRules(waveFunctionGrid[x, y], k, rules)) {
                        queue.Enqueue(new Vector2Int(nx, ny));
                    }
                }
            }
        }
    }

    void CreateTilemap()
    {
        if(n % 2 == 0)
        {
            Debug.LogWarning("n must be odd");
            n++;
        }

        // Initialize the wave function grid
        waveFunctionGrid = new WaveFunctionTile[n, n];
        for(int i = 0; i < n; i++)
        {
            for(int j = 0; j < n; j++)
            {
                waveFunctionGrid[i, j] = new WaveFunctionTile(tiles);
            }
        }

        // BFS to collapse the wave function grid
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        int[,] visited = new int[n, n];
        queue.Enqueue(new Vector2Int(0, 0));
        visited[0, 0] = 1;

        while(!(queue.Count == 0))
        {
            Vector2Int current = queue.Dequeue();
            int x = current.x;
            int y = current.y;

            // Collapse the wave function grid at (x, y)
            waveFunctionGrid[x, y].Collapse();
            ApplyRules(x, y);

            // Add the neighbors to the queue
            for(int k = 0; k < 4; k++)
            {
                int nx = x + dx[k];
                int ny = y + dy[k];
                if(nx >= 0 && nx < n && ny >= 0 && ny < n)
                {
                    if(visited[nx, ny] == 0)
                    {
                        visited[nx, ny] = 1;
                        queue.Enqueue(new Vector2Int(nx, ny));
                    }
                }
            }
        }

        for(int i = 0; i < n; i++)
        {
            for(int j = 0; j < n; j++)
            {
                Vector3Int position = new Vector3Int(i - (n/2), j -(n/2), 0);
                WaveFunctionTile waveFunctiontile = waveFunctionGrid[i, j];
                tilemap.SetTile(position, tiles[waveFunctiontile.GetIndex()]);
            }
        }
    }

    void Start()
    {
        CreateRules();
        for(int i = 0; i < tiles.Length; i++)
        {
            for(int k = 0; k < 4; k++) {
                string binary = System.Convert.ToString(rules[i, k], 2);
                Debug.Log($"Rules for tile {i} in direction {(Direction)k}: {binary}");
            }

        }
        CreateTilemap();
    }

    void Update()
    {
        
    }
}
