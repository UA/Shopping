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
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;
using System.Threading;

namespace Shopping
{
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        public MainForm()
        {
            InitializeComponent();
        }
        private void tileBar_SelectedItemChanged(object sender, TileItemEventArgs e)
        {
            navigationFrame.SelectedPageIndex = tileBarGroupTables.Items.IndexOf(e.Item);

            if((DataListType)navigationFrame.SelectedPageIndex == DataListType.Employees)
            {
                GetEmployees();
            }else if((DataListType)navigationFrame.SelectedPageIndex == DataListType.Orders)
            {
                GetOrders();
            }
            else if ((DataListType)navigationFrame.SelectedPageIndex == DataListType.Products)
            {
                GetProducts();
            }
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            Thread.Sleep(2000);
            gridView1.Columns.View.OptionsBehavior.EditorShowMode = EditorShowMode.MouseUp;
            GetEmployees();
        }

        //Grid view e click oldugunda edit ve delete islemlerini yapabilirsiniz
        private void GridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            //tiklanan row un employee id si
            int employeeId = Convert.ToInt32((sender as GridView).GetFocusedRowCellValue("EmployeeID"));

            CreateOrEditEmployeeForm editEmployeeForm = CreateOrEditEmployeeForm.GetInstance();

            editEmployeeForm.Show();
            this.Hide();
            editEmployeeForm.SetEmployeeId(employeeId);
        }

        //Yeni bir employee eklemek icin
        private void NewEmployee_Click(object sender, EventArgs e)
        {
            if ((DataListType)navigationFrame.SelectedPageIndex == DataListType.Employees)
            {
                CreateOrEditEmployeeForm editEmployeeForm = CreateOrEditEmployeeForm.GetInstance();
                editEmployeeForm.Show();
                this.Hide();
                editEmployeeForm.SetEmployeeId(0);

            }
        }

        private void GetProducts()
        {
            using (var db = new ShoppingContext())
            {
                var query = from p in db.Products
                            join c in db.Categories on p.CategoryID equals c.CategoryID
                            select new
                            {
                                Picture = c.Picture,
                                ProductName = p.ProductName,
                                UnitPrice = p.UnitPrice,
                                UnitInStock = p.UnitsInStock,
                                CategoryName = c.CategoryName
                            };

                gridControl1.DataSource = query.ToList();
            }
        }

        private void GetOrders()
        {
            using (var db = new ShoppingContext())
            {
                var query = from o in db.Orders
                            join c in db.Customers on o.CustomerID equals c.CustomerID
                            join e in db.Employees on o.EmployeeID equals e.EmployeeID
                            select new
                            {
                                CustomerId = c.CustomerID,
                                CompanyName = c.CompanyName,
                                FirstName = e.FirstName,
                                LastName = e.LastName,
                                ShipName = o.ShipName,
                                ShipCountry = o.ShipCountry,
                                Freight = o.Freight
                            };
                gcOrders.DataSource = query.ToList();
            }
        }

        private void GetEmployees()
        {
            using (var db = new ShoppingContext())
            {
                var query = from e in db.Employees
                            orderby e.FirstName
                            select new
                            {
                                Photo = e.Photo,
                                EmployeeID = e.EmployeeID,
                                FirstName = e.FirstName,
                                LastName = e.LastName,
                                Title = e.Title,
                                Birthdate = e.BirthDate,
                                Country = e.Country
                            };

                gcEmployees.DataSource = query.ToList();
            }
        }

        private enum DataListType
        {
            Employees,
            Orders,
            Products
        }
    }
}