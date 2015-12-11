using UnityEngine;
using System.Collections;

public class ArrowController : MonoBehaviour {
    public float damage = 30;
    public float maxTimeAlive = 5;
    private Rigidbody rb;
    private float timeAlive;
    private Collider collider;


	// Use this for initialization
	void Start () {
        rb = this.GetComponent<Rigidbody>();
        collider = this.GetComponent<Collider>();
        timeAlive = 0;
	}
	
	// Update is called once per frame
	void Update () {
        timeAlive += Time.deltaTime;
        if (timeAlive > maxTimeAlive)
            Destroy(this.gameObject);
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Collectable"))
        {
            Physics.IgnoreCollision(collision.collider, collider);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //if (!other.CompareTag("Player"))
        //{
            GetComponent<Collider>().enabled = false;
            Destroy(rb);

            Health healthscript;
            GameObject go = other.gameObject;
            if ((healthscript = go.GetComponent<Health>()) != null)
            {
                healthscript.TakeDamage(damage);
                Destroy(this.gameObject);
            }
        //}

    }
}
