using UnityEngine;

namespace UV.EzyRagdoll.Samples.Triggers
{
    using EzyInspector;

    /// <summary>
    /// Enables the attached <see cref="Ragdoll"/> when a collision with a specified tag occurs and applies force based on the impact
    /// </summary>
    [HideMonoGUI]
    [RequireComponent(typeof(Ragdoll))]
    [AddComponentMenu("UV/Ezy Ragdoll/Collision Trigger")]
    public class CollisionTrigger : MonoBehaviour
    {
        /// <summary>
        /// Reference to the <see cref="Ragdoll"/> component on this GameObject
        /// </summary>
        [field: Header("References")]
        [field: SerializeField, ReadOnly] public Ragdoll Ragdoll { get; private set; }

        /// <summary>
        /// Tag of the objects that can trigger the ragdoll on collision
        /// </summary>
        [field: Header("Basic Settings")]
        [field: SerializeField, TagSelector] public string TargetTag { get; private set; }

        /// <summary>
        /// Multiplier applied to the collision force before sending it to the ragdoll
        /// </summary>
        [field: SerializeField] public float ForceMultiplier { get; private set; } = 1;

        /// <summary>
        /// Automatically sets the <see cref="Ragdoll"/> reference on component reset
        /// </summary>
        private void Reset()
        {
            Ragdoll = GetComponent<Ragdoll>();
        }

        /// <summary>
        /// Enables limp mode on collision with a tagged object and applies impulse-based force to the ragdoll
        /// </summary>
        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.collider.CompareTag(TargetTag)) return;

            Ragdoll.EnableLimp();
            var contact = collision.GetContact(0);
            var force = contact.impulse.magnitude * ForceMultiplier;
            Ragdoll.AddForce(force, contact.point);
        }
    }
}