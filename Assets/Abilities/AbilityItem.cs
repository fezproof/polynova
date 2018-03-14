using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AbilityItem : MonoBehaviour
{
	public Image icon;
	public Slider cooldownSlider;
	private float cooldown;
	public bool onCooldown = false;

	public void SetSlot (Ability ability)
	{
		icon.sprite = ability.icon;
		icon.color = Color.white;
	}

	public void ClearSlot ()
	{
		icon.sprite = null;
		icon.color = new Color (0, 0, 0, 0);
	}

	public IEnumerator Cooldown (float currentCooldown, float baseCooldown)
	{
		onCooldown = true;
		cooldown = currentCooldown;
		while (cooldown > 0f) {
			cooldownSlider.value = cooldown / baseCooldown;
			if (cooldown - Time.fixedDeltaTime > 0) {
				cooldown -= Time.fixedDeltaTime;
				yield return new WaitForFixedUpdate ();
			} else {
				cooldown = 0f;
			}
		}
		cooldownSlider.value = 0f;
		onCooldown = false;
	}
}
