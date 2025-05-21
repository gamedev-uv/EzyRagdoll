using UnityEditor;
using UnityEngine;

namespace UV.EzyRagdoll.Editors
{
    /// <summary>
    /// The editor window used to create rig templates 
    /// </summary>
    public class RigTemplateCreator : BaseRigContainerWindow
    {
        protected override string WindowHeader => "Rig Template Creator";

        /// <summary>
        /// Initializes the editor window
        /// </summary>
        [MenuItem("UV/Ezy Ragdoll/Rig Template Creator", priority = 10)]
        public static void Initialize()
        {
            var window = GetWindow<RigTemplateCreator>();
            window.titleContent = new("Rig Template Creator");
            window.minSize = new(450, 150);
            window.maxSize = new(900, 400);
            window.ShowUtility();
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            DrawCreateButton();
        }

        /// <summary>
        /// Draws the create button 
        /// </summary>
        private void DrawCreateButton()
        {
            EditorGUILayout.Space(20);
            GUI.enabled = AreAllBonesValid();
            if (GUILayout.Button("Create"))
            {
                //Create the SO
                var treeAsset = CreateInstance<RigTemplate>();
                treeAsset.Initialize(_pelvis)
                         .WithSpineAndHead(_pelvis, _middleSpine, _head)
                         .WithLeftArmAndElbow(_leftArm, _leftElbow)
                         .WithRightArmAndElbow(_rightArm, _rightElbow)
                         .WithLeftLowerLimb(_leftHips, _leftKnee, _leftFoot)
                         .WithRightLowerLimb(_rightHips, _rightKnee, _rightFoot);

                //Save the asset
                AssetDatabase.CreateAsset(treeAsset, AssetDatabase.GenerateUniqueAssetPath($"Assets/Rig Template : [{_pelvis.name}].asset"));
                AssetDatabase.SaveAssetIfDirty(treeAsset);

                //Focus on the SO
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = treeAsset;
            }
            GUI.enabled = true;
        }
    }
}
