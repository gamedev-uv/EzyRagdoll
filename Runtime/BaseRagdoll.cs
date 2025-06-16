using UnityEngine;
using System.Linq;
using UnityEngine.Events;

namespace UV.EzyRagdoll
{
    using EzyInspector;

    /// <summary>
    /// Defines the base ragdoll
    /// </summary>
    public abstract class BaseRagdoll : MonoBehaviour
    {
        /// <summary>
        /// Whether the ragdoll is limp or not
        /// </summary>
        [field: Header("Settings")]
        [field: SerializeField, ReadOnly] public bool IsLimp { get; protected set; }

        /// <summary>
        /// Whether the ragdoll is to be limp on at start
        /// </summary>
        [field: SerializeField] public bool InitialLimpState { get; protected set; } = false;

        /// <summary>
        /// Whether the colliders are controlled by the ragdoll or not
        /// </summary>
        [field: SerializeField] public bool ControlColliders { get; protected set; } = true;

        /// <summary>
        /// All the rigidbodies on the children under this object 
        /// </summary>
        [field: Header("References")]
        [field: SerializeField, ReadOnly] public Rigidbody[] ChildrenBodies { get; protected set; }

        /// <summary>
        /// All the colliders on the children under this object 
        /// </summary>
        [field: SerializeField, ReadOnly]
        [field: ShowIf(nameof(ControlColliders), true)] public Collider[] ChildrenColliders { get; protected set; }

        /// <summary>
        /// Event which is called when the ragdoll's limp state is enabled
        /// </summary>
        [field: Header("Events")]
        [field: SerializeField] public UnityEvent OnLimpEnabled { get; set; }

        /// <summary>
        /// Event which is called when the ragdoll's limp state is disabled
        /// </summary>
        [field: SerializeField] public UnityEvent OnLimpDisabled { get; set; }

        protected virtual void Awake()
        {
            if (InitialLimpState) EnableLimp();
            else DisableLimp();
        }

        /// <summary>
        /// Finds the required references 
        /// </summary>
        [ContextMenu("Find References")]
        public virtual void FindReferences()
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
        /// Sets the ragdoll limp
        /// </summary>
        [Button]
        public virtual void EnableLimp() => SetLimpState(true);

        /// <summary>
        /// Un-limps the ragdoll
        /// </summary>
        [Button]
        public virtual void DisableLimp() => SetLimpState(false);

        /// <summary>
        /// Sets the limp state of the ragdoll
        /// </summary>
        /// <param name="limpState">The limp state which is to be applied</param>
        protected virtual void SetLimpState(bool limpState)
        {
            IsLimp = limpState;
            if (limpState) OnLimpEnabled?.Invoke();
            if (!limpState) OnLimpDisabled?.Invoke();
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
            EnableLimp();
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
            EnableLimp();
            for (int i = 0; i < ChildrenBodies.Length; i++)
            {
                var rb = ChildrenBodies[i];
                if (rb == null) continue;
                rb.AddForce(force, forceMode);
            }
        }
    }
}