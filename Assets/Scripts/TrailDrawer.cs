using System.Collections.Generic;
using UnityEngine;

public class TrailDrawer : MonoBehaviour
{
    public GameObject cursorPrefab;
    public float maxCursorDistance = 20f;
    public List<Vector3> points;

    private EdgeCollider2D mCollider;
    private LineRenderer currentLine;
    private Vector3 position;
    private GameObject cursorInstance;
    
    void Start() {

        points = new List<Vector3>();
        cursorInstance = Instantiate(cursorPrefab);
        CreateLine();

        // Vary the trail lenght by playing with the repeat rates
        InvokeRepeating("UpdateLine", .05f, .01f);
        InvokeRepeating("LineTrailing", .05f, .038f);
    }

    void Update() {

        UpdateCursor();

        if (points.Count > 1) {

            currentLine.positionCount = points.Count;
            currentLine.SetPositions(points.ToArray());
            mCollider.points = ToVector2Array(points.ToArray());
            currentLine.gameObject.SetActive(true);

        }
        else {

            currentLine.gameObject.SetActive(false);

        }
        
        if(points.Count > 80) {

            points.Clear();
        }
    }

    /// <summary>
    /// Set the position and rotation of the cursor based on a raycast from the spraycan forwards
    /// </summary>
    private void UpdateCursor() {

        Transform canTransform = this.transform;
        Ray ray = new Ray(canTransform.position, canTransform.forward);
        RaycastHit hit;

        // only the planes used as background have a 3D collider
        if (Physics.Raycast(ray, out hit)) {

            position = hit.point; // used as input point for the line renderer
            cursorInstance.transform.position = hit.point;
            cursorInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
        else {

            cursorInstance.transform.position = ray.origin + ray.direction.normalized * maxCursorDistance;
            cursorInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, -ray.direction);
        }
    }

    /// <summary>
    /// Initialize the line and collider used as a trail
    /// </summary>
    private void CreateLine() {

        currentLine = new GameObject("Line").AddComponent<LineRenderer>();
        currentLine.material = new Material(Shader.Find("Sprites/Default"));
        currentLine.positionCount = 0;
        currentLine.startWidth = 0.2f;
        currentLine.endWidth = 0.4f;
        currentLine.startColor = Color.red;
        currentLine.endColor = Color.blue;
        currentLine.useWorldSpace = false;

        mCollider = currentLine.gameObject.AddComponent<EdgeCollider2D>();
        mCollider.Reset();
    }

    private void UpdateLine() {

        position.z = 0;

        if (!points.Contains(position)) {

            points.Add(position);

        }
    }

    private void LineTrailing() {

        if (currentLine.positionCount > 10 && points.Count > 10) {

            points.RemoveAt(0);

        }
    }

    private Vector2[] ToVector2Array(Vector3[] input) {
        Vector2[] output = new Vector2[input.Length];
        for (int i = 0; i < input.Length; i++) {
            output[i] = input[i];
        }
        return output;
    }
}