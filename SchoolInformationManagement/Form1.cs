using System.Data;
using Microsoft.Data.SqlClient;

namespace SchoolInformationManagement
{
	// =========================================================================================
	// UI LAYER (USER INTERFACE)
	// This layer is responsible for interacting with the user, capturing inputs, 
	// and displaying data. It should not contain complex business logic or database queries.
	// =========================================================================================

	public partial class Form1 : Form
	{
		// Dependency Injection: The UI layer depends on the Interface, not the concrete class.
		private readonly IUserService CRUDService;

		// Local state management to store currently displayed records
		private List<User> _currentData = new List<User>();
		private List<Subject> _currentSubjects = new List<Subject>();

		// Tracking variables for the currently selected item in the DataGridView
		private int _selectedUserId = -1;
		private string _selectedUserRole = "";

		/// <summary>
		/// Constructor for Form1. Initializes UI components and sets up Dependency Injection.
		/// </summary>
		public Form1()
		{
			InitializeComponent();

			// Manual Dependency Injection Setup
			// 1. Define the connection string to the SQL Server database.
			string connectionString = "Server=DESKTOP-HQP0AN8\\MSSQLSERVER01;Database=SchoolDB;Trusted_Connection=True;TrustServerCertificate=True;";

			// 2. Instantiate the Repository (Data Access Layer)
			IUserRepository repository = new UserRepository(connectionString);

			// 3. Inject the Repository into the Service (Business Logic Layer)
			// 4. Inject the Service into the Form (UI Layer)
			CRUDService = new UserService(repository);
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			// Load the default view (Students) when the application starts
			LoadData("Students");
		}

		// =========================================================================================
		// EVENT HANDLERS: These methods route UI events (clicks) to the appropriate logic methods
		// =========================================================================================

		private void Student_Click(object sender, EventArgs e) => LoadData("Students");
		private void Teaching_Click(object sender, EventArgs e) => LoadData("TeachingStaffs");
		private void Administrator_Click(object sender, EventArgs e) => LoadData("Administrators");
		private void ListAll_Click(object sender, EventArgs e) => LoadData("All");
		private void Subject_Click(object sender, EventArgs e) => LoadSubjects();
		private void Add_Click(object sender, EventArgs e) => AddStudent();
		private void Remove_Click(object sender, EventArgs e) => RemoveUser();
		private void Modify_Click(object sender, EventArgs e) => UpdateUser();

		// =========================================================================================
		// CONTROLLER LOGIC: Intermediate methods handling UI state and calling the Service Layer
		// =========================================================================================

		private void AddStudent()
		{
			try
			{
				// Extract and trim user input to remove leading/trailing whitespaces
				string name = tbName.Text.Trim();
				string phone = tbPhone.Text.Trim();
				string email = tbEmail.Text.Trim();

				// Pass the raw data to the Business Logic Layer
				CRUDService.CreateStudent(name, phone, email);

				// Clear input fields only if the creation was successful
				tbName.Clear();
				tbPhone.Clear();
				tbEmail.Clear();

				// Refresh the grid to show the newly added student
				LoadData("Students");
			}
			catch (Exception ex)
			{
				// Display user-friendly error messages (e.g., validation errors from BLL)
				MessageBox.Show("Error: " + ex.Message);
			}
		}

