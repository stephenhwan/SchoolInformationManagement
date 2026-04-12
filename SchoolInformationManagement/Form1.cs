using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SchoolInformationManagement
{

	// UI LAYER 

	public partial class Form1 : Form
	{
		private readonly IUserService _userService;

		private List<User> _currentData = new List<User>();
		private List<Subject> _currentSubjects = new List<Subject>();
		private int _selectedUserId = -1;
		private string _selectedUserRole = "";

		public Form1()
		{
			InitializeComponent();

			// Dependency Injection thủ công
			string connectionString = "Server=DESKTOP-HQP0AN8\\MSSQLSERVER01;Database=SchoolDB;Trusted_Connection=True;TrustServerCertificate=True;";

			IUserRepository repository = new UserRepository(connectionString);
			_userService = new UserService(repository);
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			LoadData("Students");
		}

						// --- Event Handlers ---
		private void Student_Click(object sender, EventArgs e) => LoadData("Students");
		private void Teaching_Click(object sender, EventArgs e) => LoadData("TeachingStaffs");
		private void Administrator_Click(object sender, EventArgs e) => LoadData("Administrators");
		private void ListAll_Click(object sender, EventArgs e) => LoadData("All");
		private void Subject_Click(object sender, EventArgs e) => LoadSubjects();
		private void Add_Click(object sender, EventArgs e) => AddStudent();
		private void Remove_Click(object sender, EventArgs e) => RemoveUser();
		private void Modify_Click(object sender, EventArgs e) => UpdateUser();

						// --- Controller Logic ---
		private void AddStudent()
		{
			try
			{
				string name = tbName.Text.Trim();
				string phone = tbPhone.Text.Trim();
				string email = tbEmail.Text.Trim();
				_userService.CreateStudent(name, phone, email);
				tbName.Clear();
				tbPhone.Clear();
				tbEmail.Clear();
				LoadData("Students");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error: " + ex.Message);
			}
		}
		private void RemoveUser()
		{
			if (_selectedUserId == -1)
			{
				MessageBox.Show("Vui lòng chọn một dòng trong danh sách để xóa!");
				return;
			}

			var confirmResult = MessageBox.Show(
				$"Bạn có chắc chắn muốn xóa {_selectedUserRole} này không?",
				"Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

			if (confirmResult == DialogResult.Yes)
			{
				try
				{
					_userService.RemoveUser(_selectedUserId, _selectedUserRole);
					MessageBox.Show("Xóa thành công!");

					string roleToReload = _selectedUserRole + "s"; // Lưu lại TRƯỚC khi reset
					_selectedUserId = -1;
					_selectedUserRole = "";                         // Reset luôn role
					lbSelectedID.Text = "SelectedID is ";

					LoadData(roleToReload);                         // Dùng bản đã lưu
				}
				catch (Exception ex)
				{
					MessageBox.Show("Lỗi khi xóa: " + ex.Message);
				}
			}
		}
	

		private void UpdateUser()
		{
			if (_selectedUserId == -1 || _selectedUserRole != "Student")
			{
				MessageBox.Show("Hiện tại hệ thống chỉ hỗ trợ sửa thông tin cho Sinh viên!");
				return;
			}

			try
			{
				string name = tbName.Text.Trim();
				string phone = tbPhone.Text.Trim();
				string email = tbEmail.Text.Trim();

				_userService.ModifyStudent(_selectedUserId, name, phone, email);

				MessageBox.Show("Cập nhật thông tin thành công!");

				// Làm sạch UI và load lại dữ liệu
				tbName.Clear();
				tbPhone.Clear();
				tbEmail.Clear();
				_selectedUserId = -1;
				LoadData("Students");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi: " + ex.Message);
			}
		}
		private void LoadSubjects()
		{
			try
			{
				_currentSubjects = _userService.GetSubjects();
				dataGridView1.DataSource = null;
				dataGridView1.DataSource = _currentSubjects;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error: " + ex.Message);
			}
		}

		private void LoadData(string filterRole)
		{
			try
			{
				_currentData = _userService.GetUsers(filterRole);

				dataGridView1.DataSource = null;
				if (filterRole == "Students")
					dataGridView1.DataSource = _currentData.Cast<Student>().ToList();
				else if (filterRole == "TeachingStaffs")
					dataGridView1.DataSource = _currentData.Cast<TeachingStaff>().ToList();
				else if (filterRole == "Administrators")
					dataGridView1.DataSource = _currentData.Cast<Administrator>().ToList();
				else
					dataGridView1.DataSource = _currentData;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi kết nối: " + ex.Message);
			}
		}

		private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0) return;

			var boundItem = dataGridView1.Rows[e.RowIndex].DataBoundItem;

			if (boundItem is User selectedUser)
			{
				lbSelectedID.Text = selectedUser.getInfo();
				_selectedUserId = selectedUser.Id;
				_selectedUserRole = selectedUser.Role;
				tbName.Text = selectedUser.Name;
				tbPhone.Text = selectedUser.Phone;
				tbEmail.Text = selectedUser.Email;
			}
			else if (boundItem is Subject selectedSubject)
			{
				_selectedUserId = -1;
				lbSelectedID.Text = $"Selected Subject: {selectedSubject.SubjectId} - {selectedSubject.SubjectName}";
			}
		}

	}


	// BUSINESS LOGIC LAYER 


	public interface IUserService
	{
		void ModifyStudent(int id, string name, string phone, string email);
		void CreateStudent(string name, string phone, string email);
		void RemoveUser(int id, string role);
		List<Subject> GetSubjects();
		List<User> GetUsers(string filterRole);
	}

	public class UserService : IUserService
	{
		private readonly IUserRepository _repository;

		public UserService(IUserRepository repository)
		{
			_repository = repository;
		}
		public void CreateStudent(string name, string phone, string email)
		{
			// Bạn có thể thêm logic kiểm tra ở đây (ví dụ: email phải có đuôi @school.edu)
			if (string.IsNullOrEmpty(name)) throw new Exception("Tên không được để trống!");

			var newStudent = new Student
			{
				Name = name,
				Phone = phone,
				Email = email
			};
			_repository.AddStudent(newStudent);
		}

		public void RemoveUser(int id, string role)
		{
			if (id <= 0) throw new Exception("Vui lòng chọn người dùng cần xóa!");

			// Bạn có thể thêm logic kiểm tra quyền ở đây (vd: Chỉ Admin mới được xóa)
			_repository.DeleteUser(id, role);
		}

		public void ModifyStudent(int id, string name, string phone, string email)
		{
			if (id <= 0) throw new Exception("Chưa chọn sinh viên cần sửa!");
			if (string.IsNullOrEmpty(name)) throw new Exception("Tên không được để trống!");

			var updatedStudent = new Student
			{
				Id = id,
				Name = name,
				Phone = phone,
				Email = email
			};

			_repository.UpdateStudent(updatedStudent);
		}

		public List<Subject> GetSubjects()
		{
			return _repository.GetSubjects();
		}

		public List<User> GetUsers(string filterRole)
		{
			// Business Logic: Điều hướng lấy dữ liệu dựa trên phân loại thay vì ném thẳng chuỗi xuống SQL
			switch (filterRole)
			{
				case "Students":
					return _repository.GetStudents();
				case "TeachingStaffs":
					return _repository.GetTeachingStaffs();
				case "Administrators":
					return _repository.GetAdministrators();
				case "All":
				default:
					return _repository.GetAllUsers();
			}
		}
	}

	// ==========================================
	// DATA ACCESS LAYER (TẦNG TRUY CẬP DỮ LIỆU)
	// ==========================================

	public interface IUserRepository
	{
		void UpdateStudent(Student student);
		void AddStudent(Student student);
		void DeleteUser(int id, string role);
		List<Subject> GetSubjects();
		List<User> GetStudents();
		List<User> GetTeachingStaffs();
		List<User> GetAdministrators();
		List<User> GetAllUsers();
	}

	public class UserRepository : IUserRepository
	{
		private readonly string _connectionString;

		// management query
		private readonly string _queryStudent = @"
        SELECT st.StudentId AS Id, st.Name, st.Phone, st.Email, CAST(NULL AS DECIMAL(18,2)) AS Salary, CAST(NULL AS BIT) AS IsFullTime, 'Student' AS UserRole,
               su.SubjectId, su.SubjectName
        FROM Students st
        LEFT JOIN StudentSubjects ss ON st.StudentId = ss.StudentId
        LEFT JOIN Subjects su ON ss.SubjectId = su.SubjectId";

		private readonly string _queryTeacher = @"
        SELECT ts.TeacherId AS Id, ts.Name, ts.Phone, ts.Email, ts.Salary, CAST(NULL AS BIT) AS IsFullTime, 'TeachingStaff' AS UserRole,
               su.SubjectId, su.SubjectName
        FROM TeachingStaffs ts
        LEFT JOIN TeacherSubjects tsub ON ts.TeacherId = tsub.TeacherId
        LEFT JOIN Subjects su ON tsub.SubjectId = su.SubjectId";

		private readonly string _queryAdmin = @"
        SELECT ad.AdminId AS Id, ad.Name, ad.Phone, ad.Email, ad.Salary, ad.IsFullTime, 'Administrator' AS UserRole,
               CAST(NULL AS INT) AS SubjectId, CAST(NULL AS NVARCHAR(MAX)) AS SubjectName
        FROM Administrators ad";

		public UserRepository(string connectionString)
		{
			_connectionString = connectionString;
		}

		public List<Subject> GetSubjects()
		{
			var subjects = new List<Subject>();
			string query = "SELECT SubjectId AS Id, SubjectName FROM Subjects";
			// logic data mapping of getSubject
			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand(query, conn))
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						subjects.Add(new Subject
						{
							SubjectId = (int)reader["Id"],
							SubjectName = reader["SubjectName"]?.ToString()
						});
					}
				}
			}
			return subjects;
		}
		public void AddStudent(Student student)
		{
			string query = "INSERT INTO Students (Name, Phone, Email) VALUES (@Name, @Phone, @Email)";
			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@Name", student.Name ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@Phone", student.Phone ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@Email", student.Email ?? (object)DBNull.Value);
					cmd.ExecuteNonQuery();
				}
			}
		}
		public void DeleteUser(int id, string role)
		{
			string tableName, idColumn, relationTable, relationColumn;

			switch (role)
			{
				case "Student":
					tableName = "Students"; idColumn = "StudentId";
					relationTable = "StudentSubjects"; relationColumn = "StudentId"; break;
				case "TeachingStaff":
					tableName = "TeachingStaffs"; idColumn = "TeacherId";
					relationTable = "TeacherSubjects"; relationColumn = "TeacherId"; break;
				case "Administrator":
					tableName = "Administrators"; idColumn = "AdminId";
					relationTable = null; relationColumn = null; break;
				default: return;
			}

			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				conn.Open();
				using (SqlTransaction tx = conn.BeginTransaction())
				{
					try
					{
						// Bước 1: Xóa bảng quan hệ trước (nếu có)
						if (relationTable != null)
						{
							using (SqlCommand cmd = new SqlCommand(
								$"DELETE FROM {relationTable} WHERE {relationColumn} = @Id", conn, tx))
							{
								cmd.Parameters.AddWithValue("@Id", id);
								cmd.ExecuteNonQuery();
							}
						}
						// Bước 2: Xóa bản ghi chính
						using (SqlCommand cmd = new SqlCommand(
							$"DELETE FROM {tableName} WHERE {idColumn} = @Id", conn, tx))
						{
							cmd.Parameters.AddWithValue("@Id", id);
							cmd.ExecuteNonQuery();
						}
						tx.Commit();
					}
					catch
					{
						tx.Rollback();
						throw;
					}
				}
			}
		}

		public void UpdateStudent(Student student)
		{
			string query = "UPDATE Students SET Name = @Name, Phone = @Phone, Email = @Email WHERE StudentId = @Id";

			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@Id", student.Id);
					cmd.Parameters.AddWithValue("@Name", student.Name ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@Phone", student.Phone ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@Email", student.Email ?? (object)DBNull.Value);

					cmd.ExecuteNonQuery();
				}
			}
		}


		// Triển khai các Interface gọn gàng, truyền câu query vào hàm helper
		public List<User> GetStudents() => ExecuteUserQuery(_queryStudent);
		public List<User> GetTeachingStaffs() => ExecuteUserQuery(_queryTeacher);
		public List<User> GetAdministrators() => ExecuteUserQuery(_queryAdmin);
		public List<User> GetAllUsers() => ExecuteUserQuery($"{_queryStudent} UNION ALL {_queryTeacher} UNION ALL {_queryAdmin}");

		// Logic data mapping
		private List<User> ExecuteUserQuery(string query)
		{
			var userDictionary = new Dictionary<string, User>();

			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand(query, conn))
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						int id = Convert.ToInt32(reader["Id"]);
						string role = reader["UserRole"].ToString();
						string dictKey = $"{role}_{id}";

						if (!userDictionary.ContainsKey(dictKey))
						{
							User u;
							if (role == "Student")
								u = new Student();
							else if (role == "TeachingStaff")
								u = new TeachingStaff { Salary = reader["Salary"] != DBNull.Value ? Convert.ToDecimal(reader["Salary"]) : 0m };
							else
								u = new Administrator
								{
									Salary = reader["Salary"] != DBNull.Value ? Convert.ToDecimal(reader["Salary"]) : 0m,
									IsFullTime = reader["IsFullTime"] != DBNull.Value && Convert.ToBoolean(reader["IsFullTime"])
								};

							u.Id = id;
							u.Name = reader["Name"]?.ToString();
							u.Phone = reader["Phone"]?.ToString();
							u.Email = reader["Email"]?.ToString();
							userDictionary[dictKey] = u;
						}

						// Nếu có dữ liệu Subject đi kèm
						if (reader["SubjectId"] != DBNull.Value)
						{
							var subject = new Subject
							{
								SubjectId = Convert.ToInt32(reader["SubjectId"]),
								SubjectName = reader["SubjectName"].ToString()
							};

							var currentUser = userDictionary[dictKey];
							if (currentUser is Student st)
								st.Subjects.Add(subject);
							else if (currentUser is TeachingStaff ts)
								ts.Subjects.Add(subject);
						}
					}
				}
			}
			return userDictionary.Values.ToList();
		}
	}


	// MODEL LAYER (TẦNG DỮ LIỆU)
	

	public abstract class User
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }
		public virtual string Role => "User";
		public virtual string getInfo()
		{
			return $"{Name} - {Phone} - {Email} ({Role})";
		}
	}

	public class Subject
	{
		public int SubjectId { get; set; }
		public string SubjectName { get; set; }
	}

	public class Student : User
	{
		public override string Role => "Student";
		public List<Subject> Subjects { get; set; } = new List<Subject>();

		public string AssignedSubjects => string.Join(", ", Subjects.Select(s => s.SubjectName));

		public override string getInfo() => base.getInfo() + $" - Subjects: {AssignedSubjects}";
	}

	public class TeachingStaff : User
	{
		public override string Role => "TeachingStaff";
		public decimal Salary { get; set; }
		public List<Subject> Subjects { get; set; } = new List<Subject>();

		public string AssignedSubjects => string.Join(", ", Subjects.Select(s => s.SubjectName));

		public override string getInfo() => base.getInfo() + $" - Salary: {Salary:C} - Subjects: {AssignedSubjects}";
	}

	public class Administrator : User
	{
		public override string Role => "Administrator";
		public decimal Salary { get; set; }
		public bool IsFullTime { get; set; }
		public override string getInfo() => base.getInfo() + $" - Salary: {Salary:C} - FullTime: {IsFullTime}";
	}
}