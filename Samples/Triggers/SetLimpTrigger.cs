using UnityEngine;

namespace UV.EzyRagdoll.Samples.Triggers
{
    using EzyInspector;

    /// <summary>
    /// Sets the limp state of a <see cref="Ragdoll"/> when it passes through this collider
    /// </summary>
    [HideMonoGUI]
    [AddComponentMenu("UV/Ezy Ragdoll/Set Limp Trigger")]
    public class SetLimpTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Ragdoll ragdoll)) return;
            ragdoll.EnableLimp();
        }
    }
}