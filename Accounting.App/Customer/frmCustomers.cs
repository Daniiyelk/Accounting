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
using Accounting.App.Customer;

namespace Accounting.App
{
    public partial class frmCustomers : Form
    {

        public frmCustomers()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void frmCustomers_Load(object sender, EventArgs e)
        {
            dgvCustomers.AutoGenerateColumns = false;
            BindGrid();
        }

        void BindGrid()
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                dgvCustomers.DataSource = db.customerRepository.GetAllCustomers().ToList();
            }

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                dgvCustomers.DataSource = db.customerRepository.GetCustomersByFilter(txtSearch.Text);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = null;
            BindGrid();
        }

        private void btnDeleteCustomer_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow != null)
            {
                using (UnitOfWork db = new UnitOfWork())
                {
                    string name = dgvCustomers.CurrentRow.Cells[0].Value.ToString();
                    if (RtlMessageBox.Show($"آیا از حذف {name} مطمعن هستید؟", "هشدار", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        int contactId = int.Parse(dgvCustomers.CurrentRow.Cells[3].Value.ToString());
                        db.customerRepository.DeleteCustomer(contactId);
                        db.Save();
                        BindGrid();
                    }
                }

            }
            else
            {
                RtlMessageBox.Show("لطفا یک سطر را انتخاب کنید", "هشدار", MessageBoxButtons.OK);
            }
        }

        private void btnAddNewCustomer_Click(object sender, EventArgs e)
        {
            frmAddOrEditCustomer frm = new frmAddOrEditCustomer();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                BindGrid();
            }
        }

        private void btnEditCustomer_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow != null)
            {
                frmAddOrEditCustomer frm = new frmAddOrEditCustomer();
                frm.AddOrEditPointer = 1;
                frm.customerId = int.Parse(dgvCustomers.CurrentRow.Cells[3].Value.ToString());
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    BindGrid();
                }
            }
            else
            {
                MessageBox.Show("لطفا یک شخص را انتخاب کنید", "توجه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
