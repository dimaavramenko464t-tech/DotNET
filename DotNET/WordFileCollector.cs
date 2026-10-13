using System.IO.Compression;
using System.Runtime.CompilerServices;

namespace DotNET
{
    public static class WordFileCollector
    {
        public static List<string> Collect(string path, bool recursive, out string text)
        {
            text = string.Empty;
            List<string> candidates;

            if (File.Exists(path))
            {
                candidates = new List<string> { path };
            }
            else if (Directory.Exists(path))
            {
                SearchOption option = recursive
                    ? SearchOption.AllDirectories
                    : SearchOption.TopDirectoryOnly;

                candidates = Directory
                    .EnumerateFiles(path, "*.docx", option)
                    .Where(f => !Path.GetFileName(f).StartsWith("~$"))
                    .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            else
            {
                throw new FileNotFoundException($"Путь не найден: {path}");
            }

            var workingFiles = new List<string>();
            var failedFiles = new List<string>();

            foreach (string file in candidates)
            {
                if (TryOpenFile(file, out string error))
                    workingFiles.Add(file);
                else
                    failedFiles.Add($"{file}{Environment.NewLine}    Причина: {error}");
            }

            if (failedFiles.Count > 0)
            {
                const int maxShown = 15;

                text =
                    $"Не удалось открыть файлов: {failedFiles.Count}" + Environment.NewLine +
                    $"Успешно найдено: {workingFiles.Count}" + Environment.NewLine + Environment.NewLine +
                    string.Join(Environment.NewLine + Environment.NewLine,
                        failedFiles.Take(maxShown).Select((f, i) => $"{i + 1}. {f}"));

                if (failedFiles.Count > maxShown)
                    text += Environment.NewLine + Environment.NewLine +
                            $"... и ещё {failedFiles.Count - maxShown} файл(ов).";
            }

            return workingFiles;
        }

        private static bool TryOpenFile(string file, out string error)
        {
            error = string.Empty;

            try
            {
                using var stream = new FileStream(
                    file,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite);

                using var archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: false);
                if (archive.GetEntry("word/document.xml") == null)
                {
                    error = "Файл не является корректным документом Word (.docx).";
                    return false;
                }

                return true;
            }
            catch (InvalidDataException)
            {
                error = "Файл повреждён или не является docx.";
            }
            catch (UnauthorizedAccessException)
            {
                error = "Нет доступа к файлу.";
            }
            catch (IOException ex)
            {
                error = $"Файл занят другим процессом или недоступен ({ex.Message}).";
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return false;
        }
    }
}