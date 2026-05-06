using System.Collections.Generic;
using UnityEngine;

namespace ValheimVRM
{
    public class VrmSettingsUI : MonoBehaviour
    {
        private bool isVisible = false;
        private Rect windowRect = new Rect(20, 20, 350, 600);
        private Vector2 scrollPosition;

        void Update()
        {
            if (MainPlugin.EditorHotkey != null && Input.GetKeyDown(MainPlugin.EditorHotkey.Value))
            {
                isVisible = !isVisible;
            }
        }

        void OnGUI()
        {
            if (!isVisible) return;

            // Allow clicking on the window
            GUI.skin.window.active.background = GUI.skin.window.normal.background;
            windowRect = GUI.Window(0, windowRect, DrawWindow, "Valheim VRM Settings");
        }

        void DrawWindow(int windowID)
        {
            var controller = VrmController.GetLocalController();
            if (controller == null)
            {
                GUILayout.Label("No local VRM controller found.");
                return;
            }

            string playerName = null;
            if (Game.instance != null)
            {
                playerName = Game.instance.GetPlayerProfile().GetName();
            }

            if (string.IsNullOrEmpty(playerName))
            {
                GUILayout.Label("Local player not fully loaded.");
                return;
            }

            var settings = Settings.GetSettings(playerName);
            if (settings == null)
            {
                GUILayout.Label($"No settings loaded for {playerName}");
                return;
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            // Store old values for OnUpdate triggers
            var oldValues = new Dictionary<string, object>();

            // Scales
            DrawFloatSetting("Model Scale", ref settings.ModelScale, 0.1f, 5f, "ModelScale", oldValues);
            DrawFloatSetting("Player Height", ref settings.PlayerHeight, 0.5f, 3f, "PlayerHeight", oldValues);
            DrawFloatSetting("Player Radius", ref settings.PlayerRadius, 0.1f, 1.5f, "PlayerRadius", oldValues);

            GUILayout.Space(10);
            GUILayout.Label("Equipment Scales");
            DrawFloatSetting("Equipment Scale", ref settings.EquipmentScale, 0.1f, 3f, "EquipmentScale", oldValues);
            DrawFloatSetting("Helmet Scale", ref settings.HelmetScale, 0.1f, 3f, "HelmetScale", oldValues);
            DrawFloatSetting("Chest Scale", ref settings.ChestScale, 0.1f, 3f, "ChestScale", oldValues);
            DrawFloatSetting("Shoulder Scale", ref settings.ShoulderScale, 0.1f, 3f, "ShoulderScale", oldValues);
            DrawFloatSetting("Leg Scale", ref settings.LegScale, 0.1f, 3f, "LegScale", oldValues);
            DrawFloatSetting("Utility Scale", ref settings.UtilityScale, 0.1f, 3f, "UtilityScale", oldValues);
            DrawFloatSetting("Left Hand Scale", ref settings.LeftHandScale, 0.1f, 3f, "LeftHandScale", oldValues);
            DrawFloatSetting("Right Hand Scale", ref settings.RightHandScale, 0.1f, 3f, "RightHandScale", oldValues);
            DrawFloatSetting("Left Hand Back Scale", ref settings.LeftHandBackScale, 0.1f, 3f, "LeftHandBackScale", oldValues);
            DrawFloatSetting("Right Hand Back Scale", ref settings.RightHandBackScale, 0.1f, 3f, "RightHandBackScale", oldValues);

            GUILayout.Space(10);
            GUILayout.Label("Offsets");
            DrawVector3Setting("Helmet Offset", ref settings.HelmetOffset, "HelmetOffset", oldValues);
            DrawVector3Setting("Chest Offset", ref settings.ChestOffset, "ChestOffset", oldValues);
            DrawVector3Setting("Shoulder Offset", ref settings.ShoulderOffset, "ShoulderOffset", oldValues);
            DrawVector3Setting("Leg Offset", ref settings.LegOffset, "LegOffset", oldValues);
            DrawVector3Setting("Utility Offset", ref settings.UtilityOffset, "UtilityOffset", oldValues);
            DrawVector3Setting("Left Hand Offset", ref settings.LeftHandOffset, "LeftHandOffset", oldValues);
            DrawVector3Setting("Right Hand Offset", ref settings.RightHandOffset, "RightHandOffset", oldValues);
            DrawVector3Setting("Left Hand Back Offset", ref settings.LeftHandBackOffset, "LeftHandBackOffset", oldValues);
            DrawVector3Setting("Right Hand Back Offset", ref settings.RightHandBackOffset, "RightHandBackOffset", oldValues);

            GUILayout.Space(10);
            GUILayout.Label("Camera");
            DrawBoolSetting("Fix Camera Height", ref settings.FixCameraHeight, "FixCameraHeight", oldValues);
            DrawFloatSetting("Camera Height Offset", ref settings.CameraHeightOffset, -2f, 2f, "CameraHeightOffset", oldValues);
            DrawFloatSetting("Camera Smoothness", ref settings.CameraSmoothness, 1f, 50f, "CameraSmoothness", oldValues);
            DrawFloatSetting("Target Camera Height Offset", ref settings.TargetCameraHeightOffset, -2f, 2f, "TargetCameraHeightOffset", oldValues);

            GUILayout.Space(10);
            GUILayout.Label("Gameplay & Misc");
            DrawBoolSetting("Use MToon Shader", ref settings.UseMToonShader, "UseMToonShader", oldValues);
            DrawBoolSetting("Attempt Texture Fix", ref settings.AttemptTextureFix, "AttemptTextureFix", oldValues);
            DrawFloatSetting("Spring Bone Stiffness", ref settings.SpringBoneStiffness, 0.01f, 5f, "SpringBoneStiffness", oldValues);
            DrawFloatSetting("Spring Bone Gravity Power", ref settings.SpringBoneGravityPower, 0f, 5f, "SpringBoneGravityPower", oldValues);

            GUILayout.EndScrollView();

            // Trigger updates if changed
            if (oldValues.Count > 0)
            {
                settings.OnUpdate(oldValues);
            }

            if (GUILayout.Button("Save Settings"))
            {
                SaveSettings(playerName, settings);
            }

            if (GUILayout.Button("Close"))
            {
                isVisible = false;
            }

            GUI.DragWindow();
        }

        private void DrawFloatSetting(string label, ref float val, float min, float max, string fieldName, Dictionary<string, object> oldValues)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(150));
            float newVal = GUILayout.HorizontalSlider(val, min, max, GUILayout.Width(100));
            string textVal = GUILayout.TextField(newVal.ToString("0.000"), GUILayout.Width(60));
            GUILayout.EndHorizontal();

