using UnityEngine;
using UnityEngine.Events;

public class CharacterController : MonoBehaviour
{

    // Here I initialize some control values
    [SerializeField] private float m_JumpForce = 400f;                     // Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	//[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_GroundLayer;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundedCheckerTransform;                           // A position marking where to check if the player is grounded.
	//[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	//[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

	const float k_GroundedCheckerRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_IsGrounded;            // Whether or not the player is grounded.
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

	[Header("Events")]

	public UnityEvent m_GroundHitEvent;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (m_GroundHitEvent == null)
			m_GroundHitEvent = new UnityEvent();

		m_GroundHitEvent.AddListener(PingGrounded);
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_IsGrounded;
        m_IsGrounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundedCheckerTransform.position, k_GroundedCheckerRadius, m_GroundLayer);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
                m_IsGrounded = true;
				if (!wasGrounded && (m_Rigidbody2D.velocity.y <= 0))
					m_GroundHitEvent.Invoke();
			}
		}
	}

	void PingGrounded()
    {
		Debug.Log("Landed!");
    }


    public void Move(float move, bool canJump, bool canDoubleJump)
	{

        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
        // And then smoothing it out and applying it to the character
        m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        // If the input is moving the player right and the player is facing left...
        if (move > 0 && !m_FacingRight)
        {
            // ... flip the player.
            Flip();
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (move < 0 && m_FacingRight)
        {
            // ... flip the player.
            Flip();
        }

        // If the player should jump...
        if (m_IsGrounded && canJump)
        {
			// Add a vertical force to the player.
			m_IsGrounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			
        } else if (canDoubleJump)
        {
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
    }


	public void MouseMove(Vector2 direction, bool canJump, bool canDoubleJump)
	{

		// Move the character by finding the target velocity
		Vector3 targetVelocity = direction * 10f;
		// And then smoothing it out and applying it to the character
		//m_Rigidbody2D.velocity = targetVelocity;
		m_Rigidbody2D.AddForce(new Vector2(direction.x * m_JumpForce, direction.y * m_JumpForce));
		//m_Rigidbody2D.AddForce(direction * 1);

		// If the player should jump...
		if (m_IsGrounded && canJump)
		{
			// Add a vertical force to the player.
			m_IsGrounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));

		}
		else if (canDoubleJump)
		{
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
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
