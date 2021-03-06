﻿/*
 * Created by Joe Burchill November 14/2014
 * Idle Behaviour for the Furbull, calls
 * Idle Components
 */

#region ChangeLog
/*
 * Added the base.update to the begining of the update so the controller can take over. Dec 1 - Mathieu Elias
 *  
 */
#endregion

using UnityEngine;
using System.Collections;

public class FurbullIdleBehaviour : BaseIdleBehaviour 
{
    new public AnimatorFurbull EnemyAnimator
    {
        get 
        {
            return base.EnemyAnimator as AnimatorFurbull; 
        }
    }


	protected override void start ()
	{
		//Call all the component start functions
        m_MovementComponent.start(this);
        m_EnterCombatComponent.start(this);
		m_TargetingComponent.start (this);
    }

    public override void update()
	{
		base.update ();

        EnemyAnimator.playAnimation(AnimatorFurbull.Animations.Idle);

		//Set the target
		setTarget(Target ());

		//If we have a movement component call movement
        if (m_MovementComponent != null)
        {
			Movement();
        }

		//If we enter combat switch to the chase state
		if (getTarget() != null)
		{
			if (m_EnterCombatComponent.EnterCombat(getTarget().transform) == true)
        	{
        	    m_EnemyAI.SetState(EnemyAI.EnemyState.Chase);
        	}
		}
	}
}