		private void RemoveUser()
		{
			// Guard clause: Ensure a user is actually selected before proceeding
			if (_selectedUserId == -1)
			{
				MessageBox.Show("Please select a row from the list to delete!");
				return;
			}

			// Prompt the user for confirmation to prevent accidental deletions
			var confirmResult = MessageBox.Show(
				$"Are you sure you want to delete this {_selectedUserRole}?",
				"Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

			if (confirmResult == DialogResult.Yes)
			{
				try
				{
					// Call the Service layer to perform the deletion
					CRUDService.RemoveUser(_selectedUserId, _selectedUserRole);
					MessageBox.Show("Deleted successfully!");

					// State Management: Save the current role so we can refresh the same list
					string roleToReload = _selectedUserRole + "s";

					// Reset tracking variables so no phantom user remains selected
					_selectedUserId = -1;
					_selectedUserRole = "";
					lbSelectedID.Text = "SelectedID is ";

					// Reload the grid using the saved role state
					LoadData(roleToReload);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error during deletion: " + ex.Message);
				}
			}
		}
		private void UpdateUser()
		{
			// Guard clause: Check if a user is selected AND if the user is a Student
			if (_selectedUserId == -1 || _selectedUserRole != "Student")
			{
				MessageBox.Show("Currently, the system only supports updating Student information!");
				return;
			}

			try
			{
				// Extract updated data from UI
				string name = tbName.Text.Trim();
				string phone = tbPhone.Text.Trim();
				string email = tbEmail.Text.Trim();

				// Send the ID and new data to the Business Logic Layer
				CRUDService.ModifyStudent(_selectedUserId, name, phone, email);

				MessageBox.Show("Information updated successfully!");

				// Clear UI inputs and reset selection state
				tbName.Clear();
				tbPhone.Clear();
				tbEmail.Clear();
				_selectedUserId = -1;

				// Refresh the list to reflect changes
				LoadData("Students");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error: " + ex.Message);
			}
		}

		private void LoadSubjects()
		{
			try
			{
				_currentSubjects = CRUDService.GetSubjects();

				// Clear existing bindings before applying new ones to avoid UI glitches
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
				// Retrieve polymorphic list of users from the Service Layer
				_currentData = CRUDService.GetUsers(filterRole);

				dataGridView1.DataSource = null;

				// IMPORTANT: We must cast the generic List<User> to specific derived types.
				// If we don't cast, the DataGridView will only show properties belonging to the base User class
				// and will hide derived properties (like Salary for Teachers, or Subjects for Students).
				if (filterRole == "Students")
					dataGridView1.DataSource = _currentData.Cast<Student>().ToList();
				else if (filterRole == "TeachingStaffs")
					dataGridView1.DataSource = _currentData.Cast<TeachingStaff>().ToList();
				else if (filterRole == "Administrators")
					dataGridView1.DataSource = _currentData.Cast<Administrator>().ToList();
				else
					dataGridView1.DataSource = _currentData; // Mixed list (All users)
			}
			catch (Exception ex)
			{
				MessageBox.Show("Connection error: " + ex.Message);
			}
		}

		private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			// Ignore clicks on table headers
			if (e.RowIndex < 0) return;

			// Extract the underlying object bound to the clicked row
			var boundItem = dataGridView1.Rows[e.RowIndex].DataBoundItem;

			// Use pattern matching to check if the clicked item is a User object
			if (boundItem is User selectedUser)
			{
				// Update label using the overridden polymorphic method
				lbSelectedID.Text = selectedUser.getInfo();

				// Store ID and Role for future operations (Update/Delete)
				_selectedUserId = selectedUser.Id;
				_selectedUserRole = selectedUser.Role;

				// Populate textboxes so the user can see/edit current values
				tbName.Text = selectedUser.Name;
				tbPhone.Text = selectedUser.Phone;
				tbEmail.Text = selectedUser.Email;
			}
			// Fallback pattern matching in case the Subject list is currently displayed
			else if (boundItem is Subject selectedSubject)
			{
				_selectedUserId = -1; // Reset user ID since a subject is selected
				lbSelectedID.Text = $"Selected Subject: {selectedSubject.SubjectId} - {selectedSubject.SubjectName}";
			}
		}
	}

	// =========================================================================================
	// BUSINESS LOGIC LAYER (BLL)
	// This layer acts as a bridge between UI and DAL. It enforces business rules, 
	// validations, and determines how data is processed before it reaches the database.
	// =========================================================================================

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
		// Reference to the Data Access Layer
		private readonly IUserRepository _repository;


		public UserService(IUserRepository repository)
		{
			_repository = repository;
		}

