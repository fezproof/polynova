using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//[RequireComponent (typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	NavMeshAgent agent;
	public Vector3 velocity;
	//	Rigidbody myRigidbody;

	void Start ()
	{
		agent = GetComponent<NavMeshAgent> ();
//		myRigidbody = GetComponent<Rigidbody> ();
	}

	public void Move (Vector3 _velocity)
	{
		velocity = _velocity;
	}

	public void LookAt (Vector3 lookPoint)
	{
		Vector3 heightCorrectedPoint = new Vector3 (lookPoint.x, transform.position.y, lookPoint.z);
		transform.LookAt (heightCorrectedPoint);
	}

	protected void FixedUpdate ()
	{
//		myRigidbody.AddForce (velocity, ForceMode.Impulse);
//		myRigidbody.drag = 10f;
		agent.Move (velocity);
	}
}
