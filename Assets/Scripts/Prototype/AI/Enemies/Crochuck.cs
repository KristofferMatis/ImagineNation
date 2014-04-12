﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Last updated 04/06/2014

[RequireComponent(typeof(EnemyPathfinding))]
public class Crochuck : BaseEnemy 
{
    enum CrochuckCombatStates
    {
        Default,
        Spin,
        Chuck,
        Count
    }

    CrochuckCombatStates m_CrochuckState = CrochuckCombatStates.Default;

	//Literally a furbull 
	public GameObject m_FurbullPrefab;

    public GameObject m_SpawnPoint;

    List<GameObject> m_Furbulls = new List<GameObject>();

    float m_CrochuckTimer = 0.0f;
    const float SAWN_DELAY = 1.0f;
    const float SPIN_SPAWN_DELAY = 0.2f;

    float m_SpinTimer = 0.0f;
    const float SPIN_TIME = 2.0f;


    NavMeshAgent m_Agent;


	// Use this for initialization
	protected override void start () 
	{
        m_Health.resetHealth();

        m_CombatRange = 10.0f;

        m_Agent = this.gameObject.GetComponent<NavMeshAgent>();
	}

	protected override void die()
	{
		//TODO:play death animation and instantiate ragdoll
		Destroy (this.gameObject);
	}

	protected override void fightState()
	{
        if (m_CrochuckState == CrochuckCombatStates.Default)
        {
            combatDefault();
        }

        switch (m_CrochuckState)
        {
            case CrochuckCombatStates.Chuck:
            {
                chuck();
                break;
            }

            case CrochuckCombatStates.Spin:
            {
                spin();
                break;
            }
        }
	}

    void combatDefault()
    {
        m_Agent.enabled = false;
        int state = Random.Range(0, 10);
        if (state > 7)
        {
            m_CrochuckState = CrochuckCombatStates.Spin;
        }
        else
        {
            m_CrochuckState = CrochuckCombatStates.Chuck;
        }
    }

    void chuck()
    {
        m_CrochuckTimer += Time.deltaTime;
        if (m_CrochuckTimer >= SAWN_DELAY)
        {
			fire ();
				
			m_CrochuckTimer = 0.0f;
            m_CrochuckState = CrochuckCombatStates.Default;

            m_Agent.enabled = true;
            m_State = States.Default;
        }
    }

    void spin()
    {
        transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y + 5, transform.rotation.z));

        m_CrochuckTimer += Time.deltaTime;
        if (m_CrochuckTimer >= SPIN_SPAWN_DELAY)
        {
			fire ();
            m_CrochuckTimer = 0.0f;
        }

        m_SpinTimer += Time.deltaTime;
        if (m_SpinTimer >= SPIN_TIME)
        {
            m_SpinTimer = 0.0f;
            m_CrochuckTimer = 0.0f;

            m_CrochuckState = CrochuckCombatStates.Default;

            m_Agent.enabled = true;
            m_State = States.Default;
        }
    }

	void fire ()
	{
		((FurbullProjectile)((GameObject)Instantiate(Resources.Load("FurbulProjectile"), m_SpawnPoint.transform.forward, m_SpawnPoint.transform.rotation)).GetComponent<FurbullProjectile>()).onUse(this);
	}

    public void instantiateFurbull(Vector3 spawnPosition)
    {
		m_Furbulls.Add((GameObject)Instantiate(m_FurbullPrefab, spawnPosition, m_SpawnPoint.transform.rotation));
    }
}
