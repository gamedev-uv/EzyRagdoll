using UnityEditor;
using UnityEngine;

namespace UV.EzyRagdoll.Editors
{
    /// <summary>
    /// The base editor window which displays a rig 
    /// </summary>
    public abstract class BaseRigContainerWindow : EditorWindow
    {
        /// <summary>
        /// The pelvis of the rig
        /// </summary>
        protected Transform _pelvis;

        /// <summary>
        /// The left hip of the rig
        /// </summary>
        protected Transform _leftHips;

        /// <summary>
        /// The left knee of the rig
        /// </summary>
        protected Transform _leftKnee;

        /// <summary>
        /// The left foot of the rig
        /// </summary>
        protected Transform _leftFoot;

        /// <summary>
        /// The right hip of the rig
        /// </summary>
        protected Transform _rightHips;

        /// <summary>
        /// The right knee of the rig
        /// </summary>
        protected Transform _rightKnee;

        /// <summary>
        /// The right foot of the rig
        /// </summary>
        protected Transform _rightFoot;

        /// <summary>
        /// The left arm of the rig 
        /// </summary>
        protected Transform _leftArm;

        /// <summary>
        /// The left elbow of the rig 
        /// </summary>
        protected Transform _leftElbow;

        /// <summary>
        /// The right arm of the rig 
        /// </summary>
        protected Transform _rightArm;

        /// <summary>
        /// The right elbow of the rig 
        /// </summary>
        protected Transform _rightElbow;

        /// <summary>
        /// The middle spine of the rig 
        /// </summary>
        protected Transform _middleSpine;

        /// <summary>
        /// The head of the rig 
        /// </summary>
        protected Transform _head;

        /// <summary>
        /// The GUIStyle used for the headers
        /// </summary>
        protected GUIStyle _headerStyle;

        /// <summary>
        /// The scroll position of the bone gui 
        /// </summary>
        protected Vector2 _scrollPosition;

        /// <summary>
        /// The header for the window 
        /// </summary>
        protected virtual string WindowHeader => "Rig Window";

        private void OnEnable()
        {
            _headerStyle = new(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = EditorStyles.largeLabel.fontSize
            };
        }

        protected virtual void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DrawWindowHeader();
            DrawBoneGUI();
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Draws the header for the window 
        /// </summary>
        protected virtual void DrawWindowHeader()
        {
            _headerStyle.fontSize *= 2;
            EditorGUILayout.LabelField(WindowHeader, _headerStyle, GUILayout.MinHeight(50));
            _headerStyle.fontSize /= 2;
        }

        /// <summary>
        /// Draws the GUI for all the bones
        /// </summary>
        protected virtual void DrawBoneGUI()
        {
            //Draw the fields for the bones
            DrawBoneField(ref _pelvis, "Pelvis");
            if (_pelvis == null)
            {
                EditorGUILayout.HelpBox("Assign the pelvis to continue", MessageType.Error);
                return;
            }
            DrawBoneField(ref _middleSpine, "Middle Spine");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Upper", _headerStyle);
            DrawBoneField(ref _head, "Head");

            EditorGUILayout.BeginHorizontal();
            DrawBoneField(ref _leftArm, "Left Arm");
            DrawBoneField(ref _rightArm, "Right Arm");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            DrawBoneField(ref _leftElbow, "Left Elbow");
            DrawBoneField(ref _rightElbow, "Right Elbow");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Lower", _headerStyle);

            EditorGUILayout.BeginHorizontal();
            DrawBoneField(ref _leftHips, "Left Hips");
            DrawBoneField(ref _rightHips, "Right Hips");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            DrawBoneField(ref _leftKnee, "Left Knee");
            DrawBoneField(ref _rightKnee, "Right Knee");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            DrawBoneField(ref _leftFoot, "Left Foot");
            DrawBoneField(ref _rightFoot, "Right Foot");
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws an ObjectField for the given bone 
        /// </summary>
        /// <param name="bone">The bone which is to be referenced</param>
        /// <param name="boneName">The name of the bone; this is used for the ObjectField label</param>
        protected virtual void DrawBoneField(ref Transform bone, string boneName)
        {
            EditorGUILayout.BeginVertical();
            bone = EditorGUILayout.ObjectField($"{boneName} : ", bone, typeof(Transform), true) as Transform;
            if (bone != null && !IsUnderRoot(bone) && _pelvis != null)
                EditorGUILayout.HelpBox($"{boneName} needs to be under : {_pelvis.name}", MessageType.Error);
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Whether all the bones for the rig are valid or not 
        /// </summary>
        protected virtual bool AreAllBonesValid()
        {
            return
                //Main bones
                IsUnderRoot(_pelvis)
                && IsUnderRoot(_middleSpine)

                //Upper bones
                && IsUnderRoot(_head)
                && IsUnderRoot(_leftArm)
                && IsUnderRoot(_leftElbow)
                && IsUnderRoot(_rightArm)
                && IsUnderRoot(_rightElbow)

                //Lower bones
                && IsUnderRoot(_leftHips)
                && IsUnderRoot(_leftKnee)
                && IsUnderRoot(_leftFoot)
                && IsUnderRoot(_rightHips)
                && IsUnderRoot(_rightKnee)
                && IsUnderRoot(_rightFoot);
        }

        protected virtual bool IsUnderRoot(Transform bone)
        {
            //If the rig root or the bone transform is null
            if (_pelvis == null || bone == null) return false;
            if (_pelvis == bone) return true;

            //Fetch the parent and check it is under the rig root 
            var parent = bone.parent;
            if (parent == null) return false;
            if (parent == _pelvis) return true;
            return IsUnderRoot(parent);
        }
    }
}
