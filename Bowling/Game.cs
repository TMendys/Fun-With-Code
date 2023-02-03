using Bowling.Table;

namespace Bowling;

public class Game
{
    public BowlingTable Table { get; set; }

    public Game()
    {
        Table = new();
    }

    public void AddScore(int score) => Table.Add(score);

    public string WriteTable() => Table.ToString();
}