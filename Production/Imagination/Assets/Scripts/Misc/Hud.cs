﻿using UnityEngine;
using System.Collections;

/* Created by: kole
 * 
 * this class is will display all the hud elements to the player
 * when they are needed to be shown, such as how many collectables
 * has the player found and player health
 * 
 */


public class Hud : MonoBehaviour 
{

	//bool's to determine to show things
    bool m_ShowCheckPoint;
    bool m_ShowHiddenHud;
	bool m_ShowLightPegs;
	bool m_ShowLifes;
	bool m_ShowPuzzlePieces;

   	//Time for showing and hiding hud
    float m_HudDisplayLength;
    float m_HudDisplayTimer;
	float m_LightPegTimer;
	float m_PuzzlePieceTimer;
	float m_LifeTimer;
    float m_CheckPointDisplayTimer;

    //Varibles to display
    int LightPegCollected;
    int PuzzlePiecesCollected;
	float PlayerOneHealth;
	float PlayerTwoHealth;
	int NumberOfLives;

	//Images for our hud
    Texture m_LightPegHudImage;
    Texture[] m_PuzzlePieceHudImages;
    Texture m_LifeCounterImage;
    Texture m_CheckpointImage;
	Texture m_LeftHealthBoarder;
	Texture m_RightHealthBoarder;
	Texture[] m_DerekHealthImages;
	Texture[] m_AlexHealthImages;
	Texture[] m_ZoeyHealthImages;

    //arrays for our players health images
	Texture[] m_PlayerOneHealthImages;
	Texture[] m_PlayerTwoHealthImages;

	HudNumbers m_NumbersDrawer;
		
	Texture m_Divider;
	Vector2 m_DividerPos = new Vector2(0.4975f, 0.0f);
	Vector2 m_DividerScale = new Vector2 (0.005f, 1.0f);

	void OnLoad()
	{
		LoadHudImage();
	}

    const ScriptPauseLevel PAUSE_LEVEL = ScriptPauseLevel.Cutscene;

    //Use this for initialization
	void Start ()
    {
        //Setting all varibles to desired starting stat
        m_HudDisplayLength = Constants.HUD_ON_SCREEN_TIME;
        m_HudDisplayTimer = 0;
        m_CheckPointDisplayTimer = 0;
		m_LightPegTimer = 0;
		m_PuzzlePieceTimer = 0;
		m_LifeTimer = 0;
        m_ShowCheckPoint = false;
        m_ShowHiddenHud = false;
		m_ShowLightPegs = false;
		m_ShowLifes = false;
		m_ShowPuzzlePieces = false;

		LoadHudImage ();

		switch(GameData.Instance.PlayerOneCharacter)
		{
			case Characters.Alex:
			m_PlayerOneHealthImages = m_AlexHealthImages;
			break; 

			case Characters.Derek:
			m_PlayerOneHealthImages = m_DerekHealthImages;
			break;

			case Characters.Zoe:
			m_PlayerOneHealthImages = m_ZoeyHealthImages;
			break;
		}

		switch(GameData.Instance.PlayerTwoCharacter)
		{
			case Characters.Alex:
			m_PlayerTwoHealthImages = m_AlexHealthImages;
			break;

			case Characters.Derek:
			m_PlayerTwoHealthImages = m_DerekHealthImages;
			break;

			case Characters.Zoe:
			m_PlayerTwoHealthImages = m_ZoeyHealthImages;
			break;
		}

		ShowHiddenHud();

		m_NumbersDrawer = gameObject.AddComponent<HudNumbers> ();
	}

