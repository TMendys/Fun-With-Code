namespace Bowling;

public class Game
{
    public Table Table { get; set; }
    public Game()
    {
        Table = new();
    }

    public void AddScore(int score) => Table.Add(score);

    public string WriteTable() => Table.ToString();
}