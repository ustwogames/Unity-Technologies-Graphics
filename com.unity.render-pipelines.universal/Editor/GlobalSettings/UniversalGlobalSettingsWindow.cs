using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;
using UnityEditorInternal;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Linq;

namespace UnityEditor.Rendering.Universal
{
    using CED = CoreEditorDrawer<SerializedUniversalRenderPipelineGlobalSettings>;

    class UniversalGlobalSettingsPanelProvider
    {
        static UniversalGlobalSettingsPanelIMGUI s_IMGUIImpl = new UniversalGlobalSettingsPanelIMGUI();

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Project/Graphics/URP Global Settings", SettingsScope.Project)
            {
                activateHandler = s_IMGUIImpl.OnActivate,
                keywords = SettingsProvider.GetSearchKeywordsFromGUIContentProperties<UniversalGlobalSettingsPanelIMGUI.Styles>().ToArray(),
                guiHandler = s_IMGUIImpl.DoGUI,
                titleBarGuiHandler = s_IMGUIImpl.OnTitleBarGUI
            };
        }
    }

    internal partial class UniversalGlobalSettingsPanelIMGUI
    {
        public static readonly CED.IDrawer Inspector;

        public class DocumentationUrls
        {
            public static readonly string k_LightLayers = "Light-Layers";
        }

        static UniversalGlobalSettingsPanelIMGUI()
        {
            Inspector = CED.Group(
                SpriteSection,
                TerrainSection,
                LightLayerNamesSection,
                PostProcessingSection,
                MiscSection
            );
        }

        SerializedUniversalRenderPipelineGlobalSettings serializedSettings;
        UniversalRenderPipelineGlobalSettings settingsSerialized;

        public void OnTitleBarGUI()
        {
            if (GUILayout.Button(CoreEditorStyles.iconHelp, CoreEditorStyles.iconHelpStyle))
                Help.BrowseURL(Documentation.GetPageLink("URP-Global-Settings"));
        }

        public void DoGUI(string searchContext)
        {
            // When the asset being serialized has been deleted before its reconstruction
            if (serializedSettings != null && serializedSettings.serializedObject.targetObject == null)
            {
                serializedSettings = null;
                settingsSerialized = null;
            }

            if (serializedSettings == null || settingsSerialized != UniversalRenderPipelineGlobalSettings.instance)
            {
                if (UniversalRenderPipelineGlobalSettings.instance != null)
                {
                    settingsSerialized = UniversalRenderPipelineGlobalSettings.Ensure();
                    var serializedObject = new SerializedObject(settingsSerialized);
                    serializedSettings = new SerializedUniversalRenderPipelineGlobalSettings(serializedObject);
                }
                else
                {
                    serializedSettings = null;
                    settingsSerialized = null;
                }
            }
            else if (settingsSerialized != null && serializedSettings != null)
            {
                serializedSettings.serializedObject.Update();
            }

            DrawAssetSelection(ref serializedSettings, null);
            DrawWarnings(ref serializedSettings, null);
            if (settingsSerialized != null && serializedSettings != null)
            {
                EditorGUILayout.Space();
                Inspector.Draw(serializedSettings, null);
                serializedSettings.serializedObject?.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// Executed when activate is called from the settings provider.
        /// </summary>
        /// <param name="searchContext"></param>
        /// <param name="rootElement"></param>
        public void OnActivate(string searchContext, VisualElement rootElement)
        {
        }

        void DrawWarnings(ref SerializedUniversalRenderPipelineGlobalSettings serialized, Editor owner)
        {
            bool isURPinUse = UniversalRenderPipeline.asset != null;
            if (isURPinUse && serialized != null)
                return;

            if (isURPinUse)
            {
                ShowMessageWithFixButton(Styles.warningGlobalSettingsMissing, MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox(Styles.warningUrpNotActive, MessageType.Warning);
                if (serialized == null)
                    ShowMessageWithFixButton(Styles.infoGlobalSettingsMissing, MessageType.Info);
            }
        }

        void ShowMessageWithFixButton(string helpBoxLabel, MessageType type)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.HelpBox(helpBoxLabel, type);
                if (GUILayout.Button(Styles.fixAssetButtonLabel, GUILayout.Width(45)))
                {
                    UniversalRenderPipelineGlobalSettings.Ensure();
                }
            }
        }

        #region Universal Global Settings asset selection
        void DrawAssetSelection(ref SerializedUniversalRenderPipelineGlobalSettings serialized, Editor owner)
        {
            var oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = Styles.labelWidth;
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                var newAsset = (UniversalRenderPipelineGlobalSettings)EditorGUILayout.ObjectField(settingsSerialized, typeof(UniversalRenderPipelineGlobalSettings), false);
                if (EditorGUI.EndChangeCheck())
                {
                    UniversalRenderPipelineGlobalSettings.UpdateGraphicsSettings(newAsset);
                    Debug.Assert(newAsset == UniversalRenderPipelineGlobalSettings.instance);
                    if (settingsSerialized != null && !settingsSerialized.Equals(null))
                        EditorUtility.SetDirty(settingsSerialized);
                }

                if (GUILayout.Button(Styles.newAssetButtonLabel, GUILayout.Width(45), GUILayout.Height(18)))
                {
                    UniversalGlobalSettingsCreator.Create(useProjectSettingsFolder: true, activateAsset: true);
                }

                bool guiEnabled = GUI.enabled;
                GUI.enabled = guiEnabled && (settingsSerialized != null);
                if (GUILayout.Button(Styles.cloneAssetButtonLabel, GUILayout.Width(45), GUILayout.Height(18)))
                {
                    UniversalGlobalSettingsCreator.Clone(settingsSerialized, activateAsset: true);
                }
                GUI.enabled = guiEnabled;
            }
            EditorGUIUtility.labelWidth = oldWidth;
            EditorGUILayout.Space();
        }

        #endregion

        #region 2D/Sprite Settings
        static readonly CED.IDrawer SpriteSection = CED.Group(
            CED.Group((serialized, owner) => CoreEditorUtils.DrawSectionHeader(Styles.spriteSettingsLabel)),
            CED.Group((serialized, owner) => EditorGUILayout.Space()),
            CED.Group(DrawSpriteSettings),
            CED.Group((serialized, owner) => EditorGUILayout.Space())
        );
        static void DrawSpriteSettings(SerializedUniversalRenderPipelineGlobalSettings serialized, Editor owner)
        {
            var oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = Styles.labelWidth;

            using (new EditorGUI.IndentLevelScope())
            {
                using (var changed = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.PropertyField(serialized.transparencySortMode, Styles.transparencySortMode);

                    using (new EditorGUI.DisabledGroupScope(serialized.transparencySortMode.intValue != (int)TransparencySortMode.CustomAxis))
                        EditorGUILayout.PropertyField(serialized.transparencySortAxis, Styles.transparencySortAxis);

                    EditorGUILayout.PropertyField(serialized.defaultSpriteMaterialType, Styles.defaultMaterialType);
                    if (serialized.defaultSpriteMaterialType.intValue == (int)UniversalRenderPipelineGlobalSettings.SpriteDefaultMaterialType.Custom)
                        EditorGUILayout.PropertyField(serialized.defaultSpriteCustomMaterial, Styles.defaultCustomMaterial);
                    if (changed.changed)
                    {
                        serialized.serializedObject?.ApplyModifiedProperties();
                        (serialized.serializedObject.targetObject as UniversalRenderPipelineGlobalSettings).UpdateRenderingLayerNames();
                    }
                }
            }

            EditorGUIUtility.labelWidth = oldWidth;
        }
        #endregion

        #region Terrain Settings
        static readonly CED.IDrawer TerrainSection = CED.Group(
            CED.Group((serialized, owner) => CoreEditorUtils.DrawSectionHeader(Styles.terrainSettingsLabel)),
            CED.Group((serialized, owner) => EditorGUILayout.Space()),
            CED.Group(DrawTerrainSettings),
            CED.Group((serialized, owner) => EditorGUILayout.Space())
        );
        static void DrawTerrainSettings(SerializedUniversalRenderPipelineGlobalSettings serialized, Editor owner)
        {
            var oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = Styles.labelWidth;

            using (new EditorGUI.IndentLevelScope())
            {
                using (var changed = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.PropertyField(serialized.supportsTerrainHolesProp, Styles.supportsTerrainHolesText);
                    if (changed.changed)
                    {
                        serialized.serializedObject?.ApplyModifiedProperties();
                        (serialized.serializedObject.targetObject as UniversalRenderPipelineGlobalSettings).UpdateRenderingLayerNames();
                    }
                }
            }

            EditorGUIUtility.labelWidth = oldWidth;
        }
        #endregion

        #region Rendering Layer Names

        static readonly CED.IDrawer LightLayerNamesSection = CED.Group(
            CED.Group((serialized, owner) => CoreEditorUtils.DrawSectionHeader(Styles.lightLayersLabel, contextAction: pos => OnContextClickLightLayerNames(pos, serialized))),
            CED.Group((serialized, owner) => EditorGUILayout.Space()),
            CED.Group(DrawLightLayerNames),
            CED.Group((serialized, owner) => EditorGUILayout.Space())
        );

        static void DrawLightLayerNames(SerializedUniversalRenderPipelineGlobalSettings serialized, Editor owner)
        {
            var oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = Styles.labelWidth;

            using (new EditorGUI.IndentLevelScope())
            {
                using (var changed = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.DelayedTextField(serialized.lightLayerName0, Styles.lightLayerName0);
                    EditorGUILayout.DelayedTextField(serialized.lightLayerName1, Styles.lightLayerName1);
                    EditorGUILayout.DelayedTextField(serialized.lightLayerName2, Styles.lightLayerName2);
                    EditorGUILayout.DelayedTextField(serialized.lightLayerName3, Styles.lightLayerName3);
                    EditorGUILayout.DelayedTextField(serialized.lightLayerName4, Styles.lightLayerName4);
                    EditorGUILayout.DelayedTextField(serialized.lightLayerName5, Styles.lightLayerName5);
                    EditorGUILayout.DelayedTextField(serialized.lightLayerName6, Styles.lightLayerName6);
                    EditorGUILayout.DelayedTextField(serialized.lightLayerName7, Styles.lightLayerName7);
                    if (changed.changed)
                    {
                        serialized.serializedObject?.ApplyModifiedProperties();
                        (serialized.serializedObject.targetObject as UniversalRenderPipelineGlobalSettings).UpdateRenderingLayerNames();
                    }
                }
            }

            EditorGUIUtility.labelWidth = oldWidth;
        }

        static void OnContextClickLightLayerNames(Vector2 position, SerializedUniversalRenderPipelineGlobalSettings serialized)
        {
            var menu = new GenericMenu();
            menu.AddItem(CoreEditorStyles.resetButtonLabel, false, () =>
            {
                var globalSettings = (serialized.serializedObject.targetObject as UniversalRenderPipelineGlobalSettings);
                globalSettings.ResetRenderingLayerNames();
            });
            menu.DropDown(new Rect(position, Vector2.zero));
        }

        #endregion

        #region PostProcessing Settings

        static readonly CED.IDrawer PostProcessingSection = CED.Group(
            CED.Group((serialized, owner) => CoreEditorUtils.DrawSectionHeader(Styles.PostProcessingSettingsLabel)),
            CED.Group((serialized, owner) => EditorGUILayout.Space()),
            CED.Group(DrawPostProcessing),
            CED.Group((serialized, owner) => EditorGUILayout.Space())
        );

        static void DrawPostProcessing(SerializedUniversalRenderPipelineGlobalSettings serialized, Editor ownerEditor)
        {
            var oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = Styles.labelWidth;

            using (new EditorGUI.IndentLevelScope())
            {
                using (var changed = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUI.BeginChangeCheck();
                    var postProcessIncluded = EditorGUILayout.Toggle(Styles.PostProcessIncluded, serialized.postProcessData.objectReferenceValue != null);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serialized.postProcessData.objectReferenceValue = postProcessIncluded ? PostProcessData.GetDefaultPostProcessData() : null;
                    }
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serialized.postProcessData, Styles.PostProcessLabel);
                    EditorGUI.indentLevel--;

                    EditorGUILayout.PropertyField(serialized.colorGradingMode, Styles.colorGradingMode);

                    EditorGUILayout.DelayedIntField(serialized.colorGradingLutSize, Styles.colorGradingLutSize);
                    serialized.colorGradingLutSize.intValue = Mathf.Clamp(serialized.colorGradingLutSize.intValue, UniversalRenderPipelineGlobalSettings.k_MinLutSize, UniversalRenderPipelineGlobalSettings.k_MaxLutSize);

                    EditorGUILayout.PropertyField(serialized.useFastSRGBLinearConversion, Styles.useFastSRGBLinearConversion);
                    CoreEditorUtils.DrawPopup(Styles.volumeFrameworkUpdateMode, serialized.volumeFrameworkUpdateModeProp, Styles.volumeFrameworkUpdateOptions);
                    if (changed.changed)
                    {
                        serialized.serializedObject?.ApplyModifiedProperties();
                        (serialized.serializedObject.targetObject as UniversalRenderPipelineGlobalSettings).UpdateRenderingLayerNames();
                    }
                }
            }

            EditorGUIUtility.labelWidth = oldWidth;
        }

        #endregion

        #region Misc Settings

        static readonly CED.IDrawer MiscSection = CED.Group(
            CED.Group((serialized, owner) => CoreEditorUtils.DrawSectionHeader(Styles.miscSettingsLabel)),
            CED.Group((serialized, owner) => EditorGUILayout.Space()),
            CED.Group(DrawMiscSettings),
            CED.Group((serialized, owner) => EditorGUILayout.Space())
        );

        static void DrawMiscSettings(SerializedUniversalRenderPipelineGlobalSettings serialized, Editor owner)
        {
            var oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = Styles.labelWidth;

            using (new EditorGUI.IndentLevelScope())
            {
                using (var changed = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.PropertyField(serialized.useNativeRenderPass, Styles.RenderPassLabel);
                    EditorGUILayout.PropertyField(serialized.storeActionsOptimizationProperty, Styles.storeActionsOptimizationText);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(serialized.shaderVariantLogLevel, Styles.shaderVariantLogLevel);
                    EditorGUILayout.PropertyField(serialized.debugLevelProp, Styles.debugLevel);
                    if (changed.changed)
                    {
                        serialized.serializedObject?.ApplyModifiedProperties();
                        (serialized.serializedObject.targetObject as UniversalRenderPipelineGlobalSettings).UpdateRenderingLayerNames();
                    }
                }
                EditorGUILayout.PropertyField(serialized.stripDebugVariants, Styles.stripDebugVariantsLabel);
                EditorGUILayout.PropertyField(serialized.stripUnusedPostProcessingVariants, Styles.stripUnusedPostProcessingVariantsLabel);
                EditorGUILayout.PropertyField(serialized.stripUnusedVariants, Styles.stripUnusedVariantsLabel);
                EditorGUILayout.Space();
            }
            EditorGUIUtility.labelWidth = oldWidth;
        }
        #endregion

    }
}
