﻿///
/// Activatable light.
/// Created By: Matthew Whitlaw
/// 
/// This class inherits from activatable and simply toggles a light
/// on and off based on a switch or switches becoming active
/// 
/// 

using UnityEngine;
using System.Collections;


public class ActivatableLight : Activatable 
{
	bool m_IsActive;
	Light m_Light;
	float m_OriginalIntensity;

	//Get the necessary light component and record its
	//initial intensity
	void Start () 
	{
		m_IsActive = false;
		m_Light = GetComponent<Light> ();
		m_OriginalIntensity = m_Light.intensity;
	}
	
	void Update () 
	{
		//Check to see if the switch is already active or not
		if(!m_IsActive)
		{
			//Call the base checkswitches function
			//and if it returns true turn on the light
			if(CheckSwitches())
			{
				TurnOnLight();
				m_IsActive = true;
			}
		}
		else
		{
			//If the switch is active then check to see
			//if any of the switches have deactivated if so
			//turn off the light
			for(int i = 0; i < m_Switches.Length; i++)
			{
				if(!m_Switches[i].getActive())
				{
					ResetLight();
					m_IsActive = false;
				}
			}
		}

	}

	//Simply set the light's intensity to zero
	void TurnOnLight()
	{
		m_Light.intensity = 1.0f;
	}

	//Reset the light's intensity back to it's original
	void ResetLight()
	{
		m_Light.intensity = m_OriginalIntensity;
	}
}
