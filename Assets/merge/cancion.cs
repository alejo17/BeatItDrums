using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class cancion : MonoBehaviour {

	int[] primera = {44,44,44,44,44,44,44,44,49,42,42,42,42,42,42,42,42,42,42,42,42,42,42,42};
//	int[] segunda = { 0, 0, 0, 0, 0, 0, 0, 0,35, 0,40, 0,35, 0,40, 0,35, 0,40, 0,35, 0,40, 0};

 	// Use this for initialization
	void Start () {

		
	}
	
	// Update is called once per frame
	void Update () {
		for ( int i = 0; i < primera.Length; i++ ) {
			//for ( int j = 0; j < 24; j++ ) {
			Debug.Log("Hello1");
        		if (primera[i] == 44 && gameObject.CompareTag("hithat")){
        			Debug.Log("Hello2");
        			gameObject.GetComponent<Renderer>().material.color = Color.red;
        			//Thread.Sleep(250);
        			//GetComponent<Renderer>().material.color = Color.white;
        		}
        	//}		
        }

        gameObjectGetComponent<Renderer>().material.color = Color.white;

		



		// if (other.gameObject.CompareTag("pedal"))
  //       {
            //other.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
		
	}
}
