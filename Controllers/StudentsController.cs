using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using StudentManagementBackend.DataTransferObjects;
using StudentManagementBackend.Entities;
using StudentManagementBackend.Repositories;
using StudentManagementBackend.Utilities;

namespace StudentManagementBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentRepository _studentRepository;
        private readonly AttendanceLogRepository _attendanceLogRepository;

        public StudentsController(StudentRepository studentRepository, AttendanceLogRepository attendanceLogRepository)
        {
            _studentRepository = studentRepository;
            _attendanceLogRepository = attendanceLogRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Student>> GetStudents()
        {
            return Ok(_studentRepository.GetAllStudents());
        }

        [HttpGet("{id}")]
        public ActionResult<Student> GetStudent(Guid id)
        {
            var student = _studentRepository.GetStudentById(id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        [HttpPut("{id}")]
        public IActionResult PutStudent(Guid id, [FromForm] StudentCreateOrUpdateDto studentCreateOrUpdateDto)
        {
            _studentRepository.UpdateStudent(id, studentCreateOrUpdateDto);
            return NoContent();
        }

        [HttpPost]
        public ActionResult<Student> PostStudent([FromForm] StudentCreateOrUpdateDto studentCreateOrUpdateDto)
        {
            var student = _studentRepository.InsertStudent(studentCreateOrUpdateDto);
            return CreatedAtAction("GetStudent", new { id = student.Id }, student);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(Guid id)
        {
            var student = _studentRepository.GetStudentById(id);
            if (student == null)
            {
                return NotFound();
            }

            _studentRepository.DeleteStudent(id);
            return NoContent();
        }

        [HttpPost("{id}/Enter")]
        public IActionResult Enter(Guid id)
        {
            var student = _studentRepository.GetStudentById(id);
            if (student == null) return BadRequest();
            if (_studentRepository.IsStudentInside(id))
            {
                return BadRequest();
            }

            var attendanceLog = new AttendanceLog()
            {
                Student = student,
                LogType = AttendanceLogType.Enter,
                CreatedDate = DateTime.Now
            };
            _attendanceLogRepository.InsertAttendanceLog(attendanceLog);
            return Ok();
        }

        [HttpPost("{id}/Leave")]
        public IActionResult Leave(Guid id)
        {
            var student = _studentRepository.GetStudentById(id);
            if (student == null) return BadRequest();
            if (!_studentRepository.IsStudentInside(id))
            {
                return BadRequest();
            }

            var attendanceLog = new AttendanceLog()
            {
                Student = student,
                LogType = AttendanceLogType.Leave,
                CreatedDate = DateTime.Now
            };
            _attendanceLogRepository.InsertAttendanceLog(attendanceLog);
            return Ok();
        }

        [HttpGet("ExportToExcel")]
        public IActionResult ExportToExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Students");
                var exelRow = 1;
                worksheet.Cell(exelRow, 1).Value = "Id";
                worksheet.Cell(exelRow, 2).Value = "FullName";
                worksheet.Cell(exelRow, 2).Value = "Is inside";
                foreach (var student in _studentRepository.GetAllStudents())
                {
                    exelRow++;
                    worksheet.Cell(exelRow, 1).Value = student.Id;
                    worksheet.Cell(exelRow, 2).Value = student.FullName;
                    worksheet.Cell(exelRow, 2).Value = _studentRepository.IsStudentInside(student.Id);
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "users.xlsx");
                }
            }
        }

        [HttpGet("ExportToPdf")]
        public IActionResult ExportToPdf()
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = $"Generated at {DateTime.Now}";
            PdfPage page = document.AddPage();
            
            

            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 14, XFontStyle.Regular);
            XTextFormatter tf = new XTextFormatter(gfx);
            XRect rect = new XRect(40, 100, page.Width, page.Height);
            gfx.DrawRectangle(XBrushes.SeaShell, rect);

            foreach (var student in _studentRepository.GetAllStudents())
            {
                tf.DrawString($"{student.FullName}", font, XBrushes.Black, rect, XStringFormats.TopLeft);
            }

            using (var stream = new MemoryStream())
            {
                document.Save(stream);
                var content = stream.ToArray();
                return File(
                    content,
                    "application/pdf",
                    "users.pdf");
            }
            
        }
    }
}