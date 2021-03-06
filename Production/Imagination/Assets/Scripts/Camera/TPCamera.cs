﻿/*
*TPCamera
*
*resposible for camera controll / movement
*
*Created by: Kris Matis
*/

#region ChangeLog
/*
* 8/10/2014 Edit: Fully Commented - Kris Matis.
*
* 10/10/2014 Edit: no longer has internal classes
 * 
 * 21/10/2014 Edit: added a public CameraSnap function that does the same as CameraSnap but is activated by boolean - Greg Fortier usable for crawlspaces
 * 
 * 31/10/2014 edit: culling mask and shutter layer is being set in preperation for pause screen
*/
#endregion

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(AcceptInputFrom))]
public class TPCamera : ShutterCamera
{
    //used to get all the cameras in the scene
    static List<TPCamera> m_Cameras = new List<TPCamera>();
    public static List<TPCamera> Cameras
    {
        get { return m_Cameras; }
    }

	const string CAMERA_IGNORE_COLLISION_LAYER = "CameraCollisionIgnore";
    int m_RaycastMask;

    string[] CAMERA_IGNORE_LAYERS = { "CameraIgnore", "CameraOneIgnore", "CameraTwoIgnore", };
    int m_IgnoreLayer;

    static int m_IgnoreCounter = 1;

    const float SHUTTER_OFFSET = 0.2f;

	//what the camera accepts input from 
    AcceptInputFrom m_AcceptInputFrom;

	//the players game object or an object at the players position (camera looks at this)
    public GameObject Player;

	//the point that will be rotated (should be a parent of the lerp point)
    public GameObject RotationPoint;
	//used to speed up or slow down how fast you spin the camera
    public Vector2 RotationScale;

	//th point that the camera lerps its position to
    public GameObject LerpTo;

	//how fast the lerp is
    const float LERP_AMOUNT = 0.16f;

	//used to decide if the debug rays should be drawn
    public bool DrawRays = false;
	//the offset of the rayscasts (should be ofset more than the clipping plane
    public Vector2 RaycastOffset;

	// the camera to control
    Camera m_Camera;

	//the auto lerp amount
    const float AUTO_LERP_BASE_AMOUNT = 0.08f;

    const ScriptPauseLevel PAUSE_LEVEL = ScriptPauseLevel.PauseMenu;

    const float SPLIT_AMOUNT = 0.0025f;

    void OnDestroy()
    {
        //removes the camera from the static list
        for(int i = 0; i < m_Cameras.Count; i++)
        {
            if(m_Cameras[i] == this)
            {
                m_Cameras.RemoveAt(i);
                break;
            }
        }

		m_IgnoreCounter--;
    }

	void Awake()
	{
		m_Cameras.Add(this);
	}

