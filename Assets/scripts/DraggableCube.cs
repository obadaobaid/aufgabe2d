using UnityEngine;

public class DraggableCube : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 offset;
    private bool isDragging = false;
    private GameObject hitObject;

    // reference to the GameManager script
    private GameManager gameManager;
    private Vector3 startPosition;

    // Initializes references and the initial position of the cube.
    void Awake()
    {
        mainCamera = Camera.main;
        gameManager = FindObjectOfType<GameManager>();
        startPosition = transform.position;
    }

    // checks for mouse input to start dragging the cube and updates its position while dragging.
    void Update()
    {
        // Start dragging
        if (Input.GetMouseButtonDown(0))
        {
            DragCube();
        }

        // While dragging
        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector3 newPosition = hit.point + offset;
                newPosition.y = transform.position.y; 
                transform.position = newPosition;
            }
        }
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
        }
    }

    // Casts a ray to detect if the cube is clicked and sets up dragging behavior.
    private void DragCube()
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

    // returns the game object that was hit by the raycast.
    public GameObject GetHitObject()
    {
        return hitObject;
    }

    // Handles trigger collisions and registers user selection to the GameManager
    private void OnTriggerStay(Collider other)
    {
        if (isDragging) return;
        GameObject hitObject = other.gameObject;
        gameManager.RegisterUserSelection(hitObject);
    }

    // resets the position of the cube to its initial position.
    private void ResetPos()
    {
        transform.position = startPosition;
    }
}
