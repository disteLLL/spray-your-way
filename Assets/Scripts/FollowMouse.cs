using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour {

    // Update is called once per frame
    void Update () {

        Vector3 pos = Input.mousePosition;
        pos.z = -2f;
        transform.position = Camera.main.ScreenToWorldPoint(pos);
    }
}
