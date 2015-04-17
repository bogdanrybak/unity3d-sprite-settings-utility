using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace Staple.EditorScripts
{
    [InitializeOnLoad]
    public class SpriteSettingsUtility
    {
        const string PrefsPath = "Assets/DefaultSpritePrefs.asset";
        
        public static DefaultSpritePrefs Prefs;

        static SpriteSettingsUtility()
        {
            LoadPrefs();
        }

        public static void LoadPrefs()
        {
            Prefs = AssetDatabase.LoadAssetAtPath(PrefsPath, typeof(DefaultSpritePrefs)) as DefaultSpritePrefs;
            if (Prefs == null)
                Prefs = ScriptableObjectUtility.CreateAssetAtPath<DefaultSpritePrefs>(PrefsPath);
        }

        [MenuItem("Assets/Apply Default Texture Settings")]
        public static void ApplyDefaultTextureSettings()
        {
            if (Prefs == null)
                LoadPrefs();

            foreach (var obj in Selection.objects)
            {
                if (!AssetDatabase.Contains(obj)) continue;
 
                string path = AssetDatabase.GetAssetPath(obj);

                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                TextureImporterSettings settings = new TextureImporterSettings();

                // Try to slice it first
                var fileName = Path.GetFileNameWithoutExtension(path);
                var size = GetSize(path, Prefs.SizeDetectionRegex);

                if (size != Vector2.zero)
                {
                    importer.spriteImportMode = SpriteImportMode.Multiple;

                    var gridRects = InternalSpriteUtility.GenerateGridSpriteRectangles(
                        AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D, Vector2.zero, size, Vector2.zero);

                    importer.spritesheet = gridRects
                        .Select(x => new SpriteMetaData
                        {
                            alignment = (int)Prefs.SpriteAlignment,
                            name = fileName + "_" + Array.IndexOf(gridRects, x),
                            rect = x
                        }).ToArray();
                }
                else
                    importer.spriteImportMode = SpriteImportMode.Single;

                importer.ReadTextureSettings(settings);

                settings.filterMode = Prefs.FilterMode;
                settings.wrapMode = Prefs.WrapMode;
                settings.mipmapEnabled = Prefs.EnableMipMaps;
                settings.textureFormat = Prefs.TextureFormat;

                settings.spritePixelsPerUnit = Prefs.PixelsPerUnit;
                settings.spriteAlignment = (int)Prefs.SpriteAlignment;
                settings.spriteExtrude = Prefs.SpriteExtrude;
                settings.spriteMeshType = Prefs.SpriteMeshType;

                importer.SetTextureSettings(settings);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                EditorUtility.SetDirty(obj);
            }
        }

        

        static Vector2 GetSize(string filename, string regex)
        {
            var match = Regex.Match(filename, regex, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                if (match.Groups.Count == 3)
                {
                    return new Vector2(
                        int.Parse(match.Groups[1].Value),
                        int.Parse(match.Groups[2].Value)
                        );
                }
            }

            return Vector2.zero;
        }
    }
}