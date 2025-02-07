
using UnityEngine;
using System.Collections;

namespace Avoidance
{
	/// <summary>
	/// Applys damage to player on interaction.
	/// </summary>
	public class PlayerDeathInteractable : MonoBehaviour, Interactable 
	{
		public void Interact(GameObject interacted)
		{
			if(!GameStateController.instance.isInvincible)
			{
                interacted.GetComponent<PlayerHealth>().ApplyDamage();
            }
		}
	}
}
