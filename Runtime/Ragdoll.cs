using UnityEngine;

namespace UV.EzyRagdoll
{
    using EzyInspector;

    /// <summary>
    /// Manages the working of the ragdoll
    /// </summary>
    [HideMonoGUI]
    [AddComponentMenu("UV/Ezy Ragdoll/Ragdoll")]
    public class Ragdoll : BaseRagdoll
    {
        private void Reset() => FindReferences();

        /// <inheritdoc/>
        protected override void SetLimpState(bool limpState)
        {
            base.SetLimpState(limpState);

            ChildrenBodies ??= new Rigidbody[0];
            ChildrenColliders ??= new Collider[0];

            //Manage RigidBodies
            for (int i = 0; i < ChildrenBodies.Length; i++)
            {
                var rb = ChildrenBodies[i];
                if (rb == null) continue;

                if (rb.transform == transform)
                    rb.isKinematic = limpState;
                else
                    rb.isKinematic = !limpState;
            }

            if (!ControlColliders) return;

            //Manage Colliders
            for (int i = 0; i < ChildrenColliders.Length; i++)
            {
                var collider = ChildrenColliders[i];
                if (collider == null) continue;
                collider.enabled = limpState ^ collider.transform == transform;
            }
        }
    }
}