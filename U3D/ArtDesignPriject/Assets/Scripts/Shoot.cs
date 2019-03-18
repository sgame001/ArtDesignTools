using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{

	// Use this for initialization
	
	public float validTouchDistance; //200
	public string layerName = "Ground";         //"Ground"
	// Update is called once per frame

	private Vector3 tempPos;
	private GameObject play;


	private void Start()
	{
//		play = GameObject.Find("Cube");
//		print(play);
	}

	void Update()
	{

		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 100))
			{
				print(hit.collider.name);
				if (hit.collider.gameObject.name == "Plane")
				{
					Debug.Log(hit.point);
					//tempPos = new Vector3(hit.point.x,play.transform.position.y,hit.point.z);
//					if (Vector3.Distance(play.transform.position, tempPos) > 0.1f)
//					{
//						
//					}
					
				}
			}


		}
	}
}
