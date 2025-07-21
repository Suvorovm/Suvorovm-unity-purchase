using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PurchaseWrapper.Editor
{
#if UNITY_EDITOR
    
    [InitializeOnLoad]
    public static class TangleAsmdefAutoGenerator
    {
        private const string TangleFolderPath = "Assets/Scripts/UnityPurchasing";
        private const string GoogleTangle = "GooglePlayTangle.cs";
        private const string AppleTangle = "AppleTangle.cs";
        private const string AsmdefFileName = "generated.asmdef";

        static TangleAsmdefAutoGenerator()
        {
            // Один раз вызовем при загрузке проекта
            EditorApplication.update += TryGenerateAsmdefOnce;
        }

        private static void TryGenerateAsmdefOnce()
        {
            EditorApplication.update -= TryGenerateAsmdefOnce;
            GenerateAsmdefIfNeeded(auto: true);
        }

        [MenuItem("Tools/UnityPurchasing/Generate 'generated.asmdef'")]
        public static void GenerateAsmdefFromMenu()
        {
            GenerateAsmdefIfNeeded(auto: false);
        }

        private static void GenerateAsmdefIfNeeded(bool auto)
        {
            string googlePath = Path.Combine(TangleFolderPath, GoogleTangle);
            string applePath = Path.Combine(TangleFolderPath, AppleTangle);
            string asmdefPath = Path.Combine(TangleFolderPath, AsmdefFileName);

            if (!File.Exists(googlePath) || !File.Exists(applePath))
            {
                if (!auto)
                    Debug.LogWarning("⚠️ Tangle files not found. Make sure GooglePlayTangle.cs and AppleTangle.cs exist in 'generated' folder.");
                return;
            }

            if (File.Exists(asmdefPath))
            {
                if (!auto)
                    Debug.Log("ℹ️ 'generated.asmdef' already exists. No need to regenerate.");
                return;
            }

            string asmdefJson = @"{
    ""name"": ""generated"",
    ""rootNamespace"": """",
    ""references"": [
        ""UnityEngine.Purchasing"",
        ""UnityEngine.Purchasing.Security"",
        ""UnityEngine.Purchasing.SecurityStub""
    ],
    ""includePlatforms"": [],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}";

            File.WriteAllText(asmdefPath, asmdefJson);
            AssetDatabase.Refresh();
            Debug.Log("✅ 'generated.asmdef' created successfully.");
        }
    }
#endif

}
