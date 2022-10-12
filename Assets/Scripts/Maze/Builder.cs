using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public abstract class Builder : MonoBehaviour
{
    [HideInInspector]
    private List<GameObject> pieces = new List<GameObject>();
    [HideInInspector]
    public Bounds bounds = new Bounds();
    protected const float ceilingHeight = 2.8f;

    public List<GameObject> Pieces { get { return pieces; } }
    
    public enum SnapAxes
    {
        X, Y, Z, XY, XZ, YZ, All
    }
    
    private void Awake()
    {
        Build();
    }

    public virtual void Build()
    {
        EnumeratePieces();
        EvaulateBounds();
    }

    public abstract void EnumeratePieces();
    public void EvaulateBounds()
    {
        foreach (var piece in pieces)
        {
            // If piece has a builder script, use the bounds property from it instead
            if (piece.TryGetComponent<Builder>(out var pieceBuilderScript))
                bounds.Encapsulate(pieceBuilderScript.bounds);
            // Otherwise, use bounds from individual meshes
            else
                bounds.Encapsulate(piece.GetComponent<MeshRenderer>().bounds);
        }
    }


    public GameObject[] BuildPart(GameObject prefab, Vector3 snapPoint, Vector3 subPartSnapPointDirection, int numSubParts, float rotation = 0)
    {
        var subParts = new GameObject[numSubParts];

        for (int i = 0; i < numSubParts; i++)
        {
            ref GameObject currSubPart = ref subParts[i];

            if (i == 0)
            {
                currSubPart = PlaceObject(prefab);
                Vector3 subPartSnapPoint = GetSnapPoint(currSubPart, subPartSnapPointDirection);

                RotateAroundSnapPoint(currSubPart, subPartSnapPoint, Vector3.up, rotation);
                SnapToObject(currSubPart, subPartSnapPoint, snapPoint);
            }
            else
            {
                currSubPart = StackObjectNextTo(prefab, subParts[i - 1], Vector3.up);
            }
        }

        return subParts;
    }

    public Vector3 GetSnapPoint(GameObject obj, Vector3 direction)
    {
        var objBounds = obj.GetComponent<MeshRenderer>().bounds;

        Vector3 snapPoint = new Vector3()
        {
            x = Mathf.Lerp(objBounds.min.x, objBounds.max.x, (direction.x + 1.0f) / 2.0f),
            y = Mathf.Lerp(objBounds.min.y, objBounds.max.y, (direction.y + 1.0f) / 2.0f),
            z = Mathf.Lerp(objBounds.min.z, objBounds.max.z, (direction.z + 1.0f) / 2.0f)
        };

        //snapPoint += obj.transform.position;
        return snapPoint;
    }

    public void SnapToObject(GameObject obj, Vector3 objSnapPoint, Vector3 otherObjSnapPoint, SnapAxes snapAxes = SnapAxes.All)
    {
        SnapTo(obj, objSnapPoint, otherObjSnapPoint, snapAxes);
    }

    public void SnapTo(GameObject obj, Vector3 objSnapPoint, Vector3 location, SnapAxes snapAxes = SnapAxes.All)
    {
        Vector3 distance = location - objSnapPoint;
        SnapTo(obj, obj.transform.position + distance, snapAxes);
    }

    public void SnapTo(GameObject obj, Vector3 location, SnapAxes snapAxes = SnapAxes.All)
    {
        Vector3 newLocation = obj.transform.position;
        switch (snapAxes)
        {
            case SnapAxes.X:
                newLocation.x = location.x;
                break;
            case SnapAxes.Y:
                newLocation.y = location.y;
                break;
            case SnapAxes.Z:
                newLocation.z = location.z;
                break;
            case SnapAxes.XY:
                newLocation.x = location.x;
                newLocation.y = location.y;
                break;
            case SnapAxes.XZ:
                newLocation.x = location.x;
                newLocation.z = location.z;
                break;
            case SnapAxes.YZ:
                newLocation.y = location.y;
                newLocation.z = location.z;
                break;
            case SnapAxes.All:
                newLocation = location;
                break;
        }
        
        obj.transform.position = newLocation;
    }

    public void RotateAroundSnapPoint(GameObject obj, Vector3 objSnapPoint, Vector3 axis, float angle)
    {
        obj.transform.RotateAround(objSnapPoint, axis, angle);
    }
        
    public GameObject StackObjectNextTo(GameObject prefab, GameObject otherObject, Vector3 direction)
    {
        var meshRenderer = prefab.GetComponent<MeshRenderer>();
        Vector3 distanceVector = 2 * Vector3.Scale(meshRenderer.bounds.extents, direction);
        return PlaceObjectNextTo(prefab, otherObject, distanceVector);
    }

    public GameObject PlaceObjectNextTo(GameObject prefab, GameObject otherObject, Vector3 distanceVector)
    {
        Vector3 newPos = otherObject.transform.position + distanceVector;
        var instance = PlaceObject(prefab, newPos, otherObject.transform.localRotation);
        return instance;
    }
    
    public GameObject PlaceObject(GameObject prefab, Vector3 startingLocation, Quaternion rotation)
    {
        var instance = PlaceObject(prefab, startingLocation);
        instance.transform.localRotation = rotation;
        return instance;
    }

    public GameObject PlaceObject(GameObject prefab, Vector3 startingLocation)
    {
        var instance = PlaceObject(prefab);
        instance.transform.position = startingLocation;
        return instance;
    }

    public GameObject PlaceObject(GameObject prefab)
    {
        var instance = Instantiate(prefab, transform);
        return instance;
    }
}