﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatorEnemyBase : AnimatorController 
{
    protected Dictionary<string, List<string>> m_StatesDitctionary = new Dictionary<string, List<string>>();

    protected float m_Timer = 0.0f;

    protected const float CROSS_FADE_LENGTH = 0.1f;
    protected const float CROSS_FADE_TIMER = 0.2f;
    protected float m_CrossfadeTimer = 0.0f;
	protected bool m_IsPlayingSound;

    public override void playAnimation(string animationName)
    {
        if (!m_StatesDitctionary.ContainsKey(animationName))
            return;

        if (m_CrossfadeTimer > 0.0f)
            return;

        if (i_Animator.GetCurrentAnimatorStateInfo(0).IsTag(animationName) && m_Timer > 0.0f)
            return;

        i_Animator.CrossFade(m_StatesDitctionary[animationName][Random.Range(0, m_StatesDitctionary[animationName].Count)], CROSS_FADE_LENGTH);
        m_Timer = i_Animator.GetCurrentAnimatorStateInfo(0).length;
        m_CrossfadeTimer = (m_Timer * CROSS_FADE_LENGTH) + CROSS_FADE_TIMER;
    }

    protected virtual void Update()
    {
        if(m_Timer > 0.0f)
            m_Timer -= Time.deltaTime;

        if (m_CrossfadeTimer > 0.0f)
            m_CrossfadeTimer -= Time.deltaTime;
	
    }
}
