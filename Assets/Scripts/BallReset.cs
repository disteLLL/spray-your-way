using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReset : MonoBehaviour {

    public Vector3 startPosition;
    public GameObject completeLevelUI;

    private Rigidbody2D rb;
    private GameObject lineReset;

    void Start () {

        startPosition = this.transform.localPosition;
        rb = GetComponent<Rigidbody2D>();
        lineReset = GameObject.FindGameObjectWithTag("GameController");

    }

    private void OnCollisionEnter2D(Collision2D collision) {
      
        if(collision.gameObject.tag == "Bounds") {

            this.transform.localPosition = startPosition;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            lineReset.GetComponent<DrawLineWithCollider>().points.Clear();
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {

        if (collision.gameObject.tag == "Finish") {

            this.transform.localPosition = startPosition;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            lineReset.GetComponent<DrawLineWithCollider>().points.Clear();

            completeLevelUI.SetActive(true);

        }
    }
}
