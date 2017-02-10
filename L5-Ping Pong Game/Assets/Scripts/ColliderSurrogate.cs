using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderSurrogate : MonoBehaviour {
	private Action<Collision> callback;

	public void Initialize(Action<Collision> callback){
		this.callback = callback;
	}

	void OnCollisionEnter(Collision c){
		callback(c);
	}
}
