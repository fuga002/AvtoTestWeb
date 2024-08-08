using AvtoTest.Data.Context;
using AvtoTest.Data.Entities;
using AvtoTest.Data.Entities.TestEntities;
using AvtoTest.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using AvtoTest.Data.Repositories;
using static System.Net.Mime.MediaTypeNames;

namespace AvtoTest.MVC.Controllers;


public class TestController : Controller
{

    private readonly TestService _testService;
    private const string CorrectAnswersCount = "CorrectAnswersCount";
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IResultRepository _resultRepository;

    public TestController(TestService testService, 
        UserManager<IdentityUser> userManager, IResultRepository resultRepository)
    {
        _testService = testService;
        _userManager = userManager;
        _resultRepository = resultRepository;
    }
    [Authorize]
    public async Task<IActionResult> GetTests(byte ticketId,int testId = 0, string language = null, bool retake = false)
    {
        var user = await GetUser();

        var result = await _resultRepository.GetResultById(ticketId, user.Id);

       

        if (result is not null && retake == false)
        {
            return RedirectToAction("Results",result);
        }

        if (retake && result is not null)
        {
            await _resultRepository.DeleteResult(result);
        }

        _testService.ChangeLanguage(language, HttpContext);

        (Ticket ticket, testId) = _testService.GetTicketAndTestId(ticketId, testId);


        ViewBag.TicketId = ticket.Id;

        ViewBag.Context = HttpContext;

        ViewBag.Ticket = ticket;

        var (test, tests) = 
            _testService.GetSortedTests(ticket.StartIndex, ticket.EndIndex, testId);

        ViewBag.Tests = tests;

        return View(test);
    }

    [HttpPost]
    public async Task<IActionResult> GetTestsPost(byte ticketId = 0, int testId = 0, int choiceId = 0)
    {
        var test = _testService.GetTestById(testId);

        await AddScore(testId, choiceId, test);

        return RedirectToAction("GetTests", new {ticketId = ticketId,testId = testId});
    }

    public async Task<IActionResult> Results(Result result)
    {
        return View(result);
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
    public async Task<IActionResult> TestResults(byte ticketId)
    {
        var correctAnswerCount = GetCorrectAnswersCount();
        ViewBag.Count = correctAnswerCount;
        var ticket = new Ticket() { Id = ticketId };

        var user = await GetUser();

        await _resultRepository.AddResult(ticketId,user.Id,correctAnswerCount);
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


    private async Task<IdentityUser> GetUser()
    {
        return await _userManager!.GetUserAsync(User)!;
    }

    private async Task AddScore(int testId,int choiceId,Test test)
    {
        int count = GetCorrectAnswersCount();
        if (test.Choices[choiceId].Answer)
        {
            count++;
        }
        if (testId != 0)
        {
            AddCookie(testId.ToString(), choiceId.ToString());
            AddCookie(CorrectAnswersCount, count.ToString());
        }
    }
}