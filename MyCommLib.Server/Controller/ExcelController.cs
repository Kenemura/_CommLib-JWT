namespace MyCommLib.APServer.Controllers;

using Microsoft.AspNetCore.Mvc;
using MyCommLib.Classes;

[Route("api/[controller]/[action]")]
[ApiController]
public class ExcelController : ControllerBase
{
    public ExcelController() {}
    [HttpGet("{name}")]
    public IActionResult DownloadTemplate(string name)
    {
        var excel = new clsExcel();
        var result = excel.OpenTemplate(name);
        if (!result) return BadRequest($"Template not found! ({name})");
        var bytes = excel.GetBytes();
        return File(bytes, clsExcel.ContentType, name);
    }
    [HttpGet("{filename}")]
    public IActionResult DownloadOutput(string filename)
    {
        using (clsExcel xls = new clsExcel())
        {
            var path = xls.OutputPath(filename);
            if (!xls.Open(path)) return BadRequest($"file not found! ({filename})");
            var excelFile = xls.GetBytes();
            return File(excelFile, clsExcel.ContentType, filename);
        }
    }
}