using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid.Views.Layout.Modes;

namespace Shopping
{
    public partial class CreateOrEditEmployeeForm : DevExpress.XtraEditors.XtraForm
    {
        private static CreateOrEditEmployeeForm instance;
        private static int id = 0;
        private CreateOrEditEmployeeForm()
        {
            InitializeComponent();
      
        }

        public static CreateOrEditEmployeeForm GetInstance()
        {
            if (instance == null)
                instance = new CreateOrEditEmployeeForm();

            return instance;
        }

        private void CreateEmployee()
        {
            using (var db = new ShoppingContext())
            {
                var query = db.Employees
                            .Where(x => x.EmployeeID == 2);

                var employee = query.FirstOrDefault();

                var newEmployee = new Employee {
                    FirstName = firstName.Text.ToString(),
                    LastName = lastName.Text.ToString(),
                    Title = title.Text.ToString(),
                    TitleOfCourtesy = titleOfCoursety.Text.ToString(),
                    Extension = employee.Extension,
                    Address = address.Text.ToString(),
                    BirthDate = employee.BirthDate,
                    HireDate = DateTime.Now,
                    City = city.Text.ToString(),
                    Country = country.Text.ToString(),
                    PhotoPath = employee.PhotoPath,
                    Region = region.Text.ToString(),
                    HomePhone = homePhone.Text.ToString(),
                    Photo = employee.Photo,
                    PostalCode = postalCode.Text.ToString()
                };
                db.Employees.Add(newEmployee);
                db.SaveChanges();
                MainForm mainForm = new MainForm();
                mainForm.Show();
                Hide();

            }
        }

        private void GetEmployee()
        {
            using (var db = new ShoppingContext())
            {
                var query = db.Employees
                            .Where(x => x.EmployeeID == id);

                var employee = query.FirstOrDefault();
                if (employee == null)
                    return;
                employeeId.Text = employee.EmployeeID.ToString();
                firstName.Text = employee.FirstName == null ? "" :
                    employee.FirstName.ToString();
                lastName.Text = employee.LastName == null ? "" : 
                    employee.LastName.ToString() + "";
                title.Text = employee.Title == null ? "" : 
                    employee.Title.ToString();
                titleOfCoursety.Text = employee.TitleOfCourtesy == null ? "" :
                    employee.TitleOfCourtesy.ToString();
                address.Text = employee.Address == null ? "" :
                    employee.Address.ToString();
                city.Text = employee.City == null ? "" :
                    employee.City.ToString();
                region.Text = employee.Region == null ? "" :
                    employee.Region.ToString();
                postalCode.Text = employee.PostalCode == null ? "" :
                    employee.PostalCode.ToString();
                country.Text = employee.Country == null ? "" :
                    employee.Country.ToString();
                homePhone.Text = employee.HomePhone == null ? "" :
                    employee.HomePhone.ToString();

            }
        }

        private void CreateOrEdit_Click(object sender, EventArgs e)
        {
            if (id == 0)
                CreateEmployee();
            else
                EditEmployee(); 
        }

        private void EditEmployee()
        {
            using (var db = new ShoppingContext())
            {
                var query = db.Employees
                            .Where(x => x.EmployeeID == id);

                var employee = query.FirstOrDefault();
                if (employee == null)
                    return;
                employee.FirstName = firstName.Text.ToString() + "";
                employee.LastName = lastName.Text.ToString() + "";
                employee.Title = title.Text.ToString() + "";
                employee.TitleOfCourtesy = titleOfCoursety.Text.ToString() + "";
                employee.Address = address.Text.ToString() + "";
                employee.City = city.Text.ToString() + "";
                employee.Region = region.Text.ToString();
                employee.PostalCode = postalCode.Text.ToString() + "";
                employee.Country = country.Text.ToString() + "";
                employee.HomePhone = homePhone.Text.ToString() + "";

                db.SaveChanges();
                MainForm mainForm = new MainForm();
                mainForm.Show();
                Hide();
            }
        }

        public void SetEmployeeId(int employeeId)
        {
            id = employeeId;
            if (id != 0)
            {
                createOrEdit.Text = "Edit";
                instance.GetEmployee();
            }else
                createOrEdit.Text = "Create";

        }

        private void Delete_Click(object sender, EventArgs e)
        {
            DXMenuItem menuItem = sender as DXMenuItem;
            if (XtraMessageBox.Show("Are you sure" + " ?", "Delete Employee", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;


            using (var db = new ShoppingContext())
            {
                var query = db.Employees
                            .Where(x => x.EmployeeID == id);

                var employee = query.FirstOrDefault();
                db.Employees.Remove(employee);
                db.SaveChanges();
                MainForm mainForm = new MainForm();
                mainForm.Show();
                Hide();

            }
        }

        private void Back_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm();
            mainForm.Show();
            Hide();
        }
    }
}