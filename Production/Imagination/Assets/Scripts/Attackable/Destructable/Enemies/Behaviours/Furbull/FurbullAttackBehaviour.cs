﻿/*
 * Created by Joe Burchill November 14/2014
 * Attack Behaviour for the Furbull, calls
 * Attack Component
 */

#region ChangeLog
/*
 * 
 */
#endregion

using UnityEngine;
using System.Collections;

public class FurbullAttackBehaviour : BaseAttackBehaviour 
{
    new public AnimatorFurbull EnemyAnimator
    {
        get
        {
            return base.EnemyAnimator as AnimatorFurbull;
        }
    }

	const float MIN_CHARGE_UP_TIME = 0.2f;
	const float MAX_CHARGE_UP_TIME = 0.6f;
	float m_ChargeUpTimer;

	protected override void start ()
    {
		m_EnemyAI.m_Health = 2.0f;
		//Setting Random Charge up timer
		m_ChargeUpTimer = Random.Range (MIN_CHARGE_UP_TIME, MAX_CHARGE_UP_TIME);

        //Call start functionality for each component
        m_CombatComponent.start(this);
        m_TargetingComponent.start(this);
        m_MovementComponent.start(this);
    }

	public override void update()
	{
        //Set the target
        setTarget(Target());

        //if we dont have a target switch to idle
		if (getTarget() == null)
        {
            m_EnemyAI.SetState(EnemyAI.EnemyState.Idle);
            return;
        }

        //if we are not in attack range chase the target
        if (GetDistanceToTarget() >= Constants.FURBULL_ATTACK_RANGE)
        {
            m_EnemyAI.SetState(EnemyAI.EnemyState.Chase);
        }

        //look at the target
		transform.LookAt (getTarget().transform.position);

        //Call the movement
        Movement();

        //if our charge up timer is less than or equal to 0, call Combat
		if (m_ChargeUpTimer <= 0)
		{
            EnemyAnimator.playAnimation(AnimatorFurbull.Animations.Attack);
       	    Combat();
			m_ChargeUpTimer = Random.Range (MIN_CHARGE_UP_TIME, MAX_CHARGE_UP_TIME);
		}

        //Decrement timer
		m_ChargeUpTimer -= Time.deltaTime;
	}

    private float GetDistanceToTarget()
    {
        //Return Distance between target and current position
		return Vector3.Distance(transform.position, getTarget().transform.position);
    }
}
