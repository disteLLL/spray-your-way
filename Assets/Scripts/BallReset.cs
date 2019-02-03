using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReset : MonoBehaviour {

    public Vector3 startPosition;
    public GameObject completeLevelUI;

    private Rigidbody2D rb;
    private TrailDrawer lineReset;

    void Start () {

        startPosition = this.transform.localPosition;
        rb = GetComponent<Rigidbody2D>();
        lineReset = GameObject.FindGameObjectWithTag("GameController").GetComponent<TrailDrawer>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
      
        if(collision.gameObject.tag == "Bounds") {

            ResetBall();
            lineReset.points.Clear();
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {

        if (collision.gameObject.tag == "Finish") {

            ResetBall();
            lineReset.points.Clear();

            completeLevelUI.SetActive(true);
        }
    }

    /// <summary>
    /// Set the ball position to its start position and stop any movement it still had
    /// </summary>
    public void ResetBall() {

        this.transform.localPosition = startPosition;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
    }
}
