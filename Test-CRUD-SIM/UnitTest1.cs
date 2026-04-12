using System.Collections.Generic;
using NUnit.Framework;
namespace SchoolInformationManagement.Tests;
using SchoolInformationManagement;       

{
	// ==========================================
	// MOCK - Giả lập IUserRepository, không cần DB thật
	// ==========================================
	public class MockUserRepository : IUserRepository
	{
		// --- Ghi lại hành động được gọi ---
		public bool AddStudentCalled { get; private set; }
		public Student? LastAddedStudent { get; private set; }

		public bool DeleteUserCalled { get; private set; }
		public int LastDeletedId { get; private set; }
		public string? LastDeletedRole { get; private set; }

		public bool UpdateStudentCalled { get; private set; }
		public Student? LastUpdatedStudent { get; private set; }

		public bool GetStudentsCalled { get; private set; }
		public bool GetTeachingStaffsCalled { get; private set; }
		public bool GetAdministratorsCalled { get; private set; }
		public bool GetAllUsersCalled { get; private set; }

		// --- Dữ liệu giả trả về ---
		public List<User> FakeStudents { get; set; } = new List<User>
		{
			new Student { Id = 1, Name = "An", Phone = "111", Email = "an@mail.com" }
		};
		public List<User> FakeTeachers { get; set; } = new List<User>
		{
			new TeachingStaff { Id = 2, Name = "Binh", Salary = 5000 }
		};
		public List<User> FakeAdmins { get; set; } = new List<User>
		{
			new Administrator { Id = 3, Name = "Nam", Salary = 8000, IsFullTime = true }
		};
		public List<Subject> FakeSubjects { get; set; } = new List<Subject>
		{
			new Subject { SubjectId = 1, SubjectName = "Math" }
		};

		// --- Triển khai Interface ---
		public void AddStudent(Student student)
		{
			AddStudentCalled = true;
			LastAddedStudent = student;
		}

		public void DeleteUser(int id, string role)
		{
			DeleteUserCalled = true;
			LastDeletedId = id;
			LastDeletedRole = role;
		}

		public void UpdateStudent(Student student)
		{
			UpdateStudentCalled = true;
			LastUpdatedStudent = student;
		}

		public List<User> GetStudents()
		{
			GetStudentsCalled = true;
			return FakeStudents;
		}

		public List<User> GetTeachingStaffs()
		{
			GetTeachingStaffsCalled = true;
			return FakeTeachers;
		}

		public List<User> GetAdministrators()
		{
			GetAdministratorsCalled = true;
			return FakeAdmins;
		}

		public List<User> GetAllUsers()
		{
			GetAllUsersCalled = true;
			var all = new List<User>();
			all.AddRange(FakeStudents);
			all.AddRange(FakeTeachers);
			all.AddRange(FakeAdmins);
			return all;
		}

		public List<Subject> GetSubjects()
		{
			return FakeSubjects;
		}
	}

	// ==========================================
	// TEST CLASS
	// ==========================================
	[TestFixture]
	public class UserServiceTests
	{
		private MockUserRepository _mockRepo = null!;
		private UserService _service = null!;

		[SetUp]
		public void Setup()
		{
			_mockRepo = new MockUserRepository();
			_service = new UserService(_mockRepo);
		}

		// ==========================================
		// LOAD DATA - STUDENTS
		// ==========================================

		[Test]
		public void GetUsers_Students_CallsGetStudents()
		{
			_service.GetUsers("Students");

			Assert.IsTrue(_mockRepo.GetStudentsCalled);
			Assert.IsFalse(_mockRepo.GetTeachingStaffsCalled);
			Assert.IsFalse(_mockRepo.GetAdministratorsCalled);
			Assert.IsFalse(_mockRepo.GetAllUsersCalled);
		}

		[Test]
		public void GetUsers_Students_ReturnsStudentList()
		{
			var result = _service.GetUsers("Students");

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.IsInstanceOf<Student>(result[0]);
			Assert.AreEqual("An", result[0].Name);
		}

		// ==========================================
		// LOAD DATA - TEACHING STAFFS
		// ==========================================

		[Test]
		public void GetUsers_TeachingStaffs_CallsGetTeachingStaffs()
		{
			_service.GetUsers("TeachingStaffs");

			Assert.IsTrue(_mockRepo.GetTeachingStaffsCalled);
			Assert.IsFalse(_mockRepo.GetStudentsCalled);
			Assert.IsFalse(_mockRepo.GetAdministratorsCalled);
			Assert.IsFalse(_mockRepo.GetAllUsersCalled);
		}

		[Test]
		public void GetUsers_TeachingStaffs_ReturnsTeacherList()
		{
			var result = _service.GetUsers("TeachingStaffs");

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.IsInstanceOf<TeachingStaff>(result[0]);
			Assert.AreEqual("Binh", result[0].Name);
		}

		// ==========================================
		// LOAD DATA - ADMINISTRATORS
		// ==========================================

		[Test]
		public void GetUsers_Administrators_CallsGetAdministrators()
		{
			_service.GetUsers("Administrators");

			Assert.IsTrue(_mockRepo.GetAdministratorsCalled);
			Assert.IsFalse(_mockRepo.GetStudentsCalled);
			Assert.IsFalse(_mockRepo.GetTeachingStaffsCalled);
			Assert.IsFalse(_mockRepo.GetAllUsersCalled);
		}

		[Test]
		public void GetUsers_Administrators_ReturnsAdminList()
		{
			var result = _service.GetUsers("Administrators");

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.IsInstanceOf<Administrator>(result[0]);
			Assert.AreEqual("Nam", result[0].Name);
		}

		// ==========================================
		// LOAD DATA - ALL
		// ==========================================

		[Test]
		public void GetUsers_All_CallsGetAllUsers()
		{
			_service.GetUsers("All");

			Assert.IsTrue(_mockRepo.GetAllUsersCalled);
			Assert.IsFalse(_mockRepo.GetStudentsCalled);
			Assert.IsFalse(_mockRepo.GetTeachingStaffsCalled);
			Assert.IsFalse(_mockRepo.GetAdministratorsCalled);
		}

		[Test]
		public void GetUsers_All_ReturnsAllUsers()
		{
			var result = _service.GetUsers("All");

			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Count);
		}

		// ==========================================
		// CREATE STUDENT
		// ==========================================

		[Test]
		public void CreateStudent_ValidData_CallsAddStudent()
		{
			_service.CreateStudent("Nguyen Van A", "0901234567", "a@mail.com");

			Assert.IsTrue(_mockRepo.AddStudentCalled);
		}

		[Test]
		public void CreateStudent_ValidData_PassesCorrectDataToRepository()
		{
			_service.CreateStudent("Nguyen Van A", "0901234567", "a@mail.com");

			Assert.IsNotNull(_mockRepo.LastAddedStudent);
			Assert.AreEqual("Nguyen Van A", _mockRepo.LastAddedStudent!.Name);
			Assert.AreEqual("0901234567", _mockRepo.LastAddedStudent!.Phone);
			Assert.AreEqual("a@mail.com", _mockRepo.LastAddedStudent!.Email);
		}

		// ==========================================
		// REMOVE USER
		// ==========================================

		[Test]
		public void RemoveUser_ValidStudent_CallsDeleteUser()
		{
			_service.RemoveUser(1, "Student");

			Assert.IsTrue(_mockRepo.DeleteUserCalled);
		}

		[Test]
		public void RemoveUser_Student_PassesCorrectIdAndRole()
		{
			_service.RemoveUser(1, "Student");

			Assert.AreEqual(1, _mockRepo.LastDeletedId);
			Assert.AreEqual("Student", _mockRepo.LastDeletedRole);
		}

		[Test]
		public void RemoveUser_TeachingStaff_PassesCorrectIdAndRole()
		{
			_service.RemoveUser(2, "TeachingStaff");

			Assert.AreEqual(2, _mockRepo.LastDeletedId);
			Assert.AreEqual("TeachingStaff", _mockRepo.LastDeletedRole);
		}

		[Test]
		public void RemoveUser_Administrator_PassesCorrectIdAndRole()
		{
			_service.RemoveUser(3, "Administrator");

			Assert.AreEqual(3, _mockRepo.LastDeletedId);
			Assert.AreEqual("Administrator", _mockRepo.LastDeletedRole);
		}

		// ==========================================
		// MODIFY STUDENT
		// ==========================================

		[Test]
		public void ModifyStudent_ValidData_CallsUpdateStudent()
		{
			_service.ModifyStudent(1, "Nguyen Van B", "0909999999", "b@mail.com");

			Assert.IsTrue(_mockRepo.UpdateStudentCalled);
		}

		[Test]
		public void ModifyStudent_ValidData_PassesCorrectDataToRepository()
		{
			_service.ModifyStudent(1, "Nguyen Van B", "0909999999", "b@mail.com");

			Assert.IsNotNull(_mockRepo.LastUpdatedStudent);
			Assert.AreEqual(1, _mockRepo.LastUpdatedStudent!.Id);
			Assert.AreEqual("Nguyen Van B", _mockRepo.LastUpdatedStudent!.Name);
			Assert.AreEqual("0909999999", _mockRepo.LastUpdatedStudent!.Phone);
			Assert.AreEqual("b@mail.com", _mockRepo.LastUpdatedStudent!.Email);
		}
	}
}