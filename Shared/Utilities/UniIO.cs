namespace BonGames.Tools
{
    using System.IO;

    public static class UniIO
    {
        public const string Tag = "[" + nameof(UniIO) + "]";
        public static readonly char[] PathSeparator = new char[3] { '\\', '/', Path.PathSeparator };

        public static void CreateFolderIfNotExist(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Domain.LogI($"{Tag} Created {folderPath}");
            }
        }

        public static void DeleteFolderIfExist(string folderPath, bool recursive)
        {
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, recursive);
                Domain.LogI($"{Tag} Deleted {folderPath}");
            }
        }

        public static void DeleteFileIfExist(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public static bool RemameFile(string source, string target, bool shouldOverride)
        {
            if (IsFileInSameFolder(source, target))
            {
                if (shouldOverride)
                {
                    DeleteFileIfExist(target);
                }

                File.Move(source, target);
                return true;
            }
            else
            {
                Domain.LogW($"{Tag} Cannot rename the file, make sure the source file name and the target one are in same folder");
                return false;
            }
        }

        public static bool RenameDirectory(string source, string target)
        {
            if (IsFolderInSameFolder(source, target))
            {                
                Directory.Move(source, target);
                return true;
            }
            else
            {
                Domain.LogW($"{Tag} Cannot rename the folder, make sure the source folder name and the target one are in same folder");
                return false;
            }
        }

        public static string[] SplitPath(string path)
        {
            int elementCount = 0;
            string trimPath = path.Trim().Trim(PathSeparator);
            for (int i = 0; i < trimPath.Length; i++)
            {
                char c = trimPath[i];
                if (c == '\\' || c == '/' || c == Path.PathSeparator)
                {
                    elementCount++;
                }
            }
            if (elementCount > 0)
            {
                string[] splitedPath = new string[elementCount + 1];

                int elIndex = 0;
                string currentEl = string.Empty;

                for (int i = 0; i < trimPath.Length; i++)
                {
                    char c = trimPath[i];
                    if (c.IsPathSeparator())
                    {
                        splitedPath[elIndex] = currentEl;
                        currentEl = string.Empty;
                        elIndex++;
                    }
                    else
                    {
                        currentEl += c;
                    }
                }
                return splitedPath;
            }
            else
            {
                return new string[] { trimPath };
            }
        }

        public static bool IsPathSeparator(this char c)
        {
            for (int i = 0; i < PathSeparator.Length; i++)
            {
                if (c == PathSeparator[i])
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsFileInSameFolder(string sourcePath, string targetPath)
        {
            string[] splitedSource = SplitPath(sourcePath);
            string[] splitedTarget = SplitPath(targetPath);

            if (splitedSource.Length == splitedTarget.Length)
            {
                int length = splitedSource.Length - 1;
                for (int i = 0; i < length; i++)
                {
                    if (splitedSource[i] != splitedTarget[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static bool IsFolderInSameFolder(string sourcePath, string targetPath)
        {
            return IsFileInSameFolder(sourcePath, targetPath);
        }
    }
}
