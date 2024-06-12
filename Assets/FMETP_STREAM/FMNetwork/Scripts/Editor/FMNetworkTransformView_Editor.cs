using System;
using UnityEngine;
using UnityEditor;

namespace FMSolution.FMNetwork
{
    [CustomEditor(typeof(FMNetworkTransformView))]
    [CanEditMultipleObjects]
    public class FMNetworkTransformView_Editor : UnityEditor.Editor
    {
        private FMNetworkTransformView FMView;

        SerializedProperty FMNetworkProp;
        SerializedProperty SyncFPSProp;
        SerializedProperty SyncTypeProp;

        void OnEnable()
        {
            FMNetworkProp = serializedObject.FindProperty("FMNetwork");
            SyncFPSProp = serializedObject.FindProperty("SyncFPS");
            SyncTypeProp = serializedObject.FindProperty("SyncType");
        }

        // Update is called once per frame
        public override void OnInspectorGUI()
        {
            if (FMView == null) FMView = (FMNetworkTransformView)target;

            serializedObject.Update();


            GUILayout.Space(2);
            GUILayout.BeginVertical("box");
            {
                {
                    //Header
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.white;
                    style.alignment = TextAnchor.MiddleCenter;
                    style.fontSize = 15;

                    Texture2D backgroundTexture = new Texture2D(1, 1);
                    backgroundTexture.SetPixel(0, 0, new Color(0.02745098f, 0.1176471f, 0.254902f));
                    backgroundTexture.Apply();
                    style.normal.background = backgroundTexture;

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("(( FM Network 3.0 ))", style);
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginVertical("box");
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(FMNetworkProp, new GUIContent("FMNetwork"));
                        GUILayout.EndHorizontal();


                        GUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(SyncFPSProp, new GUIContent("Sync FPS"));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(SyncTypeProp, new GUIContent("Sync Type"));
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical("box");
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Network Info: ");
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        int _viewID = FMView.GetViewID();
                        if (_viewID == -1)
                        {
                            FMView.UpdateAllIDs();
                            _viewID = FMView.GetViewID();
                        }
                        else
                        {
                            if (FMView.CheckAllIDs()) _viewID = FMView.GetViewID();
                        }

                        GUILayout.Label("- view ID: " + _viewID);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("- Is Owner: " + FMView.IsOwner);
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}