// Assets/Scripts/SMParser.cs
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Parses all charts in a StepMania .sm file into ChartData objects.
/// </summary>
public static class SMParser
{
    static readonly Regex rOffset = new Regex(@"#OFFSET:([-.\d]+);", RegexOptions.Compiled);
    static readonly Regex rBPM    = new Regex(@"#BPMS:([^;]+);",    RegexOptions.Compiled);

    /// <summary>
    /// Parses every difficulty section in the given .sm TextAsset.
    /// Returns a map from difficulty name → ChartData.
    /// </summary>
    public static Dictionary<string, ChartData> ParseAll(TextAsset smAsset)
    {
        if (smAsset == null)
            throw new ArgumentNullException(nameof(smAsset));

        string text = smAsset.text.Replace("\r", "");
        // Extract global OFFSET
        float offset = 0f;
        var mOff = rOffset.Match(text);
        if (mOff.Success)
            offset = float.Parse(mOff.Groups[1].Value);

        // Extract global BPM
        float bpm = 120f;
        var mBpm = rBPM.Match(text);
        if (mBpm.Success)
        {
            var firstPair = mBpm.Groups[1].Value.Split(',')[0];
            bpm = float.Parse(firstPair.Split('=')[1]);
        }
        float secondsPerBeat = 60f / bpm;
        float measureBeats   = 4f;

        var result = new Dictionary<string, ChartData>();
        var parts  = text.Split(new[] { "#NOTES:" }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < parts.Length; i++)
        {
            var lines = parts[i].Split('\n');
            if (lines.Length < 6) continue;

            // difficulty is on the third header line (index 2)
            string difficulty = lines[2].Trim().TrimEnd(':');

            // collect measure lines after the 5-line header
            var measures = new List<string>();
            for (int j = 5; j < lines.Length; j++)
            {
                if (lines[j].Trim() == ";") break;
                measures.Add(lines[j]);
            }

            var chart = new ChartData { notes = new List<ChartNote>() };
            var measureArray = string.Join("\n", measures).Split(',');

            for (int m = 0; m < measureArray.Length; m++)
            {
                var rows = measureArray[m].Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                int rowCount = rows.Length;
                for (int r = 0; r < rowCount; r++)
                {
                    var row = rows[r].Trim();
                    if (row.Length < 4) continue;
                    for (int lane = 0; lane < 4; lane++)
                    {
                        if (row[lane] == '1')
                        {
                            float beatInMeasure = (r / (float)rowCount) * measureBeats;
                            float time = offset + (m * measureBeats + beatInMeasure) * secondsPerBeat;
                            chart.notes.Add(new ChartNote { time = time, laneIndex = lane });
                        }
                    }
                }
            }

            result[difficulty] = chart;
        }
        return result;
    }
}
