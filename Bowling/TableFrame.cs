namespace Bowling.Table;

internal class TableFrame
{
    enum Throw
    {
        First,
        Second,
        Third,
        NoMoreThrows
    }

    private readonly BowlingTable table;

    public bool Strike { get; set; }
    public bool Spare { get; set; }
    public int Index { get; set; }
    public int? FirstThrow { get; set; }
    public int? SecondThrow { get; set; }
    public int? ThirdThrow { get; set; }
    public TableFrame? Next { get; set; }

    public TableFrame(BowlingTable table)
    {
        this.table = table;
        this.table.Count++;
        Index += this.table.Count - 1;
    }

    protected TableFrame(int pins, BowlingTable table) : this(table)
    {
        Add(pins);
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
            Next = new(pins, table);
        }
        else if (@throw == Throw.First)
        {
            FirstThrow = pins;
            Strike = FirstThrow == 10;
        }
        else if (@throw == Throw.Second)
        {
            SecondThrow = pins;
            Spare = FirstThrow + SecondThrow == 10;
        }
        else if (@throw == Throw.Third)
        {
            ThirdThrow = pins;
        }
        // Game over check
        if (Index + 1 == BowlingTable.MaxSize &&
            (ThirdThrow is not null ||
            (SecondThrow is not null && Strike is false && Spare is false)))
        {
            table.TableIsFull = true;
        }
    }

    private Throw CheckThrow()
    {
        if (FirstThrow is null) return Throw.First;
        if (SecondThrow is null && Strike is false) return Throw.Second;
        if (Index + 1 == BowlingTable.MaxSize)
        {
            if (SecondThrow is null && Strike is true) return Throw.Second;
            if (Strike is true || Spare is true) return Throw.Third;
        }

        return Throw.NoMoreThrows;
    }
}
