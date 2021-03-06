﻿/*
 * Created by Joe Burchill & Mathieu Elias November 14/2014
 * The Base Attack Behaviour which every other attack behaviour will
 * inherit from. Contains Component variables and abstract update
 */

#region ChangeLog
/*
 * 
 */
#endregion

using UnityEngine;
using System.Collections;

public abstract class BaseAttackBehaviour : BaseBehaviour 
{
	// The components all attack behaviours must have
    public BaseCombat m_CombatComponent;
	public BaseTargeting m_TargetingComponent;
	public BaseMovement m_MovementComponent;

    public abstract void update();

	// The following functions must be called to update any of the components in order to make sure the 
	// enemy controller hasn't taken control of any of them
	protected virtual void Combat()
	{
		if (m_EnemyAI.m_UCombat)
		{
			if(m_CombatComponent != null)
				m_CombatComponent.Combat(getTarget());
		}
	}
	protected virtual GameObject Target()
	{
		if (m_EnemyAI.m_UTargeting)
		{
			if(m_TargetingComponent != null)
				return m_TargetingComponent.CurrentTarget ();
		}

		return null;
	}
	protected virtual void Movement()
	{
		if (m_EnemyAI.m_UMovement)
		{
			if(m_MovementComponent != null)
				m_MovementComponent.Movement (getTarget());
		}
	}
	
	public override void ComponentInfo (out string[] names, out BaseComponent[] components)
	{
		names = new string[3];
		components = new BaseComponent[3];

		names [0] = Constants.COMBAT_STRING;
		components [0] = m_CombatComponent;

		names [1] = Constants.TARGETING_STRING;
		components [1] = m_TargetingComponent;

		names [2] = Constants.MOVEMENT_STRING;
		components [2] = m_MovementComponent;
	}

	public override string BehaviourType()
	{
		return Constants.COMBAT_BEHAVIOUR_STRING;
	}

	public override int numbComponents ()
	{
		return 3;
	}

	public override void SetComponents (string[] components)
	{
		m_ComponentsObject = transform.FindChild (Constants.COMPONENTS_STRING).gameObject;

		m_CombatComponent = m_ComponentsObject.GetComponent (components [0]) as BaseCombat;
		m_TargetingComponent = m_ComponentsObject.GetComponent (components [1]) as BaseTargeting;
		m_MovementComponent = m_ComponentsObject.GetComponent (components [2]) as BaseMovement;

	}

}
