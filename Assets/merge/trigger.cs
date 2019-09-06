using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trigger : MonoBehaviour 
{
    public AudioClip snare_;
    public AudioClip hihat_;
    public AudioClip crash_;
    public AudioClip ride_;
    public AudioClip bass_;
    public AudioClip floor_;
    public AudioClip high_;
    public AudioClip mid_;
    private AudioSource source;
    private float volLowRange = .5f;
    private float volHighRange = 1.0f;

    private Color startcolor;

	void OnCollisionEnter(Collision other)
	{
        startcolor =  other.gameObject.GetComponent<Renderer> ().material.color;
        
        if (other.gameObject.CompareTag("snare"))
        {
            other.gameObject.GetComponent<Renderer> ().material.color = Color.yellow;

            source = GetComponent<AudioSource>();
            float vol = Random.Range (volLowRange, volHighRange);
            source.PlayOneShot(snare_,vol);
        }

        if (other.gameObject.CompareTag("high"))
        {
            other.gameObject.GetComponent<Renderer> ().material.color = Color.yellow;

            source = GetComponent<AudioSource>();
            float vol = Random.Range (volLowRange, volHighRange);
            source.PlayOneShot(high_,vol);
        }

        if (other.gameObject.CompareTag("mid"))
        {
            other.gameObject.GetComponent<Renderer> ().material.color = Color.yellow;

            source = GetComponent<AudioSource>();
            float vol = Random.Range (volLowRange, volHighRange);
            source.PlayOneShot(mid_,vol);
        }

        if (other.gameObject.CompareTag("crash"))
        {
            other.gameObject.GetComponent<Renderer> ().material.color = Color.yellow;

            source = GetComponent<AudioSource>();
            float vol = Random.Range (volLowRange, volHighRange);
            source.PlayOneShot(crash_,vol);
        }

        if (other.gameObject.CompareTag("floor"))
        {
            other.gameObject.GetComponent<Renderer> ().material.color = Color.yellow;

            source = GetComponent<AudioSource>();
            float vol = Random.Range (volLowRange, volHighRange);
            source.PlayOneShot(floor_,vol);
        }

        if (other.gameObject.CompareTag("hihat"))
        {
            other.gameObject.GetComponent<Renderer> ().material.color = Color.yellow;

            source = GetComponent<AudioSource>();
            float vol = Random.Range (volLowRange, volHighRange);
            source.PlayOneShot(hihat_,vol);
        }

        if (other.gameObject.CompareTag("ride"))
        {
            other.gameObject.GetComponent<Renderer> ().material.color = Color.yellow;

            source = GetComponent<AudioSource>();
            float vol = Random.Range (volLowRange, volHighRange);
            source.PlayOneShot(ride_,vol);
        }

        if (other.gameObject.CompareTag("bass"))
        {
            other.gameObject.GetComponent<Renderer> ().material.color = Color.yellow;

            source = GetComponent<AudioSource>();
            float vol = Random.Range (volLowRange, volHighRange);
            source.PlayOneShot(bass_,vol);
        }

        if (other.gameObject.CompareTag("pedal"))
        {
            other.gameObject.GetComponent<Renderer>().material.color = Color.yellow;

            source = GetComponent<AudioSource>();
            float vol = Random.Range(volLowRange, volHighRange);
            source.PlayOneShot(bass_, vol);
        }
    }

    void OnCollisionExit(Collision other)
    {
        other.gameObject.GetComponent<Renderer> ().material.color = startcolor;
    }
}