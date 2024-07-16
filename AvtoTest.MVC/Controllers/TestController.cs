using AvtoTest.Data.Entities.TestEntities;
using AvtoTest.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace AvtoTest.MVC.Controllers;

public class TestController : Controller
{

    private readonly TestService _testService;

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
        return View(tests);
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


}