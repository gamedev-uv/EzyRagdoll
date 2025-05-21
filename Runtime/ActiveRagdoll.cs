using UnityEngine;

namespace UV.EzyRagdoll
{
    using EzyInspector;

    /// <summary>
    /// Moves an active ragdoll to match an animated object
    /// </summary>
    [HideMonoGUI]
    [AddComponentMenu("UV/Ezy Ragdoll/Active Ragdoll")]
    public class ActiveRagdoll : BaseRagdoll
    {
        /// <summary>
        /// The follow strength for the active ragdoll
        /// </summary>
        [field: Header("Strength Settings")]
        [field: SerializeField, ShowIf(nameof(IsLimp), false)] public float FollowStrength { get; private set; } = 10;

        /// <summary>
        /// The root of the ragdoll
        /// </summary>
        [field: Header("References")]
        [field: SerializeField] public Transform RagdollRoot { get; private set; }

        /// <summary>
        /// The root of the animation controlled object
        /// </summary>
        [field: SerializeField] public Transform AnimatedRoot { get; private set; }

        /// <summary>
        /// The joints of the ragdoll
        /// </summary>
        [field: SerializeField] public ConfigurableJoint[] Joints { get; private set; }

        /// <summary>
        /// The bones from the ragdoll
        /// </summary>
        [field: SerializeField, ReadOnly] public Transform[] RagdollBones { get; private set; }

        /// <summary>
        /// The bones from the animated object
        /// </summary>
        [field: SerializeField, ReadOnly] public Transform[] AnimatedBones { get; private set; }

        private void Reset() => Initialize();

        /// <summary>
        /// Initializes the active ragdoll with the given containers
        /// </summary>
        /// <param name="ragdollContainer">The object which contains the ragdoll</param>
        /// <param name="animatedContainer">The object which is animated</param>
        public void Initialize(Transform ragdollContainer, Transform animatedContainer)
        {
            RagdollRoot = ragdollContainer;
            AnimatedRoot = animatedContainer;
            Initialize();
        }

        /// <summary>
        /// Initializes the active ragdoll
        /// </summary>
        [Button]
        protected void Initialize()
        {
            FindReferences();
            InitializeRigidbodies();
        }

        protected override void SetLimpState(bool limpState)
        {
            base.SetLimpState(limpState);

            for (int i = 0; i < ChildrenBodies.Length; i++)
            {
                var rb = ChildrenBodies[i];
                if (rb == null) continue;

                //Remove drag if it is to be limp, else have drag to prevent shaking of bones
                rb.drag = limpState ? 1 : 20;
                rb.angularDrag = limpState ? 0 : 1;
            }
        }

        /// <summary>
        /// Finds all references for the active ragdoll
        /// </summary>
        public override void FindReferences()
        {
            base.FindReferences();

            //Initialize the joints
            Joints = new ConfigurableJoint[ChildrenBodies.Length];

            //Find all the bones of the active ragdoll
            if (RagdollRoot == null || AnimatedRoot == null)
            {
                Debug.Log($"Either or both root(s) for : '{transform.name}' hasn't been assigned! Can't initialize bones", this);
                return;
            }

            RagdollBones = new Transform[ChildrenBodies.Length];
            AnimatedBones = new Transform[ChildrenBodies.Length];

            for (int i = 0; i < ChildrenBodies.Length; i++)
            {
                var rb = ChildrenBodies[i];
                RagdollBones[i] = rb.transform;

                //Find the animated bone pair for a given rb
                string path = GetPath(RagdollRoot, rb.transform);
                AnimatedBones[i] = AnimatedRoot.Find(path);

                if (!rb.TryGetComponent(out ConfigurableJoint joint)) continue;
                Joints[i] = joint;
            }
        }

        /// <summary>
        /// Initializes the rigidbodies
        /// </summary>
        protected virtual void InitializeRigidbodies()
        {
            for (int i = 0; i < ChildrenBodies.Length; i++)
            {
                var rb = ChildrenBodies[i];
                if (rb == null) continue;

                rb.drag = 20;
                rb.angularDrag = 1;
                rb.isKinematic = false;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
        }

        private void FixedUpdate()
        {
            if (IsLimp) return;

            for (int i = 0; i < ChildrenBodies.Length; i++)
            {
                var rb = ChildrenBodies[i];
                var target = AnimatedBones[i];
                if (target == null) continue;

                //Calculate the differences
                Vector3 positionDelta = target.position - rb.position;
                Quaternion rotationDelta = target.rotation * Quaternion.Inverse(rb.rotation); //Diff between rb.rotation and target.rotation
                rotationDelta.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis); //Find out the angle and axis of rotation (inverse of AngleAxis)
                if (float.IsInfinity(rotationAxis.x)) rotationAxis = Vector3.zero;
                float angleInRadians = angleInDegrees * Mathf.Deg2Rad; //Convert to radians

                //Calculate the target velocities
                Vector3 targetVelocity = positionDelta / Time.fixedDeltaTime;
                Vector3 targetAngularVelocity = rotationAxis * angleInRadians / Time.fixedDeltaTime;

                //Interpolate to the targets
                rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, FollowStrength * Time.fixedDeltaTime);
                rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, targetAngularVelocity, FollowStrength * Time.fixedDeltaTime);
            }
        }


        // Recursively builds path to find corresponding animated bone
        private string GetPath(Transform root, Transform target)
        {
            if (target == root) return "";
            return GetPath(root, target.parent) + (string.IsNullOrEmpty(GetPath(root, target.parent)) ? "" : "/") + target.name;
        }
    }
}