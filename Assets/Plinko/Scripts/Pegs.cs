using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pegs : MonoBehaviour
{

    [SerializeField] private GameObject pegsPrefab;
    [SerializeField] private int n;

    private List<GameObject> pegs = new List<GameObject>();

    private Vector3 GetPegPosition(int i, int j)
    {
        return this.transform.position + new Vector3(i - (n/2), j - (n/2), 0);
    }

    void Start()
    {
        if(n % 2 == 0) 
        {
            Debug.LogWarning("n must be an odd number");
            n++; 
        }
        for(int i = 0; i < n; i++) 
        {
            for(int j = 0; j < n; j++) 
            {
                if((i + j) % 2 == 0) 
                    continue;
                GameObject peg = Instantiate(pegsPrefab);
                peg.transform.position = GetPegPosition(i, j);
                pegs.Add(peg);
            }
        }
    }

    void OnDrawGizmos()
    {
        for(int i = 0; i < n; i++) 
        {
            for(int j = 0; j < n; j++) 
            {
                if((i + j) % 2 == 0) 
                    continue;

            SpriteRenderer spriteRenderer = pegsPrefab.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                Bounds bounds = spriteRenderer.bounds;
                bounds.center = GetPegPosition(i, j);

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(bounds.center, bounds.size.x / 2);
            }
            else
            {
                Debug.LogWarning("No SpriteRenderer found in pegsPrefab");
                Vector3 size = transform.localScale; 
                Vector3 position = transform.position;

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(position, size);
            }

            }
        }
    }

    void Update()
    {
        
    }
}