    // Use this for initialization
	protected override void Start ()
    { 	//find the camera on this gameobject
        m_Camera = gameObject.GetComponent<Camera>();
        
        //===================================================================
        //move the shutter to be slightly in front of the near clipping plane
        ShutterRotationPoint.transform.forward = transform.forward;

        ShutterRotationPoint.transform.position = m_Camera.transform.position + (m_Camera.transform.forward * (m_Camera.nearClipPlane + 0.1f))
                                                    - transform.up + (transform.right * SHUTTER_OFFSET);
		base.Start ();

        m_IgnoreLayer = m_IgnoreCounter++;
        m_Camera.cullingMask = LayerMask.GetMask(CAMERA_IGNORE_LAYERS[m_IgnoreLayer]) | m_Camera.cullingMask;

		setShutterLayer (CAMERA_IGNORE_LAYERS [0]);

		SetLayerMask();

		//===================================================================
		//find the player the camera is on
		//then find out if theyre player one or two
		//adjust the screen to be on the correct players side
        Characters currentCharacter;
        switch (transform.parent.name)
        {
            case Constants.ALEX_STRING:
                currentCharacter = Characters.Alex;
                break;
            case Constants.DEREK_STRING:
                currentCharacter = Characters.Derek;
                break;
            case Constants.ZOE_STRING:
                currentCharacter = Characters.Zoe;
                break;
            default:
				#if DEBUG || UNITY_EDITOR
               		Debug.LogError("parent is named wrong");
				#endif
                currentCharacter = Characters.Zoe;
                break;
        }
		/* horizontal split
        if (GameData.Instance.PlayerOneCharacter == currentCharacter)
        {
           m_Camera.rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f);
        }
        else
        {
            m_Camera.rect = new Rect(0.0f, 0.0f, 1.0f, 0.5f);
        }*/

		//vertical split
		if (GameData.Instance.PlayerOneCharacter == currentCharacter)
		{
            m_Camera.rect = new Rect(0.0f, 0.0f, 0.5f - SPLIT_AMOUNT, 1.0f);
		}
		else
		{
            m_Camera.rect = new Rect(0.5f + SPLIT_AMOUNT, 0.0f, 0.5f, 1.0f);
		}
		//==================================================================================
        //get the accept input from script on the camera game object
		m_AcceptInputFrom = gameObject.GetComponent<AcceptInputFrom>();

        //===================================================================
		//make sure that game data updates our rotation scale modifyer
		GameData.Instance.updateCameraRotationSpeeds();
	}

	void SetLayerMask()
	{
		m_RaycastMask = LayerMask.GetMask(CAMERA_IGNORE_COLLISION_LAYER) | LayerMask.GetMask(Constants.PLAYER_STRING) | 
			LayerMask.GetMask(Constants.COLLIDE_WITH_MOVING_PLATFORM_LAYER_STRING) | LayerMask.GetMask(Constants.CHECKPOINT_STRING);
		for(int i = 0; i < CAMERA_IGNORE_LAYERS.Length; i++)
		{
			m_RaycastMask = m_RaycastMask | LayerMask.GetMask(CAMERA_IGNORE_LAYERS[i]);
		}
		m_RaycastMask = ~m_RaycastMask;	
	}

	void setShutterLayer(string layer)
	{
		ShutterRotationPoint.gameObject.layer = LayerMask.NameToLayer(layer);
		foreach (Transform objTransform in base.ShutterRotationPoint.transform)
		{
			objTransform.gameObject.layer = LayerMask.NameToLayer(layer);
		}
	}
	
	// Update is called once per frame
	void Update ()
    {
        //update the shutter (base class)
        updateShutter();

        if (PauseScreen.shouldPause(PAUSE_LEVEL)) { return; }

		//move the rotation points position to the player
        RotationPoint.transform.position = Vector3.Lerp(RotationPoint.transform.position, Player.transform.position, LERP_AMOUNT);

		//update rotation
        Rotation2();
        
        //see if the camera should snap
        CameraSnap();
        
        //lerp the camera position
        LerpToPosition();
        
        //look at the player
        LookAt();
        
        //check for collision
        Collision();
	}

	protected override void updateShutter ()
	{
		base.updateShutter ();

		if(m_ShowShutter == false && m_IsDoneShutterMove == true)
		{
			setShutterLayer(CAMERA_IGNORE_LAYERS[0]);
		}
		else
		{
			setShutterLayer(CAMERA_IGNORE_LAYERS[m_IgnoreLayer]);
		}
	}

    //=================================================================================================
    //=================================================================================================
    //behaviors
    #region Behaviors

    //public so other things can snap the camera
    void CameraSnap()
    {
        if (InputManager.getCameraSnapDown(m_AcceptInputFrom.ReadInputFrom))
        {
            RotationPoint.transform.rotation = Player.transform.rotation;
        }
    }

	public void CameraSnap(bool snap)
	{
		if (snap)
		{
			RotationPoint.transform.rotation = Player.transform.rotation;
		}
	}

    //=================================================================================================

