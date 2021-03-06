﻿using UnityEngine;
using System.Collections;

public class ButtonV2CameraRotationSpeed : ButtonV2
{

	public Slider i_Slider;
	
	protected float m_Increment = Constants.CAMERA_ROTATE_SPEED_INCREMENT;
	
	protected override void start ()
	{
		base.start ();
		i_Slider.MaxValue = Constants.CAMERA_ROTATE_SPEED_MAX;
		i_Slider.MinValue = Constants.CAMERA_ROTATE_SPEED_MIN;
		i_Slider.Value = Constants.CAMERA_ROTATE_SPEED_DEFAULT;
	}

	public override void use (PlayerInput usedBy = PlayerInput.None)
	{
        if (PlayerInfo.getPlayer(usedBy).m_Player == Players.PlayerOne)
        {

            GameData.Instance.PlayerOneCameraRotationScaleModifyer = Mathf.Clamp(GameData.Instance.PlayerOneCameraRotationScaleModifyer + m_Increment,
                                                                        Constants.CAMERA_ROTATE_SPEED_MIN,
                                                                        Constants.CAMERA_ROTATE_SPEED_MAX);
        }
        else if (PlayerInfo.getPlayer(usedBy).m_Player == Players.PlayerTwo)
        {
            GameData.Instance.PlayerTwoCameraRotationScaleModifyer = Mathf.Clamp(GameData.Instance.PlayerTwoCameraRotationScaleModifyer + m_Increment,
                                                                        Constants.CAMERA_ROTATE_SPEED_MIN,
                                                                        Constants.CAMERA_ROTATE_SPEED_MAX);
        }		
	}

    protected override void defaultState()
    {
        base.defaultState();
       	updateSlider();
    }


    protected void updateSlider()
    {
        if (PlayerInfo.getPlayer((PlayerInput)ParentMenu.ReadInputFromBits).m_Player == Players.PlayerOne)
        {
            i_Slider.Value = GameData.Instance.PlayerOneCameraRotationScaleModifyer;
        }
        else if (PlayerInfo.getPlayer((PlayerInput)ParentMenu.ReadInputFromBits).m_Player == Players.PlayerTwo)
        {
            i_Slider.Value = GameData.Instance.PlayerTwoCameraRotationScaleModifyer;
        }	       
    }
}
