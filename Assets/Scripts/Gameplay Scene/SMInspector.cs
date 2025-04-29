// Assets/Scripts/SMInspector.cs
using System.Linq;
using UnityEngine;

/// <summary>
/// Utility to log available charts and difficulties in a .sm file.
/// </summary>
public static class SMInspector
{
    public static void Inspect(TextAsset smAsset)
    {
        if (smAsset == null)
        {
            Debug.LogError("SMInspector: No TextAsset provided.");
            return;
        }

        var parts = smAsset.text.Split(new[] { "#NOTES:" }, System.StringSplitOptions.RemoveEmptyEntries)
                                 .Skip(1)
                                 .ToArray();
        Debug.Log($"Found {parts.Length} chart(s) in {smAsset.name}.sm");
        for (int i = 0; i < parts.Length; i++)
        {
            var lines = parts[i].Split('\n')
                                 .Select(l => l.Trim())
                                 .Where(l => l.Length > 0)
                                 .ToArray();
            string diff = lines.Length >= 2 ? lines[1].TrimEnd(':') : "Unknown";
            Debug.Log($"  Chart {i+1}: Difficulty = {diff}");
        }
    }
}