	//behaviour that handles auto rotation and player rotation of the camera
    void Rotation()
    {

        //get the input for camera Rotation
        Vector2 rotationInput = InputManager.getCamera(m_AcceptInputFrom.ReadInputFrom);

        //rotate in the y to move the camera horizontally, since the camera needs to be on a set horizontal axis this is done in world space
        RotationPoint.transform.Rotate(0.0f, -rotationInput.x * RotationScale.x, 0.0f, Space.World);

        //rotate the camera up and down since were spinning inb the y this needs to be done in local space
        RotationPoint.transform.Rotate(rotationInput.y * RotationScale.y, 0.0f, 0.0f, Space.Self);

        //get the current euler angles
        Vector3 eulerangles = RotationPoint.transform.rotation.eulerAngles;

        //since the we need the angle to stay between 0 - 75 and 295-360 we need two different clamps
        if(eulerangles.x <= 180)
        {
            eulerangles.x = Mathf.Clamp(eulerangles.x, 0.0f, 75.0f);
        }
        else
        {
            eulerangles.x = Mathf.Clamp(eulerangles.x, 295.0f, 360.0f);
        }
        //the z axis slowly collects tiny amounts of rotation (might be rounding) but we reset it to 0.0 since were never rotating in the z
        eulerangles.z = 0.0f;

        //reset the rotation with the clamped values
        RotationPoint.transform.rotation = Quaternion.Euler(eulerangles);
    }

	// second behaviour for rotation (note: only use one rotation beaviour)
    void Rotation2()
	{
		//get the input for camera Rotation
		Vector2 rotationInput = InputManager.getCamera(m_AcceptInputFrom.ReadInputFrom);

		//if there was input
		if(rotationInput.magnitude != 0.0f)
		{
			//rotate in the y to move the camera horizontally, since the camera needs to be on a set horizontal axis this is done in world space
			RotationPoint.transform.Rotate(0.0f, -rotationInput.x * RotationScale.x, 0.0f, Space.World);
				
			//rotate the camera up and down since were spinning inb the y this needs to be done in local space
			RotationPoint.transform.Rotate(rotationInput.y * RotationScale.y, 0.0f, 0.0f, Space.Self);
				
			//get the current euler angles
			Vector3 eulerangles = RotationPoint.transform.rotation.eulerAngles;
				
			//since the we need the angle to stay between 0 - 75 and 295-360 we need two different clamps
			if(eulerangles.x <= 180)
			{
				eulerangles.x = Mathf.Clamp(eulerangles.x, 0.0f, 75.0f);
			}
			else
			{
				eulerangles.x = Mathf.Clamp(eulerangles.x, 295.0f, 360.0f);
			}
			//the z axis slowly collects tiny amounts of rotation (might be rounding) but we reset it to 0.0 since were never rotating in the z
			eulerangles.z = 0.0f;
				
			//reset the rotation with the clamped values
			RotationPoint.transform.rotation = Quaternion.Euler(eulerangles);
		}
		else
		{
			//there was no input,
			//if the player is moving
			if(InputManager.getMove(m_AcceptInputFrom.ReadInputFrom).magnitude != 0.0f)
			{
				//get the camera rotaion point euler angles
                Vector3 currentEulerAngles = RotationPoint.transform.rotation.eulerAngles;
				//get the rotation of the "player" object
                Vector3 targetEulerAngles = Player.transform.rotation.eulerAngles;

				//come up with a percentage of how similar the angles are
                float percentLerp = 1.0f - (Mathf.Abs(currentEulerAngles.y - targetEulerAngles.y) / 180.0f);
				//square it
                percentLerp *= percentLerp;
				//lerp the camera rotation points rotation to the players rotation 
                RotationPoint.transform.rotation = Quaternion.Slerp(RotationPoint.transform.rotation, Quaternion.Euler(currentEulerAngles.x, targetEulerAngles.y, targetEulerAngles.z), AUTO_LERP_BASE_AMOUNT * percentLerp);
				
				//end result of this if is the camera lerps behind you while you move
				//and it will lerp faster if its closer to being behind you
			}
		}
	}

