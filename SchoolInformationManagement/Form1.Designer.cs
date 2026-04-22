using System.Xml.Linq;

namespace SchoolInformationManagement
{
	partial class Form1
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			btnListStudent = new Button();
			btnTeachingStaff = new Button();
			btnAdministrator = new Button();
			btnListAll = new Button();
			dataGridView1 = new DataGridView();
			label1 = new Label();
			btnAdd = new Button();
			btnRemove = new Button();
			btnModify = new Button();
			btnSubject = new Button();
			lbSelectedID = new Label();
			tbName = new TextBox();
			tbPhone = new TextBox();
			tbEmail = new TextBox();
			label2 = new Label();
			label3 = new Label();
			label4 = new Label();
			label5 = new Label();
			panel1 = new Panel();
			label6 = new Label();
			panel2 = new Panel();
			((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
			panel1.SuspendLayout();
			panel2.SuspendLayout();
			SuspendLayout();
			// 
			// btnListStudent
			// 
			btnListStudent.BackColor = SystemColors.Highlight;
			btnListStudent.ForeColor = SystemColors.ButtonHighlight;
			btnListStudent.Location = new Point(828, 122);
			btnListStudent.Name = "btnListStudent";
			btnListStudent.Size = new Size(165, 61);
			btnListStudent.TabIndex = 0;
			btnListStudent.Text = "Student";
			btnListStudent.UseVisualStyleBackColor = false;
			btnListStudent.Click += Student_Click;
			// 
			// btnTeachingStaff
			// 
			btnTeachingStaff.BackColor = SystemColors.Highlight;
			btnTeachingStaff.ForeColor = SystemColors.Control;
			btnTeachingStaff.Location = new Point(999, 122);
			btnTeachingStaff.Name = "btnTeachingStaff";
			btnTeachingStaff.Size = new Size(170, 61);
			btnTeachingStaff.TabIndex = 1;
			btnTeachingStaff.Text = "Teaching staff";
			btnTeachingStaff.UseVisualStyleBackColor = false;
			btnTeachingStaff.Click += Teaching_Click;
			// 
			// btnAdministrator
			// 
			btnAdministrator.BackColor = SystemColors.Highlight;
			btnAdministrator.ForeColor = SystemColors.Control;
			btnAdministrator.Location = new Point(1175, 122);
			btnAdministrator.Name = "btnAdministrator";
			btnAdministrator.Size = new Size(165, 61);
			btnAdministrator.TabIndex = 2;
			btnAdministrator.Text = "Administrator";
			btnAdministrator.UseVisualStyleBackColor = false;
			btnAdministrator.Click += Administrator_Click;
			// 
			// btnListAll
			// 
			btnListAll.BackColor = SystemColors.Highlight;
			btnListAll.ForeColor = Color.White;
			btnListAll.Location = new Point(657, 121);
			btnListAll.Name = "btnListAll";
			btnListAll.Size = new Size(165, 63);
			btnListAll.TabIndex = 3;
			btnListAll.Text = "List All";
			btnListAll.UseVisualStyleBackColor = false;
			btnListAll.Click += ListAll_Click;
			// 
			// dataGridView1
			// 
			dataGridView1.BackgroundColor = SystemColors.ButtonHighlight;
			dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridView1.Location = new Point(12, 377);
			dataGridView1.Name = "dataGridView1";
			dataGridView1.RowHeadersWidth = 51;
			dataGridView1.Size = new Size(839, 353);
			dataGridView1.TabIndex = 4;
			dataGridView1.CellClick += dataGridView1_CellClick;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.BackColor = Color.White;
			label1.FlatStyle = FlatStyle.System;
			label1.Font = new Font("Segoe UI", 22F);
			label1.Location = new Point(466, 30);
			label1.Name = "label1";
			label1.Size = new Size(565, 50);
			label1.TabIndex = 5;
			label1.Text = "School Information Management";

			// 
			// btnAdd
			// 
			btnAdd.BackColor = Color.Lime;
			btnAdd.ForeColor = Color.Black;
			btnAdd.Location = new Point(27, 259);
			btnAdd.Name = "btnAdd";
			btnAdd.Size = new Size(138, 29);
			btnAdd.TabIndex = 6;
			btnAdd.Text = "Add Student";
			btnAdd.UseVisualStyleBackColor = false;
			btnAdd.Click += Add_Click;
			// 
			// btnRemove
			// 
			btnRemove.BackColor = Color.Red;
			btnRemove.ForeColor = Color.White;
			btnRemove.Location = new Point(12, 342);
			btnRemove.Name = "btnRemove";
			btnRemove.Size = new Size(117, 29);
			btnRemove.TabIndex = 7;
			btnRemove.Text = "Remove user";
			btnRemove.UseVisualStyleBackColor = false;
			btnRemove.Click += Remove_Click;
			// 
			// btnModify
			// 
			btnModify.BackColor = Color.Yellow;
			btnModify.Location = new Point(187, 259);
			btnModify.Name = "btnModify";
			btnModify.Size = new Size(138, 29);
			btnModify.TabIndex = 8;
			btnModify.Text = "Update Student";
			btnModify.UseVisualStyleBackColor = false;
			btnModify.Click += Modify_Click;
			// 
			// btnSubject
			// 
			btnSubject.BackColor = SystemColors.Highlight;
			btnSubject.ForeColor = SystemColors.Control;
			btnSubject.Location = new Point(1346, 122);
			btnSubject.Name = "btnSubject";
			btnSubject.Size = new Size(165, 61);
			btnSubject.TabIndex = 9;
			btnSubject.Text = "Subject";
			btnSubject.UseVisualStyleBackColor = false;
			btnSubject.Click += Subject_Click;
			// 
			// lbSelectedID
			// 
			lbSelectedID.AutoSize = true;
			lbSelectedID.BackColor = Color.AliceBlue;
			lbSelectedID.Location = new Point(12, 319);
			lbSelectedID.Name = "lbSelectedID";
			lbSelectedID.RightToLeft = RightToLeft.No;
			lbSelectedID.Size = new Size(122, 20);
			lbSelectedID.TabIndex = 10;
			lbSelectedID.Text = "User selected is : ";
			// 
			// tbName
			// 
			tbName.Location = new Point(90, 96);
			tbName.Name = "tbName";
			tbName.Size = new Size(219, 27);
			tbName.TabIndex = 11;
			// 
			// tbPhone
			// 
			tbPhone.Location = new Point(90, 149);
			tbPhone.Name = "tbPhone";
			tbPhone.Size = new Size(219, 27);
			tbPhone.TabIndex = 12;
			// 
			// tbEmail
			// 
			tbEmail.Location = new Point(90, 204);
			tbEmail.Name = "tbEmail";
			tbEmail.Size = new Size(219, 27);
			tbEmail.TabIndex = 13;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Font = new Font("Segoe UI", 12F);
			label2.Location = new Point(11, 96);
			label2.Name = "label2";
			label2.Size = new Size(73, 28);
			label2.TabIndex = 15;
			label2.Text = "Name :";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Font = new Font("Segoe UI", 12F);
			label3.Location = new Point(8, 145);
			label3.Name = "label3";
			label3.Size = new Size(76, 28);
			label3.TabIndex = 16;
			label3.Text = "Phone :";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Font = new Font("Segoe UI", 12F);
			label4.Location = new Point(11, 200);
			label4.Name = "label4";
			label4.Size = new Size(73, 28);
			label4.TabIndex = 17;
			label4.Text = "Email  :";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Font = new Font("Segoe UI", 16F);
			label5.Location = new Point(78, 30);
			label5.Name = "label5";
			label5.Size = new Size(181, 37);
			label5.TabIndex = 18;
			label5.Text = "Student panel";
			// 
			// panel1
			// 
			panel1.BackgroundImage = (Image)resources.GetObject("panel1.BackgroundImage");
			panel1.Controls.Add(label1);
			panel1.Controls.Add(btnListStudent);
			panel1.Controls.Add(btnSubject);
			panel1.Controls.Add(btnAdministrator);
			panel1.Controls.Add(btnTeachingStaff);
			panel1.Controls.Add(btnListAll);
			panel1.Location = new Point(-17, 12);
			panel1.Name = "panel1";
			panel1.Size = new Size(1543, 183);
			panel1.TabIndex = 19;
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Font = new Font("Segoe UI", 22F);
			label6.Location = new Point(67, 225);
			label6.Name = "label6";
			label6.Size = new Size(273, 50);
			label6.TabIndex = 20;
			label6.Text = "The List of User";
			// 
			// panel2
			// 
			panel2.BackColor = Color.White;
			panel2.Controls.Add(label5);
			panel2.Controls.Add(label2);
			panel2.Controls.Add(label3);
			panel2.Controls.Add(tbEmail);
			panel2.Controls.Add(btnModify);
			panel2.Controls.Add(label4);
			panel2.Controls.Add(tbPhone);
			panel2.Controls.Add(btnAdd);
			panel2.Controls.Add(tbName);
			panel2.Location = new Point(857, 377);
			panel2.Name = "panel2";
			panel2.Size = new Size(347, 353);
			panel2.TabIndex = 21;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(8F, 20F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1528, 763);
			Controls.Add(panel2);
			Controls.Add(label6);
			Controls.Add(panel1);
			Controls.Add(lbSelectedID);
			Controls.Add(btnRemove);
			Controls.Add(dataGridView1);
			Name = "Form1";
			Text = "Form1";
			Load += Form1_Load;
			((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			panel2.ResumeLayout(false);
			panel2.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Button btnListStudent;
		private Button btnTeachingStaff;
		private Button btnAdministrator;
		private Button btnListAll;
		private DataGridView dataGridView1;
		private Label label1;
		private Button btnAdd;
		private Button btnRemove;
		private Button btnModify;
		private Button btnSubject;
		private Label lbSelectedID;
		private TextBox tbName;
		private TextBox tbPhone;
		private TextBox tbEmail;
		private Label label2;
		private Label label3;
		private Label label4;
		private Label label5;
		private Panel panel1;
		private Label label6;
		private Panel panel2;
	}
}
