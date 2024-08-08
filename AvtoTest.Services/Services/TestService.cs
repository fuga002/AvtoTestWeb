using AvtoTest.Data.Entities.TestEntities;
using AvtoTest.Data.Repositories;
using Microsoft.AspNetCore.Http;

namespace AvtoTest.Services.Services;

public class TestService
{
    private readonly TestRepository _repository;
    public List<Test> Tests { get; set; }

    public TestService(TestRepository repository)
    {
        _repository = repository;
        Tests = _repository.ReadFromFile();
    }

    public void ChangeLanguage(string language,HttpContext context)
    {
        if (!string.IsNullOrEmpty(language))
        {
            AddCookie("language", language,context);
        }
        else
        {
            language = GetCookie("language",context);
        }

        Tests = _repository.ReadFromFile(language);
    }

    public string GetPath()
    {
        return _repository.GetPath();
    }

    public Tuple<Test, List<Test>> GetSortedTests(ushort startIndex, ushort endIndex,int testId)
    {
        var tests = Tests.
            Where(t => t.Id >= startIndex &&
                       t.Id <= endIndex).
            ToList();

        var test = tests.Find(t => t.Id == testId);
        return new(test, tests);
    }

    public Test GetTestById(int testId)
    {
       return Tests.Find(t => t.Id == testId)!;
    }

    private void AddCookie(string key, string value, HttpContext context)
    {
        var check = CheckCookie(key,context);
        if (!check)
        {
            context.Response.Cookies.Delete(key);
        }
        context.Response.Cookies.Append(key, value);
    }

    private string GetCookie(string key, HttpContext context)
    {
        string value = context.Request.Cookies[key];
        if (string.IsNullOrEmpty(value))
            return string.Empty;
        return value;
    }

    private bool CheckCookie(string key,HttpContext context)
    {
        var value = context.Request.Cookies[key];
        return string.IsNullOrEmpty(value);
    }

    public Tuple<Ticket, int> GetTicketAndTestId(byte ticketId,int testId)
    {
        var ticket = new Ticket() { Id = ticketId };


        if (testId == 0)
        {
            testId = ticket.StartIndex;
        }

        return new(ticket, testId);
    }
}