	void LoadHudImage()
	{
		m_LightPegHudImage = (Texture)Resources.Load(Constants.HudImages.LIGHT_PEG_HUD_IMAGE);
		if(m_LightPegHudImage == null)
		{
			Debug.Log("Failure");
		}
		m_LifeCounterImage = (Texture)Resources.Load(Constants.HudImages.LIFE_COUNT_IMAGE);
		m_CheckpointImage = (Texture)Resources.Load(Constants.HudImages.CHECKPOINT_IMAGE);
		m_LeftHealthBoarder = (Texture)Resources.Load(Constants.HudImages.LEFT_HEALTH_BOARDER);
		m_RightHealthBoarder = (Texture)Resources.Load(Constants.HudImages.RIGHT_HEALTH_BOARDER);

		m_AlexHealthImages = new Texture[4];
		m_ZoeyHealthImages = new Texture[4];
		m_DerekHealthImages = new Texture[4];
		m_PuzzlePieceHudImages = new Texture[7];

		m_AlexHealthImages[3] = (Texture)Resources.Load(Constants.HudImages.ALEX_FULL_HEALTH_IMAGE);
		m_AlexHealthImages[2] = (Texture)Resources.Load(Constants.HudImages.ALEX_INJURED_HEALTH_IMAGE);
		m_AlexHealthImages[1] = (Texture)Resources.Load(Constants.HudImages.ALEX_CRITICAL_HEALTH_IMAGE);
		m_AlexHealthImages[0] = (Texture)Resources.Load(Constants.HudImages.ALEX_DEAD_HEALTH_IMAGE);

		m_DerekHealthImages[3] = (Texture)Resources.Load(Constants.HudImages.DEREK_FULL_HEALTH_IMAGE);
		m_DerekHealthImages[2] = (Texture)Resources.Load(Constants.HudImages.DEREK_INJURED_HEALTH_IMAGE);
		m_DerekHealthImages[1] = (Texture)Resources.Load(Constants.HudImages.DEREK_CRITICAL_HEALTH_IMAGE);
		m_DerekHealthImages[0] = (Texture)Resources.Load(Constants.HudImages.DEREK_DEAD_HEALTH_IMAGE);

		m_ZoeyHealthImages[3] = (Texture)Resources.Load(Constants.HudImages.ZOEY_FULL_HEALTH_IMAGE);
		m_ZoeyHealthImages[2] = (Texture)Resources.Load(Constants.HudImages.ZOEY_INJURED_HEALTH_IMAGE);
		m_ZoeyHealthImages[1] = (Texture)Resources.Load(Constants.HudImages.ZOEY_CRITICAL_HEALTH_IMAGE);
		m_ZoeyHealthImages[0] = (Texture)Resources.Load(Constants.HudImages.ZOEY_DEAD_HEALTH_IMAGE);

		m_PuzzlePieceHudImages[0] = (Texture)Resources.Load(Constants.HudImages.PUZZLEPIECE_ZERO_IMAGE);
		m_PuzzlePieceHudImages[1] = (Texture)Resources.Load(Constants.HudImages.PUZZLEPIECE_ONE_IMAGE);
		m_PuzzlePieceHudImages[2] = (Texture)Resources.Load(Constants.HudImages.PUZZLEPIECE_TWO_IMAGE);
		m_PuzzlePieceHudImages[3] = (Texture)Resources.Load(Constants.HudImages.PUZZLEPIECE_THREE_IMAGE);
		m_PuzzlePieceHudImages[4] = (Texture)Resources.Load(Constants.HudImages.PUZZLEPIECE_FOUR_IMAGE);
		m_PuzzlePieceHudImages[5] = (Texture)Resources.Load(Constants.HudImages.PUZZLEPIECE_FIVE_IMAGE);
		m_PuzzlePieceHudImages[6] = (Texture)Resources.Load(Constants.HudImages.PUZZLEPIECE_SIX_IMAGE);
	
		m_Divider = Resources.Load<Texture> (Constants.HudImages.HUD_DIVIDER);
	}

