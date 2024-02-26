using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
	public class Fighter : MonoBehaviour , IAction
	{
		[SerializeField] float weaponRange = 2f;
		[SerializeField] float timeBetweenAttacks = 1f;
		[SerializeField] float weaponDamage = 5f;
		Health target;
		float timeSienceLastAttack = 0;
		
		private void Update()
		{
			timeSienceLastAttack += Time.deltaTime;

			if (target == null) return;
			if (target.IsDead()) return;

			if (!GetIsInRange())
			{
				GetComponent<Mover>().MoveTo(target.transform.position, 1f);
			}
			else
			{
				GetComponent<Mover>().Cancel();
				AttackBehavior();
			}
		}

		private void AttackBehavior()
		{
			transform.LookAt(target.transform);
			if (timeSienceLastAttack > timeBetweenAttacks)
			{
				//triger the Hit() event in animation.
				TriggerAttack();
				timeSienceLastAttack = Mathf.Infinity;
			}
		}

		private void TriggerAttack()
		{
			GetComponent<Animator>().ResetTrigger("stopAttack");
			GetComponent<Animator>().SetTrigger("attack");
		}

		//Animation event
		void Hit()
		{
			if (target == null) return;
			target.TakeDamage(weaponDamage);
		}

		private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < weaponRange;
        }

		public bool CanAttack(GameObject combatTarget)
		{
			if (combatTarget == null) { return false; }
			Health targetToTest = combatTarget.GetComponent<Health>();
			return targetToTest != null && !targetToTest.IsDead();
		}

		public void Attack(GameObject combatTarget)
		{
			GetComponent<ActionScheduler>().StartAction(this);
			target = combatTarget.GetComponent<Health>();
		}

		public void Cancel()
		{
			TriggerStopAttack();
			target = null;
			GetComponent<Mover>().Cancel();
		}

		private void TriggerStopAttack()
		{
			GetComponent<Animator>().ResetTrigger("attack");
			GetComponent<Animator>().SetTrigger("stopAttack");
		}
	}
}
