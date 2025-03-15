// author: Omnistudio
// version: 2025.03.15

using UnityEngine;
using UnityEngine.Events;

namespace Omnis
{
    [RequireComponent(typeof(Rigidbody))]
    public class Actor3D : ActorBase
    {
        #region Serialized fields
        [SerializeField] private MovingType movingType;
        [SerializeField] private float moveScale;
        [SerializeField] private float initJumpForce;
        [SerializeField] private float jumpActionSpeed;
        #endregion

        #region Fields
        private Rigidbody rb;
        private Coroutine jumpCoroutine;
        private UnityAction<float> jumpAction;
        #endregion

        #region Properties
        public override float JumpAxis
        {
            get => base.JumpAxis;
            set
            {
                if (!Jumpable && value > 0f) return;

                base.JumpAxis = value;
                if (value == 0f)
                {
                    if (jumpCoroutine != null) StopCoroutine(jumpCoroutine);
                }
                else if (value > 0f)
                {
                    jumpCoroutine = StartCoroutine(Util.YieldTweaker.LerpFixed(jumpAction, jumpActionSpeed));
                }
            }
        }
        #endregion

        #region Functions
        private void Move()
        {
            Vector3 moveForce = movingType switch
            {
                MovingType.Platform2D => moveScale * new Vector3(HorizontalAxis, 0f, 0f),
                _ => moveScale * new Vector3(HorizontalAxis, 0f, VerticalAxis)
            };
            rb.AddForce(moveForce);
        }
        #endregion

        #region Unity methods
        protected override void Start()
        {
            base.Start();
            rb = GetComponent<Rigidbody>();
            jumpAction = (value) =>
            {
                Vector3 jumpForce = movingType switch
                {
                    MovingType.Plane2D => Vector3.zero,
                    _ => initJumpForce * new Vector3(0f, 1f - value, 0f)
                };
                rb.AddForce(jumpForce);
            };
        }
        private void FixedUpdate()
        {
            Move();
        }
        #endregion
    }
}
