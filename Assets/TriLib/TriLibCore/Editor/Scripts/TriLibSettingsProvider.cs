using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TriLibCore.Editor
{
    public class TriLibSettingsProvider : SettingsProvider
    {
        private const string ReadersFileTemplate = "//Auto-generated: Do not modify this file!\n\nusing System.Collections;\nusing System.Collections.Generic;\n{0}\nnamespace TriLibCore\n{{\n    public class Readers\n    {{\n        public static IList<string> Extensions\n        {{\n            get\n            {{\n                var extensions = new List<string>();{1}\n                return extensions;\n            }}\n        }}\n        public static ReaderBase FindReaderForExtension(string extension)\n        {{\n\t\t\t{2}\n            return null;\n        }}\n    }}\n}}";

        private class Styles
        {
            public static readonly GUIStyle Group = new GUIStyle { padding = new RectOffset(10, 10, 5, 5) };
        }

        private readonly List<ImporterOption> _importerOptions = new List<ImporterOption>();
        private readonly string _readersFilePath;

        public TriLibSettingsProvider(string path, SettingsScope scopes = SettingsScope.User, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
            var triLibReadersAssets = AssetDatabase.FindAssets("TriLibReaders");
            if (triLibReadersAssets.Length > 0)
            {
                _readersFilePath = AssetDatabase.GUIDToAssetPath(triLibReadersAssets[0]);
            }
            else
            {
                throw new Exception("Could not find TriLibReaders.cs file. Please re-import TriLib package.");
            }
            _importerOptions.Clear();
            var pluginImporters = PluginImporter.GetAllImporters();
            foreach (var pluginImporter in pluginImporters)
            {
                if (!pluginImporter.isNativePlugin && pluginImporter.assetPath.Contains("TriLibCore."))
                {
                    var assembly = Assembly.LoadFile(pluginImporter.assetPath);
                    foreach (var type in assembly.ExportedTypes)
                    {
                        if (type.BaseType == typeof(ReaderBase))
                        {
                            _importerOptions.Add(new ImporterOption(type.Name,  type.Namespace, pluginImporter));
                        }
                    }
                }
            }
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.Space();
            var contentWidth = GUILayoutUtility.GetLastRect().width * 0.5f;
            EditorGUIUtility.labelWidth = contentWidth;
            EditorGUIUtility.fieldWidth = contentWidth;
            EditorGUILayout.BeginVertical(Styles.Group);
            EditorGUILayout.LabelField($"TriLibReaders path: {_readersFilePath}");
            foreach (var importerOption in _importerOptions)
            {
                ShowToggle(importerOption.PluginImporter.GetCompatibleWithAnyPlatform(), importerOption, delegate(bool value)
                {
                    importerOption.PluginImporter.SetCompatibleWithAnyPlatform(value);
                    string usings = null;
                    string extensions = null;
                    string findReader = null;
                    foreach (var importerOption2 in _importerOptions)
                    {
                        if (importerOption.PluginImporter.GetCompatibleWithAnyPlatform())
                        {
                            extensions += $"\n\t\t\t\textensions.AddRange({importerOption2.text}.GetExtensions());";
                            usings += $"using {importerOption2.Namespace};\n";
                            findReader += $"\n\t\t\tif (((IList) {importerOption2.text}.GetExtensions()).Contains(extension))\n\t\t\t{{\n\t\t\t\treturn new {importerOption2.text}();\n\t\t\t}}";
                        }
                    }
                    var text = string.Format(ReadersFileTemplate, usings, extensions, findReader);
                    using (var streamWriter = new StreamWriter(_readersFilePath))
                    {
                        streamWriter.Write(text);
                    }
                    AssetDatabase.SaveAssets();
                });
            }
            EditorGUILayout.EndVertical();
            base.OnGUI(searchContext);
        }

        private static void ShowToggle(bool value, GUIContent guiContent, Action<bool> onValueChanged)
        {
            var newValue = EditorGUILayout.Toggle(guiContent, value);
            if (newValue != value)
            {
                onValueChanged(newValue);
            }
        }

        [SettingsProvider]
        public static SettingsProvider Register()
        {
            var provider = new TriLibSettingsProvider("Project/TriLibCore", SettingsScope.Project)
            {
                keywords = GetSearchKeywordsFromGUIContentProperties<Styles>()
            };
            return provider;
        }
    }
}
