using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

public class CharacterController : MonoBehaviour
{

    // Here I initialize some control values
    [SerializeField] private float m_JumpForce = 400f;								// Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;		// How much to smooth out the movement
	[SerializeField] private LayerMask m_GroundLayer;								// A mask determining what is ground to the character
	[SerializeField] private LayerMask m_StickyLayer;								// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundedCheckerTransform;					// A position marking where to check if the player is grounded.


	public float k_GroundedCheckerRadius = .49f;		// Radius of the overlap circle to determine if grounded
	private bool m_IsGrounded;							// Whether or not the player is grounded.
	private bool m_FacingRight = true;                  // For determining which way the player is currently facing.
	private Rigidbody2D m_Rigidbody2D;					// RigidBody of the player


	private LineRenderer m_LineRenderer;				// Line renderer to draw the tongue hook shot move


	public UnityEvent m_GroundHitEvent;					// Event to handle state when player landed	

	// Wallsticking
	float m_WallJumpTime = 0.0f;					
	float m_WallSlideSpeed = 0.3f;
	public float m_WallDistance = 0.5f;
	bool m_IsWallSliding = false;
	RaycastHit2D m_WallCheckHit;
	float m_jumpTime;

	[SerializeField] public float m_NumberOfJumps = 2;

	public GameObject m_TongueTipPrefab;
	bool m_TongueReady = true;
	bool m_TonguePulling = false;

	string m_parentNameMoving;
	public GameObject m_LavaTransform;
	LavaControl lavaScript;


	// Player death variables
	int m_RemainingLives = 3;
	bool m_ShouldRespawn = false;
	float k_CameraHeight;
	float k_CameraWidth;
	bool m_CheckingBounds = false;

	float k_PlayerMinX;			// Min X coordinate before counting as dead
	float k_PlayerMaxX;    // Max X coordinate before counting as dead

	public GameObject m_CavcasObject;
	public GameObject m_LivesObject;
	public GameObject m_ScoreObject;
	Text livesText;
	Text scoreText;
	SceneHandler m_SceneHandlerScript;

	// Scoring: TODO: make this script?
	int m_Score = 0;
	float m_TimeAlive = 0;
	int m_ObjectsCollected = 0;
	int m_ScoreHeigth = 0;
	int m_ScoreHeigthOld = 0;
	int m_TongueShotsAvailable = 3;
	List<int> m_CollectedObjectsIDs;

	// shot
	public Sprite spriteZero;
	public Sprite spriteOne;
	public Sprite spriteTwo;
	public Sprite spriteThree;
	public GameObject cursorNumber;
	SpriteRenderer cursorRenderer;

	// final scene
	public GameObject m_StatsScreen;
	bool GameOver = false;
	public GameObject m_ScoreFinal;
	public GameObject m_FliesFinal;
	public GameObject m_TimeFinal;
	Text m_ScoreFinalText;
	Text m_FliesFinalText;
	Text m_TimeFinalText;

	float m_TimeToAcc = 0;

	//Sound



	Animator m_PlayerAnimator;
	Animator m_PlayerBodyAnimator;

    [System.Obsolete]
    private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		m_LineRenderer = GetComponent<LineRenderer>();
		m_LineRenderer.startWidth = .07f;
		m_LineRenderer.endWidth = .07f;

		if (m_GroundHitEvent == null)
			m_GroundHitEvent = new UnityEvent();
		m_GroundHitEvent.AddListener(PingGrounded);

		lavaScript = m_LavaTransform.GetComponent<LavaControl>();                                 // script that checks tongue tips collisions

		m_PlayerAnimator = GetComponent<Animator>();
		m_PlayerBodyAnimator = transform.Find("BODY").GetComponent<Animator>();

		k_CameraHeight = 2f * Camera.main.orthographicSize;
		k_CameraWidth = k_CameraHeight * Camera.main.aspect;

		k_PlayerMinX = -1f * k_CameraWidth / 2f;
		k_PlayerMaxX = k_CameraWidth / 2f;

		m_SceneHandlerScript = m_CavcasObject.GetComponent<SceneHandler>();
		m_CollectedObjectsIDs = new List<int>();

		livesText = m_LivesObject.GetComponent<Text>();
		scoreText = m_ScoreObject.GetComponent<Text>();

		cursorRenderer = cursorNumber.GetComponent<SpriteRenderer>();

		m_FliesFinalText = m_FliesFinal.GetComponent<Text>();
		m_ScoreFinalText = m_ScoreFinal.GetComponent<Text>();
		m_TimeFinalText = m_TimeFinal.GetComponent<Text>();

	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_IsGrounded;
        m_IsGrounded = false;

