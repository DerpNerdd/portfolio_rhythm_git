// Assets/Editor/StepManiaImporter.cs
using System.IO;
using UnityEngine;


[UnityEditor.AssetImporters.ScriptedImporter(1, new[] { "sm", "ssc" })]
public class StepManiaImporter : UnityEditor.AssetImporters.ScriptedImporter
{
    public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
    {
        // read the raw text of the .sm or .ssc file
        string text = File.ReadAllText(ctx.assetPath);

        // wrap it in a TextAsset
        var ta = new TextAsset(text)
        {
            name = Path.GetFileNameWithoutExtension(ctx.assetPath)
        };

        // register it as the main imported object
        ctx.AddObjectToAsset("main", ta);
        ctx.SetMainObject(ta);
    }
}
