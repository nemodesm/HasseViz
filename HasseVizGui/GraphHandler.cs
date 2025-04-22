using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using HasseVizGui.Util;
using HasseVizLib;
using Mapper.Util;
using Microsoft.Xna.Framework;

namespace HasseVizGui;

public static class GraphHandler
{
    private static Dictionary<GraphNode, float> _nodeXPos;
    private static bool _graphChanged = false;
    private static Graph _graph;
    
    public static Graph? Graph
    {
        get => _graph;
        set
        {
            if (_graph is not null)
            {
                _graph.GraphChanged -= HandleGraphChange;
            }
            _graphChanged = true;
            _nodeXPos = new Dictionary<GraphNode, float>();
            _graph = value;
            _graph!.GraphChanged += HandleGraphChange;
        }
    }

    public static bool GraphChanged => _graphChanged;

    public static void LoadGraphGeneral(string path)
    {
        var firstChar = File.ReadLines(path).First().First();
        switch (firstChar)
        {
            case '0':
            case '1':
                LoadGraphBinText(path);
                break;
            case 'f':
            case 't':
                LoadGraphText(path);
                break;
        }
    }
    
    public static void LoadGraphTGF(string path)
    {
        // TODO
    }

    public static void LoadGraphBinary(string path)
    {
        
    }
    
    public static void LoadGraphBinText(string path)
    {
        var text = File.ReadAllLines(path);
        
        var matrix = new bool[text.Length, text.Length];
        for (var i = 0; i < text.Length; i++)
        {
            var line = text[i].Split(';');
            for (var j = 0; j < text.Length; j++)
            {
                matrix[i, j] = line[j] == "1";
            }
        }
        
        AppData.AddRecentFile(path);
        
        Graph = Graph.Build(matrix);
    }

    public static void LoadGraphText(string path)
    {
        var text = File.ReadAllLines(path);
        
        var matrix = new bool[text.Length, text.Length];
        for (var i = 0; i < text.Length; i++)
        {
            var line = text[i].Split(';');
            for (var j = 0; j < text.Length; j++)
            {
                matrix[i, j] = line[j] == "true";
            }
        }
        
        AppData.AddRecentFile(path);
        
        Graph = Graph.Build(matrix);
    }

    public static void DrawGraph(int winWidth, int winHeight)
    {
        if (Graph is null)
        {
            return;
        }
        
        var levels = Graph.Levels.Length;
        
        var levelSpacingH = winHeight / (levels + 1);
        
        //var levelHeight = winHeight / levels;
        var nodeDiameter = levelSpacingH / 3f;

        var nodeCenters = new Dictionary<GraphNode, Vector2>();
        
        for (var i = 0; i < levels; i++)
        {
            var level = Graph.Levels[i];
            var nodeSpacingW = (winWidth - nodeDiameter * level.Count) / (level.Count + 1);
            var j = 0;
            foreach (var node in level.ToImmutableSortedSet(new NodeXPosComparer()))
            {
                var x = nodeSpacingW * (j + 1f);
                var y = (levels - i) * levelSpacingH;
                var nodeX = x + nodeDiameter / 2;
                var nodeY = y - nodeDiameter / 2;
                
                nodeCenters[node] = new Vector2(nodeX, nodeY);
                _nodeXPos[node] = nodeX;
                
                Draw.Circle(nodeX, nodeY, nodeDiameter / 2f, Color.White, 10);
                Draw.TextCentered(Draw.DefaultFont, node.Key.ToString(), new Vector2(nodeX, nodeY), Color.White, nodeDiameter / 50f);
                j++;
            }
        }
        
        foreach (var level in Graph.Levels)
        {
            foreach (var node in level)
            {
                foreach (var child in node.Children)
                {
                    if (child == node)
                    {
                        // TODO: self reference, should do a loop
                        continue;
                    }
                    Draw.Line(nodeCenters[node] + new Vector2(0, nodeDiameter / 2), nodeCenters[child] - new Vector2(0, nodeDiameter / 2), Color.White, 2);
                }
            }
        }

        _graphChanged = false;
    }

    internal static void HandleGraphChange()
    {
        _graphChanged = true;
        _nodeXPos = new Dictionary<GraphNode, float>();
    }
    
    private class NodeXPosComparer : IComparer<GraphNode>
    {
        public int Compare(GraphNode x, GraphNode y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (y is null) return 1;
            if (x is null) return -1;
            
            var sumParX = 0f;
            sumParX = Single.NaN;
            var parC = 0;
            for (; parC < x.Children.Count; parC++)
            {
                var child = x.Children[parC];
                if (child == x)
                {
                    // FIXME: there will be an incorrect division (child count is incremented without children)
                    continue;
                }
                sumParX += _nodeXPos[child];
            }

            var xChildX = sumParX / parC;

            sumParX = parC = 0;
            
            for (; parC < y.Children.Count; parC++)
            {
                var child = y.Children[parC];
                if (child == y)
                {
                    // FIXME: there will be an incorrect division (child count is incremented without children)
                    continue;
                }
                sumParX += _nodeXPos[child];
            }

            return xChildX.CompareTo(sumParX / parC);
        }
    }
}