		int groundMask1 = 1 << LayerMask.NameToLayer("Default");
		int stickyMask1 = 1 << LayerMask.NameToLayer("MasterPlatform");
		int finalMask = groundMask1 | stickyMask1;
		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundedCheckerTransform.position, k_GroundedCheckerRadius, finalMask);

		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
                m_IsGrounded = true;
				if (!wasGrounded && (m_Rigidbody2D.velocity.y <= 0))
                {
					m_GroundHitEvent.Invoke();
				}
					
			}
		}


		//Wall Jumping physics
		if (m_FacingRight)
        {
			m_WallCheckHit = Physics2D.Raycast(transform.position, new Vector2(m_WallDistance, 0), m_WallDistance, m_StickyLayer);
			//Debug.DrawRay(transform.position, new Vector2(m_WallDistance, 0), Color.blue);
		} 
		else
        {
			m_WallCheckHit = Physics2D.Raycast(transform.position, new Vector2(-m_WallDistance, 0), m_WallDistance, m_StickyLayer);
			//Debug.DrawRay(transform.position, new Vector2(-m_WallDistance, 0), Color.red);
		}


		if (m_WallCheckHit && !m_IsGrounded)
		{
			m_IsWallSliding = true;
			m_GroundHitEvent.Invoke();
			if (m_WallCheckHit.transform.tag == "Moving")
			{ 
				m_WallJumpTime = 0.0f;
				m_WallSlideSpeed = -1.0f;
			}
			else
			{
				m_WallJumpTime = 0.0f;
				m_WallSlideSpeed = -0.3f;
			}
			m_jumpTime = Time.time + m_WallJumpTime;
		}
		else if (m_jumpTime < Time.time)
        {
			m_PlayerAnimator.SetBool("stickyR", false);
			m_PlayerAnimator.SetBool("stickyL", false);
			m_IsWallSliding = false;
        }

		if (m_IsWallSliding)
        {
			if (m_FacingRight)
				m_PlayerAnimator.SetBool("stickyR", true);
			else m_PlayerAnimator.SetBool("stickyL", true);

			// set animation bool
			m_PlayerAnimator.SetBool("jump", false);
			m_PlayerBodyAnimator.SetBool("jump", false);

			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, Mathf.Clamp(m_Rigidbody2D.velocity.y, m_WallSlideSpeed, float.MaxValue));
        }

		// Check if out of bounds to kill player
		if (transform.position.x > k_PlayerMaxX || transform.position.x < k_PlayerMinX)
        {
			if (!m_CheckingBounds)
            {
				StartCoroutine(SideDeatchCheck());
			}
        }

		if (m_ShouldRespawn)
        {
			//Debug.Log("YOU DIED");
			m_ShouldRespawn = false;
			m_RemainingLives--;
			StartCoroutine(RespawnPlayer(new Vector2(0, m_LavaTransform.transform.position.y + 5)));
		}

		if (m_ScoreHeigth / 10 < (int)(transform.position.y))
        {
			m_ScoreHeigthOld = m_ScoreHeigth;
			m_ScoreHeigth = (int)transform.position.y * 10;
			m_Score += m_ScoreHeigth - m_ScoreHeigthOld;
		}

		livesText.text = m_RemainingLives.ToString();
		scoreText.text = m_Score.ToString();

        switch (m_TongueShotsAvailable)
        {
			case 0:
				cursorRenderer.sprite = spriteZero;
				break;
			case 1:
				cursorRenderer.sprite = spriteOne;
				break;
			case 2:
				cursorRenderer.sprite = spriteTwo;
				break;
			case 3:
				cursorRenderer.sprite = spriteThree;
				break;
			default:
				cursorRenderer.sprite = spriteZero;
				break;
        }

		if (m_RemainingLives <= 0)
		{
			GameOver = true;
			m_FliesFinalText.text = m_ObjectsCollected.ToString();
			m_ScoreFinalText.text = m_Score.ToString();

			TimeSpan finalTime = TimeSpan.FromSeconds(m_TimeAlive);
			m_Rigidbody2D.gravityScale = 0;
			m_Rigidbody2D.velocity = Vector2.zero;
			lavaScript.gameOver = true;
			m_StatsScreen.SetActive(true);
		}

		if (!GameOver)
        {
			m_TimeAlive += Time.deltaTime;
			if (m_TimeToAcc >= 15)
            {
				m_TimeToAcc = 0;
				lavaScript.speed += 0.3f;
			}
			m_TimeToAcc += Time.deltaTime;
		}
		
		//Debug.Log(m_TimeAlive);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.gameObject.layer == LayerMask.NameToLayer("Lava"))
		{
			m_ShouldRespawn = true;
		}

		if (collision.gameObject.layer == LayerMask.NameToLayer("Fly"))
		{
			if (!m_CollectedObjectsIDs.Contains(collision.gameObject.GetInstanceID()))
            {
				m_Score += 100;
				m_ObjectsCollected++;
				m_CollectedObjectsIDs.Add(collision.gameObject.GetInstanceID());
			}
			SoundManagerHandler.PlaySound("PickupSound");
			m_TongueShotsAvailable = 3;
		}
	}



	// stick to moving platform, reset jumps
	private void OnCollisionStay2D(Collision2D collision)
    {

		if (collision.gameObject.CompareTag("Moving") && m_Rigidbody2D.velocity == new Vector2(0,0))
        {
			m_GroundHitEvent.Invoke();
			this.transform.parent = collision.transform;
			m_parentNameMoving = collision.transform.name;
        }

		if (m_TonguePulling)
        {
			this.transform.parent = null;
		}

		if (lavaScript.parentName == m_parentNameMoving)
		{
			this.transform.parent = null;
		}
	}

    private void OnCollisionExit2D(Collision2D collision)
    {
		if (collision.gameObject.CompareTag("Moving"))
		{
			this.transform.parent = null;
		}
	}

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
    //    Gizmos.DrawSphere(m_GroundedCheckerTransform.position, k_GroundedCheckerRadius);
    //}

    void PingGrounded()
    {
		// set animation bool
		m_PlayerAnimator.SetBool("jump", false);
		m_PlayerBodyAnimator.SetBool("jump", false);

		// reset jumps when landed
		m_NumberOfJumps = 2;
	}

	public void MouseMove(Vector2 direction)
	{
		if (!GameOver)
        {
			m_FacingRight = (direction.x >= 0);

			// If the player should jump... Must be grounded or sliding, must have enough possible jumps
			if ((m_IsGrounded || (m_IsWallSliding)) || m_NumberOfJumps > 0)
			{
				int jumpSound = UnityEngine.Random.Range(1, 4);
                switch (jumpSound)
                {
					case 1:
						SoundManagerHandler.PlaySound("JumpSoundOne");
						break;
					case 2:
						SoundManagerHandler.PlaySound("JumpSoundTwo");
						break;
					case 3:
						SoundManagerHandler.PlaySound("JumpSoundThree");
						break;
					default:
						SoundManagerHandler.PlaySound("JumpSoundOne");
						break;
                }
                
				// set animation bool
				m_PlayerAnimator.SetBool("jump", true);
				m_PlayerBodyAnimator.SetTrigger("jumpTrigger");

				//substract one possible jump
				m_NumberOfJumps--;

				// Add a vertical force to the player.
				//m_IsSticked = false;
				m_Rigidbody2D.velocity = Vector2.zero;
				m_Rigidbody2D.AddForce(new Vector2(direction.x * 200, 500));

			}
		}		
	}

	public void TongueGrapple(Vector2 direction)
	{
		if (!GameOver)
		{
			m_FacingRight = (direction.x >= 0);
			StartCoroutine(DrawTongue(direction));
		}
	}

	IEnumerator DrawTongue(Vector2 direction)
	{
		// Prevent Tongue ability spam
        if (m_TongueReady && m_TongueShotsAvailable > 0)
        {
			int jumpSound = UnityEngine.Random.Range(1, 3);
			switch (jumpSound)
			{
				case 1:
					SoundManagerHandler.PlaySound("TongueSoundOne");
					break;
				case 2:
					SoundManagerHandler.PlaySound("TongueSoundTwo");
					break;
				default:
					SoundManagerHandler.PlaySound("TongueSoundOne");
					break;
			}
			m_TongueReady = false;
			float shootOutDrawTime = .55f;						// Time to draw the out of mouth animation animation
			float returnDrawTime = .2f;							// Time to draw the return animation if you miss
			Vector3 originPos = transform.position;				// Start position of the tongue line, changes based on transfrom current position (moves with mouth)			
			Vector3 endPos;                                     // End position of the tongue line. Start position + direction	

			Vector3 newMidPos = new Vector3(0, 0, 0);              // Middle value between origin and end, used to draw the line over time creating animation
			Vector3 newOrigin;                                  // New origin used when "pulling the player to platform". RB is moved, new origin is based on new transfrom position

			m_LineRenderer.positionCount = 2;					// Initialize line between 2 points

			GameObject tongueTip = Instantiate(m_TongueTipPrefab, new Vector2(originPos.x, originPos.y), Quaternion.identity);  // Game object of the spawned tongue tip
			Rigidbody2D tongueTipRb = tongueTip.GetComponent<Rigidbody2D>();													// rb of tongue tip
			CheckTongueCollision tongueScript = tongueTip.GetComponent<CheckTongueCollision>();                                 // script that checks tongue tips collisions


			int groundMask = 1 << LayerMask.NameToLayer("Default");
			int stickyMask = 1 << LayerMask.NameToLayer("Sticky");
			int finalMask = groundMask | stickyMask;
			float rayCastLength = 0.23f;
			RaycastHit2D platformCheckHit = Physics2D.Raycast(tongueTipRb.position, direction, rayCastLength, finalMask);
			
			


			// Draw over shootOut time
			for (float t = 0; t < shootOutDrawTime; t += Time.deltaTime)
			{
				

				// move origin and endpositions based on the movement
				// calculate mid position as lerp over these 2
				// draw the line and redo the origin based on movement
				originPos = transform.position;
				endPos = originPos + new Vector3(direction.x, direction.y) * 7f;
				newMidPos = Vector3.Lerp(originPos, endPos, t / shootOutDrawTime);

				//if (platformCheckHit)
				//{
				//	tongueTipRb.MovePosition(newMidPos);
				//	tongueTipRb.transform.parent = platformCheckHit.transform;
				//	yield return new WaitForFixedUpdate();
				//	break;
				//}

				//platformCheckHit = Physics2D.Raycast(tongueTipRb.position, direction, rayCastLength, finalMask);

				//Debug.DrawRay(tongueTipRb.position, direction, Color.blue);
				

				// If tongue tip collides, break drawing

				if (tongueScript.m_IsTriggered)
				{
					break;
				}

				tongueTipRb.MovePosition(newMidPos);
				m_LineRenderer.SetPosition(0, originPos);
				m_LineRenderer.SetPosition(1, tongueTipRb.position);
				yield return null;
			}

			// New end is current mid position
			endPos = newMidPos;


			if (!tongueScript.m_IsTriggered && !platformCheckHit)
			{
				// if nothing collided, return tongue over return time
				for (float t = 0; t < returnDrawTime; t += Time.deltaTime)
				{
					// move origin and endpositions based on the movement
					// calculate mid position as lerp over these 2 but from end back to origin
					// draw the line and redo the origin based on movement

					newOrigin = transform.position;
					newMidPos = Vector3.Lerp(endPos, newOrigin, t / returnDrawTime);
					tongueTipRb.MovePosition(newMidPos);
					m_LineRenderer.SetPosition(0, newOrigin);
					m_LineRenderer.SetPosition(1, tongueTipRb.position);
					yield return null;
				}
			} 
			else
            {
				// if collided, zero the rb velocity, set tonguePulling to true
				yield return new WaitForFixedUpdate();
				m_Rigidbody2D.velocity = Vector2.zero;
				m_TonguePulling = true;
				m_TongueShotsAvailable--;

				// pull the player toward tongue tip over shoot out time
				for (float t = 0; t < shootOutDrawTime; t += Time.deltaTime)
				{
					// end position is static, calculate mid position from origin to end. Origin moves
					// Move the rb of the player towards the end position
					endPos = tongueTipRb.transform.position;
					newMidPos = Vector3.Lerp(originPos, endPos, t / shootOutDrawTime);
					m_Rigidbody2D.MovePosition(newMidPos);
					newOrigin = transform.position;
					m_LineRenderer.SetPosition(0, newOrigin);
					m_LineRenderer.SetPosition(1, endPos);
					yield return null;
				}

				// move to end and wait for physics tic to end movement
				// now add force as tongue "push" off or over the platform
				// reset jumps
				m_Rigidbody2D.MovePosition(endPos);
				yield return new WaitForFixedUpdate();
				m_Rigidbody2D.AddForce(new Vector2(direction.x * 100, 500));
				m_NumberOfJumps = 2;
			}

			// remove the tongue line animation, destroy the tongue tip prefab
			m_LineRenderer.positionCount = 0;
			tongueTip.transform.DetachChildren();
			GameObject.Destroy(tongueTip);
			m_TongueReady = true;
			m_TonguePulling = false;
		} 
		else
        {
			SoundManagerHandler.PlaySound("NoToungueSound");
		}

	}

	private IEnumerator SideDeatchCheck()
    {
		m_CheckingBounds = true;
		yield return new WaitForSeconds(1.5f);
		if (transform.position.x > k_PlayerMaxX || transform.position.x < k_PlayerMinX)
		{
			m_ShouldRespawn = true;
		}
		m_CheckingBounds = false;
		yield return null;
    }

	private IEnumerator RespawnPlayer(Vector2 position)
	{
		if (!GameOver)
		{
			SoundManagerHandler.PlaySound("DeathSound");
			m_NumberOfJumps = 2;
			m_Rigidbody2D.MovePosition(position);
			yield return new WaitForFixedUpdate();
			m_Rigidbody2D.velocity = Vector2.zero;
			m_Rigidbody2D.AddForce(new Vector2(0, 1500));
			yield return null;
		}
	}
}
