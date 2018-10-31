using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLineWithCollider : MonoBehaviour
{

    private EdgeCollider2D collider;
    private Camera camera;
    private List<Vector2> points;
    private LineRenderer currentLine;

    void Awake() {
        if (camera == null) {
            CreateDefaultCamera();
        }
        points = new List<Vector2>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            CreateLine();
        }
        if (Input.GetMouseButton(0)) {
            UpdateLine();
        }
        if (Input.GetMouseButtonUp(0)) {
            ResetLine();
        }
    }

    private void CreateLine() {
        currentLine = new GameObject("Line").AddComponent<LineRenderer>();
        currentLine.material = new Material(Shader.Find("Particles/Additive"));
        currentLine.positionCount = 0;
        currentLine.startWidth = 0.1f;
        currentLine.endWidth = 0.1f;
        currentLine.startColor = Color.white;
        currentLine.endColor = Color.white;
        currentLine.useWorldSpace = true;

        collider = currentLine.gameObject.AddComponent<EdgeCollider2D>();
        collider.Reset();
    }

    private void UpdateLine() {
        Vector2 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
        if (!points.Contains(mousePosition)) {
            points.Add(mousePosition);
            currentLine.positionCount = points.Count;
            currentLine.SetPosition(currentLine.positionCount - 1, mousePosition);
            if (collider != null && points.Count > 1) {
                collider.points = points.ToArray();
            }
        }
    }

    private void ResetLine() {
        if (currentLine.positionCount < 2) {
            Destroy(currentLine.gameObject);
        }
        currentLine = null;
        points.Clear();
    }

    private void CreateDefaultCamera() {
        camera = Camera.main;
        if (camera == null) {
            camera = gameObject.AddComponent<Camera>();
        }
        camera.orthographic = true;
    }
}
