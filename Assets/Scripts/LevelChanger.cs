﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChanger : MonoBehaviour {

    public GameObject level1;
    public GameObject level2;
    public GameObject level3;
    public GameObject level4;

    /// <summary>
    /// Enabling and disabling the parent GameObjects holding the levels
    /// </summary>
    public void ChangeLevel() {

        if (level1.activeSelf) {

            level1.SetActive(false);
            level2.SetActive(true);
        }
        else if (level2.activeSelf) {

            level2.SetActive(false);
            level3.SetActive(true);
        }
        else if (level3.activeSelf) {

            level3.SetActive(false);
            level4.SetActive(true);
        }
        else if (level4.activeSelf) {

            level4.SetActive(false);
            level1.SetActive(true);
        }
    }

    /// <summary>
    /// Used by the LevelComplete animation to hide itself when finished
    /// </summary>
    private void DisableSelf() {

        this.gameObject.SetActive(false);
    }
}