    //=================================================================================================

	//lerp the cameras position
    void LerpToPosition()
    {
        //lerp to the target position
        gameObject.transform.position = Vector3.Lerp(transform.position, LerpTo.transform.position, LERP_AMOUNT);
    }

    //=================================================================================================

	//the look at behaviour
    void LookAt()
    {
        //for now just look at the rotaion point
        transform.LookAt(RotationPoint.transform.position);
    }

    //=================================================================================================

	//behaviour for camera collision
    void Collision()
    {
		//raycast four times to areas around the cameras point based off the rotation of the camera and the offset
		//each function returns the minimum distance (whether or not its ray cast hit an object first
        float minDist = float.MaxValue;
        minDist = raycast(minDist, transform.right *  RaycastOffset.x,  transform.up * RaycastOffset.y);
        minDist = raycast(minDist, transform.right * -RaycastOffset.x,  transform.up * RaycastOffset.y);
        minDist = raycast(minDist, transform.right *  RaycastOffset.x, -transform.up * RaycastOffset.y);
        minDist = raycast(minDist, transform.right * -RaycastOffset.x, -transform.up * RaycastOffset.y);

		//if none of the ray casts hit anything then the min dist is still the max value
        if (minDist == float.MaxValue)
        {
            minDist = (transform.position - Player.transform.position).magnitude;
        }
		//get the direction to move the camera
        Vector3 direction = transform.position - Player.transform.position;

        direction.Normalize();

		//do we need to draw debug rays?
        if (DrawRays)
        {
			#if DEBUG || UNITY_EDITOR
                Debug.DrawRay(Player.transform.position, direction * minDist, Color.red);
			#endif
        }

		//if min dist is less that the cameras current distance from the player we need to move the camera
        if (minDist < (transform.position - Player.transform.position).magnitude)
        {
            transform.position = Player.transform.position + (direction * minDist);
        }
    }

	//returnsthe closer distance between the raycast hit and the value passed in relative to the player
    float raycast(float currentMinDist, Vector3 offsetX, Vector3 offsetY)
    {
		//calulate the direction to raycast in
        Vector3 RayDirection = (transform.position + offsetX + offsetY) - Player.transform.position;
        float raycastDistance;

		//calculate the distance we need to raycast
        if ((LerpTo.transform.position - Player.transform.position).magnitude > (transform.position - Player.transform.position).magnitude)
        {
            raycastDistance = (LerpTo.transform.position - Player.transform.position).magnitude;
        }
        else
        {
            raycastDistance = (transform.position - Player.transform.position).magnitude;
        }

		//do the raycast
        RaycastHit raycastInfo;
        Physics.Raycast(Player.transform.position, RayDirection, out raycastInfo, raycastDistance, m_RaycastMask);

		//do we need to draw debug rays
        if (DrawRays)
        {
			#if DEBUG || UNITY_EDITOR
	            if (raycastInfo.collider != null)
	            {
	                Debug.DrawRay(Player.transform.position, raycastInfo.point - Player.transform.position, Color.yellow);
	            }
	            else
	            {
	                Debug.DrawRay(Player.transform.position, transform.position + offsetX + offsetY - Player.transform.position, Color.green);
	            }
			#endif
        }

		//if we didnt hit anything
        if (raycastInfo.collider == null)
        {
            return currentMinDist;
        }

		//de we return the old min distance or ours (which is smaller)
        if (currentMinDist <= (raycastInfo.point - Player.transform.position).magnitude)
        {
            return currentMinDist;
        }
        else
        {
            return (raycastInfo.point - Player.transform.position).magnitude;
        }
    }

    //=================================================================================================
    #endregion
}
