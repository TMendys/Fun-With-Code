using System.Text;

namespace Bowling;

public class BowlingTable
{
    public const int MaxSize = 10;
    public int Count { get; set; }
    public TableFrame Frame { get; set; }
    public bool TableIsFull { get; set; }
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
        Frame.Add(pins);
        Frame.CountScore();
    }

    public override string ToString()
    {
        StringBuilder sbThrows = new();
        StringBuilder sbScore = new();
        WriteTable(Frame);

        void WriteTable(TableFrame column)
        {
            var firstString = column.FirstThrow == 10 ? "X" : column.FirstThrow.ToString();
            var secondString = column.FirstThrow + column.SecondThrow == 10 ? "/" : column.SecondThrow?.ToString() ?? "_";
            if (column.FirstThrow != 0 && column.SecondThrow == 10)
            {
                secondString = "X";
            }
            var thirdString = column.ThirdThrow == 10 ? "X" : column.ThirdThrow.ToString();
            sbThrows.Append($" {firstString} {secondString} {thirdString}|");

            sbScore.Append($" {column.ColumnScore,3} |");

            if (column.Next is null) { return; }
            WriteTable(column.Next);
        }

        var sThrows = sbThrows.ToString();
        var sScore = sbScore.ToString();

        string result = sThrows[..^1] + Environment.NewLine + sScore[..^1];
        return result;
    }

    public class TableFrame
    {
        enum Throw
        {
            First,
            Second,
            Third,
            NoMoreThrows
        }

        private readonly BowlingTable Table;
        private readonly int index = 0;
        private bool strike = false;
        private bool spare = false;
        private int? tempColumnScore;

        public int? ColumnScore { get; set; }
        public int? FirstThrow { get; set; }
        public int? SecondThrow { get; set; }
        public int? ThirdThrow { get; set; }
        public TableFrame? Next { get; set; }

        public TableFrame(BowlingTable table)
        {
            Table = table;
            Table.Count++;
            index += Table.Count;
        }

        protected TableFrame(int score, BowlingTable table) : this(table)
        {
            Add(score);
        }

        public void Add(int pins)
        {
            Throw @throw = CheckThrow();

            if (Next is not null)
            {
                Next.Add(pins);
            }
            else if (@throw == Throw.NoMoreThrows)
            {
                Next = new(pins, Table);
            }
            else if (@throw == Throw.First)
            {
                FirstThrow = pins;
                strike = FirstThrow == 10;
            }
            else if (@throw == Throw.Second)
            {
                SecondThrow = pins;
                spare = FirstThrow + SecondThrow == 10;
            }
            else if (@throw == Throw.Third)
            {
                ThirdThrow = pins;
            }
            if (index == MaxSize &&
                (ThirdThrow is not null ||
                (SecondThrow is not null && strike is false && spare is false)))
            {
                Table.TableIsFull = true;
            }
        }

        private Throw CheckThrow()
        {
            if (FirstThrow is null) return Throw.First;
            if (SecondThrow is null && strike is false) return Throw.Second;
            if (index == MaxSize)
            {
                if (SecondThrow is null && strike is true) return Throw.Second;
                if (strike is true || spare is true) return Throw.Third;
            }

            return Throw.NoMoreThrows;
        }

        public void CountScore(int? columnScore = null)
        {
            tempColumnScore ??= columnScore;
            ColumnScore ??= AddScore();
            Next?.CountScore(ColumnScore);
        }

        private int? AddScore()
        {
            if (Table.TableIsFull == true && index == MaxSize)
            {
                // Score for last column
                return tempColumnScore + FirstThrow + SecondThrow + (ThirdThrow ?? 0);
            }

            int? score = null;

            if (Next is not null)
            {
                if (strike is true)
                {
                    score = Next.AddFutureScore(2);
                    if (score is null) { return null; }
                }
                else if (spare is true)
                {
                    score = Next.AddFutureScore(1);
                    if (score is null) { return null; }
                }
            }
            else if (strike is true || spare is true || SecondThrow is null) { return null; }

            var columnScore = FirstThrow + (SecondThrow ?? 0);
            score = score is null ? columnScore : score + columnScore;
            score += tempColumnScore ?? 0;
            tempColumnScore = score ?? tempColumnScore;
            return score;
        }

        protected int? AddFutureScore(int throws)
        {
            int? score;
            if (strike is true && throws > 1)
            {
                if (index == MaxSize)
                {
                    score = SecondThrow;
                }
                else
                {
                    score = Next?.AddFutureScore(1);
                }

                if (score is null)
                {
                    return null;
                }
                else
                {
                    score += FirstThrow;
                }
            }
            else if (throws > 1)
            {
                if (SecondThrow is null) { return null; }
                score = FirstThrow + SecondThrow;
            }
            else
            {
                score = FirstThrow;
            }

            return score;
        }
    }
}