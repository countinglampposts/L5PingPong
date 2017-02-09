using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderSurrogate : MonoBehaviour {
	private Action<GameObject> callback;

	public void Initialize(Action<GameObject> callback){
		this.callback = callback;
	}

	void OnCollisionEnter(Collision c){
		callback(c.gameObject);
	}
}
