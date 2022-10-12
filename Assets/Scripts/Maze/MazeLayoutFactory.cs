/// Maze Generation Algorithm

using System;
using System.Collections.Generic;
using System.Linq;
using Valve.VR.InteractionSystem;

/// <summary>
/// Coordinates for maze cells
/// </summary>
public struct MazeCoords
{
    public int x;
    public int y;

    public MazeCoords(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

/// <summary>
/// Information stored for a cell in the maze grid
/// </summary>
public struct MazeCell
{
    public MazeCoords position;
    public MazeCell[] neighbours; // 0 = North, 1 = East, 2 = South, 3 = West
    public bool visited;
    public List<Tuple<MazeCell, PathDirection>> paths;

    public MazeCell(int posX, int posY)
    {
        position = new MazeCoords(posX, posY);
        neighbours = new MazeCell[4];
        visited = false;
        paths = new List<Tuple<MazeCell, PathDirection>>();
    }
}

/// <summary>
/// All possible directions a maze path can go in
/// </summary>
public enum PathDirection
{
    North, South, East, West
}

/// <summary>
/// Factory for generating a random maze
/// </summary>
public static class MazeLayoutFactory
{
    private static int mazeDimX;
    private static int mazeDimY;
    private static MazeCell[,] maze;

    /// <summary>
    /// Randomly generates a maze with a random seed and the specified dimensions
    /// </summary>
    /// <param name="dimX"></param>
    /// <param name="dimY"></param>
    /// <returns></returns>
    public static MazeCell[,] GenerateMaze(int dimX, int dimY)
    {
        Random random = new Random();
        int seed = random.Next();
        return GenerateMaze(dimX, dimY, seed);
    }
    /// <summary>
    /// Randomly generates a maze with the specified dimensions
    /// </summary>
    /// <param name="dimX"></param>
    /// <param name="dimY"></param>
    /// <param name="seed"></param>
    /// <returns></returns>
    public static MazeCell[,] GenerateMaze(int dimX, int dimY, int seed)
    {
        mazeDimX = dimX;
        mazeDimY = dimY;
        maze = GenerateCells(dimX, dimY);

        var cellStack = new Stack<MazeCell>();
        cellStack.Push(maze[0, 0]);

        // Explore cells until all paths have been exhausted
        do
        {
            var currCell = cellStack.Peek();
            currCell.visited = true;

            MazeCell nextCell = default;
            Random random = new Random(seed);

            // Pick a random neighbour that hasn't yet been visited. Break when one is found
            for (int i = random.Next(0, 3); currCell.neighbours.Count(x => x.visited) == currCell.neighbours.Count(); i = random.Next(0, 3))
            {
                if (!currCell.neighbours[i].visited)
                {
                    nextCell = currCell.neighbours[i];
                    var pathSegment = new Tuple<MazeCell, PathDirection>(nextCell, (PathDirection)i);
                    currCell.paths.Add(pathSegment);
                    break;
                }
            }

            if (!nextCell.Equals(default))
                cellStack.Push(nextCell);
            else
                cellStack.Pop();
        }
        while (cellStack.Count > 0);

        return maze;
    }

    /// <summary>
    /// Generates the cells for the maze
    /// </summary>
    /// <param name="dimX"></param>
    /// <param name="dimY"></param>
    /// <returns></returns>
    private static MazeCell[,] GenerateCells(int dimX, int dimY)
    {
        MazeCell[,] maze = new MazeCell[dimX, dimY];

        for (int x = 0; x < dimX; x++)
        {
            for (int y = 0; y < dimY; y++)
            {
                // Assign position
                maze[x, y].position = new MazeCoords(x, y);
            }
        }

        for (int x = 0; x < dimX; x++)
        {
            for (int y = 0; y < dimY; y++)
            {
                // Determine neighbouring cells
                if (!IsOutOfBounds(x, y + 1)) maze[x, y].neighbours[0] = maze[x, y + 1];
                if (!IsOutOfBounds(x + 1, y)) maze[x, y].neighbours[1] = maze[x + 1, y];
                if (!IsOutOfBounds(x, y - 1)) maze[x, y].neighbours[2] = maze[x, y - 1];
                if (!IsOutOfBounds(x - 1, y)) maze[x, y].neighbours[3] = maze[x - 1, y];
            }
        }

        return maze;
    }

    /// <summary>
    /// Checks if a particular cell coordinate is out of bounds
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private static bool IsOutOfBounds(int x, int y) { return x >= mazeDimX || x < 0 || y >= mazeDimY || y < 0; }
}
