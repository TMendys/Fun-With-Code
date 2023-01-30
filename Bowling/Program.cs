using Bowling;

Game game = new();

List<int> scores = new() { 1, 4, 4, 5, 6, 4, 5, 5, 10, 0, 1, 7, 3, 6, 4, 10, 2, 8, 6 }; // 133
List<int> scores2 = new() { 1, 4, 4, 5, 6, 4, 5, 5, 10, 0, 0, 7, 3, 6, 4, 10, 2, 8, 6 }; // 131
List<int> scores3 = new() { 1, 4, 4, 5, 6, 4, 5, 5, 10, 0, 0, 7, 3, 6, 4, 10, 2, 7, 6 }; // Last 6 is ilegal, 123
List<int> scores4 = new() { 10, 9, 1, 10, 9, 1, 9, 1, 10, 10, 9, 1, 9, 1, 10, 5, 4 }; // 206
List<int> strikes = new() { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; // 300
List<int> strikes2 = new() { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 9 }; // Too manny, 300

foreach (var score in scores4)
{
    game.AddScore(score);
}

//Console.WriteLine(game.WriteTable());

foreach (var item in game.Table.Throws)
{
    Console.Write(item + " ");
}
Console.WriteLine();
foreach (var item in game.Table.ScorePerFrame)
{
    Console.Write(item + " ");
}