	// Update is called once per frame
	void Update ()
    {
        if (PauseScreen.shouldPause(PAUSE_LEVEL)) { return; }

		if(InputManager.getShowHud())
		{
			ShowHiddenHud();
		}

        if (m_HudDisplayTimer > 0)
        {
            m_HudDisplayTimer -= Time.deltaTime;
        }
        else
        {
            m_HudDisplayTimer = 0;
            m_ShowHiddenHud = false;
        }

        if (m_CheckPointDisplayTimer > 0)
        {
            m_CheckPointDisplayTimer -= Time.deltaTime;
        }
        else
        {
            m_CheckPointDisplayTimer = 0;
            m_ShowCheckPoint = false;
        }

		if (m_LightPegTimer > 0)
		{
			m_LightPegTimer -= Time.deltaTime;
		}
		else
		{
			m_LightPegTimer = 0;
			m_ShowLightPegs = false;
		}

		if (m_PuzzlePieceTimer > 0)
		{
			m_PuzzlePieceTimer -= Time.deltaTime;
		}
		else
		{
			m_PuzzlePieceTimer = 0;
			m_ShowPuzzlePieces = false;
		}

		if (m_LifeTimer > 0)
		{
			m_LifeTimer -= Time.deltaTime;
		}
		else
		{
			m_LifeTimer = 0;
			m_ShowLifes = false;
		}


	}

	public void SetHealth(float Health, int Player)
	{
		if(Player == 1)
		{
			PlayerOneHealth = Health;
		}
		else
		{
			PlayerTwoHealth = Health;
		}

	}

    public void ShowCheckpoint()
    {
        m_ShowCheckPoint = true;
        m_CheckPointDisplayTimer = m_HudDisplayLength;
    }

	public void ShowLightPegs()
	{
		m_ShowLightPegs = true;
		m_LightPegTimer = m_HudDisplayLength;
        LightPegCollected = GameData.Instance.TotalLightPegs();
	}

	public void ShowPuzzlePieces()
	{
		m_ShowPuzzlePieces = true;
		m_PuzzlePieceTimer = m_HudDisplayLength;

		PuzzlePiecesCollected = GameData.Instance.CalcPuzzlePieces();
	}

	public void ShowLifes()
	{
		m_ShowLifes = true;
		m_LifeTimer = m_HudDisplayLength;		
	}

	public void GetNumberOfLifes()
	{
		NumberOfLives = GameData.Instance.GetLivesRemaining();
	}

	public void ShowHiddenHud()
    {
		m_ShowLightPegs = false;
		m_ShowLifes = false;
		m_ShowPuzzlePieces = false;
        m_ShowHiddenHud = true;
        m_HudDisplayTimer = m_HudDisplayLength;
        PuzzlePiecesCollected = GameData.Instance.CalcPuzzlePieces();
        LightPegCollected = GameData.Instance.TotalLightPegs();
    }

    public void UpdateLightPegs(int NumberOfLightPegs)
    {
        LightPegCollected = NumberOfLightPegs;
		ShowLightPegs();
    }

