using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour {

	Vector3 startPosition;
	void Start () {
		startPosition = transform.position;
	}

	void OnCollisionEnter2D (Collision2D collision) {
		if (collision.gameObject.layer == 10) {
			Kill();
		}
	}

	void Kill () {
		transform.position = startPosition;
	}
}
