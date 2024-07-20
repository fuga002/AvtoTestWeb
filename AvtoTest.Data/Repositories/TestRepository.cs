using AvtoTest.Data.Entities.TestEntities;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace AvtoTest.Data.Repositories;

public class TestRepository
{
   private readonly IHostingEnvironment _environment;

   private string Path => _environment.WebRootPath + "\\Autotest\\";

   public TestRepository(IHostingEnvironment environment)
   {
       _environment = environment;
   }

   

    public List<Test> ReadFromFile(string? language = null)
    {
        string filePath = Path;
        if (string.IsNullOrEmpty(language))
            language = "uzb";

        switch (language)
        {
            case "uzb": filePath += "uzlotin.json"; break;
            case "ru": filePath += "rus.json"; break;
            case "krill": filePath += "uzkiril.json"; break;
        }

        var jsonData = File.ReadAllText(filePath);
        List<Test> tests = JsonConvert.DeserializeObject<List<Test>>(jsonData)!;
        return tests!;
    }

    public string GetPath()
    {
        var path = _environment.WebRootPath;
        var m = path + "\\Autotest\\uzlotin.json";
        return m;
    }
}