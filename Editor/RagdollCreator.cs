using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace UV.EzyRagdoll.Editors
{
    using EzyReflection;

    /// <summary>
    /// The editor window which helps create ragdolls 
    /// </summary>
    public class RagdollCreator : BaseRigContainerWindow
    {
        protected override string WindowHeader => "Ragdoll Creator";

        [MenuItem("UV/Ragdoll Creator")]
        public static void Initialize()
        {
            var window = GetWindow<RagdollCreator>();
            window.titleContent = new("Ragdoll Creator");
            window.minSize = new(450, 420);
            window.maxSize = new(900, 420);
            window.ShowUtility();
        }

        /// <summary>
        /// The rig template to be used
        /// </summary>
        private RigTemplate _rigTemplate;

        /// <summary>
        /// The total mass of the ragdoll 
        /// </summary>
        private float _totalMass = 20;

        /// <summary>
        /// Whether the ragdoll is to be flipped forward
        /// </summary>
        private bool _flipForward;

        /// <summary>
        /// The strength of the ragdoll
        /// </summary>
        private float _strength;

        protected override void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DrawWindowHeader();

            //Draw the rig template field and button
            EditorGUILayout.BeginHorizontal();

            _rigTemplate = EditorGUILayout.ObjectField("Rig Template : ", _rigTemplate, typeof(RigTemplate), false) as RigTemplate;
            if (_rigTemplate != null && _pelvis != null && GUILayout.Button("Load Bones"))
                LoadBones();

            EditorGUILayout.EndHorizontal();

            //Draw the GUI for the bones  
            DrawBoneGUI();

            //Draw the build ragdoll button 
            EditorGUILayout.Space();
            GUI.enabled = AreAllBonesValid();
            if (GUILayout.Button("Build Ragdoll"))
                CreateRagdoll();

            GUI.enabled = true;
            EditorGUILayout.EndScrollView();
        }

        protected override void DrawBoneGUI()
        {
            base.DrawBoneGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Other Settings", _headerStyle);
            _strength = EditorGUILayout.FloatField("Strength : ", _strength);
            _totalMass = EditorGUILayout.FloatField("Total Mass : ", _totalMass);
            _flipForward = EditorGUILayout.Toggle("Flip Forward : ", _flipForward);
        }

        /// <summary>
        /// Loads the bones with the current RigTemplate
        /// </summary>
        public virtual void LoadBones()
        {
            //Main
            _middleSpine = _rigTemplate.GetTransformFromPath(_pelvis, _rigTemplate.MiddleSpine);

            //Upper 
            _head = _rigTemplate.GetTransformFromPath(_pelvis, _rigTemplate.Head);
            _leftArm = _rigTemplate.GetTransformFromPath(_pelvis, _rigTemplate.LeftArm);
            _leftElbow = _rigTemplate.GetTransformFromPath(_pelvis, _rigTemplate.LeftElbow);
            _rightArm = _rigTemplate.GetTransformFromPath(_pelvis, _rigTemplate.RightArm);
            _rightElbow = _rigTemplate.GetTransformFromPath(_pelvis, _rigTemplate.RightElbow);

            //Lower
            _leftHips = _rigTemplate.GetTransformFromPath(_pelvis, _rigTemplate.LeftHips);
            _leftKnee = _rigTemplate.GetTransformFromPath(_pelvis, _rigTemplate.LeftKnee);
            _leftFoot = _rigTemplate.GetTransformFromPath(_pelvis, _rigTemplate.LeftFoot);
            _rightHips = _rigTemplate.GetTransformFromPath(_pelvis, _rigTemplate.RightHips);
            _rightKnee = _rigTemplate.GetTransformFromPath(_pelvis, _rigTemplate.RightKnee);
            _rightFoot = _rigTemplate.GetTransformFromPath(_pelvis, _rigTemplate.RightFoot);
        }

        /// <summary>
        /// Creates a ragdoll with the given bones 
        /// </summary>
        public virtual void CreateRagdoll()
        {
            // Create an instance of RagdollBuilder
            var assembly = typeof(EditorWindow).Assembly;
            var builderInstance = System.Activator.CreateInstance(assembly.GetType("UnityEditor.RagdollBuilder"));

            //Create a Member representing the builderInstance; and find all its children
            var member = new Member(builderInstance);
            member.FindChildren();

            //Set the values of all the bones for the current instance
            member.FindMember("pelvis").SetValue(_pelvis);
            member.FindMember("middleSpine").SetValue(_middleSpine);
            member.FindMember("head").SetValue(_head);
            member.FindMember("leftArm").SetValue(_leftArm);
            member.FindMember("leftElbow").SetValue(_leftElbow);
            member.FindMember("rightArm").SetValue(_rightArm);
            member.FindMember("rightElbow").SetValue(_rightElbow);
            member.FindMember("leftHips").SetValue(_leftHips);
            member.FindMember("leftKnee").SetValue(_leftKnee);
            member.FindMember("leftFoot").SetValue(_leftFoot);
            member.FindMember("rightHips").SetValue(_rightHips);
            member.FindMember("rightKnee").SetValue(_rightKnee);
            member.FindMember("rightFoot").SetValue(_rightFoot);

            //Set values for the settings 
            member.FindMember("totalMass").SetValue(_totalMass);
            member.FindMember("flipForward").SetValue(_flipForward);
            member.FindMember("strength").SetValue(_strength);

            //Find the create methods
            var prepareBones = member.FindMember<Member>("PrepareBones").MemberInfo as MethodInfo;
            var createRagdoll = member.FindMember<Member>("OnWizardCreate").MemberInfo as MethodInfo;

            //Call the methods 
            prepareBones.Invoke(builderInstance, null);
            createRagdoll.Invoke(builderInstance, null);
        }
    }
}
