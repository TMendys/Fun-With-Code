namespace Bowling.Table;

internal static class Scorer
{
    internal static List<int?> CountScore(BowlingTable table)
    {
        List<int?> scorePerFrame = new();

        NestedCountScore(table, table.Frame);
        void NestedCountScore(BowlingTable table, TableFrame frame)
        {
            int LastScore = scorePerFrame.LastOrDefault() ?? 0;
            int? frameScore = AddScore(table, frame);
            scorePerFrame.Add(LastScore + frameScore);
            if (frame.Next is not null && frameScore is not null)
            {
                NestedCountScore(table, frame.Next);
            }
        }

        return scorePerFrame;
    }

    private static int? AddScore(BowlingTable table, TableFrame frame)
    {
        int? score = 0;

        // Score for the last column.
        if (table.TableIsFull == true && frame.Index + 1 == BowlingTable.MaxSize)
        {
            score = frame.TotalFrameScore;
            return score;
        }

        // If strike or spare, add the future score if possible.
        if (frame.Next is not null)
        {
            if (frame.Strike is true)
            {
                score = AddFutureScore(frame.Next, 2);
                if (score is null) { return null; }
            }
            else if (frame.Spare is true)
            {
                score = AddFutureScore(frame.Next, 1);
                if (score is null) { return null; }
            }
        }
        else if (frame.Strike is true || frame.Spare is true || frame.SecondThrow is null) { return null; }

        var columnScore = frame.TotalFrameScore;
        score += columnScore;
        return score;
    }

    private static int? AddFutureScore(TableFrame frame, int throws)
    {
        int? score = null;
        if (frame.Strike is true && throws == 2)
        {
            if (frame.Index + 1 == BowlingTable.MaxSize)
            {
                score = frame.SecondThrow;
            }
            else if (frame.Next is not null)
            {
                score = AddFutureScore(frame.Next, 1);
                if (score is null) { return null; }
            }

            score += frame.FirstThrow;
        }
        else if (throws == 2)
        {
            if (frame.SecondThrow is null) { return null; }
            score = frame.FirstThrow + frame.SecondThrow;
        }
        else
        {
            score = frame.FirstThrow;
        }

        return score;
    }
}