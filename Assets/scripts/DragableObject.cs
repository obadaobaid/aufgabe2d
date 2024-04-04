using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 offset;
    private bool isDragging = false;
    private GameObject hitObject;

    // Reference to the GameManager script
    public GameManager gameManager;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Start dragging
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                // Calculate offset to keep object under cursor center
                offset = transform.position - hit.point;
            }
        }

        // While dragging
        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector3 newPosition = hit.point + offset;
                // Optional: Adjust newPosition based on your game's specific requirements
                newPosition.y = transform.position.y; // Maintain original y-coordinate
                transform.position = newPosition;
            }
        }

        // Release and check for target beneath
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            CheckForTargetBelow();
        }
    }

    void CheckForTargetBelow()
    {
        // Cast a ray downwards to detect if we are over a target
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            // Check if the object hit is a target
            if (hit.collider.CompareTag("left_box") || hit.collider.CompareTag("right_box"))
            {
                // If it's a target, inform the GameManager
                GameObject hitObject = hit.collider.gameObject;
                Debug.Log($"hitObject: {hitObject.name}");
                gameManager.RegisterUserSelection(hitObject);
            }
        }
    }
    public GameObject GetHitObject()
    {
        return hitObject;
    }


}