    //All our graphics have to be done in on gui
    void OnGUI()
    {
        if (PauseScreen.shouldPause(PAUSE_LEVEL)) { return; }

		GUI.DrawTexture (new Rect (Screen.width * m_DividerPos.x, Screen.height * m_DividerPos.y,
		                          Screen.width * m_DividerScale.x, Screen.height * m_DividerScale.y), m_Divider, ScaleMode.StretchToFill);

		float SizeOfHudElements = Screen.width / 10;
		Rect PositionRect = new Rect(0, 0, SizeOfHudElements, SizeOfHudElements);

		//Hidden hud elements such as collectables and Lives remaining
        if(m_ShowHiddenHud)
        {
			GetNumberOfLifes();

            //Light Pegs
         	GUI.DrawTexture(PositionRect, m_LightPegHudImage);

            PositionRect.Set(PositionRect.width, 0, SizeOfHudElements, SizeOfHudElements);
            if (LightPegCollected < 10)
            {
				m_NumbersDrawer.drawNumber("0" + LightPegCollected.ToString(), PositionRect);
            }
            else
            {
				m_NumbersDrawer.drawNumber(LightPegCollected.ToString(), PositionRect);
            }

			//PuzzlePieces
			PositionRect.Set( Screen.width /2 - SizeOfHudElements, 0, SizeOfHudElements * 2, SizeOfHudElements);
			GUI.DrawTexture(PositionRect, m_PuzzlePieceHudImages[PuzzlePiecesCollected]);

			//Life Counter image
			PositionRect.Set(Screen.width - SizeOfHudElements, 0, SizeOfHudElements, SizeOfHudElements);
			GUI.DrawTexture(PositionRect, m_LifeCounterImage);

			//Number repersenting lives remaining;
			PositionRect.Set(Screen.width - SizeOfHudElements * 2, 0, SizeOfHudElements, SizeOfHudElements);
			if(NumberOfLives < 10)
			{
				//No Lifes implemented yet
				m_NumbersDrawer.drawNumber("0" + NumberOfLives.ToString(), PositionRect);
			}
			else
			{
				//No Lives imlemented yet
				m_NumbersDrawer.drawNumber(NumberOfLives.ToString(), PositionRect);
			}
        }

		//Light Pegs by themselves
		if(m_ShowLightPegs)
		{
			PositionRect.Set(0, 0, SizeOfHudElements, SizeOfHudElements);
			GUI.DrawTexture(PositionRect, m_LightPegHudImage);

			PositionRect.Set(PositionRect.width, 0, SizeOfHudElements, SizeOfHudElements);
			if (LightPegCollected < 10)
			{
				m_NumbersDrawer.drawNumber("0" + LightPegCollected.ToString(), PositionRect);
			}
			else
			{
				m_NumbersDrawer.drawNumber(LightPegCollected.ToString(), PositionRect);
			}
		}

		if(m_ShowLifes)
		{
			GetNumberOfLifes();

			//Life Counter image
			PositionRect.Set(Screen.width - SizeOfHudElements, 0, SizeOfHudElements, SizeOfHudElements);
			GUI.DrawTexture(PositionRect, m_LifeCounterImage);
			
			//Number repersenting lives remaining;
			PositionRect.Set(Screen.width - SizeOfHudElements * 2, 0, SizeOfHudElements, SizeOfHudElements);
			if(NumberOfLives < 10)
			{
				//No Lifes implemented yet
				m_NumbersDrawer.drawNumber("0" + NumberOfLives.ToString(), PositionRect);
			}
			else
			{
				//No Lives imlemented yet
				m_NumbersDrawer.drawNumber(NumberOfLives.ToString(), PositionRect);
			}
		}

		if(m_ShowPuzzlePieces)
		{
			// no Puzzle piece texture yet
			//PuzzlePieces
			PositionRect.Set( Screen.width /2 - SizeOfHudElements, 0, SizeOfHudElements * 2, SizeOfHudElements);
			GUI.DrawTexture(PositionRect, m_PuzzlePieceHudImages[PuzzlePiecesCollected]);
		}

		//CheckPoints
		if(m_ShowCheckPoint)
		{
			PositionRect.Set (Screen.width/4, Screen.height/3, Screen.width/ 2 , Screen.height / 3);
			GUI.DrawTexture(PositionRect, m_CheckpointImage);
		}

		//Health
		PositionRect.Set (0, Screen.height - SizeOfHudElements, SizeOfHudElements, SizeOfHudElements);

		GUI.DrawTexture(PositionRect, m_PlayerOneHealthImages[(int)PlayerOneHealth]);
		GUI.DrawTexture (PositionRect, m_LeftHealthBoarder);

		PositionRect.Set (Screen.width - SizeOfHudElements, Screen.height - SizeOfHudElements, SizeOfHudElements, SizeOfHudElements);

		GUI.DrawTexture(PositionRect, m_PlayerTwoHealthImages[(int)PlayerTwoHealth]);
		GUI.DrawTexture (PositionRect, m_RightHealthBoarder);
    }
}
