using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapOnSceneView : MonoBehaviour
{
    public PolygonCollider2D polygonCollider2D;
    public float gridSize = 0.5f;
    [SerializeField] private bool isWall = true;
    void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        Vector3 snapPosition = new Vector3(
            Mathf.Round(transform.position.x / gridSize) * gridSize,
            Mathf.Round(transform.position.y / gridSize) * gridSize,
            transform.position.z
        );
        if (isWall) snapPosition.z = -(snapPosition.x + snapPosition.y) * 0.001f;
        else snapPosition.z = -(snapPosition.y - snapPosition.x) * 0.001f;
        snapPosition.z = snapPosition.y * 0.001f;
        transform.position = snapPosition;

        if (polygonCollider2D != null)
        {
            Vector2[] points = polygonCollider2D.points;

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new Vector2(
                    Mathf.Round(points[i].x / gridSize) * gridSize,
                    Mathf.Round(points[i].y / gridSize) * gridSize
                );
            }
            polygonCollider2D.SetPath(0, points);
        }
    }
}
