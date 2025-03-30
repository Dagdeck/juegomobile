using System.Collections;
using UnityEngine;
namespace Assets.JSGAONA.Unidad1.Scripts
{

    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {

        [Header("Adjust movement")]
        [SerializeField] private float speedMove;
        [SerializeField] private float speedRotation;
        [SerializeField] private float jumpForce = 3.0f;
        [SerializeField] private int maxJumpCount = 1;
        [SerializeField] private float minFallVelocity = -2;
        [SerializeField][Range(0, 5)] private float gravityMultiplier = 1;
        [SerializeField] private Joystick joystick;
        [SerializeField] private float dashDistance = 5.0f;
        [SerializeField] private float dashTime = 0.2f;
        [SerializeField] private float dashCooldown = 3.0f;
        [SerializeField] private float dashCheckDistance = 5.0f;
        [SerializeField] private LayerMask obstacleLayers;

        [Header("Adjust to ground")]
        [SerializeField] private float radiusDetectedGround = 0.2f;
        [SerializeField] private float groundCheckDistance = 0.0f;
        [SerializeField] private LayerMask ignoreLayer;

        private bool isDashing = false;
        private bool canDash = true;
        private readonly float gravity = -9.8f;
        private int currentJumpCount = 0;
        public bool onGround = false;
        public float fallVelocity = 0;
        private Vector3 dirMove;
        private CharacterController charController;

        private void Awake()
        {
            charController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            dirMove = new Vector3(joystick.Horizontal, 0, joystick.Vertical).normalized;

            if (!isDashing)
            {
                if (onGround)
                {
                    fallVelocity = Mathf.Max(minFallVelocity, fallVelocity + gravity * Time.deltaTime);
                }
                else
                {
                    fallVelocity += gravity * gravityMultiplier * Time.deltaTime;
                }
            }

            if (dirMove != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dirMove);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speedRotation * Time.deltaTime);
            }

            dirMove *= speedMove;
            dirMove.y = fallVelocity;
            charController.Move(dirMove * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            Vector3 positionFoot = transform.position + Vector3.down * groundCheckDistance;
            onGround = Physics.CheckSphere(positionFoot, radiusDetectedGround, ~ignoreLayer);
        }

        public void Jump()
        {
            if (!isDashing)
            {
                if (onGround)
                {
                    onGround = false;
                    CountJump(false);
                }
                else
                {
                    if (currentJumpCount < maxJumpCount)
                    {
                        dirMove.y = 0;
                        CountJump(true);
                    }
                }
            }
        }

        private void CountJump(bool accumulate)
        {
            currentJumpCount = accumulate ? (currentJumpCount + 1) : 1;
            fallVelocity = jumpForce;
        }

        public void Dash()
        {
            if (!isDashing && canDash)
            {
                Vector3 dashDirection = transform.forward;
                if (!Physics.Raycast(transform.position, dashDirection, dashCheckDistance, obstacleLayers))
                {
                    StartCoroutine(DashCoroutine(dashDirection));
                }
            }
        }

        private IEnumerator DashCoroutine(Vector3 dashDirection)
        {
            isDashing = true;
            canDash = false;
            float elapsedTime = 0;
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = startPosition + dashDirection * dashDistance;

            while (elapsedTime < dashTime)
            {
                charController.Move((targetPosition - transform.position) / (dashTime - elapsedTime) * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            isDashing = false;
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Vector3 positionFoot = transform.position + Vector3.down * groundCheckDistance;
            Gizmos.DrawSphere(positionFoot, radiusDetectedGround);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * dashCheckDistance);
        }
#endif
    }
}
