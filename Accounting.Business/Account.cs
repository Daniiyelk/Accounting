using Accounting.ViewModels.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.DataLayer.Context;

namespace Accounting.Business
{
    public class Account
    {
        public static ReportViewModel ReportMainForm()
        {
            ReportViewModel rvm = new ReportViewModel();
            using(UnitOfWork db = new UnitOfWork())
            {
                DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01);
                DateTime endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                var recive = db.AccountingRepository.Get(p => p.TypeID == 1 && p.DateTitle >= startDate && p.DateTitle <= endDate).Select(a => a.Amount).ToList();
                var pay = db.AccountingRepository.Get(p => p.TypeID == 2 && p.DateTitle >= startDate && p.DateTitle <= endDate).Select(a => a.Amount).ToList();

                rvm.pay = pay.Sum();
                rvm.recive = recive.Sum();
                rvm.profit = (recive.Sum() - pay.Sum());
            }

            return rvm;
        }
    }
}
