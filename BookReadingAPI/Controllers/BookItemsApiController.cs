using Microsoft.AspNetCore.Mvc;
using ReadingBookAPI.Models;
using Microsoft.EntityFrameworkCore;  // ToListAsync için gerekli



[Route("api/[controller]")]
[ApiController]
public class BookItemsApiController : ControllerBase
{
    
    private readonly BookContext _context;

    public BookItemsApiController(BookContext context)
    {
        _context = context;
    }

    // GET: api/BookItems
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetLeaderboard()
    {
        var leaderboard = await _context.BookItems
            .OrderByDescending(u => u.TodayPages)
            .ThenByDescending(u => u.Streak)
            .ToListAsync();

        var result = leaderboard.Select((item, index) => new
        {
            Id = item.Id,
            Rank = index + 1,
            UserName = item.Username,
            TotalPages = item.TotalPages,
            TodayPages = item.TodayPages,
            Streak = item.Streak > 0 ? $"🔥 {item.Streak} days" : "-"
        }).ToList();

        return Ok(result);
    }

    // GET: api/BookItems/5
    [HttpGet("GetBookItems{id}")]
    public async Task<ActionResult<BookItem>> GetBookItem(long id)
    {
        var bookItem = await _context.BookItems.FindAsync(id);
        if (bookItem == null)
        {
            return NotFound();
        }
        return bookItem;
    }

    // POST: api/BookItems
    [HttpPost]
    public async Task<ActionResult<BookItem>> PostBookItem(BookItem log)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);  // Hata varsa açıklamalarla birlikte geri dön
        }
        var existingLog = await _context.BookItems.FirstOrDefaultAsync(u => u.Username == log.Username);

        if (existingLog != null)
        {
            if (existingLog.LastUpdate.Date == DateTime.UtcNow.Date)
            {
                existingLog.TodayPages += log.TodayPages;
            }
            else
            {
                if (existingLog.LastUpdate.Date == DateTime.UtcNow.AddDays(-1).Date)
                {
                    existingLog.Streak++;
                }
                else
                {
                    existingLog.Streak = 1;
                }
                existingLog.TodayPages = log.TodayPages;
            }
            existingLog.TotalPages = log.TotalPages;
            existingLog.LastUpdate = DateTime.UtcNow;
        }
        else
        {
            log.LastUpdate = DateTime.UtcNow;
            log.Streak = 1;
            log.TotalPages = log.TotalPages;
            log.TodayPages = log.TodayPages;
            _context.BookItems.Add(log);
        }

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBookItem), new { id = log.Id }, log);
    }
    [HttpPut("PutBookItems{id}")]
    public async Task<IActionResult> PutBookItem(long id, BookItem bookItem)
    {
        if (id != bookItem.Id)
        {
            return BadRequest();
        }

        _context.Entry(bookItem).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (bookItem == null)
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent(); // Başarıyla güncellendi
    }



    // DELETE: api/BookItems/5
    [HttpDelete("DeleteBookItems{id}")]
    public async Task<IActionResult> DeleteBookItem(long id)
    {
        Console.WriteLine($"Gelen ID: {id}"); 
        var bookItem = await _context.BookItems.FindAsync(id);
        if (bookItem == null)
        {
            return NotFound();
        }

        _context.BookItems.Remove(bookItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
