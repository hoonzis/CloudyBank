using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloudyBank.Core.Services;
using CloudyBank.Services.Categorization;
using CloudyBank.Core.Aggregations;

namespace CloudyBank.Web.AdministrationServices
{
    public partial class Admin : System.Web.UI.Page
    {
        #region Services
        private IOperationServices _operationServices;

        public IOperationServices OperationServices
        {
            get
            {
                if (_operationServices == null)
                {
                    _operationServices = Global.GetObject<IOperationServices>("OperationServices");
                }
                return _operationServices;
            }
        }

        private CategorizationServices _categorizationServices;

        public CategorizationServices CategorizationServices
        {
            get
            {
                if (_categorizationServices == null)
                {
                    _categorizationServices = Global.GetObject<CategorizationServices>("CategorizationServices");
                }

                return _categorizationServices;
            }
        }

        private IAggregationServices _aggregationServices;

        public IAggregationServices AggregationServices
        {
            get {
                if (_aggregationServices == null) {
                    _aggregationServices = Global.GetObject<IAggregationServices>("AggregationServices");
                }
                return _aggregationServices; 
            }
            set { _aggregationServices = value; }
        }
        #endregion



        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var date = DateTime.Now.Date.ToString("dd/MM/yyyy");
                OperationTextBox.Text = "date,description,amount\n" +
                date+",RETRAIT DAB " + DateTime.Now.Date.ToString("dd/MM/yy") + " 19H35 169770 BNP PARIBAS PARIS ,\"-60,00\"\n" +
                date+",FACTURE CARTE DU " + DateTime.Now.Date.ToString("ddMMyy") + " SUPERMARCHE PARIS CARTE 4974XX ,\"-4,26\"\n" +
                date+",FACTURE CARTE DU " + DateTime.Now.Date.ToString("ddMMyy") + " H & M 014 75 PARIS CARTE 4974X ,\"-9,95\"\n" +
                date+",VIREMENT RECU TIERS OCTO TECHNOLOGY VIREMENT SALAIRES NOVEMB ,\"2.023,96\"";
            }
        }

        public void InsertPayment(String iban, DateTime date, String description, Decimal amount)
        {
            OperationServices.InsertOperationFromCBS(iban, description, amount, date);
        }

        public void InsertPaymentsButton_Click(Object sender, EventArgs e)
        {
            var data = OperationTextBox.Text;
            var accountNumber = AccountNumberTextBox.Text;
            OutputLabel.Text = String.Format("Inporting payments into account: {0}",accountNumber);
            OperationServices.InsertOperationsBulkData(data, accountNumber);
            OutputLabel.Text = String.Format("Payments inported");
        }

        public void Categorize_Click(Object sender, EventArgs e)
        {
            var id = Convert.ToInt32(CustomerIDTextBox.Text);
            OutputLabel.Text = String.Format("Categorizing payments of account: {0}", id);
            var suc = CategorizationServices.CategorizePayments(id);
            if (suc)
            {
                OutputLabel.Text = String.Format("Payments of account: {0} categorized", id);
            }
            else
            {
                OutputLabel.Text = String.Format("Error while categoriing payments of account {0}", id);
            }
        }

        public void ComputeBalancePoints_Click(Object sender, EventArgs e)
        {
            AggregationServices.UpdateAllBalancePoints();
            OutputLabel.Text = "Account evolution computed";
        }
    }
}
