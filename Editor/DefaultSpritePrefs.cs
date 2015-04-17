using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System;

namespace Staple.EditorScripts
{
    public class DefaultSpritePrefs : ScriptableObject
    {
        public string SizeDetectionRegex = @"_(\d+)x(\d+)";

        public FilterMode FilterMode;
        public TextureWrapMode WrapMode;
        public int PixelsPerUnit = 100;

        public SpriteMeshType SpriteMeshType;
        public uint SpriteExtrude = 1;
        public SpriteAlignment SpriteAlignment;
        public TextureImporterFormat TextureFormat = TextureImporterFormat.AutomaticTruecolor;
        public bool EnableMipMaps;

        void OnEnable()
        {
            hideFlags = HideFlags.DontSaveInBuild;
        }
    }
}
