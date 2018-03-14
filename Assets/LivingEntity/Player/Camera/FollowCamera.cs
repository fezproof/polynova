using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FollowCamera : NetworkBehaviour
{

	public float moveSpeed = 10f;
	public Vector3 offset;
	Camera mainCamera;
	Ray ray;
	Plane groundPlane;
	float rayDistance;
	Vector3 point;

	public void Intialise (Camera cam)
	{
		mainCamera = cam;
	}

	void FixedUpdate ()
	{
		if (mainCamera == null)
			return;
		ray = mainCamera.ScreenPointToRay (Input.mousePosition);
		groundPlane = new Plane (Vector3.up, transform.position);
		point = Vector3.zero;

		if (groundPlane.Raycast (ray, out rayDistance)) { //check mouse are over the groundplane
			point = ray.GetPoint (rayDistance);
		}
		Vector3 desiredPosition = transform.position * 0.9f + point * 0.1f - offset;
		Vector3 smoothPostion = Vector3.Lerp (mainCamera.transform.position, desiredPosition, moveSpeed * Time.fixedDeltaTime);
		mainCamera.transform.position = smoothPostion;

	}
}
