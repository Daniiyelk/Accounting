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
using ValidationComponents;
using Accounting.DataLayer;
using Accounting.Utility.Convertor;

namespace Accounting.App
{
    public partial class frmNewAccounting : Form
    {
        UnitOfWork db;
        public int accountId = 0;
        public frmNewAccounting()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void frmNewAccounting_Load(object sender, EventArgs e)
        {
            db = new UnitOfWork();
            dgvTransactionList.AutoGenerateColumns = false;
            dgvTransactionList.DataSource = db.customerRepository.GetNameCustomers();
            if (accountId != 0)
            {
                var account = db.AccountingRepository.GetByID(accountId);
                txtAmount.Value = account.Amount;
                txtDescription.Text = account.Description;
                txtName.Text = db.customerRepository.GetCustomerNameById(account.CustomerID);
                if (account.AccountingTypes.TypeID == 1)
                {
                    rbRecive.Checked = true;
                }
                else
                {
                    rbPay.Checked = true;
                }

                btnSave.Text = "ویرایش";
                this.Text = "ویرایش";
            }
            db.Dispose();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            dgvTransactionList.DataSource = db.customerRepository.GetNameCustomers(txtFilter.Text);
        }

        private void dgvTransactionList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtName.Text = dgvTransactionList.CurrentRow.Cells[0].Value.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (BaseValidator.IsFormValid(this.components))
            {
                if (rbPay.Checked || rbRecive.Checked)
                {
                    db = new UnitOfWork();
                    DataLayer.Accounting accounting = new DataLayer.Accounting()
                    {
                        Amount = int.Parse(txtAmount.Value.ToString()),
                        CustomerID = db.customerRepository.GetCustomerIdByName(txtName.Text),
                        DateTitle = DateTime.Now,
                        TypeID = (rbRecive.Checked) ? 1 : 2,
                        Description = txtDescription.Text,
                    };
                    if (accountId == 0)
                    {
                        db.AccountingRepository.Insert(accounting);
                    }
                    else
                    {
                        accounting.ID = accountId;
                        db.AccountingRepository.Update(accounting);
                    }
                    db.Save();
                    db.Dispose();
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("لطفا نوع تراکنش را انتخاب کنید", "توجه", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
