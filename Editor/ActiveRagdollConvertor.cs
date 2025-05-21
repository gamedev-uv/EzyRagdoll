using UnityEditor;
using UnityEngine;

namespace UV.EzyRagdoll.Editors
{
    /// <summary>
    /// Window for converting normal ragdolls into active ones
    /// </summary>
    public class ActiveRagdollConvertor : EditorWindow
    {
        [MenuItem("UV/Ezy Ragdoll/Active Ragdoll Convertor", priority = 12)]
        static void Initialize()
        {
            GetWindow<ActiveRagdollConvertor>("Active Ragdoll Convertor").ShowUtility();
        }

        /// <summary>
        /// The ragdoll container which contains the ragdoll
        /// </summary>
        private Transform _ragdollContainer;

        /// <summary>
        /// The strength of the ragdoll
        /// </summary>
        private float _strength = 10;

        /// <summary>
        /// Draws the header for the window 
        /// </summary>
        protected virtual void DrawWindowHeader()
        {
            GUIStyle _headerStyle = new(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = EditorStyles.largeLabel.fontSize
            };

            _headerStyle.fontSize *= 2;
            EditorGUILayout.LabelField("Active Ragdoll Convertor", _headerStyle, GUILayout.MinHeight(50));
            _headerStyle.fontSize /= 2;
        }

        /// <summary>
        /// Draws the Unity Editor window GUI
        /// </summary>
        protected virtual void OnGUI()
        {
            DrawWindowHeader();

            _ragdollContainer = EditorGUILayout.ObjectField("Ragdoll Container : ", _ragdollContainer, typeof(Transform), true) as Transform;
            _strength = EditorGUILayout.FloatField("Strength : ", _strength);

            if (GUILayout.Button("Convert to Active Ragdoll"))
            {
                ConvertToActiveRagdoll();
            }
        }

        /// <summary>
        /// Converts the assigned ragdoll to an active ragdoll
        /// </summary>
        private void ConvertToActiveRagdoll()
        {
            if (_ragdollContainer == null)
            {
                Debug.LogError("Please assign a ragdoll container");
                return;
            }

            //Create the animated object
            var animatedObjectContainer = InitializeAnimatedObject();

            //Remove animator from ragdoll container
            if (_ragdollContainer.TryGetComponent(out Animator animator))
                DestroyImmediate(animator);

            //Convert all character joints to configurable joints
            ConvertToConfigurableJoints();

            //Create a parent to contain both the parts
            var parent = new GameObject($"{_ragdollContainer.transform.name}").transform;
            parent.SetPositionAndRotation(_ragdollContainer.position, _ragdollContainer.rotation);
            parent.gameObject.AddComponent<ActiveRagdoll>().Initialize(_ragdollContainer, animatedObjectContainer);

            //Parent the animated and ragdoll to new parent
            _ragdollContainer.SetParent(parent, true);
            animatedObjectContainer.SetParent(parent, true);

            //Rename objects
            _ragdollContainer.name = "Ragdoll";
            animatedObjectContainer.name = "Animated";

            Debug.Log($"Converted : '{_ragdollContainer.name} to an active ragdoll'");
        }

        /// <summary>
        /// Instantiates and prepares the animated object version of the ragdoll
        /// </summary>
        /// <returns>The prepared animated object Transform</returns>
        private Transform InitializeAnimatedObject()
        {
            //Create the animated object 
            var animatedObjectContainer = Instantiate(_ragdollContainer);

            //Add animator to animated object container
            if (!animatedObjectContainer.TryGetComponent(out Animator animator))
                animator = animatedObjectContainer.gameObject.AddComponent<Animator>();

            animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            //Remove all ragdoll components from the animated object
            foreach (var rb in animatedObjectContainer.GetComponentsInChildren<Rigidbody>())
            {
                if (rb.TryGetComponent(out CharacterJoint joint))
                    DestroyImmediate(joint);

                if (rb.TryGetComponent(out Collider collider))
                    DestroyImmediate(collider);

                DestroyImmediate(rb);
            }

            foreach (var renderer in animatedObjectContainer.GetComponentsInChildren<Renderer>())
                renderer.enabled = false;

            return animatedObjectContainer;
        }

        /// <summary>
        /// Converts all CharacterJoint components under the ragdoll container into ConfigurableJoint components
        /// </summary>
        private void ConvertToConfigurableJoints()
        {
            var rigidBodies = _ragdollContainer.GetComponentsInChildren<Rigidbody>();

            foreach (var rb in rigidBodies)
            {
                if (!rb.TryGetComponent<CharacterJoint>(out var cj)) continue;
                var connectedBody = cj.connectedBody;

                //Create new joint
                var joint = cj.gameObject.AddComponent<ConfigurableJoint>();

                //Copy basic settings
                joint.anchor = cj.anchor;
                joint.connectedBody = connectedBody;
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = cj.connectedAnchor;

                //Copy axes
                joint.axis = cj.axis;
                joint.secondaryAxis = cj.swingAxis;

                //Set muscle-like spring behaviour
                SoftJointLimitSpring angularSpring = new()
                {
                    spring = _strength,
                    damper = _strength * 0.2f
                };
                joint.angularXLimitSpring = angularSpring;
                joint.angularYZLimitSpring = angularSpring;

                //Set motion limits
                joint.xMotion = ConfigurableJointMotion.Locked;
                joint.yMotion = ConfigurableJointMotion.Locked;
                joint.zMotion = ConfigurableJointMotion.Locked;
                joint.angularXMotion = ConfigurableJointMotion.Limited;
                joint.angularYMotion = ConfigurableJointMotion.Limited;
                joint.angularZMotion = ConfigurableJointMotion.Limited;

                //Set angular limits roughly to match humanoid range
                joint.lowAngularXLimit = new SoftJointLimit { limit = -45f };
                joint.highAngularXLimit = new SoftJointLimit { limit = 45f };
                joint.angularYLimit = new SoftJointLimit { limit = 30f };
                joint.angularZLimit = new SoftJointLimit { limit = 30f };

                //Destroy old character joint
                Object.DestroyImmediate(cj);
            }

            Debug.Log("Converted all CharacterJoints to ConfigurableJoints for active ragdoll");
        }
    }
}