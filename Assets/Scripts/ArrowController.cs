using UnityEngine;
using System.Collections;
using System;

public class ArrowController : MonoBehaviour {
    public GameObject arrowPrefab;
    public float maxLoad = 70.0f;
    public float growFactor = 1.0f;

    private float loadBow;

    public delegate void LoadingArrow(float loadBow, float maxLoad);
    public static event LoadingArrow OnBarUpdate;

    // Use this for initialization
    void Start () {
        loadBow = 0.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        checkForClicks();
        updateLoadBar();
    }

    private void checkForClicks()
    {

        if (Input.GetMouseButton(0) && loadBow <= maxLoad)
        {
            loadBow += maxLoad /growFactor;
            Debug.Log("boton apretado " + loadBow.ToString());
        }

        else if (!Input.GetMouseButton(0) && loadBow > 0.0f)
        {
            Debug.Log("boton disparo "+loadBow.ToString());
            shotArrow();
            loadBow = 0.0f;
        }

    }

    private void updateLoadBar()
    {
        if (OnBarUpdate != null)
            OnBarUpdate(loadBow, maxLoad);
    }

    private void shotArrow()
    {
        GameObject arrow = (GameObject)Instantiate(arrowPrefab, transform.position + transform.forward, transform.rotation);
        Rigidbody rbArrow = arrow.GetComponent<Rigidbody>();
        rbArrow.velocity = transform.forward * loadBow;
    }
}
