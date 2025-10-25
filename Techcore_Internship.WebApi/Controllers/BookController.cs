using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Techcore_Internship.WebApi.Dto;
using Techcore_Internship.WebApi.Services;
using Techcore_Internship.WebApi.Services.Interfaces;

namespace Techcore_Internship.WebApi.Controllers;

// Task339_3_BookApi
[ApiController]
[Route("[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly MySettings _mySettings;

    public BookController(IBookService bookService, IOptions<MySettings> mySettings)
    {
        _mySettings = mySettings.Value;
        _bookService = bookService;
    }

    [HttpGet("settings")]
    public IActionResult Settings()
    {
        return Ok(_mySettings);
    }

    [HttpGet("error")]
    public IActionResult Error()
    {
        throw new NotImplementedException();
    }

    // Task339_8_AsyncContollers
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _bookService.GetAll());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var book = await _bookService.Get(id);

        return book == null
            ? NotFound()
            : Ok(book);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookDto book)
    {
        var newBook = await _bookService.Create(book);

        return Ok(newBook);
    }
    
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] BookDto request)
    {
        return await _bookService.Update(request) 
            ? Ok() 
            : BadRequest();
    }

    [HttpPatch("update-title/{id}")]
    public async Task<IActionResult> UpdateTitle([FromRoute] Guid id, [FromBody] string request)
    {
        return await _bookService.UpdateTitle(id, request)
            ? Ok()
            : BadRequest();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        return await _bookService.Delete(id)
            ? Ok()
            : BadRequest();
    }
}
