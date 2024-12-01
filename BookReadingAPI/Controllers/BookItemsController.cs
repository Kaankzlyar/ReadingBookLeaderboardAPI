using Microsoft.AspNetCore.Mvc;
using ReadingBookAPI.Models;
using Microsoft.EntityFrameworkCore;


public class BookItemsController : Controller
{
    private readonly BookContext _context;

    public BookItemsController(BookContext context)
    {
        _context = context;
    }

    // GET: /BookItems
    public async Task<IActionResult> Index()
    {
        var leaderboard = await _context.BookItems
            .OrderByDescending(u => u.TodayPageNumber)
            .ThenByDescending(u => u.StreakNumber)
            .ToListAsync();

        var rankedBooks = leaderboard.Select((book, index) => new BookItem
        {
            Id = book.Id,
            UserName = book.UserName,
            TotalPages = book.TotalPages,
            TodayPageNumber = book.TodayPageNumber,
            StreakNumber = book.StreakNumber,
            rank = index + 1
        }).ToList();





        return View(rankedBooks); // Razor View'a veri gönder
    }
    
    // POST: /BookItems
    [HttpPost]
    public async Task<ActionResult<BookItem>> PostBookItem(BookItem log)
    {
        var existingLog = await _context.BookItems.FirstOrDefaultAsync(u => u.UserName == log.UserName);

        if (existingLog != null)
        {
            // Update logic
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

        return RedirectToAction(nameof(Index)); // Veriyi tekrar ana sayfaya yönlendir
    }

    [HttpPost]
    [Route("BookItems/DeleteBookItem/{id}")]
    public async Task<IActionResult> DeleteBookItem(long id)
    {
        var bookItem = await _context.BookItems.FindAsync(id);
        if (bookItem == null)
        {
            return NotFound();
        }

        _context.BookItems.Remove(bookItem);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


}
