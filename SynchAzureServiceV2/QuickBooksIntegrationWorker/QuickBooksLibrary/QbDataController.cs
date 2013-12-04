using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intuit.Ipp.Core;
using Intuit.Ipp.Security;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Diagnostics;
using Intuit.Ipp.Exception;
using Intuit.Ipp.Retry;
using Intuit.Ipp.Utility;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.LinqExtender;

using QuickBooksIntegrationWorker.SynchLibrary.Models;
using QuickBooksIntegrationWorker.Utility;

namespace QuickBooksIntegrationWorker.QuickBooksLibrary
{
    class QbDataController
    {
        ServiceContext qbServiceContext;
        DataService QbdDataService;

        public QbDataController(int synchBusinessId, QbCredentialEntity qbCredential)
        {
            OAuthRequestValidator oauthValidator =  QbAuthorizationController.InitializeOAuthValidator(
                qbCredential.accessToken, qbCredential.accessTokenSecret, qbCredential.consumerKey, qbCredential.consumerSecret);
            this.qbServiceContext = QbAuthorizationController.InitializeServiceContext(oauthValidator, qbCredential.realmId, IntuitServicesType.QBD);
            qbServiceContext.IppConfiguration.RetryPolicy = new IntuitRetryPolicy(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(2));
            this.QbdDataService = new DataService(qbServiceContext);

            // TO-DO: make a test connection
        }

        #region SAFE ACTION SECION: get
        public IEnumerable<Invoice> getInvoicesFromDate(DateTime startDate)
        {
            int maxResults = 500;
            int startPosition = 0;
            int prevInvoiceCount = 0;
            QueryService<Invoice> invoiceQueryService = new QueryService<Invoice>(qbServiceContext);
            IEnumerable<Invoice> invoices = invoiceQueryService.Where(c => c.MetaData.CreateTime >= startDate).Skip(startPosition).Take(maxResults);
            while (invoices.Count() > prevInvoiceCount)
            {
                prevInvoiceCount = invoices.Count();
                startPosition = invoices.Count();
                invoices = invoices.Concat(invoiceQueryService.Where(c => c.MetaData.CreateTime >= startDate).Skip(startPosition).Take(maxResults));

            }

            return invoices;
        }

        

        public IEnumerable<Item> getActiveItems()
        {
            int maxResults = 500;
            int startPosition = 0;
            int prevItemCount = 0;
            QueryService<Item> itemQueryService = new QueryService<Item>(qbServiceContext);
            IEnumerable<Item> items = itemQueryService.Where(c => c.Active == true).Skip(startPosition).Take(maxResults);
            while (items.Count() > prevItemCount)
            {
                prevItemCount = items.Count();
                startPosition = items.Count();
                items = items.Concat(itemQueryService.Where(c => c.Active == true).Skip(startPosition).Take(maxResults));

            }

            return items;
        }

        public IEnumerable<Customer> getActiveCustomers()
        {
            int maxResults = 500;
            int startPosition = 0;
            int prevCustomerCount = 0;
            QueryService<Customer> customerQueryService = new QueryService<Customer>(qbServiceContext);
            IEnumerable<Customer> customers = customerQueryService.Where(c => c.Active == true).Skip(startPosition).Take(maxResults);
            while (customers.Count() > prevCustomerCount)
            {
                prevCustomerCount = customers.Count();
                startPosition = customers.Count();
                customers = customers.Concat(customerQueryService.Where(c => c.Active == true).Skip(startPosition).Take(maxResults));
            }

            return customers;
        }

        #endregion

        
        #region UNSAFE ACTION SECTION: create, update, (delete)
        public Intuit.Ipp.Data.Invoice createInvoice(SynchRecord recordFromSynch, Dictionary<string, Item> upcToItemMap,
            Dictionary<int, Customer> customerIdToCustomerMap, string timezone)
        {
            Invoice invoice = new Invoice();

            // add all the items in the record into inovice lines
            decimal balance = Decimal.Zero;
            List<Line> lines = new List<Line>();
            foreach (SynchRecordLine lineFromSynch in recordFromSynch.recordLines)
            {
                Item item = upcToItemMap[lineFromSynch.upc];
                
                // defines header for this line
                Line line = new Line();
                line.Amount = lineFromSynch.price * lineFromSynch.quantity;
                line.Description = item.Description;

                // defines detail for this line
                line.AnyIntuitObject = new SalesItemLineDetail()
                {
                    Qty = lineFromSynch.quantity,
                    QtySpecified = true,
                    ItemRef = new ReferenceType() { Value = item.Id }
                };
                line.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                line.DetailTypeSpecified = true;
                lines.Add(line);

                balance += line.Amount;
            }

            Customer customer = customerIdToCustomerMap[recordFromSynch.clientId];
            invoice.CustomerRef = new ReferenceType() { Value = customer.Id };

            invoice.ShipDate = TimeZoneInfo.ConvertTimeFromUtc(
                ((DateTimeOffset)recordFromSynch.deliveryDate).UtcDateTime, TimeZoneInfo.FindSystemTimeZoneById(timezone));
            invoice.ShipDateSpecified = true;

            invoice.TotalAmt = balance;
            invoice.PrivateNote = recordFromSynch.comment;
            invoice.Line = lines.ToArray();

            try
            {
                return QbdDataService.Add(invoice) as Invoice;
            }
            catch (Exception e)
            {
                DateTime currentDateTimePST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
                System.Diagnostics.Trace.TraceError(currentDateTimePST.ToString() + ":" + e.ToString());
                throw e;
            }
        }

        public Intuit.Ipp.Data.SalesOrder createSalesOrder(SynchRecord recordFromSynch, Dictionary<string, Item> upcToItemMap,
            Dictionary<int, Customer> customerIdToCustomerMap, string timezone)
        {
            SalesOrder salesOrder = new SalesOrder();

            // add all the items in the record into inovice lines
            decimal balance = Decimal.Zero;
            List<Line> lines = new List<Line>();
            foreach (SynchRecordLine lineFromSynch in recordFromSynch.recordLines)
            {
                Item item = upcToItemMap[lineFromSynch.upc];

                // defines header for this line
                Line line = new Line();
                line.Amount = lineFromSynch.price * lineFromSynch.quantity;
                line.Description = item.Description;

                // defines detail for this line
                line.AnyIntuitObject = new SalesItemLineDetail()
                {
                    Qty = lineFromSynch.quantity,
                    QtySpecified = true,
                    ItemRef = new ReferenceType() { Value = item.Id }
                };
                line.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                line.DetailTypeSpecified = true;
                lines.Add(line);

                balance += line.Amount;
            }

            Customer customer = customerIdToCustomerMap[recordFromSynch.clientId];
            salesOrder.CustomerRef = new ReferenceType() { Value = customer.Id };

            salesOrder.ShipDate = TimeZoneInfo.ConvertTimeFromUtc(
                ((DateTimeOffset)recordFromSynch.deliveryDate).UtcDateTime, TimeZoneInfo.FindSystemTimeZoneById(timezone));
            salesOrder.ShipDateSpecified = true;

            salesOrder.TotalAmt = balance;
            salesOrder.PrivateNote = recordFromSynch.comment;
            salesOrder.Line = lines.ToArray();

            try
            {
                return QbdDataService.Add(salesOrder) as SalesOrder;
            }
            catch (Exception e)
            {
                DateTime currentDateTimePST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
                System.Diagnostics.Trace.TraceError(currentDateTimePST.ToString() + ":" + e.ToString());
                throw e;
            }
        }

        #endregion
    }
}
