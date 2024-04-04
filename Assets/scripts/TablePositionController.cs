using UnityEngine;

public class TablePositionController : MonoBehaviour
{
    public Vector3 minPosition;
    public Vector3 maxPosition;

    void LateUpdate()
    {
        // Begrenzen Sie die Position des Tisches innerhalb des minPosition und maxPosition Bereichs
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minPosition.x, maxPosition.x),
            Mathf.Clamp(transform.position.y, minPosition.y, maxPosition.y),
            Mathf.Clamp(transform.position.z, minPosition.z, maxPosition.z)
        );
    }
}
