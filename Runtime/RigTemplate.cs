using UnityEngine;

namespace UV.EzyRagdoll
{
    using EzyInspector;

    /// <summary>
    /// Defines the transform, which contains the tree made from the Root object 
    /// </summary>
    [HideMonoGUI]
    public class RigTemplate : ScriptableObject
    {
        /// <summary>
        /// The middle spine of the rig 
        /// </summary>
        [field: SerializeField, HideInInspector] public string MiddleSpine { get; private set; }

        /// <summary>
        /// The head of the rig 
        /// </summary>
        [field: SerializeField, HideInInspector] public string Head { get; private set; }

        /// <summary>
        /// The left arm of the rig 
        /// </summary>
        [field: SerializeField, HideInInspector] public string LeftArm { get; private set; }

        /// <summary>
        /// The left elbow of the rig 
        /// </summary>
        [field: SerializeField, HideInInspector] public string LeftElbow { get; private set; }

        /// <summary>
        /// The right arm of the rig 
        /// </summary>
        [field: SerializeField, HideInInspector] public string RightArm { get; private set; }

        /// <summary>
        /// The right elbow of the rig 
        /// </summary>
        [field: SerializeField, HideInInspector] public string RightElbow { get; private set; }

        /// <summary>
        /// The left hip of the rig
        /// </summary>
        [field: SerializeField, HideInInspector] public string LeftHips { get; private set; }

        /// <summary>
        /// The left knee of the rig
        /// </summary>
        [field: SerializeField, HideInInspector] public string LeftKnee { get; private set; }

        /// <summary>
        /// The left foot of the rig
        /// </summary>
        [field: SerializeField, HideInInspector] public string LeftFoot { get; private set; }

        /// <summary>
        /// The right hip of the rig
        /// </summary>
        [field: SerializeField, HideInInspector] public string RightHips { get; private set; }

        /// <summary>
        /// The right knee of the rig
        /// </summary>
        [field: SerializeField, HideInInspector] public string RightKnee { get; private set; }

        /// <summary>
        /// The right foot of the rig
        /// </summary>
        [field: SerializeField, HideInInspector] public string RightFoot { get; private set; }

        /// <summary>
        /// The root transform of the rig [Used which Initializing the rig]
        /// </summary>
        private Transform _rootTransform;

        /// <summary>
        /// Initializes the RigTemplate with the given pelvis transform.
        /// </summary>
        /// <param name="pelvis">The pelvis of the rig</param>
        /// <returns>The initialized RigTemplate instance.</returns>
        public RigTemplate Initialize(Transform pelvis)
        {
            _rootTransform = pelvis;
            return this;
        }

        /// <summary>
        /// Sets the middle spine, and head transforms and returns the updated RigTemplate instance.
        /// </summary>
        /// <param name="pelvisTransform">The Transform to set as the pelvis.</param>
        /// <param name="middleSpineTransform">The Transform to set as the middle spine.</param>
        /// <param name="headTransform">The Transform to set as the head.</param>
        /// <returns>The updated RigTemplate instance.</returns>
        public RigTemplate WithSpineAndHead(Transform pelvisTransform, Transform middleSpineTransform, Transform headTransform)
        {
            MiddleSpine = GetTransformPath(middleSpineTransform, _rootTransform);
            Head = GetTransformPath(headTransform, _rootTransform);
            return this;
        }

        /// <summary>
        /// Sets the left arm and elbow transforms and returns the updated RigTemplate instance.
        /// </summary>
        /// <param name="leftArmTransform">The Transform to set as the left arm.</param>
        /// <param name="leftElbowTransform">The Transform to set as the left elbow.</param>
        /// <returns>The updated RigTemplate instance.</returns>
        public RigTemplate WithLeftArmAndElbow(Transform leftArmTransform, Transform leftElbowTransform)
        {
            LeftArm = GetTransformPath(leftArmTransform, _rootTransform);
            LeftElbow = GetTransformPath(leftElbowTransform, _rootTransform);
            return this;
        }

