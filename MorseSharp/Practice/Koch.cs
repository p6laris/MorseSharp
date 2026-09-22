namespace MorseSharp;

/// <summary>
/// Lessons for learning to receive Morse by the Koch method.
/// </summary>
/// <remarks>
/// <para>
/// The method is to send at the speed you want to end up at from the very first lesson, and to start with only two
/// characters. Once you can copy those at better than ninety per cent, a third is added, and so on. Learning slowly
/// and speeding up later teaches you to count the dots, which is a habit that then has to be unlearned; keeping the
/// speed fixed and growing the alphabet instead teaches the sound of each character straight away.
/// </para>
/// <para>
/// <see cref="Order"/> is the sequence the characters are introduced in. It is deliberately not alphabetical: the
/// first pair is chosen to sound nothing like each other, and easily confused characters are kept apart.
/// </para>
/// </remarks>
public static class Koch
{
    /// <summary>The accuracy a lesson must reach before the next character is added.</summary>
    public const double Threshold = 0.9;

    /// <summary>Characters in the order the method introduces them.</summary>
    public static ReadOnlySpan<char> Order => "KMRSUAPTLOWI.NJEF0Y,VG5/Q9ZH38B?427C1D6X";

    /// <summary>The highest level there is, which is every character in <see cref="Order"/>.</summary>
    public static int MaxLevel => Order.Length;

    /// <summary>How many characters a lesson of this shape will be, counting the spaces between groups.</summary>
    /// <param name="groups">Number of groups.</param>
    /// <param name="groupSize">Characters per group.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when either argument is not positive.</exception>
    public static int LengthFor(int groups, int groupSize = 5)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(groups);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(groupSize);
        return (groups * groupSize) + groups - 1;
    }

    /// <summary>
    /// Writes a lesson into <paramref name="destination"/> and returns how many characters it wrote.
    /// </summary>
    /// <param name="destination">Buffer of at least <see cref="LengthFor"/> characters.</param>
    /// <param name="level">How many characters of <see cref="Order"/> to draw from, from 1 to <see cref="MaxLevel"/>.</param>
    /// <param name="groups">Number of groups to write.</param>
    /// <param name="groupSize">Characters per group.</param>
    /// <param name="random">Source of randomness; pass a seeded one for a repeatable lesson.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="level"/> is outside 1 to <see cref="MaxLevel"/>, or a count is not positive.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="destination"/> is too short.</exception>
    public static int Generate(Span<char> destination, int level, int groups, int groupSize = 5, Random? random = null)
    {
        int length = Validate(level, groups, groupSize);
        if (destination.Length < length)
            throw new ArgumentException($"A lesson of {groups} groups of {groupSize} needs {length} characters, but only {destination.Length} were given.", nameof(destination));

        Write(destination, level, groups, groupSize, random ?? Random.Shared);
        return length;
    }

    /// <summary>
    /// Returns a lesson: <paramref name="groups"/> random groups drawn from the first <paramref name="level"/>
    /// characters, separated by single spaces.
    /// </summary>
    /// <inheritdoc cref="Generate(Span{char}, int, int, int, Random)" path="/param"/>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="level"/> is outside 1 to <see cref="MaxLevel"/>, or a count is not positive.</exception>
    public static string Generate(int level, int groups, int groupSize = 5, Random? random = null)
    {
        int length = Validate(level, groups, groupSize);

        return string.Create(length, (Level: level, Groups: groups, Size: groupSize, Random: random ?? Random.Shared),
            static (span, state) => Write(span, state.Level, state.Groups, state.Size, state.Random));
    }

    /// <summary>
    /// Compares what was copied against what was sent, one character at a time.
    /// </summary>
    /// <remarks>
    /// Spacing is ignored on both sides, since the groups are only there to make a lesson readable, and the comparison
    /// is case-insensitive. It is strictly positional: a dropped character shifts everything after it, which counts
    /// against you, and is meant to.
    /// </remarks>
    /// <param name="sent">The lesson as it was sent.</param>
    /// <param name="copied">What the learner wrote down.</param>
    public static KochScore Score(ReadOnlySpan<char> sent, ReadOnlySpan<char> copied)
    {
        int correct = 0;
        int total = 0;
        int read = 0;

        for (int i = 0; i < sent.Length; i++)
        {
            if (sent[i] == ' ')
                continue;

            while (read < copied.Length && copied[read] == ' ')
                read++;

            total++;
            if (read < copied.Length && char.ToUpperInvariant(copied[read]) == char.ToUpperInvariant(sent[i]))
                correct++;

            read++;
        }

        return new KochScore(correct, total);
    }

    private static int Validate(int level, int groups, int groupSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(level, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(level, MaxLevel);
        return LengthFor(groups, groupSize);
    }

    private static void Write(Span<char> destination, int level, int groups, int groupSize, Random random)
    {
        ReadOnlySpan<char> pool = Order[..level];
        int written = 0;

        for (int group = 0; group < groups; group++)
        {
            if (group > 0)
                destination[written++] = ' ';

            for (int i = 0; i < groupSize; i++)
                destination[written++] = pool[random.Next(pool.Length)];
        }
    }
}
