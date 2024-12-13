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
            .OrderByDescending(u => u.TodayPageNumber)
            .ThenByDescending(u => u.StreakNumber)
            .ToListAsync();

        var result = leaderboard.Select((item, index) => new
        {
            Rank = index + 1,
            UserName = item.UserName,
            TotalPages = item.TotalPages,
            TodayPages = item.TodayPageNumber,
            Streak = item.StreakNumber > 0 ? $"🔥 {item.StreakNumber} days" : "-"
        }).ToList();

        return Ok(result);
    }

    // GET: api/BookItems/5
    [HttpGet("{id}")]
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
        var existingLog = await _context.BookItems.FirstOrDefaultAsync(u => u.UserName == log.UserName);

        if (existingLog != null)
        {
            if (existingLog.LastUpdate.Date == DateTime.UtcNow.Date)
            {
                existingLog.TodayPageNumber += log.TodayPageNumber;
            }
            else
            {
                if (existingLog.LastUpdate.Date == DateTime.UtcNow.AddDays(-1).Date)
                {
                    existingLog.StreakNumber++;
                }
                else
                {
                    existingLog.StreakNumber = 1;
                }
                existingLog.TodayPageNumber = log.TodayPageNumber;
            }
            existingLog.TotalPages += log.TodayPageNumber;
            existingLog.LastUpdate = DateTime.UtcNow;
        }
        else
        {
            log.LastUpdate = DateTime.UtcNow;
            log.StreakNumber = 1;
            log.TotalPages = log.TodayPageNumber;
            _context.BookItems.Add(log);
        }

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBookItem), new { id = log.Id }, log);
    }
    [HttpPut("{id}")]
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
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBookItem(long id)
    {
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
