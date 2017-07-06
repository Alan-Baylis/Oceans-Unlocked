using System.Collections;
using UnityEngine;

public class damageSound : MonoBehaviour {

	public AudioSource damageAudio;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {

	}

	void OnCollisionEnter(Collision collision) {

		print("Collision");
		if (collision.relativeVelocity.magnitude > 2) {
		}

	}


}