		public void CreateStudent(string name, string phone, string email)
		{
			// Business Validation: Ensure the required fields are not empty before proceeding to the database
			// Real-world systems would add email formatting checks (Regex) or duplicate checks here.
			if (string.IsNullOrEmpty(name)) throw new Exception("Name cannot be empty!");

			// Map primitive strings to a structured Domain Model (Student)
			var newStudent = new Student
			{
				Name = name,
				Phone = phone,
				Email = email
			};

			// Delegate the actual database insertion to the DAL
			_repository.AddStudent(newStudent);
		}


		public void RemoveUser(int id, string role)
		{
			// Business Validation: Validate ID. 
			if (id <= 0) throw new Exception("Please select a user to delete!");

			// Note: Authorization logic (e.g., Check if CurrentUser.Role == "Admin") 
			// would typically be placed here before allowing the deletion to proceed.

			_repository.DeleteUser(id, role);
		}

		public void ModifyStudent(int id, string name, string phone, string email)
		{
			// Business Validation: Prevent processing of invalid IDs or empty names
			if (id <= 0) throw new Exception("No student selected for modification!");
			if (string.IsNullOrEmpty(name)) throw new Exception("Name cannot be empty!");

			// Reconstruct the model with updated values
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
			// Direct pass-through to DAL. No complex business logic needed for fetching subjects.
			return _repository.GetSubjects();
		}

