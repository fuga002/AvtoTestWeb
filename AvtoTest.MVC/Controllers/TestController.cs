using AvtoTest.Data.Entities.TestEntities;
using AvtoTest.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AvtoTest.MVC.Controllers;


public class TestController : Controller
{

    private readonly TestService _testService;
    private const string CorrectAnswersCount = "CorrectAnswersCount";

    public TestController(TestService testService)
    {
        _testService = testService;
    }
    [Authorize]
    public IActionResult GetTests(byte ticketId,int testId = 0, string language = null)
    {
        var ticket = new Ticket() { Id = ticketId };

        if (!string.IsNullOrEmpty(language))
        {
            AddCookie("language",language);
        }
        else
        {
            language = GetCookie("language");
        }
        _testService.ChangeLanguage(language);

        if (testId == 0)
        {
            testId = ticket.StartIndex;
        }

        var tests = _testService.Tests.
            Where(t => t.Id >= ticket.StartIndex &&
                       t.Id <= ticket.EndIndex).
            ToList();

        var test = tests.Find(t => t.Id == testId);

        ViewBag.TicketId = ticket.Id;

        ViewBag.Context = HttpContext;

        ViewBag.Ticket = ticket;


        ViewBag.Tests = tests;

        return View(test);
    }

    [HttpPost]
    public IActionResult GetTestsPost(byte ticketId = 0, int testId = 0, int choiceId = 0)
    {
        int count = GetCorrectAnswersCount();
       

        var test = _testService.Tests.Find(t => t.Id == testId);
        if (test.Choices[choiceId].Answer)
        {
            count++;
        }
        if (testId != 0)
        {
            AddCookie(testId.ToString(),choiceId.ToString());
            AddCookie(CorrectAnswersCount,count.ToString());
        }
        return RedirectToAction("GetTests", new {ticketId = ticketId,testId = testId});
    }

    public IActionResult Tickets()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Tickets(byte id)
    {
        var ticket = new Ticket() { Id = id };

        DeleteCookies(ticket);

        return RedirectToAction("GetTests", new { ticketId = id, testId = 0});
    }
    [Authorize]
    public IActionResult TestResults(byte ticketId)
    {
        var correctAnswerCount = GetCorrectAnswersCount();
        ViewBag.Count = correctAnswerCount;
        var ticket = new Ticket() { Id = ticketId };
        DeleteCookies(ticket);
        DeleteCookie("language");
        return View();
    }


    private void AddCookie(string key, string value)
    {
        var check = CheckCookie(key);
        if (!check)
        {
            HttpContext.Response.Cookies.Delete(key);
        }
        HttpContext.Response.Cookies.Append(key, value);
    }

    private string GetCookie(string key)
    {
        string value = HttpContext.Request.Cookies[key];
        if (string.IsNullOrEmpty(value))
            return string.Empty;
        return value;
    }

    private void DeleteCookie(string key)
    {
        var check = CheckCookie(key);
        if (!check)
        {
            HttpContext.Response.Cookies.Delete(key);
        }
    }

    private void DeleteCookies(Ticket ticket)
    {
        for (int i = ticket.StartIndex; i <= ticket.EndIndex; i++)
        {
            DeleteCookie(i.ToString());
            if (i == ticket.StartIndex)
            {
                DeleteCookie(CorrectAnswersCount);
            }
        }
    }

    private bool CheckCookie(string key)
    {
        var value = HttpContext.Request.Cookies[key];
        return string.IsNullOrEmpty(value);
    }

    private int GetCorrectAnswersCount()
    {
        string correctAnswersCount = HttpContext.Request.Cookies[CorrectAnswersCount]!;
        var count = string.IsNullOrEmpty(correctAnswersCount) ? 0 :
            Convert.ToInt32(correctAnswersCount);
        return count;
    }


    public IActionResult GetPath()
    {
        var path = _testService.GetPath();
        ViewBag.Path = path;
        return View();
    }

}