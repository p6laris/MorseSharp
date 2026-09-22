namespace MorseSharp;

/// <summary>
/// Makes up a contact between two stations to practise on.
/// </summary>
/// <remarks>
/// <para>
/// Copying random groups perfectly and then freezing on a real contact is the usual way of coming unstuck, because
/// on-air traffic is full of things random letters never teach: callsigns, abbreviations, signal reports, and the
/// procedural signals that hold it together. This produces a whole contact in that shape.
/// </para>
/// <para>
/// Every transmission is plain text the library can key as it stands, including the <c>=</c> separator and the
/// <c>&lt;SK&gt;</c> that closes a contact, both of which are keyed as single unbroken signals.
/// </para>
/// </remarks>
public static class Qso
{
    private static readonly string[] Names =
    [
        "JOHN", "BOB", "MIKE", "ANNA", "PETER", "MARIA", "SAM", "LUIS", "HANS", "YUKI", "OMAR", "LENA",
        "KOZHEN", "HANAR", "PAMO", "ARAM", "ROJIN", "DILAN", "RONAK", "ZANA", "KAWA", "HIWA", "SHILAN", "KARWAN",
    ];

    private static readonly string[] Places =
        ["LONDON", "BOSTON", "MUNICH", "TOKYO", "MADRID", "OSLO", "CAIRO", "LIMA", "ERBIL", "PRAGUE", "PERTH", "DELHI"];

    private static readonly string[] Reports =
        ["599", "579", "569", "559", "479", "339"];

    private static readonly string[] Greetings =
        ["GM", "GA", "GE"];

    /// <summary>
    /// Returns one contact, as the transmissions that make it up in order. Each one is ready to pass to
    /// <c>ToMorse</c> on its own.
    /// </summary>
    /// <param name="random">Source of randomness; pass a seeded one to get the same contact every time.</param>
    public static string[] Generate(Random? random = null)
    {
        Random source = random ?? Random.Shared;

        string caller = Callsign.Next(source);
        string answerer = Callsign.Next(source);
        while (answerer == caller)
            answerer = Callsign.Next(source);

        string greeting = Pick(Greetings, source);

        return
        [
            $"CQ CQ CQ DE {caller} {caller} {caller} K",
            $"{caller} DE {answerer} {answerer} K",
            $"{answerer} DE {caller} = {greeting} = TNX FER CALL = UR RST {Pick(Reports, source)} = NAME {Pick(Names, source)} = QTH {Pick(Places, source)} = HW? = {answerer} DE {caller} K",
            $"{caller} DE {answerer} = {greeting} = UR RST {Pick(Reports, source)} = NAME {Pick(Names, source)} = QTH {Pick(Places, source)} = {caller} DE {answerer} K",
            $"{answerer} DE {caller} = TNX FER QSO = 73 ES CUL = {answerer} DE {caller} <SK>",
        ];
    }

    private static string Pick(string[] options, Random random) => options[random.Next(options.Length)];
}
