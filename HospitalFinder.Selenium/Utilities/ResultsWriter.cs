public static class ResultsWriter
{
    public static string WriteHospitalResults(List<string> hospitalNames, string testName)
    {
        var projectRoot = Path.GetFullPath(Path.Combine(
            Directory.GetCurrentDirectory(), "..", "..", ".."));

        var resultsDirectory = Path.Combine(
            projectRoot,
            ConfigurationManager.Settings.ResultsDirectory);
        Directory.CreateDirectory(resultsDirectory);

        // Same sanitization pattern as ScreenshotUtils, in case this is ever
        // called from a parameterized test.
        var safeTestName = string.Join("_", testName.Split(Path.GetInvalidFileNameChars()));

        var path = Path.Combine(resultsDirectory,
            $"{safeTestName}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

        File.WriteAllLines(path, hospitalNames);

        return path;
    }
}