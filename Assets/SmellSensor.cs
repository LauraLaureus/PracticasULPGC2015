using UnityEngine;
using System.Collections;

public class SmellSensor : MonoBehaviour {

	public float smellRadius;
	private GregarianSpacialMemory mem;
	// Use this for initialization
	void Start () {
		mem = gameObject.GetComponent<GregarianSpacialMemory> ();
		smellRadius = 2f;
	}
	

	void FixedUpdate () {
		RaycastHit[] hits = Physics.SphereCastAll (this.transform.position, smellRadius, Vector3.forward);
		foreach (RaycastHit hit in hits) {
			if(hit.collider.gameObject.tag == "Fruit") mem.setFruit(hit.collider.gameObject);
		}
	}
}
