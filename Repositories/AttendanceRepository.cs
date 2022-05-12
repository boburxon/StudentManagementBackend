using Microsoft.EntityFrameworkCore;
using StudentManagementBackend.Entities;
using StudentManagementBackend.Utilities;

namespace StudentManagementBackend.Repositories;

public interface IAttendanceLogRepository
{
    void InsertAttendanceLog(AttendanceLog attendanceLog);
    void UpdateAttendanceLog(AttendanceLog attendanceLog);
    void DeleteAttendanceLog(Guid attendanceLogId);
    AttendanceLog? GetAttendanceLogById(Guid attendanceLogId);
    IEnumerable<AttendanceLog> GetAllAttendanceLogs();
}

public class AttendanceLogRepository : IAttendanceLogRepository
{
    private readonly DataContext _dbContext;

    public AttendanceLogRepository(DataContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void InsertAttendanceLog(AttendanceLog attendanceLog)
    {
        _dbContext.AttendanceLogs.Add(attendanceLog);
        _dbContext.SaveChanges();
    }

    public void UpdateAttendanceLog(AttendanceLog attendanceLog)
    {
        _dbContext.Entry(attendanceLog).State = EntityState.Modified;
        _dbContext.SaveChanges();
    }

    public void DeleteAttendanceLog(Guid attendanceLogId)
    {
        var attendanceLog = _dbContext.AttendanceLogs.Find(attendanceLogId);
        if (attendanceLog != null)
        {
            _ = _dbContext.AttendanceLogs.Remove(attendanceLog);
            _dbContext.SaveChanges();
        }
    }

    public AttendanceLog? GetAttendanceLogById(Guid attendanceLogId)
    {
        return _dbContext.AttendanceLogs.Find(attendanceLogId);
    }

    public IEnumerable<AttendanceLog> GetAllAttendanceLogs()
    {
        return _dbContext.AttendanceLogs.ToList();
    }
}