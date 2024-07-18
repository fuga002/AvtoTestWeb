using AvtoTest.Data.Entities.TestEntities;
using AvtoTest.Services.Services;
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

    public IActionResult GetTests(Ticket ticket)
    {
        var tests = _testService.Tests.
            Where(t => t.Id >= ticket.StartIndex &&
                       t.Id <= ticket.EndIndex).
            ToList();
        ViewBag.TicketId = ticket.Id;

        ViewBag.Context = HttpContext;

        return View(tests);
    }

    [HttpPost]
    public IActionResult GetTests(byte ticketId = 0, int testId = 0, int choiceId = 0)
    {
        int count = GetCorrectAnswersCount();
       
        var ticket = new Ticket() { Id = ticketId };

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
        return RedirectToAction("GetTests", ticket);
    }

    public IActionResult Tickets()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Tickets(byte id)
    {
        var ticket = new Ticket() { Id = id };
        return RedirectToAction("GetTests",ticket);
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




}