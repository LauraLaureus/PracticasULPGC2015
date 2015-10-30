using UnityEngine;
using System.Collections;

public class ArrowController : MonoBehaviour {
    public float damage = 30;
    public float maxTimeAlive = 5;
    private Rigidbody rb;
    private float timeAlive;



	// Use this for initialization
	void Start () {
        rb = this.GetComponent<Rigidbody>();
        timeAlive = 0;
	}
	
	// Update is called once per frame
	void Update () {
        timeAlive += Time.deltaTime;
        if (timeAlive > maxTimeAlive)
            Destroy(this.gameObject);
	}

    void OnTriggerEnter(Collider other)
    {
        GetComponent<Collider>().enabled = false;
        Destroy(rb);

        Health healthscript;
        GameObject go = other.gameObject;
        if ( (healthscript = go.GetComponent<Health>()) != null)
        {
            healthscript.TakeDamage(damage);
        }

    }
}
