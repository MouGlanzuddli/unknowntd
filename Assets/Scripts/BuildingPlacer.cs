using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BuildingPlacer : MonoBehaviour
{
    [Header("Placement")]
    [SerializeField] private GameObject confirmPopup;
    [SerializeField] private Vector3 popupOffset = new Vector3(0f, 1.2f, 0f);
    [SerializeField] private float popupSpacing = 0.5f;


    private GameObject buildingPrefab;
    private GameObject ghostBuilding;

    private bool isPlacing;
    private bool waitingConfirm;

    private Camera mainCamera;
    private Vector3 pendingBuildPosition;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (confirmPopup != null)
            confirmPopup.SetActive(false);
    }

    private void Update()
    {
        if (!isPlacing || ghostBuilding == null)
            return;

        if (waitingConfirm)
            return;

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
            return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        ghostBuilding.transform.position = mouseWorldPos;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            pendingBuildPosition = mouseWorldPos;
            ShowConfirmPopup();
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelPlacement();
        }
    }

    public void SetBuildingPrefab(GameObject prefab)
    {
        buildingPrefab = prefab;
        PrepareGhost();
    }

    private void PrepareGhost()
    {
        if (ghostBuilding != null)
            Destroy(ghostBuilding);

        ghostBuilding = Instantiate(buildingPrefab);
        ghostBuilding.name = "Ghost";

        isPlacing = true;
        waitingConfirm = false;

        SpriteRenderer sr = ghostBuilding.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = new Color(1f, 1f, 1f, 0.5f);

        Collider2D col = ghostBuilding.GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;
    }

    private Vector3 GetRightOffset(GameObject target)
    {
        SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
        if (sr == null)
            return Vector3.zero;

        Bounds b = sr.bounds;

        Vector3 worldPos = new Vector3(
         b.max.x + popupSpacing,
         b.center.y,
         target.transform.position.z
     );


        // Convert to local position relative to ghost
        return target.transform.InverseTransformPoint(worldPos);
    }

    private void ShowConfirmPopup()
    {
        waitingConfirm = true;

        if (confirmPopup != null)
        {
            confirmPopup.SetActive(true);
            confirmPopup.transform.SetParent(ghostBuilding.transform);

            Vector3 localOffset = GetRightOffset(ghostBuilding);
            confirmPopup.transform.localPosition += new Vector3(0.2f, 0f, 0f);
            confirmPopup.transform.localPosition = localOffset;
        }
    }


    public void ConfirmBuild()
    {
        Instantiate(buildingPrefab, pendingBuildPosition, Quaternion.identity);

        confirmPopup.transform.SetParent(null);
        confirmPopup.SetActive(false);

        Destroy(ghostBuilding);

        ghostBuilding = null;
        buildingPrefab = null;
        isPlacing = false;
        waitingConfirm = false;
    }

    public void CancelBuild()
    {
        waitingConfirm = false;

        if (confirmPopup != null)
        {
            confirmPopup.transform.SetParent(null);
            confirmPopup.SetActive(false);
        }
    }

    public void CancelPlacement()
    {
        isPlacing = false;
        waitingConfirm = false;

        if (ghostBuilding != null)
            Destroy(ghostBuilding);

        if (confirmPopup != null)
        {
            confirmPopup.transform.SetParent(null);
            confirmPopup.SetActive(false);
        }

        ghostBuilding = null;
        buildingPrefab = null;
    }
}
