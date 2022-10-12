using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer))]
public class Corridor : Builder
{
    public enum WallType { Wall, Window, Door }

    public bool buildFrontWall;
    public bool buildBackWall;
    public bool buildLeftWall;
    public bool buildRightWall;

    [HideInInspector]
    public GameObject ceiling;
    [HideInInspector]
    public GameObject floor;
    [HideInInspector]
    public GameObject frontWall;
    [HideInInspector]
    public GameObject backWall;
    [HideInInspector]
    public GameObject leftWall;
    [HideInInspector]
    public GameObject rightWall;

    // Types: 
    public WallType frontWallType = 0;
    public WallType backWallType = 0;
    public WallType leftWallType = 0;
    public WallType rightWallType = 0;

    public GameObject wallPrefab;
    public GameObject wallWindowPrefab;
    public GameObject wallDoorPrefab;
    public GameObject floorPrefab;

    public float floorSize = 1;

    public override void Build()
    {
        floor = PlaceObject(floorPrefab);
        floor.transform.localScale = new Vector3(floorSize, 1, floorSize);
        
        // Ceiling
        ceiling = PlaceObject(floorPrefab);
        ceiling.transform.localScale = new Vector3(floorSize, 1, floorSize);

        // Ceiling snap point
        var ceilingSnapPoint = GetSnapPoint(ceiling, new Vector3(0, -1, 0));
        var ceilingHeightSnapPoint = new Vector3(ceiling.transform.position.x, ceiling.transform.position.y + ceilingHeight, ceiling.transform.position.z);
        SnapTo(ceiling, ceilingSnapPoint, ceilingHeightSnapPoint);


        // Floor snap points
        var floorForwardSnapPoint = GetSnapPoint(floor, new Vector3(0, 1, 1));
        var floorBackSnapPoint = GetSnapPoint(floor, new Vector3(0, 1, -1));
        var floorLeftSnapPoint = GetSnapPoint(floor, new Vector3(-1, 1, 0));
        var floorRightSnapPoint = GetSnapPoint(floor, new Vector3(1, 1, 0));

        // Walls
        if (buildFrontWall)
        {
            frontWall = BuildWall(frontWallType, floorForwardSnapPoint, 0.0f);
        }
        if (buildBackWall)
        {
            backWall = BuildWall(backWallType, floorBackSnapPoint, 180.0f);
        }
        if (buildLeftWall)
        {
            leftWall = BuildWall(leftWallType, floorLeftSnapPoint, -90.0f);
        }
        if (buildRightWall) 
        {
            rightWall = BuildWall(rightWallType, floorRightSnapPoint, 90.0f);
        }

        // Enumerate pieces
        base.Build();
    }

    public GameObject BuildWall(WallType type, Vector3 floorSnapPoint, float rotation)
    {
        // Choose prefab based on type number
        GameObject prefab;
        switch (type)
        {
            case WallType.Wall:
                prefab = wallPrefab;
                break;
            case WallType.Window:
                prefab = wallWindowPrefab;
                break;
            case WallType.Door:
                prefab = wallDoorPrefab;
                break;
            default:
                prefab = wallPrefab;
                break;
        }

        // Place wall object and snap it to the floor
        var wall = PlaceObject(prefab);
        var wallSnapPoint = GetSnapPoint(wall, new Vector3(0, -1, 1));

        wall.transform.localScale = new Vector3(floorSize, 1.0f, 1.0f);
        RotateAroundSnapPoint(wall, wallSnapPoint, Vector3.up, rotation);
        SnapToObject(wall, wallSnapPoint, floorSnapPoint);

        return wall;
    }

    public override void EnumeratePieces()
    {
        Pieces.Add(floor);
        Pieces.Add(ceiling);
        if (frontWall) Pieces.Add(frontWall);
        if (backWall) Pieces.Add(backWall);
        if (leftWall) Pieces.Add(leftWall);
        if (rightWall) Pieces.Add(rightWall);
    }
}
