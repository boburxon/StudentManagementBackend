using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    }
}