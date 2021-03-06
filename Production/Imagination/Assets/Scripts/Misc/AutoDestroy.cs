﻿using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour 
{

	public float timer;
    const ScriptPauseLevel PAUSE_LEVEL = ScriptPauseLevel.Cutscene;

	public bool m_IsParticleEffect = false;
	ParticleEmitter m_Emitter;
	public float m_ShutOffEmitterTime;


	public bool m_DestroyWithTrigger = false;


	void Start()
	{
		if (m_IsParticleEffect)
			m_Emitter = GetComponent<ParticleEmitter> ();
	}

	// Update is called once per frame
	void Update ()
	{
        if (PauseScreen.shouldPause(PAUSE_LEVEL)) { return; }

		if (m_DestroyWithTrigger)
			return;

		timer -= Time.deltaTime;

		if(timer <= 0)
		{
			Destroy(this.gameObject);
		}

		if (!m_IsParticleEffect)
			return;

		m_ShutOffEmitterTime -= Time.deltaTime;

		if (m_ShutOffEmitterTime <= 0)
		{
			m_Emitter.emit = false;
			m_IsParticleEffect = false;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (!m_DestroyWithTrigger)
			return;

		if (collider.tag == Constants.PLAYER_STRING)
			Destroy (this.gameObject);
	}
}
