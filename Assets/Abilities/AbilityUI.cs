using UnityEngine;

public class AbilityUI : MonoBehaviour
{

	public AbilityRS abilitiesInv;

	AbilityItem[] slots;

	void Awake ()
	{
		slots = GetComponentsInChildren<AbilityItem> ();
		UpdateAbilities ();
	}

	public void UpdateAbilities ()
	{
		Ability[] abilities = abilitiesInv.GetAbilities ();
		for (int i = 0; i < slots.Length; i++) {
			if (i < abilities.Length && abilities [i] != null) {
				slots [i].SetSlot (abilities [i]);
			} else {
				slots [i].ClearSlot ();
			}
		}
	}

	public void UpdateCooldowns ()
	{
		Ability[] abilities = abilitiesInv.GetAbilities ();
		for (int i = 0; i < slots.Length; i++) {
			if (i < abilities.Length && abilities [i] != null) {
				if (abilities [i].OnCooldown ()) {
					if (!slots [i].onCooldown) {
						StartCoroutine (slots [i].Cooldown (abilities [i].currentCooldown, abilities [i].baseCoolDown));
					}
				}
			} else {
				slots [i].ClearSlot ();
			}
		}
	}
}
