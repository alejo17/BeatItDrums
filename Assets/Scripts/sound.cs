using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sound : MonoBehaviour 
{
    public AudioClip shootSound;
    private AudioSource source;
    private float volLowRange = .5f;
    private float volHighRange = 1.0f;
	
	void OnMouseEnter()
	{
	    source = GetComponent<AudioSource>();
	    float vol = Random.Range (volLowRange, volHighRange);
        source.PlayOneShot(shootSound,vol);
    }
}