using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sinewave : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] float amp = 1f;
    [SerializeField] int n;
    [SerializeField] float width = 0.1f;
    [SerializeField] float gap = 1f;


    [Header("Prefabs")]
    [SerializeField] GameObject rectanglePrefab;

    List<GameObject> rectangles = new List<GameObject>();

    void Start()
    {
        for(int i = 0; i < n; i++)
        {
            GameObject rectangle = Instantiate(rectanglePrefab);
            rectangle.transform.position = new Vector3(i * gap, 0, 0);
            rectangle.transform.localScale = new Vector3(width, amp * Mathf.Sin(i*gap), 1);

            rectangles.Add(rectangle);
        }
    }

    void Update()
    {
        for(int i = 0; i < n; i++) {
            rectangles[i].transform.localScale = new Vector3(width, amp * Mathf.Sin(i*gap + Time.time), 1);

        }
        
    }
}
