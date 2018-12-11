using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetLevel : MonoBehaviour {

	void Update () {

        float x = this.transform.position.x / 2.204f;
        float y = (this.transform.position.y / 2.204f) - 20.529f;

        if (x < -13 || x > 13) {
            SceneManager.LoadScene(0);
        }

        if (y < -20) {
            SceneManager.LoadScene(0);
        }
	}

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Finish") {
            SceneManager.LoadScene(0);
        }
    }
}
