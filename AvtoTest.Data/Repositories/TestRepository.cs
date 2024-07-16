using AvtoTest.Data.Entities.TestEntities;
using Newtonsoft.Json;

namespace AvtoTest.Data.Repositories;

public class TestRepository
{
    // "D:\.Net Projects\Avto Test Web\AvtoTest.Data\bin\Debug\net7.0\uzlotin.json"
    private string Path { get; set; } = "D:\\.Net Projects\\Avto Test Web\\AvtoTest.Data\\bin\\Debug\\net7.0\\";

    public List<Test> ReadFromFile(string? language = null)
    {
        if (string.IsNullOrEmpty(language))
            language = "uzb";

        switch (language)
        {
            case "uzb": Path += "uzlotin.json"; break;
            case "ru": Path += "rus.json"; break;
            case "krill": Path += "uzkiril.json"; break;
            default: Path += "uzlotin.json"; break;
        }

        var jsonData = File.ReadAllText(Path);
        List<Test> tests = JsonConvert.DeserializeObject<List<Test>>(jsonData)!;
        return tests!;
    }
}