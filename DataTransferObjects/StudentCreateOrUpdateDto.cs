using StudentManagementBackend.Entities;

namespace StudentManagementBackend.DataTransferObjects;

public class StudentCreateOrUpdateDto
{
        public string FullName { get; set; }

        public IFormFile ProfilePhoto { get; set; }
}