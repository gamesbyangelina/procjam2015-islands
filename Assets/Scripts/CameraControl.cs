using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public float speed = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 cpos = Camera.main.transform.position;
		if(Input.GetKey(KeyCode.UpArrow)){
			cpos.y += speed * Time.deltaTime;
		}
		else if(Input.GetKey(KeyCode.DownArrow)){
			cpos.y -= speed * Time.deltaTime;
		}
		if(Input.GetKey(KeyCode.RightArrow)){
			cpos.x += speed * Time.deltaTime;
		}
		else if(Input.GetKey(KeyCode.LeftArrow)){
			cpos.x -= speed * Time.deltaTime;
		}
		Camera.main.transform.position = cpos;

		if(Input.GetKeyDown(KeyCode.Equals)){
			if(Camera.main.orthographicSize > 1)
				Camera.main.orthographicSize--;
		}
		if(Input.GetKeyDown(KeyCode.Minus)){
			Camera.main.orthographicSize++;
		}

	}
}
