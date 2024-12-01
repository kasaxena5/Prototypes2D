using UnityEngine;
using UnityEngine.Tilemaps;

public class CubeWave : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase tile;
    [SerializeField] private int n;

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

    private Color TileColorFunction(Vector3Int position) 
    {
        float x = position.x;
        float y = position.y;
        float d = Mathf.Sqrt(x*x + y*y);
        float r = Mathf.Sin(d + Time.time);
        return new Color(r, 0, 0);
    }

    void Start()
    {
        CreateTilemap();
    }

    void Update()
    {
        for (int x = -(n/2); x <= (n/2); x++)
        {
            for (int y = -(n/2); y <= (n/2); y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0); 
                Color color = TileColorFunction(position);
                SetTileColor(position, color);
            }
        }
        
    }
}
