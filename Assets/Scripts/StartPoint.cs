using UnityEngine;
using System.Collections;

public class StartPoint : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 3, Camera.main.nearClipPlane));
    }
	
}
