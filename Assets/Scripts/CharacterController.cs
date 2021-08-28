using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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
	float m_WallJumpTime = 0.2f;					
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

	//float 



	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		m_LineRenderer = GetComponent<LineRenderer>();
		m_LineRenderer.startWidth = .2f;
		m_LineRenderer.endWidth = .2f;

		if (m_GroundHitEvent == null)
			m_GroundHitEvent = new UnityEvent();
		m_GroundHitEvent.AddListener(PingGrounded);

		lavaScript = m_LavaTransform.GetComponent<LavaControl>();                                 // script that checks tongue tips collisions
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_IsGrounded;
        m_IsGrounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundedCheckerTransform.position, k_GroundedCheckerRadius, m_GroundLayer);

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
			m_NumberOfJumps = 2;
			if (m_WallCheckHit.transform.tag == "Moving")
			{ 
				m_WallJumpTime = 0.0f;
				m_WallSlideSpeed = -1.0f;
			}
			else
			{
				m_WallJumpTime = 0.2f;
				m_WallSlideSpeed = -0.3f;
			}
			m_jumpTime = Time.time + m_WallJumpTime;
		}
		else if (m_jumpTime < Time.time)
        {
			m_IsWallSliding = false;
        }

		if (m_IsWallSliding)
        {
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, Mathf.Clamp(m_Rigidbody2D.velocity.y, m_WallSlideSpeed, float.MaxValue));
        }
    }

	// stick to moving platform, reset jumps
    private void OnCollisionStay2D(Collision2D collision)
    {

		if (collision.gameObject.CompareTag("Moving") && m_Rigidbody2D.velocity == new Vector2(0,0))
        {
			m_NumberOfJumps = 2;
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
    //	Gizmos.color = Color.red;
    // //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
    //	Gizmos.DrawSphere(m_GroundedCheckerTransform.position, k_GroundedCheckerRadius);
    //}

    void PingGrounded()
    {
		// reset jumps when landed
		m_NumberOfJumps = 2;
	}

	public void MouseMove(Vector2 direction)
	{
		m_FacingRight = (direction.x >= 0);

		// If the player should jump... Must be grounded or sliding, must have enough possible jumps
		if ((m_IsGrounded || (m_IsWallSliding)) || m_NumberOfJumps > 0)
		{

			//substract one possible jump
			m_NumberOfJumps--;

			// Add a vertical force to the player.
			//m_IsSticked = false;
			m_Rigidbody2D.velocity = Vector2.zero;
			m_Rigidbody2D.AddForce(new Vector2(direction.x * 200, 500));

		}
		
	}

	public void TongueGrapple(Vector2 direction)
	{
		m_FacingRight = (direction.x >= 0);
		StartCoroutine(DrawTongue(direction));
	}

	IEnumerator DrawTongue(Vector2 direction)
	{
		// Prevent Tongue ability spam
        if (m_TongueReady)
        {
			m_TongueReady = false;
			float shootOutDrawTime = .4f;						// Time to draw the out of mouth animation animation
			float returnDrawTime = .2f;							// Time to draw the return animation if you miss
			Vector3 originPos = transform.position;				// Start position of the tongue line, changes based on transfrom current position (moves with mouth)			
			Vector3 endPos;                                     // End position of the tongue line. Start position + direction	

			Vector3 newMidPos = new Vector3(0, 0, 0);              // Middle value between origin and end, used to draw the line over time creating animation
			Vector3 newOrigin;                                  // New origin used when "pulling the player to platform". RB is moved, new origin is based on new transfrom position

			m_LineRenderer.positionCount = 2;					// Initialize line between 2 points

			GameObject tongueTip = Instantiate(m_TongueTipPrefab, new Vector2(originPos.x, originPos.y), Quaternion.identity);  // Game object of the spawned tongue tip
			Rigidbody2D tongueTipRb = tongueTip.GetComponent<Rigidbody2D>();													// rb of tongue tip
			CheckTongueCollision tongueScript = tongueTip.GetComponent<CheckTongueCollision>();									// script that checks tongue tips collisions

            
			// Draw over shootOut time
			for (float t = 0; t < shootOutDrawTime; t += Time.deltaTime)
			{
				// move origin and endpositions based on the movement
				// calculate mid position as lerp over these 2
				// draw the line and redo the origin based on movement
				originPos = transform.position;
				endPos = originPos + new Vector3(direction.x, direction.y) * 7f;
				newMidPos = Vector3.Lerp(originPos, endPos, t / shootOutDrawTime);
				tongueTipRb.MovePosition(newMidPos);
				m_LineRenderer.SetPosition(0, originPos);
				m_LineRenderer.SetPosition(1, newMidPos);

				// If tongue tip collides, break drawing
				if (tongueScript.m_IsTriggered)
				{
					break;
				}

				yield return null;
			}

			// New end is current mid position
			endPos = newMidPos;


			if (!tongueScript.m_IsTriggered)
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
					m_LineRenderer.SetPosition(1, newMidPos);
					yield return null;
				}
			} 
			else
            {
				// if collided, zero the rb velocity, set tonguePulling to true
				yield return new WaitForFixedUpdate();
				m_Rigidbody2D.velocity = Vector2.zero;
				m_TonguePulling = true;

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
			GameObject.Destroy(tongueTip);
			m_TongueReady = true;
			m_TonguePulling = false;
		}

	}

		private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
