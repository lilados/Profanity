using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Ability/Heal")]
public class HealAbility : Ability
{
   public int amountHealed;

   public override void Activate(GameObject parent)
   {
      PlayerHealth health = parent.GetComponent<PlayerHealth>();

      health.health += amountHealed;
   }
}
