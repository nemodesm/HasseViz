namespace HasseVizLib;

public class GraphNode
{
    public int Key; 
    public int IndexKey => Key - 1;
    private List<GraphNode> _children;
    
    public List<GraphNode> Children => _children;

    public GraphNode(int key, params GraphNode[] children)
    {
        Key = key;
        _children = children.ToList();
    }
    
    public override string ToString()
    {
        return $"{Key}: {string.Join(", ", _children.Select(c => c.Key))}";
    }
}