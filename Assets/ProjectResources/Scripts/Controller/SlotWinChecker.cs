using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SymbolValue
{
    public string symbolName;
    public int baseValue;
    public int mixedValue; // Lower value for mixed patterns
}

[Serializable]
public class WinLine
{
    public string lineName;
    public Vector2Int[] positions; // (reel, position) pairs
    public float multiplier = 1f; // Optional multiplier for this line
}

[Serializable]
public class WinPattern
{
    public string patternName;
    public int requiredMatches = 3;
    public bool allowMixed = false; // Allow different line types to contribute
    public float multiplier = 1f;
}

public class SlotWinChecker : MonoBehaviour
{
    [Header("Symbol Configuration")]
    public SymbolValue[] symbolValues;
    
    [Header("Win Lines Configuration")]
    public WinLine[] winLines;
    
    
    private Dictionary<string, SymbolValue> symbolValueDict;
    
    void Start()
    {
        InitializeSymbolDictionary();
        SetupDefaultWinLines();
    }
    
    void InitializeSymbolDictionary()
    {
        symbolValueDict = new Dictionary<string, SymbolValue>();
        foreach (var symbol in symbolValues)
        {
            symbolValueDict[symbol.symbolName] = symbol;
        }
    }
    
    void SetupDefaultWinLines()
    {
        if (winLines == null || winLines.Length == 0)
        {
            winLines = new WinLine[]
            {
                // Horizontal lines
                new WinLine { lineName = "Top Horizontal", positions = new Vector2Int[] { new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(2,0) } },
                new WinLine { lineName = "Center Horizontal", positions = new Vector2Int[] { new Vector2Int(0,1), new Vector2Int(1,1), new Vector2Int(2,1) } },
                new WinLine { lineName = "Bottom Horizontal", positions = new Vector2Int[] { new Vector2Int(0,2), new Vector2Int(1,2), new Vector2Int(2,2) } },
                
                // Diagonal lines
                new WinLine { lineName = "Diagonal Down", positions = new Vector2Int[] { new Vector2Int(0,0), new Vector2Int(1,1), new Vector2Int(2,2) } },
                new WinLine { lineName = "Diagonal Up", positions = new Vector2Int[] { new Vector2Int(0,2), new Vector2Int(1,1), new Vector2Int(2,0) } },
                
                // V-shapes and other patterns
                new WinLine { lineName = "V-Shape Down", positions = new Vector2Int[] { new Vector2Int(0,0), new Vector2Int(1,1), new Vector2Int(2,0) } },
                new WinLine { lineName = "V-Shape Up", positions = new Vector2Int[] { new Vector2Int(0,2), new Vector2Int(1,1), new Vector2Int(2,2) } },
                
                // Additional mixed patterns
                new WinLine { lineName = "Mixed Pattern 1", positions = new Vector2Int[] { new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(2,1) } },
                new WinLine { lineName = "Mixed Pattern 2", positions = new Vector2Int[] { new Vector2Int(0,1), new Vector2Int(1,0), new Vector2Int(2,0) } },
                new WinLine { lineName = "Mixed Pattern 3", positions = new Vector2Int[] { new Vector2Int(0,1), new Vector2Int(1,0), new Vector2Int(2,1) } },
                new WinLine { lineName = "Mixed Pattern 4", positions = new Vector2Int[] { new Vector2Int(0,1), new Vector2Int(1,2), new Vector2Int(2,1) } },
                new WinLine { lineName = "Mixed Pattern 5", positions = new Vector2Int[] { new Vector2Int(0,1), new Vector2Int(1,1), new Vector2Int(2,0) } },
                new WinLine { lineName = "Mixed Pattern 6", positions = new Vector2Int[] { new Vector2Int(0,1), new Vector2Int(1,1), new Vector2Int(2,2) } }
            };
        }
    }
    
    public WinResult CheckForWins(string[][] reelSymbols)
    {
        WinResult result = new WinResult();
        
        foreach (var winLine in winLines)
        {
            var lineResult = CheckWinLine(winLine, reelSymbols);
            if (lineResult.hasWin)
            {
                result.winningLines.Add(lineResult);
                result.totalScore += lineResult.score;
                result.hasAnyWin = true;
            }
        }
        
        return result;
    }
    
