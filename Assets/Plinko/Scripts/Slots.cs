using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slots : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private int n;

    private List<GameObject> slots = new List<GameObject>();

    void Start()
    {
        if(n % 2 == 0) 
        {
            Debug.LogWarning("n must be an odd number");
            n++; 
        }

        for (int i = -(n/2); i <= (n/2); i++)
        {
            GameObject slot = Instantiate(slotPrefab);
            slot.transform.position = this.transform.position + new Vector3(i, 0, 0);
            slots.Add(slot);
        }
        
    }

    void OnDrawGizmos()
    {
        for(int i = -(n/2); i <= (n/2); i++)
        {
            SpriteRenderer spriteRenderer = slotPrefab.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                // Get the bounds of the SpriteRenderer
                Bounds bounds = spriteRenderer.bounds;
                bounds.center = this.transform.position + new Vector3(i, 0, 0);

                // Set the Gizmo color
                Gizmos.color = Color.red;

                // Draw a wireframe box that matches the bounds of the GameObject
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
            else
            {
                // Fallback for GameObjects without a SpriteRenderer
                // Use the GameObject's Transform for size and position
                Vector3 size = transform.localScale; // Assuming size is proportional to scale
                Vector3 position = transform.position;

                // Set the Gizmo color
                Gizmos.color = Color.red;

                // Draw a wireframe box
                Gizmos.DrawWireCube(position, size);
            }
        }
    }

    void Update()
    {
        
    }
}
