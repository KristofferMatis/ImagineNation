﻿using UnityEngine;
using System.Collections;

public class AnimatorPlayers : AnimatorController
{
    public enum Animations
    {
        Idle,
        Walk,
        Run,
        Jump,
        Falling,
        Landing,
        Ability,
        Combo_X,
        Combo_XX,
        Combo_XXX,
        Combo_Y,
        Combo_XY,
        Combo_XXY,
        Combo_XXXY,
        Death
    }

    protected override void Start()
    {
        base.Start();

        m_States = new string[]
        {
            "Idle",
            "Walk",
            "Run",
            "Jump",
            "Falling",
            "Landing",
            "Ability",
            "Combo_X",
            "Combo_XX",
            "Combo_XXX",
            "Combo_Y",
            "Combo_XY",
            "Combo_XXY",
            "Combo_XXXY",
            "Death"
        };
    }

    const string COMBO_ = "Combo_";
    const string ATTACK = "Attack";

    int m_LastAnimationPlayed = 0;

    public virtual void playAnimation(Animations animation)
    {
        playAnimation((int)animation);
    }

    public override void playAnimation(int animationNumber)
    {
        requestAnimation(animationNumber);
    }

    public override void playAnimation(string animationName)
    {
        requestAnimation(animationName);
    }

    void requestAnimation(string animation)
    {
        for (int i = 0; i < m_States.Length; i++)
        {
            if(m_States[i].CompareTo(animation) == 0)
            {
                requestAnimation(i);
                return;
            }
        }
    }

    void requestAnimation(int animation)
    {
        if (!m_States[animation].Contains(COMBO_))
        {
            if (!i_Animator.GetCurrentAnimatorStateInfo(0).IsTag(ATTACK))
            {
                i_Animator.Play(m_States[animation]);
                m_LastAnimationPlayed = animation;
            }
        }
        else
        {
            i_Animator.Play(m_States[animation], 0 , 0);
            m_LastAnimationPlayed = animation;
        }        
    }
}
