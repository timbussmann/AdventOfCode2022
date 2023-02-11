namespace AdventOfCode22;

public class Day7
{
    [Fact]
    public async Task Part1()
    {
        var root = await ParseFileSystem();

        int total = 0;
        CountSmallFolders(root, ref total);

        Assert.Equal(1582412, total);

        void CountSmallFolders(Folder folder, ref int total)
        {
            foreach (var subfolder in folder.Subfolders)
            {
                CountSmallFolders(subfolder.Value, ref total);
            }

            if (folder.DirectorySize <= 100_000)
            {
                total += folder.DirectorySize;
            }
        }
    }

    [Fact]
    public async Task Part2()
    {
        var root = await ParseFileSystem();

        var free = 70_000_000 - root.DirectorySize;
        var required = 30_000_000 - free;

        var folderSizes = new List<int>();
        GetFolderSizes(root, folderSizes);

        var best = folderSizes.OrderBy(i => i).SkipWhile(i => i < required).First();
        Assert.Equal(3696336, best);

        void GetFolderSizes(Folder current, List<int> folders)
        {
            folders.Add(current.DirectorySize);
            foreach (var subfolder in current.Subfolders)
            {
                GetFolderSizes(subfolder.Value, folders);
            }
        }
    }

    async Task<Folder> ParseFileSystem()
    {
        var lines = await File.ReadAllLinesAsync("day7.txt");
        var posititon = new Stack<string>();
        var root = new Folder("/");

        foreach (var line in lines)
        {
            var lineElements = line.Split(" ");

            switch (lineElements)
            {
                case ["$", "cd", "/"]:
                    posititon = new Stack<string>();
                    break;
                case ["$", "cd", ".."]:
                    posititon.Pop();
                    break;
                case ["$", "cd", var dir]:
                    posititon.Push(dir);
                    break;
                case ["$", "ls"]:
                    break;
                case ["dir", var _]:
                    break;
                case [var size, var file]:
                    var folder2 = GetFolder(posititon, root);
                    folder2.FilesSize += int.Parse(size);
                    break;
            }
        }

        CalculateFolderSize(root);
        return root;
    }

    void CalculateFolderSize(Folder folder)
    {
        foreach (var subfolder in folder.Subfolders)
        {
            CalculateFolderSize(subfolder.Value);
            folder.DirectorySize += subfolder.Value.DirectorySize;
        }

        folder.DirectorySize += folder.FilesSize;
    }

    Folder GetFolder(Stack<string> posititon, Folder root)
    {
        var current = root;

        foreach (var dir in posititon.Reverse())
        {
            current.Subfolders.TryAdd(dir, new Folder(dir));
            current = current.Subfolders[dir];
        }

        return current;
    }

    class Folder
    {
        public Folder(string name)
        {
            Name = name;
            Subfolders = new Dictionary<string, Folder>();
        }

        public string Name { get; set; }
        public Dictionary<string, Folder> Subfolders { get; set; }
        public int FilesSize { get; set; }
        public int DirectorySize { get; set; }
    }
}