        /// <summary>
        /// Sets the left lower limb transforms and returns the updated RigTemplate instance.
        /// </summary>
        /// <param name="leftHipsTransform">The Transform to set as the left hips.</param>
        /// <param name="leftKneeTransform">The Transform to set as the left knee.</param>
        /// <param name="leftFootTransform">The Transform to set as the left foot.</param>
        /// <returns>The updated RigTemplate instance.</returns>
        public RigTemplate WithLeftLowerLimb(Transform leftHipsTransform, Transform leftKneeTransform, Transform leftFootTransform)
        {
            LeftHips = GetTransformPath(leftHipsTransform, _rootTransform);
            LeftKnee = GetTransformPath(leftKneeTransform, _rootTransform);
            LeftFoot = GetTransformPath(leftFootTransform, _rootTransform);
            return this;
        }

        /// <summary>
        /// Sets the right arm and elbow transforms and returns the updated RigTemplate instance.
        /// </summary>
        /// <param name="rightArmTransform">The Transform to set as the right arm.</param>
        /// <param name="rightElbowTransform">The Transform to set as the right elbow.</param>
        /// <returns>The updated RigTemplate instance.</returns>
        public RigTemplate WithRightArmAndElbow(Transform rightArmTransform, Transform rightElbowTransform)
        {
            RightArm = GetTransformPath(rightArmTransform, _rootTransform);
            RightElbow = GetTransformPath(rightElbowTransform, _rootTransform);
            return this;
        }

        /// <summary>
        /// Sets the right lower limb transforms and returns the updated RigTemplate instance.
        /// </summary>
        /// <param name="rightHipsTransform">The Transform to set as the right hips.</param>
        /// <param name="rightKneeTransform">The Transform to set as the right knee.</param>
        /// <param name="rightFootTransform">The Transform to set as the right foot.</param>
        /// <returns>The updated RigTemplate instance.</returns>
        public RigTemplate WithRightLowerLimb(Transform rightHipsTransform, Transform rightKneeTransform, Transform rightFootTransform)
        {
            RightHips = GetTransformPath(rightHipsTransform, _rootTransform);
            RightKnee = GetTransformPath(rightKneeTransform, _rootTransform);
            RightFoot = GetTransformPath(rightFootTransform, _rootTransform);
            return this;
        }

        /// <summary>
        /// Generates a hierarchical index path (tree path) for a given <see cref="Transform"/> relative to another <see cref="Transform"/>.
        /// </summary>
        /// <param name="childTransform">The Transform for which the path is generated.</param>
        /// <param name="relativeTo">The Transform relative to which the path is calculated.</param>
        /// <returns>
        /// A string representing the hierarchical path from relativeTo to childTransform.
        /// Returns "X" if childTransform is the same as relativeTo.
        /// Returns an empty string if childTransform is null or if it has no parent.
        /// Otherwise, returns a dot-separated string of sibling indices.
        /// </returns>
        public string GetTransformPath(Transform childTransform, Transform relativeTo)
        {
            if (childTransform == null) return "";
            if (childTransform == relativeTo) return "X";

            //Find the siblingIndex and parent 
            var siblingIndex = childTransform.GetSiblingIndex();
            var parent = childTransform.parent;

            if (parent == null) return "";
            if (parent == relativeTo) return $"{siblingIndex}";

            return $"{GetTransformPath(parent, relativeTo)}.{siblingIndex}";
        }

        /// <summary>
        /// Retrieves a <see cref="Transform"/> based on a hierarchical index path (tree path) from a given parent <see cref="Transform"/>.
        /// </summary>
        /// <param name="parentTransform">The parent Transform to start the search from.</param>
        /// <param name="transformString">A dot-separated string representing the hierarchical path of indices.</param>
        /// <returns>
        /// The Transform at the specified path relative to the parentTransform.
        /// If the transformString is null, returns the parentTransform.
        /// If the path is invalid, returns null
        /// </returns>
        public Transform GetTransformFromPath(Transform parentTransform, string transformString)
        {
            if (transformString == null || transformString == "X") return parentTransform;

            //Find the childIndex from the beginning of the string
            int childIndex = int.Parse(transformString[0].ToString());
            if (parentTransform.childCount <= childIndex) return null;

            //Strip the string and search the child object  
            return GetTransformFromPath(parentTransform.GetChild(childIndex), 
                                        transformString.Length > 1 ? transformString[2..] : null);
        }
    }
}
