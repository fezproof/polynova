using UnityEngine;

public interface IRanged
{
	void Shoot (Vector3 position, Quaternion direction);

	void StopShooting ();

}
