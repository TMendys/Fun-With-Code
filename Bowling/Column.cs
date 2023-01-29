using System.Text;

namespace Bowling;

public class Table
{
    public const int MaxSize = 10;
    public int Count { get; set; }
    public Frame Column { get; set; }
    public Table()
    {
        Column = new(this);
    }

    public void Add(int score) => Column.Add(score);

    public override string ToString()
    {
        StringBuilder sbThrows = new();
        StringBuilder sbScore = new();
        WriteTable(Column);

        void WriteTable(Frame column)
        {
            string? firstString = column.FirstThrow == 10 ? "X" : column.FirstThrow.ToString();
            string secondString = column.FirstThrow + column.SecondThrow == 10 ? "/" : column.SecondThrow?.ToString() ?? "_";
            secondString = column.FirstThrow + column.SecondThrow > 10 ? "X" : secondString;
            string? thirdString = column.ThirdThrow == 10 ? "X" : column.ThirdThrow.ToString();
            sbThrows.Append($"{firstString} {secondString} {thirdString}");

            sbScore.Append($"{column.ColumnScore,3} ");

            if (column.Next is null) { return; }
            WriteTable(column.Next);
        }

        string result = sbThrows.ToString() + Environment.NewLine + sbScore.ToString();
        return result;
    }

    public class Frame
    {
        private readonly Table Table;
        private readonly int order = 0;
        private bool strike = false;
        private bool spare = false;
        private int? tempColumnScore;

        public int? ColumnScore { get; set; }
        public int? FirstThrow { get; set; }
        public int? SecondThrow { get; set; }
        public int? ThirdThrow { get; set; }
        public Frame? Next { get; set; }

        public Frame(Table table)
        {
            this.Table = table;
            Table.Count++;
            order += Table.Count;
        }
        protected Frame(int score, int? columnScore, Table table) : this(table)
        {
            tempColumnScore = columnScore;
            Add(score);
        }

        public void Add(int score)
        {
            this.Add(score, ColumnScore);
        }

        protected void Add(int score, int? columnScore)
        {
            tempColumnScore ??= columnScore;
            ColumnScore ??= CountScore();

            if (Next is not null)
            {
                Next.Add(score, ColumnScore);
            }
            else if (FirstThrow is null)
            {
                FirstThrow = score;
                strike = FirstThrow == 10;
            }
            else if (strike is false && SecondThrow is null)
            {
                SecondThrow = score;
                spare = FirstThrow + SecondThrow == 10;
            }
            else if (Table.Count == MaxSize)
            {
                if (strike is true && SecondThrow is null)
                {
                    SecondThrow = score;
                }
                else if (strike is true || spare is true)
                {
                    ThirdThrow = score;
                }
                if (ThirdThrow is not null || (SecondThrow is not null && strike is false && spare is false))
                {
                    ColumnScore = tempColumnScore + FirstThrow + SecondThrow + (ThirdThrow ?? 0);
                }
            }
            else
            {
                Next = new(score, ColumnScore, Table);
            }
        }

        private int? CountScore()
        {
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
                if (order == MaxSize)
                {
                    score = FirstThrow;
                }
                else
                {
                    score = Next?.AddFutureScore(1);
                }
                if (score is null) { return null; }
                else { score += FirstThrow; }
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