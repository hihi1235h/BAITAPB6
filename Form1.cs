using BTB6_01.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTB6_01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Model1 context = new Model1();

                List<Faculty> listFaculties = context.Faculties.ToList();
                List<Student> listStudents = context.Students.ToList();

                FillFacultyCombobox(listFaculties);
                BindGrid(listStudents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillFacultyCombobox(List<Faculty> listFaculties)
        {
            cmbFaculty.DataSource = listFaculties;
            cmbFaculty.DisplayMember = "FacultyName";
            cmbFaculty.ValueMember = "FacultyID";
        }

        private void BindGrid(List<Student> listStudents)
        {
            dgvStudent.Rows.Clear();
            foreach (var item in listStudents)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = item.FullName;
                dgvStudent.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                dgvStudent.Rows[index].Cells[3].Value = item.AverageScore;
            }
        }

        void ResetForm()
        {
            txtMa.Clear();
            txtTen.Clear();
            txtDiem.Clear();
            cmbFaculty.SelectedIndex = 0;
        }

        bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtMa.Text) ||
                string.IsNullOrWhiteSpace(txtTen.Text) ||
                string.IsNullOrWhiteSpace(txtDiem.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return false;
            }

            if (txtMa.Text.Length != 10)
            {
                MessageBox.Show("Mã sinh viên phải có 10 ký tự!");
                return false;
            }

            if (!double.TryParse(txtDiem.Text, out _))
            {
                MessageBox.Show("Điểm trung bình phải là số!");
                return false;
            }

            return true;
        }

        void LoadData()
        {
            try
            {
                using (Model1 context = new Model1())
                {
                    List<Faculty> listFaculties = context.Faculties.ToList();
                    List<Student> listStudents = context.Students.ToList();

                    FillFacultyCombobox(listFaculties);
                    BindGrid(listStudents);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void dgvStudent_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtMa.Text = dgvStudent.Rows[e.RowIndex].Cells[0].Value.ToString();
                txtTen.Text = dgvStudent.Rows[e.RowIndex].Cells[1].Value.ToString();
                cmbFaculty.Text = dgvStudent.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtDiem.Text = dgvStudent.Rows[e.RowIndex].Cells[3].Value.ToString();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            using (Model1 context = new Model1())
            {
                var exist = context.Students.Find(txtMa.Text);
                if (exist != null)
                {
                    MessageBox.Show("MSSV đã tồn tại!");
                    return;
                }

                Student sv = new Student
                {
                    StudentID = txtMa.Text,
                    FullName = txtTen.Text,
                    AverageScore = double.Parse(txtDiem.Text),
                    FacultyID = (int)cmbFaculty.SelectedValue
                };

                context.Students.Add(sv);
                context.SaveChanges();
            }

            MessageBox.Show("Thêm mới dữ liệu thành công!");
            LoadData();
            ResetForm();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            using (Model1 context = new Model1())
            {
                var sv = context.Students.Find(txtMa.Text);
                if (sv == null)
                {
                    MessageBox.Show("Không tìm thấy MSSV cần sửa!");
                    return;
                }

                sv.FullName = txtTen.Text;
                sv.AverageScore = double.Parse(txtDiem.Text);
                sv.FacultyID = (int)cmbFaculty.SelectedValue;

                context.SaveChanges();
            }

            MessageBox.Show("Cập nhật dữ liệu thành công!");
            LoadData();
            ResetForm();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            using (Model1 context = new Model1())
            {
                var sv = context.Students.Find(txtMa.Text);
                if (sv == null)
                {
                    MessageBox.Show("Không tìm thấy MSSV cần xóa!");
                    return;
                }

                if (MessageBox.Show("Bạn có chắc muốn xóa sinh viên này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    context.Students.Remove(sv);
                    context.SaveChanges();
                    MessageBox.Show("Xóa sinh viên thành công!");
                }
            }

            LoadData();
            ResetForm();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}