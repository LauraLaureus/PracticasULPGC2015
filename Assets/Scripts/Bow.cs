using UnityEngine;
using System.Collections;
using System;

public class Bow : MonoBehaviour {
    public GameObject arrowPrefab;
    public float maxLoad = 70.0f;
    public float growFactor = 1.0f;
    public BowBar bowBar;

    private float loadBow;

    // Use this for initialization
    void Start () {
        loadBow = 0.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        checkForClicks();
        updateHealthBar();
    }

    private void checkForClicks()
    {

        if (Input.GetMouseButton(0) && loadBow <= maxLoad)
        {
            loadBow += maxLoad /growFactor;
        }

        else if (!Input.GetMouseButton(0) && loadBow > 0.0f)
        {
            shotArrow();
            loadBow = 0.0f;
        }

    }

    private void updateHealthBar()
    {
        if (bowBar != null)
        {
            bowBar.UpdateLoadBar(loadBow, maxLoad);
        }
    }

    private void shotArrow()
    {
        GameObject arrow = (GameObject)Instantiate(arrowPrefab, transform.position + transform.forward, transform.rotation);
        Rigidbody rbArrow = arrow.GetComponent<Rigidbody>();
        rbArrow.velocity = transform.forward * loadBow;
    }
}
