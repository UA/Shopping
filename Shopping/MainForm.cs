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
            else if ((DataListType)navigationFrame.SelectedPageIndex == DataListType.Summary1)
            {
                GetSummary1();
            }
            else if ((DataListType)navigationFrame.SelectedPageIndex == DataListType.Summary2)
            {
                GetSummary2();
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

        private void GetSummary1()
        {
            //Her bir Employee ID tarafindan isleme tabii tutulan 
            //order’lari gruplayarak toplam order quantity,
            //averaj unit price ve average discount miktari hesaplanacak.
            //“Employee ID”, “Toplam SIparis Miktari”, “Averaj Birim Fiyat”, 
            //“Average Iskonto Miktari”
            /* select EmployeeID, 
                        COUNT(A.OrderID) as OrderID, 
                        SUM(A.Quantity) as TotalQuantity, 
                        AVG(A.UnitPrice) as AverageUnitPrice,
                        AVG(A.Discount) as AverageDiscount
                        from 
                        [Order Details] as A, 
                        Orders as B
                        where A.OrderID = B.OrderID
                        group by B.EmployeeID, B.OrderID
                        order by EmployeeID*/
            using (var db = new ShoppingContext())
            {
                var query = from orderDetails in db.Order_Details
                            join orders in db.Orders
                            on orderDetails.OrderID equals orders.OrderID
                            group new { orderDetails, orders } by new { orders.OrderID, orders.EmployeeID } into grouping
                            orderby grouping.Key.EmployeeID
                            select new
                            {
                                EmployeeID = grouping.Key.EmployeeID,
                                ToplamSiparisMiktari = grouping.Sum(x => x.orderDetails.Quantity),
                                AverajBirimFiyat = grouping.Average(x => x.orderDetails.UnitPrice),
                                AverageIskontoMiktari = grouping.Average(x => x.orderDetails.Discount)
                            };
                gcSummary1.DataSource = query.ToList();

            }
        }

        private void GetSummary2()
        {
            //Her bir Customer ID tarafindan siparis edilen order’lari 
            //categori’leri bazinda gruplayarak toplam order quantity, 
            //averaj unit price ve average discount miktari hesaplanacak.
            //“Customer ID”, “Category ID”, “Toplam SIparis Miktari”, 
            //“Averaj Birim Fiyat”, “Average Iskonto Miktari”
            /*select CustomerID, C.CategoryID,
						 COUNT(A.OrderID) as OrderID, 
						 SUM(A.Quantity) as TotalQuantity, 
						 AVG(A.UnitPrice) as AverageUnitPrice,
						 AVG(A.Discount) as AverageDiscount
                         from
                         [Order Details] as A, 
						 Orders as B,
						 Products as P,
						 Categories as C
                         where A.OrderID = B.OrderID and P.ProductID = A.ProductID and P.CategoryID = C.CategoryID
                         group by CustomerID, C.CategoryID*/


            using (var db = new ShoppingContext())
            {
                var query = from orderDetails in db.Order_Details
                            join orders in db.Orders on orderDetails.OrderID equals orders.OrderID
                            join products in db.Products on orderDetails.ProductID equals products.ProductID
                            join categories in db.Categories on products.ProductID equals categories.CategoryID
                            group new { orderDetails, categories, orders } by new { categories.CategoryID, orders.CustomerID } into grouping
                            select new
                            {
                                CustomerID = grouping.Key.CustomerID,
                                CategoryID = grouping.Key.CategoryID,
                                ToplamSiparisMiktari = grouping.Sum(x => x.orderDetails.Quantity),
                                AverajBirimFiyat = grouping.Average(x => x.orderDetails.UnitPrice),
                                AverageIskontoMiktari = grouping.Average(x => x.orderDetails.Discount)
                            };
                gcSummary2.DataSource = query.ToList();

            }
        }


        private enum DataListType
        {
            Employees,
            Orders,
            Products,
            Summary1,
            Summary2
        }
    }
}