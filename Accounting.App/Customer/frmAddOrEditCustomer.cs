using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accounting.DataLayer.Context;
using Accounting.DataLayer;
using ValidationComponents;
using System.IO;
using Accounting.App.Customer;

namespace Accounting.App.Customer
{
    public partial class frmAddOrEditCustomer : Form
    {
        public int AddOrEditPointer = 0;
        public int customerId;

        UnitOfWork db = new UnitOfWork();

        public frmAddOrEditCustomer()
        {
            InitializeComponent();
        }

        private void btnAddPicture_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                pcCustomer.ImageLocation = openFile.FileName;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (BaseValidator.IsFormValid(this.components))
            {
                string ImageName = Guid.NewGuid().ToString() + Path.GetExtension(pcCustomer.ImageLocation);
                string path = Application.StartupPath + "/Images/";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                pcCustomer.Image.Save(path + ImageName);


                Customers customer = new Customers()
                {
                    FullName = txtName.Text,
                    Mobile = txtMobile.Text,
                    Address = txtAddress.Text,
                    Email = txtEmail.Text,
                    CustomerImage = ImageName
                };
                if (AddOrEditPointer == 1)
                {
                    customer.CustomerID = customerId;
                    db.customerRepository.UpdateCustomer(customer);
                }
                else
                {
                    db.customerRepository.InsertCustomer(customer);
                }
                AddOrEditPointer = 0;
                db.Save();
                DialogResult = DialogResult.OK;
            }
        }

        private void frmAddOrEditCustomer_Load(object sender, EventArgs e)
        {
            if (AddOrEditPointer == 1)
            {
                btnSave.Text = "ویرایش";
                this.Text = "ویرایش شخص";
                var customer = db.customerRepository.GetCustomerById(customerId);
                txtAddress.Text = customer.Address;
                txtEmail.Text = customer.Email;
                txtMobile.Text = customer.Mobile;
                txtName.Text = customer.FullName;
                pcCustomer.ImageLocation = Application.StartupPath + "/Images/" + customer.CustomerImage;
            }
        }
    }
}
