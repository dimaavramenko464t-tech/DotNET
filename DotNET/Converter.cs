using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DotNET
{
    public class Converter
    {
        private static readonly string LogPath =
            Path.Combine(AppContext.BaseDirectory, "errors.log");

        private static int errorCount;
        private static string? logWriteError;
        private static string stage = "Запуск";

        private static void LogError(string message)
        {
            errorCount++;

            try
            {
                File.AppendAllText(
                    LogPath,
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                logWriteError = ex.Message;
            }
        }

        private static void ReportLog()
        {
            if (errorCount == 0)
                return;

            if (logWriteError == null)
                Console.WriteLine($"Ошибок: {errorCount}. Лог сохранён: {LogPath}");
            else
                Console.WriteLine($"Ошибок: {errorCount}. Не удалось сохранить лог {LogPath}: {logWriteError}");
        }

        public static string Main(List<string> word,string excel)
        {
            errorCount = 0;
            logWriteError = null;
            stage = "Запуск";

            try
            {
                Execute(word,excel);
            }
            catch (Exception ex)
            {
                LogError($"Критическая ошибка на этапе [{stage}] ({ex.GetType().Name}): {ex.Message}");
            }
            finally
            {
                ReportLog();
            }
            return errorCount == 0 ? "Success" : "Error";
        }

        private static void Execute(List<string> files, string excelPath)
        {
            Converter pr = new Converter();

            if (files.Count == 0)
            {
                LogError($"Ошибка на этапе [{stage}]: Word-файлы не найдены");
                return;
            }

            List<WordTable> tables = new List<WordTable>();
            foreach (string file in files)
            {
                stage = $"Чтение Word-файла: {file}";
                try
                {
                    tables.AddRange(pr.WordTableParse(file));
                    Console.WriteLine($"Обработан: {file}");
                }
                catch (Exception ex)
                {
                    LogError($"Ошибка на этапе [{stage}] ({ex.GetType().Name}): {ex.Message}");
                }
            }

            stage = "Группировка таблиц по заголовкам";
            List<WordTable> grouped = pr.GroupByHeader(tables);

            WorkWithExel excel = new WorkWithExel();

            stage = $"Создание Excel-файла: {excelPath}";
            excel.OpenExel(excelPath);

            stage = $"Запись таблиц в Excel: {excelPath}";
            excel.WriteGroupedTables(grouped);

            stage = $"Закрытие Excel-файла: {excelPath}";
            excel.Close();

            stage = "Завершение";
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