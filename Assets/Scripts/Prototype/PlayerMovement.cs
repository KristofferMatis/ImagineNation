﻿/*

TO USE

Attach this component to the player.

Create a camera with the camera controller.

Give player movement the camera's transform.





Created by Jason Hein 3/25/2014


3/25/2014
	Added can move function to disable movement while in the character is busy
	Added moveRegular which is a test variable for now. You can remove it when real ground movement is added.
3/28/2014
	Movement is now based on camera projection
3/29/2014
	Added basic jumping (still very buggy)
	Added air, jump, and gliding movement
3/30/2014
	Added movement for pushing blocks
	Now initializes Enviroment interaction script
 */









using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	public Transform m_CameraTransform;

	CharacterController m_Controller;
	bool m_CanMove = true;

	//Speeds
	const float MOVE_SPEED = 6.0f;
	const float CLIMB_SPEED = 3.0f;
	const float FALL_SPEED = 10.0f;
	const float JUMP_SPEED = 10.0f;
	const float AIR_MOVE_SPEED = 3.0f;
	const float GLIDING_FALL_SPEED = 4.0f;
	const float PUSHING_BLOCK_SPEED = 3.0f;
	
	//Jumping timer
	const float JUMP_TIME = 0.2f;
	float m_JumperTimer = 0.0f;

	void Start ()
	{
		//Get character controller
		m_Controller = gameObject.GetComponent<CharacterController>();
		gameObject.AddComponent ("EnvironmentInteraction");
	}

	/// <summary>
	/// Allows you to enable and renable movement during interactions.
	/// </summary>
	public void setCanMove(bool move)
	{
		m_CanMove = move;
	}

	/// <summary>
	/// Returns a normalized input vector based on camera's rotation.
	/// </summary>
	/// <returns>Controller input in relation to camera's rotation.</returns>
	Vector3 getControllerProjection()
	{
		//movementInput move = PlayerInput.Instance.getCameraMovement();

		Vector3 projection = m_CameraTransform.forward * PlayerInput.Instance.getMovementInput().y;
		projection += m_CameraTransform.right * PlayerInput.Instance.getMovementInput().x;

		//Vector3 projection = m_CameraTransform.forward * move.y;
		//projection += m_CameraTransform.right * move.x;

		projection.y = 0;
		return projection.normalized;
	}
	
	void Update ()
	{
		//Temporary testing of movement
		if (IsGrounded ())
		{
			if (m_JumperTimer > 0.0f)
			{
				m_JumperTimer = 0.0f;
			}

			if (Input.GetButtonDown("Jump"))
			{
				JumpMovement();
				m_JumperTimer = JUMP_TIME;
				m_Controller.Move(transform.up);
			}
			else
			{
				GroundMovement();
			}
		}
		else if (m_JumperTimer > 0.0f)
		{
			JumpMovement();
		}
		else
		{
			AirMovement();
		}
	}

	// Checks to see if we are on the ground
	public bool IsGrounded()
	{
		return m_Controller.isGrounded;
		//isGrounded = (characterController.Move (forwardDirection * (Time.deltaTime * movementSpeed)) & CollisionFlags.Below) != 0;
	}

	/// <summary>
	/// Basic walking movement
	/// </summary>
	public void GroundMovement()
	{
		if (!m_CanMove || PlayerInput.Instance.getMovementInput().x == 0 && PlayerInput.Instance.getMovementInput().y == 0)
		{
			return;
		}

		//Moves the player and looks where the player is going
		transform.LookAt (transform.position + getControllerProjection());
		m_Controller.Move (transform.forward * MOVE_SPEED * Time.deltaTime);
	}

	/// <summary>
	/// Player can rotate while aiming.
	/// </summary>
	public void AimMovement()
	{
		if (PlayerInput.Instance.getCameraMovement().x == 0)
		{
			return;
		}
		transform.Rotate(new Vector3 (0.0f, PlayerInput.Instance.getCameraMovement().x * Time.deltaTime, 0.0f));
	}

	/// <summary>
	/// Climbs the player.
	/// </summary>
	public void ClimbMovement()
	{
		if ((PlayerInput.Instance.getMovementInput().x == 0 && PlayerInput.Instance.getMovementInput().y == 0))
		{
			return;
		}

		//Climbing up and down
		Vector3 move = new Vector3 (0, PlayerInput.Instance.getMovementInput().y * CLIMB_SPEED, 0);

		//Climbing left and right
		if (m_CameraTransform.forward.x > 0)
		{
			move += PlayerInput.Instance.getMovementInput().x * CLIMB_SPEED * transform.right;
		}
		else
		{
			move -= PlayerInput.Instance.getMovementInput().x * CLIMB_SPEED * transform.right;
		}

		//Move
		m_Controller.Move (move * Time.deltaTime);
	}

	/// <summary>
	/// Glides the player.
	/// </summary>
	public void GlideMovement()
	{
		//Falling
		m_Controller.Move (-transform.up * GLIDING_FALL_SPEED * Time.deltaTime);

		if (PlayerInput.Instance.getMovementInput().x == 0 && PlayerInput.Instance.getMovementInput().y == 0)
		{
			return;
		}
		
		//Moves the player and looks where the player is going
		transform.LookAt (transform.position + getControllerProjection());
		m_Controller.Move (transform.forward * MOVE_SPEED * Time.deltaTime);
	}

	public void AirMovement()
	{
		if (!m_CanMove)
		{
			return;
		}

		//Falling
		m_Controller.Move (-transform.up * FALL_SPEED * Time.deltaTime);
		
		if (PlayerInput.Instance.getMovementInput().x == 0 && PlayerInput.Instance.getMovementInput().y == 0)
		{
			return;
		}
		
		//Moves the player and looks where the player is going
		transform.LookAt (transform.position + getControllerProjection());

		m_Controller.Move (transform.forward * MOVE_SPEED * Time.deltaTime);
	}

	public void JumpMovement()
	{
		if (!m_CanMove || m_JumperTimer < 0.0f)
		{
			return;
		}
		m_JumperTimer -= Time.deltaTime;
		
		//Jumping up
		if (m_JumperTimer < JUMP_TIME / 0.35f)
		{
			m_Controller.Move (transform.up * (JUMP_SPEED / 2 * Time.deltaTime));
		}
		else
		{
			m_Controller.Move (transform.up * (JUMP_SPEED * Time.deltaTime));
		}
		
		if (PlayerInput.Instance.getMovementInput().x == 0 && PlayerInput.Instance.getMovementInput().y == 0)
		{
			return;
		}
		
		//Moves the player and looks where the player is going
		transform.LookAt (transform.position + getControllerProjection());
		m_Controller.Move (transform.forward * AIR_MOVE_SPEED * Time.deltaTime);
	}

	public void BlockHeldMovement (Size blockSize)
	{
		if (PlayerInput.Instance.getMovementInput().y == 0)
		{
			return;
		}

		//Smaller characters cannot move blocks too large to push
		if (blockSize == Size.Large && (gameObject.name == "Zoey" || gameObject.name == "Derek"))
		{
			return;
		}
		else if (blockSize == Size.Medium && gameObject.name == "Zoey")
		{
			return;
		}

		Vector3 move = Vector3.zero;
		if (m_CameraTransform.forward.z > 0)
		{
			move += PlayerInput.Instance.getMovementInput().y * transform.forward;
		}
		else
		{
			move -= PlayerInput.Instance.getMovementInput().y * transform.forward;
		}

		//Moves the player
		m_Controller.Move (move * PUSHING_BLOCK_SPEED * Time.deltaTime);
	}
}
