using Microsoft.EntityFrameworkCore;
using StudentManagementBackend.Entities;
using StudentManagementBackend.Utilities;
using StudentManagementBackend.DataTransferObjects;

namespace StudentManagementBackend.Repositories;

public interface IStudentRepository
{
    bool IsStudentInside(Guid studentId);
    Student InsertStudent(StudentCreateOrUpdateDto studentCreateOrUpdateDto);
    Student UpdateStudent(Guid studentId, StudentCreateOrUpdateDto studentCreateOrUpdateDto);
    void DeleteStudent(Guid studentId);
    Student? GetStudentById(Guid studentId);
    IEnumerable<Student> GetAllStudents();
}

public class StudentRepository : IStudentRepository
{
    private readonly DataContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly AttendanceLogRepository _attendanceLogRepository;

    public StudentRepository(DataContext dbContext, IConfiguration config, AttendanceLogRepository attendanceLogRepository)
    {
        _dbContext = dbContext;
        _configuration = config;
        _attendanceLogRepository = attendanceLogRepository;
    }

    public bool IsStudentInside(Guid studentId)
    {
        var student = this.GetStudentById(studentId);
        if (student == null) return false;
        if (student.AttendanceLogs.Count < 1) return false;
        return student.AttendanceLogs.Last().LogType == AttendanceLogType.Enter;
    } 

    private string SaveProfilePhoto(IFormFile file)
    {
        var fileName = $"{Path.GetRandomFileName()}{file.FileName}";
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetValue<string>("UploadsFolder"));
        var filePath = Path.Combine(uploadsPath, fileName);
        using (var stream = System.IO.File.Create(filePath))
        {
            file.CopyTo(stream);
        }
        return $@"/{Path.Combine(_configuration.GetValue<string>("UploadsFolder"), fileName)}";
    }

    public Student InsertStudent(StudentCreateOrUpdateDto studentCreateOrUpdateDto)
    {
        var fileUrl = this.SaveProfilePhoto(studentCreateOrUpdateDto.ProfilePhoto);
        var student = new Student()
        {
            FullName = studentCreateOrUpdateDto.FullName,
            ProfilePhoto = fileUrl
        };
        _dbContext.Students.Add(student);
        _dbContext.SaveChanges();
        return student;
    }

    public Student UpdateStudent(Guid studentId, StudentCreateOrUpdateDto studentCreateOrUpdateDto)
    {
        var fileUrl = this.SaveProfilePhoto(studentCreateOrUpdateDto.ProfilePhoto);
        var student = new Student()
        {
            Id = studentId,
            FullName = studentCreateOrUpdateDto.FullName,
            ProfilePhoto = fileUrl
        };
        _dbContext.Entry(student).State = EntityState.Modified;
        _dbContext.SaveChanges();
        return student;
    }

    public void DeleteStudent(Guid studentId)
    {
        var student = _dbContext.Students.Find(studentId);
        if (student != null)
        {
            _ = _dbContext.Students.Remove(student);
            _dbContext.SaveChanges();
        }
    }

    public Student? GetStudentById(Guid studentId)
    {
        return _dbContext.Students.Include(s=>s.AttendanceLogs).First(s=> s.Id == studentId);
    }

    public IEnumerable<Student> GetAllStudents()
    {
        return _dbContext.Students.ToList();
    }
}