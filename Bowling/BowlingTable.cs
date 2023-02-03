using System.Text;

namespace Bowling.Table;

public class BowlingTable
{
    public const int MaxSize = 10;
    public int Count { get; internal set; }
    public bool TableIsFull { get; internal set; }
    internal TableFrame Frame { get; private set; }
    public List<int?> Throws { get; private set; } = new();
    public List<int?> ScorePerFrame { get; private set; } = new();

    public BowlingTable()
    {
        TableIsFull = false;
        Frame = new(this);
    }

    public void Add(int pins)
    {
        if (TableIsFull is true)
        {
            return;
        }
        if (pins < 0 || pins > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(pins), pins, "Have to be a number between 0 and 10.");
        }
        Throws.Add(pins);
        Frame.Add(pins);
        ScorePerFrame = Scorer.CountScore(this);
    }

    public override string ToString()
    {
        StringBuilder sbThrows = new();
        StringBuilder sbScore = new();
        WriteTable(Frame);

        void WriteTable(TableFrame frame)
        {
            var firstString = frame.FirstThrow == 10 ? "X" : frame.FirstThrow.ToString();
            var secondString = frame.FirstThrow + frame.SecondThrow == 10 ? "/" : frame.SecondThrow?.ToString() ?? "_";
            if (frame.FirstThrow != 0 && frame.SecondThrow == 10)
            {
                secondString = "X";
            }
            var thirdString = frame.ThirdThrow == 10 ? "X" : frame.ThirdThrow.ToString();
            sbThrows.Append($" {firstString} {secondString} {thirdString}|");

            sbScore.Append($" {ScorePerFrame.ElementAtOrDefault(frame.Index),3} |");

            if (frame.Next is null) { return; }
            WriteTable(frame.Next);
        }

        var sThrows = sbThrows.ToString();
        var sScore = sbScore.ToString();

        string result = sThrows[..^1] + Environment.NewLine + sScore[..^1];
        return result;
    }
}