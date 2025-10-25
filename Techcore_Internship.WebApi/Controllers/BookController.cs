using Microsoft.AspNetCore.Mvc;
using Techcore_Internship.WebApi.Dto;
using Techcore_Internship.WebApi.Services.Interfaces;

namespace Techcore_Internship.WebApi.Controllers;

// Task339_3_BookApi
[ApiController]
[Route("[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet("error")]
    public IActionResult Error()
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_bookService.GetAll());
    }

    [HttpGet("{id}")]
    public IActionResult Get([FromRoute] Guid id)
    {
        var book = _bookService.Get(id);

        return book == null
            ? NotFound()
            : Ok(book);
    }

    [HttpPost]
    public IActionResult Create([FromBody] BookDto book)
    {
        if (string.IsNullOrWhiteSpace(book.Title) || string.IsNullOrWhiteSpace(book.Author))
        {
            return BadRequest("Title and Author cannot be empty.");
        }

        var newBook = _bookService.Create(book);

        return Ok(newBook);
    }
    
    [HttpPut]
    public IActionResult Update([FromBody] BookDto request)
    {
        return _bookService.Update(request) 
            ? Ok() 
            : BadRequest();
    }

    [HttpPatch("update-title/{id}")]
    public IActionResult UpdateTitle([FromRoute] Guid id, [FromBody] string request)
    {
        return _bookService.UpdateTitle(id, request)
            ? Ok()
            : BadRequest();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] Guid id)
    {
        return _bookService.Delete(id)
            ? Ok()
            : BadRequest();
    }
}
