using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[ExecuteInEditMode]
public class AlignWithTerrainNormal : MonoBehaviour
{
    private void Update()
    {
        Vector3 rayStartPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 10.0f);
        Ray ray = new Ray(rayStartPos, Vector3.down);
        Physics.Raycast(ray, out RaycastHit hit, 20.0f);
        transform.rotation = Quaternion.LookRotation(hit.normal);

    }
}