		/// <summary>
		/// Routes data retrieval requests based on classification strings.
		/// </summary>
		public List<User> GetUsers(string filterRole)
		{
			// Business Logic: Route the UI request to the specific repository method.
			// This prevents passing raw UI strings directly into SQL queries (Security & Separation of Concerns).
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

	// =========================================================================================
	// DATA ACCESS LAYER (DAL)
	// This layer is strictly responsible for communicating with the SQL Server Database.
	// It executes queries, handles transactions, and maps SQL result sets to C# Objects.
	// =========================================================================================

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
		// Connection string defining Server, DB name, and Authentication method
		private readonly string _connectionString;
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

			// Using statements ensure that database connections are properly closed and disposed of,
			// even if an exception occurs during execution.
			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand(query, conn))
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					// Read rows sequentially from the result set
					while (reader.Read())
					{
						subjects.Add(new Subject
						{
							SubjectId = (int)reader["Id"],
							SubjectName = reader["SubjectName"]?.ToString() // Handle potential NULLs
						});
					}
				}
			}
			return subjects;
		}

		public void AddStudent(Student student)
		{
			// Using parameterized queries (@Name, @Phone) instead of string concatenation 
			// is crucial to prevent SQL Injection attacks.
			string query = "INSERT INTO Students (Name, Phone, Email) VALUES (@Name, @Phone, @Email)";

			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					// If the object property is null, we must insert DBNull.Value into SQL Server.
					// C#'s null is not automatically recognized as SQL's NULL by ADO.NET.
					cmd.Parameters.AddWithValue("@Name", student.Name ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@Phone", student.Phone ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@Email", student.Email ?? (object)DBNull.Value);

					cmd.ExecuteNonQuery(); // Used for INSERT, UPDATE, DELETE statements
				}
			}
		}
		public void DeleteUser(int id, string role)
		{
			string tableName, idColumn, relationTable, relationColumn;

			// Dynamically determine which tables and columns to target based on the role string.
			// This prevents writing 3 separate delete methods for Student, Teacher, and Admin.
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
					relationTable = null; relationColumn = null; break; // Admins have no many-to-many subject table
				default: return; // Abort if role is unrecognized
			}

			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				conn.Open();

				// TRANSACTION: Ensures Atomic operations.
				// If deleting from the relationship table succeeds, but deleting from the main table fails,
				// the transaction will ROLLBACK and undo the first step to prevent orphaned or corrupted data.
				using (SqlTransaction tx = conn.BeginTransaction())
				{
					try
					{
						// Step 1: Delete from the many-to-many bridging table first (if it exists).
						// Failing to do this first would cause a Foreign Key Constraint violation in SQL Server.
						if (relationTable != null)
						{
							using (SqlCommand cmd = new SqlCommand(
								$"DELETE FROM {relationTable} WHERE {relationColumn} = @Id", conn, tx))
							{
								cmd.Parameters.AddWithValue("@Id", id);
								cmd.ExecuteNonQuery();
							}
						}

						// Step 2: Delete the main record from the parent table
						using (SqlCommand cmd = new SqlCommand(
							$"DELETE FROM {tableName} WHERE {idColumn} = @Id", conn, tx))
						{
							cmd.Parameters.AddWithValue("@Id", id);
							cmd.ExecuteNonQuery();
						}

						// Step 3: Confirm and save changes permanently to the database.
						tx.Commit();
					}
					catch
					{
						// If any error occurs, revert all changes made inside the transaction block.
						tx.Rollback();
						throw; // Re-throw exception to UI layer for error handling
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

					// Safely handle null updates
					cmd.Parameters.AddWithValue("@Name", student.Name ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@Phone", student.Phone ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@Email", student.Email ?? (object)DBNull.Value);

					cmd.ExecuteNonQuery();
				}
			}
		}

		// =========================================================================================
		// ABSTRACTION HELPERS: Methods that route specific queries into a general execution engine.
		// =========================================================================================

		public List<User> GetStudents() => ExecuteUserQuery(_queryStudent);
		public List<User> GetTeachingStaffs() => ExecuteUserQuery(_queryTeacher);
		public List<User> GetAdministrators() => ExecuteUserQuery(_queryAdmin);

		public List<User> GetAllUsers() => ExecuteUserQuery($"{_queryStudent} UNION ALL {_queryTeacher} UNION ALL {_queryAdmin}");

	
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
						// Extract base identifiers
						int id = Convert.ToInt32(reader["Id"]);
						string role = reader["UserRole"].ToString();

						// Generate a unique key for the dictionary (e.g., "Student_1" or "Administrator_5")
						string dictKey = $"{role}_{id}";

						// OBJECT INSTANTIATION: If this user hasn't been parsed yet, create them.
						if (!userDictionary.ContainsKey(dictKey))
						{
							User u; // Use base class reference for polymorphism

							// Instantiate specific derived class based on the SQL 'UserRole' column
							if (role == "Student")
							{
								u = new Student();
							}
							else if (role == "TeachingStaff")
							{
								u = new TeachingStaff { Salary = reader["Salary"] != DBNull.Value ? Convert.ToDecimal(reader["Salary"]) : 0m };
							}
							else
							{
								u = new Administrator
								{
									Salary = reader["Salary"] != DBNull.Value ? Convert.ToDecimal(reader["Salary"]) : 0m,
									IsFullTime = reader["IsFullTime"] != DBNull.Value && Convert.ToBoolean(reader["IsFullTime"])
								};
							}

							// Populate common base class properties
							u.Id = id;
							u.Name = reader["Name"]?.ToString();
							u.Phone = reader["Phone"]?.ToString();
							u.Email = reader["Email"]?.ToString();

							// Add newly created user to dictionary
							userDictionary[dictKey] = u;
						}

						// ONE-TO-MANY HANDLING: Process subjects if the SQL join returned subject data
						if (reader["SubjectId"] != DBNull.Value)
						{
							var subject = new Subject
							{
								SubjectId = Convert.ToInt32(reader["SubjectId"]),
								SubjectName = reader["SubjectName"].ToString()
							};

							// Retrieve the existing user from the dictionary
							var currentUser = userDictionary[dictKey];

							// Depending on their type, add the subject to their internal list
							if (currentUser is Student st)
								st.Subjects.Add(subject);
							else if (currentUser is TeachingStaff ts)
								ts.Subjects.Add(subject);
						}
					}
				}
			}
			// Convert Dictionary Values collection back into a standard List
			return userDictionary.Values.ToList();
		}
	}


	// =========================================================================================
	// MODEL LAYER (DOMAIN ENTITIES)
	// Represents the structure of the data in the application. Models should only contain
	// properties and lightweight data-formatting methods, no complex logic or SQL references.
	// =========================================================================================

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