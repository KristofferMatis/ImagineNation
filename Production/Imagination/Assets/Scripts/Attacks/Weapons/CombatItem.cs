﻿using UnityEngine;
using System.Collections;
/// <summary>
/// Combat item.
/// Created by Zach Dubuc
/// 
/// Handles the combos for players, and calls the attack function from the attacks
/// 
/// </summary>
public class CombatItem : MonoBehaviour 
{

	BaseAttack[] m_BaseAttacks = new BaseAttack[3]; //Array of the attacks. Combo's are 3 attacks.

	int m_CurrentAttack = 0; //The current Attack


	// Use this for initialization
	void Start () 
	{
		m_BaseAttacks [0] = new BaseAttack (); //Two base attacks, and one special wich is an AOE around the character
		m_BaseAttacks [1] = new BaseAttack ();
		m_BaseAttacks [2] = new SpecialAttack ();

		for(int i = 0; i < m_BaseAttacks.Length; i++)
		{
			m_BaseAttacks[i].loadPrefab();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.A))
		{

		if(m_BaseAttacks[m_CurrentAttack].getAttackTimer() <= 0.0f) //Check if the character is attacking
		{
			if(m_BaseAttacks[m_CurrentAttack].getGraceTimer() <= 0.0f) //Check if the grace timer is over
			{
				m_CurrentAttack = 0; //If so, the combo get's reset
			}

			else
			{
				if(m_CurrentAttack >= m_BaseAttacks.Length) //If the currentAttack is the last one in the array, reset it
				{
					m_CurrentAttack = 0;
						m_BaseAttacks[m_CurrentAttack].startAttack(transform.position, transform.rotation); //Call the start attack function
				}
			
				else
				{
					m_CurrentAttack++;	//Increment the current attack	
						m_BaseAttacks[m_CurrentAttack].startAttack(transform.position, transform.rotation); //Call attack function
				}
			}
		}
		}

	}
}
