using AvtoTest.Data.Context;
using AvtoTest.Data.Entities;
using AvtoTest.Data.Entities.TestEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AvtoTest.Data.Repositories;

public class ResultRepository: IResultRepository
{

    private readonly AppDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private const string ResultKey = "results";

    public ResultRepository(AppDbContext context, IMemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
    }

    public async Task<List<Result>> GetAllResults()
    {
        var results = await GetResults();
        return results;
    }

    public async Task AddResult(byte ticketId,string userId, int correctAnswerCount)
    {
        var result = new Result()
        {
            TicketId = ticketId,
            CorrectAnswersCount = (byte)correctAnswerCount,
            UserId = userId
        };

        _context.Results.Add(result);
        await _context.SaveChangesAsync();
        await GetOrUpdateResults();
    }

    public async Task DeleteResult(Result result)
    {
        _context.Results.Remove(result);
        await _context.SaveChangesAsync();
        await GetOrUpdateResults();
    }

    public async Task<Result?> GetResultById(byte ticketId, string userId)
    {
        var results = await GetResults();
        var result = results.FirstOrDefault(r => r.TicketId ==
            ticketId && r.UserId == userId);
        return result;
    }

    private async Task<List<Result>> GetOrUpdateResults()
    {
        var results = await _context.Results.ToListAsync();
        _memoryCache.Set(ResultKey, results);
        return results;
    }

    private async Task<List<Result>> GetResults()
    {
        if (_memoryCache.TryGetValue(ResultKey, out object value))
        {
            var results = (List<Result>) value!;
            return results;
        }

        return await GetOrUpdateResults();
    }

}