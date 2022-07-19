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
using Accounting.ViewModels.Customers;
using Accounting.Utility.Convertor;

namespace Accounting.App
{
    public partial class frmReport : Form
    {
        public int TypeId = 0;

        public frmReport()
        {
            InitializeComponent();
        }

        private void frmReport_Load(object sender, EventArgs e)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                List<ListCustomersViewModel> list = new List<ListCustomersViewModel>();
                list.Add(new ListCustomersViewModel()
                {
                    FullName = "لطفا انتخاب کنید",
                    CustomerId = 0
                });
                list.AddRange(db.customerRepository.GetNameCustomers());
                cbCustomer.DataSource = list;
                cbCustomer.DisplayMember = "FullName";
                cbCustomer.ValueMember = "CustomerID";
            }

            dgvReport.AutoGenerateColumns = false;
            if (TypeId == 1)
            {
                this.Text = "گزارش دریافتی ها";
            }
            else
            {
                this.Text = "گزارش پرداختی ها";
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            Filter();
        }

        void Filter()
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                List<DataLayer.Accounting> result = new List<DataLayer.Accounting>();

                DateTime? StartDate;
                DateTime? EndDate;

                if ((int)cbCustomer.SelectedValue != 0)
                {
                    result.AddRange(db.AccountingRepository.Get(p => p.TypeID == TypeId && p.CustomerID == (int)cbCustomer.SelectedValue));
                }
                else
                {
                    result.AddRange(db.AccountingRepository.Get(p => p.TypeID == TypeId));
                }

                if (txtFromDate.Text != "    /  /")
                {
                    StartDate = Convert.ToDateTime(txtFromDate.Text);
                    result = result.Where(r => r.DateTitle >= StartDate.Value).ToList();
                }
                if (txtToDate.Text != "    /  /")
                {
                    EndDate = Convert.ToDateTime(txtToDate.Text);
                    result = result.Where(r => r.DateTitle <= EndDate.Value).ToList();
                }

                dgvReport.Rows.Clear();
                foreach (var accounting in result)
                {
                    string customerName = db.customerRepository.GetCustomerNameById(accounting.CustomerID);
                    dgvReport.Rows.Add(accounting.ID, customerName, accounting.Amount, accounting.DateTitle.ToShamsi(), accounting.Description);
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Filter();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvReport.CurrentRow != null)
            {
                using (UnitOfWork db = new UnitOfWork())
                {
                    int id = int.Parse(dgvReport.CurrentRow.Cells[0].Value.ToString());
                    if (MessageBox.Show("آیا از حذف این حساب مطمعن هستید؟", "توجه", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        db.AccountingRepository.Delete(db.AccountingRepository.GetByID(id));
                        db.Save();
                        Filter();
                    }
                }
            }
            else
            {
                MessageBox.Show("لطفا یک ردیف را انتخاب کنید", "توجه", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvReport.CurrentRow != null)
            {
                using (UnitOfWork db = new UnitOfWork())
                {
                    int id = int.Parse(dgvReport.CurrentRow.Cells[0].Value.ToString());
                    frmNewAccounting frm = new frmNewAccounting();
                    frm.accountId = id;
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        Filter();
                    }
                }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DataTable dtPrint = new DataTable();
            dtPrint.Columns.Add("Customer");
            dtPrint.Columns.Add("Amount");
            dtPrint.Columns.Add("Date");
            dtPrint.Columns.Add("Description");

            foreach(DataGridViewRow item in dgvReport.Rows)
            {
                dtPrint.Rows.Add(
                    item.Cells[1].Value.ToString(),
                    item.Cells[2].Value.ToString(),
                    item.Cells[3].Value.ToString(),
                    item.Cells[4].Value.ToString()
                );
            }

            stiPrint.Load(Application.StartupPath + "/Report.mrt");
            stiPrint.RegData("DT", dtPrint);
            stiPrint.Show();
        }
    }
}
