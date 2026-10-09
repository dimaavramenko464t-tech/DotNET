using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DotNET
{
    public class Programm
    {
        public static void Main(string[] args)
        {
            string wordPath = @"C:\Users\User\Desktop\бИПТ-251_Авраменко_КП.docx";
            string excelPath = Path.ChangeExtension(wordPath, ".xlsx");

            Programm pr = new Programm();
            List<WordTable> tables = pr.WordTableParse(wordPath);
            List<WordTable> grouped = pr.GroupByHeader(tables);

            WorkWithExel excel = new WorkWithExel();
            excel.OpenExel(excelPath);
            excel.WriteGroupedTables(grouped);
            excel.Close();

            Console.WriteLine($"Записано: {excelPath}");
        }

        private List<WordTable> WordTableParse(string path)
        {
            List<WordTable> tables = new List<WordTable>();

            using WordprocessingDocument document = WordprocessingDocument.Open(path, false);
            MainDocumentPart mainPart = document.MainDocumentPart
                ?? throw new InvalidOperationException("В документе Word нет основной части.");
            Body body = mainPart.Document?.Body
                ?? throw new InvalidOperationException("В документе Word нет тела.");
            IEnumerable<Table> wordTables = body.Elements<Table>();

            foreach (Table table in wordTables)
            {
                WordTable parsed = ParseTable(table);
                if (parsed.Header.Count == 0 && parsed.DataRows.Count == 0)
                    continue;

                tables.Add(parsed);
            }

            return tables;
        }

        private static WordTable ParseTable(Table table)
        {
            WordTable result = new WordTable();
            bool headerRead = false;

            foreach (TableRow row in table.Elements<TableRow>())
            {
                SubRow subRow = new SubRow();
                foreach (TableCell cell in row.Elements<TableCell>())
                {
                    subRow.Add(cell.InnerText);
                }

                if (subRow.Texts.Count == 0)
                    continue;

                if (!headerRead)
                {
                    result.Header.AddRange(subRow.Texts);
                    headerRead = true;
                    continue;
                }

                result.DataRows.Add(subRow);
            }

            return result;
        }

        private List<WordTable> GroupByHeader(List<WordTable> tables)
        {
            List<WordTable> groups = new List<WordTable>();

            foreach (WordTable table in tables)
            {
                WordTable? existing = groups.FirstOrDefault(g => HeadersEqual(g.Header, table.Header));
                if (existing != null)
                {
                    existing.DataRows.AddRange(table.DataRows);
                    continue;
                }

                groups.Add(table);
            }

            return groups;
        }

        internal static bool HeadersEqual(IReadOnlyList<string> a, IReadOnlyList<string> b)
        {
            if (a.Count != b.Count)
                return false;

            for (int i = 0; i < a.Count; i++)
            {
                if (!string.Equals(a[i].Trim(), b[i].Trim(), StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }
    }

    internal class WordTable
    {
        public List<string> Header { get; } = new List<string>();
        public List<SubRow> DataRows { get; } = new List<SubRow>();
    }

    internal class SubRow
    {
        private readonly List<string> innerTexts = new List<string>();

        internal IReadOnlyList<string> Texts => innerTexts;

        internal void Add(string text)
        {
            innerTexts.Add(text);
        }

        internal void PrintTexts()
        {
            foreach (string text in innerTexts)
                Console.WriteLine(text);
        }
    }
}