            if (float.TryParse(textVal, out float parsed) && Mathf.Abs(parsed - val) > 0.001f)
            {
                newVal = parsed;
            }

            if (Mathf.Abs(newVal - val) > 0.001f)
            {
                oldValues[fieldName] = val;
                val = newVal;
            }
        }

        private void DrawVector3Setting(string label, ref Vector3 val, string fieldName, Dictionary<string, object> oldValues)
        {
            GUILayout.Label(label);
            GUILayout.BeginHorizontal();
            GUILayout.Label("X", GUILayout.Width(15));
            string xStr = GUILayout.TextField(val.x.ToString("0.000"), GUILayout.Width(50));
            GUILayout.Label("Y", GUILayout.Width(15));
            string yStr = GUILayout.TextField(val.y.ToString("0.000"), GUILayout.Width(50));
            GUILayout.Label("Z", GUILayout.Width(15));
            string zStr = GUILayout.TextField(val.z.ToString("0.000"), GUILayout.Width(50));
            GUILayout.EndHorizontal();

            float nx = val.x, ny = val.y, nz = val.z;
            float.TryParse(xStr, out nx);
            float.TryParse(yStr, out ny);
            float.TryParse(zStr, out nz);

            Vector3 newVal = new Vector3(nx, ny, nz);
            if (Vector3.Distance(newVal, val) > 0.001f)
            {
                oldValues[fieldName] = val;
                val = newVal;
            }
        }

        private void DrawBoolSetting(string label, ref bool val, string fieldName, Dictionary<string, object> oldValues)
        {
            bool newVal = GUILayout.Toggle(val, label);
            if (newVal != val)
            {
                oldValues[fieldName] = val;
                val = newVal;
            }
        }

        private void SaveSettings(string playerName, Settings.VrmSettingsContainer settings)
        {
            var path = Settings.PlayerSettingsPath(playerName, false);
            System.IO.File.WriteAllText(path, settings.ToString());
            Debug.Log($"[ValheimVRM] Settings saved to {path}");

            // If multiplayer, share updated settings
            var controller = VrmController.GetLocalController();
            if (controller != null)
            {
                controller.ShareVrm();
            }
        }
    }
}