    private LineWinResult CheckWinLine(WinLine winLine, string[][] reelSymbols)
    {
        LineWinResult result = new LineWinResult
        {
            lineName = winLine.lineName,
            positions = winLine.positions
        };
        
        if (winLine.positions.Length == 0) return result;
        
        // Get symbols in this line
        string[] lineSymbols = new string[winLine.positions.Length];
        for (int i = 0; i < winLine.positions.Length; i++)
        {
            var pos = winLine.positions[i];
            if (pos.x < reelSymbols.Length && pos.y < reelSymbols[pos.x].Length)
            {
                lineSymbols[i] = reelSymbols[pos.x][pos.y];
            }
        }
        
        // Check for matches
        var matchResult = CheckMatches(lineSymbols);
        if (matchResult.matchCount >= 3) // Minimum 3 matches for a win
        {
            result.hasWin = true;
            result.matchingSymbol = matchResult.symbol;
            result.matchCount = matchResult.matchCount;
            
            // Calculate score
            if (symbolValueDict.ContainsKey(matchResult.symbol))
            {
                var symbolValue = symbolValueDict[matchResult.symbol];
                
                // Use mixed value for non-horizontal lines or base value for straight lines
                bool isStraightLine = IsHorizontalLine(winLine) || IsDiagonalLine(winLine);
                int baseScore = isStraightLine ? symbolValue.baseValue : symbolValue.mixedValue;
                
                result.score = Mathf.RoundToInt(baseScore * winLine.multiplier);
            }
        }
        
        return result;
    }
    
    private MatchResult CheckMatches(string[] symbols)
    {
        if (symbols.Length == 0) return new MatchResult();
        
        // Count occurrences of each symbol
        Dictionary<string, int> symbolCounts = new Dictionary<string, int>();
        foreach (string symbol in symbols)
        {
            if (symbolCounts.ContainsKey(symbol))
                symbolCounts[symbol]++;
            else
                symbolCounts[symbol] = 1;
        }
        
        // Find the symbol with the highest count
        string bestSymbol = "";
        int maxCount = 0;
        foreach (var kvp in symbolCounts)
        {
            if (kvp.Value > maxCount)
            {
                maxCount = kvp.Value;
                bestSymbol = kvp.Key;
            }
        }
        
        return new MatchResult { symbol = bestSymbol, matchCount = maxCount };
    }
    
    private bool IsHorizontalLine(WinLine winLine)
    {
        if (winLine.positions.Length < 2) return false;
        int row = winLine.positions[0].y;
        for (int i = 1; i < winLine.positions.Length; i++)
        {
            if (winLine.positions[i].y != row) return false;
        }
        return true;
    }
    
    private bool IsDiagonalLine(WinLine winLine)
    {
        if (winLine.positions.Length < 3) return false;
        
        // Check if it's a diagonal (consistent slope)
        int deltaX = winLine.positions[1].x - winLine.positions[0].x;
        int deltaY = winLine.positions[1].y - winLine.positions[0].y;
        
        for (int i = 2; i < winLine.positions.Length; i++)
        {
            int currentDeltaX = winLine.positions[i].x - winLine.positions[i-1].x;
            int currentDeltaY = winLine.positions[i].y - winLine.positions[i-1].y;
            
            if (currentDeltaX != deltaX || currentDeltaY != deltaY) return false;
        }
        
        return Mathf.Abs(deltaX) == Mathf.Abs(deltaY); // True diagonal
    }
}

[Serializable]
public class WinResult
{
    public bool hasAnyWin = false;
    public int totalScore = 0;
    public List<LineWinResult> winningLines = new List<LineWinResult>();
}

[Serializable]
public class LineWinResult
{
    public bool hasWin = false;
    public string lineName;
    public string matchingSymbol;
    public int matchCount;
    public int score;
    public Vector2Int[] positions;
}

[Serializable]
public class MatchResult
{
    public string symbol = "";
    public int matchCount = 0;
}

// Updated GameController integration
