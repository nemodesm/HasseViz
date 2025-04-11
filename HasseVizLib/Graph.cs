using System.Collections.Immutable;

namespace HasseVizLib;

public class Graph
{
    private HashSet<GraphNode>[] _levels;
    
    public HashSet<GraphNode>[] Levels => _levels;

    /// <summary>
    /// Removes instances o a->b and a->c and b->c, and simplify them to a->b and b->c
    /// </summary>
    public void Simplify()
    {
        // direct visibility
        var visibility = new Dictionary<GraphNode, HashSet<GraphNode>>();
        for (var i = 0; i < _levels.Length; i++)
        {
            var level = _levels[i];
            foreach (var node in level)
            {
                visibility[node] = node.Children.ToHashSet();
            }
        }
        
        // extended visibility
        bool added;
        do
        {
            added = false;
            foreach (var visibilitySet in visibility)
            {
                var toAdd = new HashSet<GraphNode>();
                foreach (var visibleNode in visibilitySet.Value)
                {
                    var nodeVisibility = visibility[visibleNode];
                    foreach (var visibleVisibleNode in nodeVisibility)
                    {
                        if (!visibilitySet.Value.Contains(visibleVisibleNode))
                        {
                            added = true;
                            toAdd.Add(visibleVisibleNode);
                        }
                    }
                }
                visibilitySet.Value.UnionWith(toAdd);
            }
        } while (added);

        // remove children
        foreach (var level in _levels)
        {
            foreach (var node in level)
            {
                for (int i = 0; i < node.Children.Count; i++)
                {
                    var child = node.Children[i];

                    if (child == node)
                    {
                        continue;
                    }

                    foreach (var childVisible in visibility[child])
                    {
                        if (childVisible != child && node.Children.Contains(childVisible))
                        {
                            node.Children.Remove(childVisible);
                        }
                    }
                }
            }
        }
    }
    
    public static Graph Build(bool[,] matrix)
    {
        var graph = new Graph();
        var size = matrix.GetLength(0);
        GraphNode[] unsorted = new GraphNode[size];
        for (var i = 0; i < size; i++)
        {
            unsorted[i] = new GraphNode(i+1);
        }

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (matrix[i, j])
                {
                    unsorted[i].Children.Add(unsorted[j]);
                }
            }
        }
        
        graph._levels = SortLevels(unsorted);
        
        return graph;
    }
    
    public static Graph BuildSimplified(bool[,] matrix)
    {
        var graph = Build(matrix);
        graph.Simplify();
        return graph;
    }

    private static HashSet<GraphNode>[] SortLevels(GraphNode[] unsorted)
    {
        var levels = new List<HashSet<GraphNode>>();
        var levelsDict = new Dictionary<int, int>(); // {node key -> level}
        var viewed = new HashSet<GraphNode>();
        foreach (var node in unsorted)
        {
            if (!levelsDict.ContainsKey(node.IndexKey))
            {
                SortNode(node);
            }
        }
        
        for (var i = 0; i < unsorted.Length; i++)
        {
            var level = levelsDict[unsorted[i].IndexKey];
            if (levels.Count <= level)
            {
                var missing = level - levels.Count + 1;
                for (var j = 0; j < missing; j++)
                {
                    levels.Add(new HashSet<GraphNode>());
                }
            }
            levels[level].Add(unsorted[i]);
        }

        return levels.ToArray();
        
        void SortNode(GraphNode node)
        {
            if (viewed.Contains(node))
            {
                throw new Exception("Cycle detected");
            }
            viewed.Add(node);
            
            var placedOn = 0;
            foreach (var child in node.Children)
            {
                if (child.IndexKey == node.IndexKey)
                {
                    // related to self
                    continue;
                }
                if (!levelsDict.ContainsKey(child.IndexKey))
                {
                    SortNode(child);
                }
                placedOn = Math.Max(placedOn, levelsDict[child.IndexKey] + 1);
            }

            levelsDict[node.IndexKey] = placedOn;
        }
    }

    public void SaveToTGF(string path)
    {
        using (var file = File.CreateText(path))
        {
            foreach (var level in _levels)
            {
                foreach (var node in level)
                {
                    file.WriteLine(node.Key);
                }
            }
            file.WriteLine("#");
            foreach (var level in _levels)
            {
                foreach (var node in level)
                {
                    foreach (var child in node.Children)
                    {
                        file.WriteLine($"{node.Key} {child.Key}");
                    }
                }
            }
        }
    }

    public override string ToString()
    {
        return $"{{\n    {string.Join(",\n    ", _levels.SelectMany(l => l))}\n}}";
    }

    private class NodeComparer : IComparer<GraphNode>
    {
        public int Compare(GraphNode? x, GraphNode? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (y is null) return 1;
            if (x is null) return -1;
            return x.Children.Count.CompareTo(y.Children.Count);
        }
    }
}