using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Enemies/State")]
public class State : ScriptableObject
{

	public Actions lastAction;
	public Actions firstAction;
	public Actions[] actions;
	public Transition[] transitions;
	public Aggrovation aggro;
	public Color sceneGizmoColor = Color.grey;

	private bool removeThisLater;

	public void CheckAggro (EnemyController controller)
	{
		aggro.Aggro (controller);
	}

	public IEnumerator DoFirstAction(EnemyController  controller)
	{
		if (firstAction != null)
		{
			yield return firstAction.Act (controller);
		}
	}

	public IEnumerator DoLastAction(EnemyController controller)
	{
		if (lastAction != null)
		{
			yield return lastAction.Act (controller);
		}
	}

	public IEnumerator DoActions (EnemyController controller)
	{
		for (int i = 0; i < actions.Length; i++) {
			yield return actions [i].Act (controller);
		}
		yield return null;
	}

	public IEnumerator CheckTransitions (EnemyController controller)
	{
		for (int i = 0; i < transitions.Length; i++) {
			bool decisionSucceeded = transitions [i].decision.Decide (controller);

			if (decisionSucceeded) {
				yield return controller.TransitionToState (transitions [i].trueState);
			} else {
				yield return controller.TransitionToState (transitions [i].falseState);
			}
		}
		yield return null;
	} 


}


