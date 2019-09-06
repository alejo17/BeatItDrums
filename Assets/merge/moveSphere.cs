using UnityEngine;
using System.Collections;
 
public class moveSphere : MonoBehaviour {

	float speed = 7.0f;
	float spacing = 1.0f;
	Vector3 pos;
	 
	void Start() {
    	pos = transform.position;
 	}
	 
	void Update() {
	    if (Input.GetKeyDown(KeyCode.T))
	        pos.y += spacing;
	    if (Input.GetKeyDown(KeyCode.G))
	        pos.y -= spacing;
	    if (Input.GetKeyDown(KeyCode.F))
	        pos.x -= spacing;
	    if (Input.GetKeyDown(KeyCode.H))
	        pos.x += spacing;
	    if (Input.GetKeyDown(KeyCode.B))
	        pos.z -= spacing;
	    if (Input.GetKeyDown(KeyCode.V))
	        pos.z += spacing;    
	 
	    transform.position = Vector3.MoveTowards(transform.position, pos, speed * Time.deltaTime);
	}
}