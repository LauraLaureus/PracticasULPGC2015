using UnityEngine;
using System.Collections;

public class GregarianEnemiesDetector : MonoBehaviour {

	private GregarianSpacialMemory mem;
	// Use this for initialization
	void Start () {
		mem = gameObject.GetComponent<GregarianSpacialMemory> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		RaycastHit[] hits = Physics.SphereCastAll (this.transform.position, 7f, Vector3.forward);
		foreach (RaycastHit hit in hits) {
			if(hit.collider.gameObject.tag =="Bunny" || hit.collider.gameObject.tag =="Player" ){
				mem.setCloseEnemy(hit.collider.gameObject);
			}
		}
	}
}
