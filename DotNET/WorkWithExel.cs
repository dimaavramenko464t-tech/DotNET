using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace DotNET
{
    internal class WorkWithExel
    {
        private SpreadsheetDocument? document;

        public void OpenExel(string path)
        {
            if (File.Exists(path))
                File.Delete(path);

            document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook);
            WorkbookPart workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();
            workbookPart.Workbook.AppendChild(new Sheets());
        }

        public void WriteGroupedTables(IReadOnlyList<WordTable> groups)
        {
            SpreadsheetDocument spreadsheet = document
                ?? throw new InvalidOperationException("Excel-файл не открыт.");
            WorkbookPart workbookPart = spreadsheet.WorkbookPart
                ?? throw new InvalidOperationException("Excel-файл не открыт.");
            Workbook workbook = workbookPart.Workbook
                ?? throw new InvalidOperationException("Excel-файл не открыт.");
            Sheets sheets = workbook.GetFirstChild<Sheets>() ?? workbook.AppendChild(new Sheets());
            HashSet<string> usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            uint sheetId = 1;

            foreach (WordTable group in groups)
            {
                string sheetName = UniqueSheetName(group.Header, usedNames);

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                SheetData sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                uint rowIndex = 1;
                if (group.Header.Count > 0)
                {
                    sheetData.Append(CreateRow(group.Header, rowIndex));
                    rowIndex++;
                }

                foreach (SubRow dataRow in group.DataRows)
                {
                    sheetData.Append(CreateRow(dataRow.Texts, rowIndex));
                    rowIndex++;
                }

                sheets.Append(new Sheet
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = sheetId,
                    Name = sheetName
                });

                sheetId++;
            }

            workbook.Save();
        }

        public void Close()
        {
            document?.Dispose();
            document = null;
        }

        private static Row CreateRow(IReadOnlyList<string> cells, uint rowIndex)
        {
            Row row = new Row { RowIndex = rowIndex };

            for (int i = 0; i < cells.Count; i++)
            {
                string reference = $"{ColumnName(i + 1)}{rowIndex}";
                Cell cell = new Cell
                {
                    CellReference = reference,
                    DataType = CellValues.InlineString,
                    InlineString = new InlineString(new Text(cells[i] ?? string.Empty)
                    {
                        Space = SpaceProcessingModeValues.Preserve
                    })
                };
                row.Append(cell);
            }

            return row;
        }

        private static string UniqueSheetName(IReadOnlyList<string> header, HashSet<string> usedNames)
        {
            string baseName = SanitizeSheetName(header.Count > 0 ? header[0] : "Sheet");
            string name = baseName;
            int suffix = 2;

            while (!usedNames.Add(name))
            {
                string suffixText = $"_{suffix}";
                int maxBase = Math.Max(1, 31 - suffixText.Length);
                name = (baseName.Length > maxBase ? baseName[..maxBase] : baseName) + suffixText;
                suffix++;
            }

            return name;
        }

        private static string SanitizeSheetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Sheet";

            char[] invalid = { '\\', '/', '*', '?', ':', '[', ']' };
            StringBuilder builder = new StringBuilder();

            foreach (char c in name.Trim())
            {
                if (Array.IndexOf(invalid, c) < 0)
                    builder.Append(c);
            }

            string result = builder.ToString().Trim();
            if (result.Length == 0)
                result = "Sheet";
            if (result.Length > 31)
                result = result[..31];

            return result;
        }

        private static string ColumnName(int index)
        {
            string name = string.Empty;
            while (index > 0)
            {
                index--;
                name = (char)('A' + index % 26) + name;
                index /= 26;
            }

            return name;
        }
    }
}
