using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace UV.EzyRagdoll
{
    using EzyInspector;

    /// <summary>
    /// Manages the working of the ragdoll
    /// </summary>
    [HideMonoGUI]
    [AddComponentMenu("UV/EzyRagdoll/Ragdoll")]
    public class Ragdoll : MonoBehaviour
    {
        /// <summary>
        /// Whether the ragdoll is to be turned off at start
        /// </summary>
        [field: Header("Settings")]
        [field: SerializeField] public bool DisableRagdollAtStart { get; private set; } = true;

        /// <summary>
        /// Whether the colliders are controlled by the ragdoll or not
        /// </summary>
        [field: SerializeField] public bool ControlColliders { get; private set; } = true;

        /// <summary>
        /// All the rigidbodies on the children under this object 
        /// </summary>
        [field: Header("References")]
        [field: SerializeField, ReadOnly] public Rigidbody[] ChildrenBodies { get; private set; }

        /// <summary>
        /// All the colliders on the children under this object 
        /// </summary>
        [field: SerializeField, ReadOnly]
        [field: ShowIf(nameof(ControlColliders), true)] public Collider[] ChildrenColliders { get; private set; }

        /// <summary>
        /// Event which is called when the ragdoll is enabled
        /// </summary>
        [field: Header("Events")]
        [field: SerializeField] public UnityEvent OnRagdollEnabled { get; private set; }

        /// <summary>
        /// Event which is called when the ragdoll is disabled
        /// </summary>
        [field: SerializeField] public UnityEvent OnRagdollDisabled { get; private set; }

        private void Reset() => FindReferences();

        private void Awake()
        {
            FindReferences();
            if (DisableRagdollAtStart) DisableRagdoll();
        }

        /// <summary>
        /// Finds all the required references 
        /// </summary>
        [Button]
        public void FindReferences()
        {
            //Find all the rigidbodies and colliders on the children of the object
            ChildrenBodies = GetComponentsInChildren<Rigidbody>()
                             .Where(x => x.transform != this)
                             .ToArray();

            ChildrenColliders = GetComponentsInChildren<Collider>()
                                .Where(x => x.transform != this)
                                .ToArray();
        }

        /// <summary>
        /// Sets the activation state of the ragdoll as per as the given bool
        /// </summary>
        /// <param name="activationState">The animation state (whether the ragdoll is to be enabled or disabled)</param>
        public void SetRagdoll(bool activationState)
        {
            ChildrenBodies ??= new Rigidbody[0];
            ChildrenColliders ??= new Collider[0];

            //Manage Rigidbodies
            for (int i = 0; i < ChildrenBodies.Length; i++)
            {
                var rb = ChildrenBodies[i];
                if (rb == null) continue;
                rb.isKinematic = !activationState;
            }

            if (!ControlColliders) return;

            //Manage Colliders
            for (int i = 0; i < ChildrenColliders.Length; i++)
            {
                var collider = ChildrenColliders[i];
                if (collider == null) continue;
                collider.enabled = activationState;
            }
        }

        /// <summary>
        /// Activates the ragdoll
        /// </summary>
        [Button("Enable Ragdoll")]
        public void EnableRagdoll()
        {
            SetRagdoll(true);
            OnRagdollEnabled?.Invoke();
        }

        /// <summary>
        /// Deactivates the ragdoll
        /// </summary>
        [Button("Disable Ragdoll")]
        public void DisableRagdoll()
        {
            SetRagdoll(false);
            OnRagdollDisabled?.Invoke();
        }

        /// <summary>
        /// Activates the ragdoll and adds an explosive force to it 
        /// </summary>
        /// <param name="force">The magnitude of the force to be added</param>
        public void AddForce(float force)
        {
            AddForce(force, transform.position);
        }

        /// <summary>
        /// Activates the ragdoll and adds an explosive force to it at the given point 
        /// </summary>
        /// <param name="force">The magnitude of the force to be applied</param>
        /// <param name="forcePoint">The point at which the force to be added</param>
        /// <param name="forceMode">The force mode which is to be used</param>
        public void AddForce(float force, Vector3 forcePoint, ForceMode forceMode = ForceMode.Impulse)
        {
            EnableRagdoll();
            for (int i = 0; i < ChildrenBodies.Length; i++)
            {
                var rb = ChildrenBodies[i];
                if (rb == null) continue;
                rb.AddForce((rb.transform.position - forcePoint).normalized * force, forceMode);
            }
        }

        /// <summary>
        /// Activates the ragdoll and adds an explosive force to it at the given point 
        /// </summary>
        /// <param name="force">The force to be applied</param>
        /// <param name="forceMode">The force mode which is to be used</param>
        public void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Impulse)
        {
            EnableRagdoll();
            for (int i = 0; i < ChildrenBodies.Length; i++)
            {
                var rb = ChildrenBodies[i];
                if (rb == null) continue;
                rb.AddForce(force, forceMode);
            }
        }
    }
}
