using System.Diagnostics;
using System.Formats.Tar;
using System.IO.Compression;

(string runtime, string binaryExtension, Action<string, Func<string, Stream>> compressionMethod)[] targets =
[
    ("osx-arm64", string.Empty, CompressTar),
    ("osx-x64", string.Empty, CompressTar),
    ("linux-arm64", string.Empty, CompressTar),
    ("linux-x64", string.Empty, CompressTar),
    ("win-arm64", ".exe", CompressZip),
    ("win-x64", ".exe", CompressZip),
];

var vvDir = Path.Combine(new DirectoryInfo(AppContext.BaseDirectory).Parent.Parent.FullName, "vv");

var artifactsDir = PrepareOutputDirectory("artifacts");

List<string> artifactFiles = [];

foreach ((string runtime, string binaryExt, var compressionMethod) in targets)
{
    var publishDirectory = Publish(runtime);
    var sourceFile = Path.Combine(publishDirectory, "vv" + binaryExt);

    compressionMethod.Invoke(sourceFile, ext =>
    {
        var outputFile = Path.Combine(artifactsDir, $"vv-{runtime}") + ext;

        Console.WriteLine($"Created {outputFile}");

        artifactFiles.Add(outputFile);

        return File.Create(outputFile);
    });
    Console.WriteLine();
}

return 0;

string Publish(string runtime)
{
    Run("dotnet", $"publish --runtime {runtime}", vvDir);
    return Path.Combine(vvDir, "bin", runtime, "publish");
}

void CompressTar(string sourceFile, Func<string, Stream> streamFactory)
{
    using var stream = streamFactory.Invoke(".tar.gz");
    using var gzipStream = new GZipStream(stream, CompressionMode.Compress);
    using var tarWriter = new TarWriter(gzipStream);
    tarWriter.WriteEntry(sourceFile, "vv");
}

void CompressZip(string sourceFile, Func<string, Stream> streamFactory)
{
    using var stream = streamFactory.Invoke(".zip");
    using var archive = new ZipArchive(stream, ZipArchiveMode.Create);
    archive.CreateEntryFromFile(sourceFile, "vv.exe", CompressionLevel.Optimal);
}

string PrepareOutputDirectory(string name)
{
    var artifactsDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, name));

    if (!artifactsDirectory.Exists)
        artifactsDirectory.Create();
    else
    {
        foreach (FileInfo file in artifactsDirectory.EnumerateFiles())
            file.Delete();

        foreach (DirectoryInfo subDir in artifactsDirectory.EnumerateDirectories())
            subDir.Delete(recursive: true);
    }

    return artifactsDirectory.FullName;
}

Process Run(string fileName, string arguments, string workingDirectory)
{
    using var process = Process.Start(new ProcessStartInfo
    {
        FileName = fileName,
        Arguments = arguments,
        WorkingDirectory = workingDirectory,
    });
    process.WaitForExit();
    return process;
}