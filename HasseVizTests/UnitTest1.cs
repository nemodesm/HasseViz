using HasseVizLib;

namespace HasseVizTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Build()
    {
        var g = Graph.Build(new[,]
        {
            { false, false, false },
            { true, true, false },
            { true, true, true }
        });

        //Assert.AreEqual("{\n    1: ,\n    2: 0, 1\n}", g.ToString());
        
        const int size = 10000;
        var matrix = new bool[size, size];
        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                matrix[i, j] = Random.Shared.Next() % 2 == 0 && i <= j;
            }
        }
        
        Console.WriteLine("Built matrix");
        var start = DateTime.Now;
        var g2 = Graph.Build(matrix);
        var end = DateTime.Now;
        Console.WriteLine($"Time: {end - start}");
        Assert.That(g2.ToString(), Is.EqualTo(""));
    }
}