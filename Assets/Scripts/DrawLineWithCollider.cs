using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLineWithCollider : MonoBehaviour
{
    public GameObject cursorPrefab;
    public float maxCursorDistance = 20f;

    private EdgeCollider2D collider;
    private Camera camera;
    private List<Vector3> points;
    private LineRenderer currentLine;
    private Vector3 position;
    private GameObject cursorInstance;

    void Awake() {
        CreateDefaultCamera();
        points = new List<Vector3>();
        cursorInstance = Instantiate(cursorPrefab);
    }

    void Update() {

        UpdateCursor();

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

    private void UpdateCursor() {

        Transform canTransform = camera.transform;
        Ray ray = new Ray(canTransform.position, canTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            position = hit.point;
            cursorInstance.transform.position = hit.point;
            cursorInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
        else {
            currentLine = null;
            points.Clear();
            cursorInstance.transform.position = ray.origin + ray.direction.normalized * maxCursorDistance;
            cursorInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, -ray.direction);
        }
    }

    private void CreateLine() {
        currentLine = new GameObject("Line").AddComponent<LineRenderer>();
        currentLine.transform.parent = GameObject.FindGameObjectWithTag("ImageTarget").transform;
        currentLine.material = new Material(Shader.Find("Sprites/Default"));
        currentLine.positionCount = 0;
        currentLine.startWidth = 0.5f;
        currentLine.endWidth = 0.5f;
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

    private Vector2[] ToVector2Array(Vector3[] input) {
        Vector2[] output = new Vector2[input.Length];
        for(int i = 0; i<input.Length; i++) {
            output[i] = input[i];
        }
        return output;
    }
}