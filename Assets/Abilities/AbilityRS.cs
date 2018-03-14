using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AbilityRS : RuntimeSet<Ability>
{
	public void refreshAbilities ()
	{
		foreach (Ability ability in Items) {
			ability.Refresh ();
		}
	}

	public Ability[] GetAbilities ()
	{
		return Items.ToArray ();
	}
}
