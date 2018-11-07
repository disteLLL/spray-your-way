using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLineWithCollider : MonoBehaviour
{

    private EdgeCollider2D collider;
    private Camera camera;
    private List<Vector3> points;
    private LineRenderer currentLine;
    private Vector3 position;
    private Renderer renderer;

    void Awake() {
        if (camera == null) {
            CreateDefaultCamera();
        }
        points = new List<Vector3>();
        renderer = GetComponent<Renderer>();
    }

    void Update() {
        position = renderer.bounds.center;
        Debug.DrawRay(position, Vector3.forward*2, Color.red);

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
        currentLine.material = new Material(Shader.Find("Sprites/Default"));
        currentLine.positionCount = 0;
        currentLine.startWidth = 0.15f;
        currentLine.endWidth = 0.15f;
        currentLine.startColor = Color.red;
        currentLine.endColor = Color.red;
        currentLine.useWorldSpace = true;

        collider = currentLine.gameObject.AddComponent<EdgeCollider2D>();
        collider.Reset();
    }

    private void UpdateLine() {
        
        position.z = 0;

        if (!points.Contains(position)) {
            points.Add(position);
            currentLine.positionCount = points.Count;
            currentLine.SetPosition(currentLine.positionCount - 1, position);
            if (collider != null && points.Count > 1) {
                collider.points = ToVector2Array(points.ToArray());
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
    }

    private Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z) {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

    private Vector2[] ToVector2Array(Vector3[] input) {
        Vector2[] output = new Vector2[input.Length];
        for(int i = 0; i<input.Length; i++) {
            output[i] = input[i];
        }
        return output;
    }
}
