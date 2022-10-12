using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class MazeComponent : MonoBehaviour
{
    private MazeCell[,] maze;
    private float cellScale = 5.0f;

    public Vector2Int dimensions = new Vector2Int(10, 10);
    public int seed;

    // Start is called before the first frame update
    private void Awake()
    {
        maze = MazeLayoutFactory.GenerateMaze(dimensions.x, dimensions.y, seed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}