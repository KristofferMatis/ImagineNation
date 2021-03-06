﻿using UnityEngine;
using System.Collections;

/*Created by Kole
 * 
 * Light peg will act simularly to coins in mario
 * When a peg is Collected, it will increment the total  
 *  
 * This Class inherits from BaseCollecatable
 */

public class LightPeg : BaseCollectable
{
	public SceneLights[] m_Lights;
	public float AmountToAdd = 2.0f;

	//All the puzzle pieces should have a trigger on them, when enter, this function will be called
    void OnTriggerEnter(Collider other)
    {
		//Check if we are still spawning
		if(this.GetComponent<EnemyLightPegSpawn>() != null)
		return;		

		//checks to see if the object in our trigger is a player.
        if (other.gameObject.tag == Constants.PLAYER_STRING)
        {
			//Increase the intesnity of the players point light
			PlayerLightController playerLight = other.gameObject.GetComponent<PlayerLightController>();
			if (playerLight != null)
			{
				playerLight.AddToIntesnity();
			}

            //Tell GameData this peg was collected
            GameData.Instance.LightPegCollected(m_ID);

            //increment collectable counter
            m_CollectableManager.IncrementCounter();

			//Play Collected sound 
			PlaySound();

			for (int i = 0; i < m_Lights.Length; i++)
			{
				//Safety check so when light pegs that don't have lights but have array values 
				//won't throw an error because some people might not have a clue how to set the prefab.
				if(m_Lights[i] != null)
				{
					m_Lights[i].AddToLightIntensity(AmountToAdd);
				}
			}

            //destroy this gameobject
            Destroy(this.gameObject);            
        }
    }
}