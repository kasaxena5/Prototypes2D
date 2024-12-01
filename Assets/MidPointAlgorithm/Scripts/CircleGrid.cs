using UnityEngine;
using UnityEngine.Tilemaps;

public class CircleGrid: MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase tile;
    [SerializeField] private int n;
    [SerializeField] private Color color;
    [SerializeField] private int r;

    private void CreateTilemap()
    {
        if(n % 2 == 0) 
        {
            Debug.LogWarning("n must be an odd number");
            n++; 
        }

        for (int x = -(n/2); x <= (n/2); x++)
        {
            for (int y = -(n/2); y <= (n/2); y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0); 
                tilemap.SetTile(position, tile);
            }
        }
    }

    private TileBase GetTile(Vector3Int position)
    {
        if (tilemap.HasTile(position))
        {
            return tilemap.GetTile(position);
        }
        else
        {
            Debug.LogWarning($"No tile found at {position}");
            return null;
        }
    }

    private void SetTileColor(Vector3Int position, Color color)
    {
        if (tilemap.HasTile(position))
        {
            TileBase tile = tilemap.GetTile(position);
            tilemap.SetTileFlags(position, TileFlags.None); 
            tilemap.SetColor(position, color);
        }
        else
        {
            Debug.LogWarning($"No tile found at {position}");
        }
    }

    void Start()
    {
        CreateTilemap();
        for (int i = -(n/2); i <= (n/2); i++)
        {
            for (int j = -(n/2); j <= (n/2); j++)
            {
                Vector3Int position = new Vector3Int(i, j, 0); 
                SetTileColor(position, Color.black);
            }
        }

        int x = 0;
        int y = r;
        while (x <= y) {

            SetTileColor(new Vector3Int(x, y, 0), color);
            SetTileColor(new Vector3Int(-x, y, 0), color);
            SetTileColor(new Vector3Int(x, -y, 0), color);
            SetTileColor(new Vector3Int(-x, -y, 0), color);
            SetTileColor(new Vector3Int(y, x, 0), color);
            SetTileColor(new Vector3Int(-y, x, 0), color);
            SetTileColor(new Vector3Int(y, -x, 0), color);
            SetTileColor(new Vector3Int(-y, -x, 0), color);

            float ymid = y - 0.5f;
            x++;
            if (x*x + ymid * ymid > r*r) {
                y--;
            }

        }


    }

    void Update()
    {
        
    }
}
