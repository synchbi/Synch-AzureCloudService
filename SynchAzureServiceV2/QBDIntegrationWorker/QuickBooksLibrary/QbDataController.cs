using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intuit.Ipp.Core;
using Intuit.Ipp.Security;
using Intuit.Ipp.Data;
using Intuit.Ipp.Data.Qbd;
using Intuit.Ipp.Services;
using Intuit.Ipp.Diagnostics;
using Intuit.Ipp.Exception;
using Intuit.Ipp.Retry;
using Intuit.Ipp.Utility;

using QBDIntegrationWorker.SynchLibrary.Models;
using QBDIntegrationWorker.Utility;

namespace QBDIntegrationWorker.QuickBooksLibrary
{
    public class QbDataController
    {
        ServiceContext qbServiceContext;
        DataServices qbdDataService;
        QBDIntegrationWorker.IntegrationDataflow.IntegrationConfiguration config;

        public QbDataController(int synchBusinessId, QbCredentialEntity qbCredential, QBDIntegrationWorker.IntegrationDataflow.IntegrationConfiguration config)
        {
            OAuthRequestValidator oauthValidator =  QbAuthorizationController.InitializeOAuthValidator(
                qbCredential.accessToken, qbCredential.accessTokenSecret, qbCredential.consumerKey, qbCredential.consumerSecret);

            this.qbServiceContext = QbAuthorizationController.InitializeServiceContext(oauthValidator, qbCredential.realmId, IntuitServicesType.QBD);
            qbServiceContext.RetryPolicy = new IntuitRetryPolicy(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(2));
            this.qbdDataService = new DataServices(qbServiceContext);

            this.config = config;

            // TO-DO: make a test connection
        }

        #region SAFE ACTION SECION: get
        public List<Invoice> getInvoicesFromDate(DateTime startDate)
        {
            // uses InvoiceQuery to repeatedly get invoice information
            List<Invoice> result = new List<Invoice>();
            int pageNumber = 1;
            int chunkSize = 500;
            Intuit.Ipp.Data.Qbd.InvoiceQuery qbdInvoiceQuery = new InvoiceQuery();
            qbdInvoiceQuery.ItemElementName = ItemChoiceType4.StartPage;
            qbdInvoiceQuery.Item = pageNumber.ToString();
            qbdInvoiceQuery.ChunkSize = chunkSize.ToString();
            qbdInvoiceQuery.StartCreatedTMS = startDate;
            qbdInvoiceQuery.StartCreatedTMSSpecified = true;
            IEnumerable<Invoice> invoicesFromQBD = qbdInvoiceQuery.ExecuteQuery<Invoice>
            (qbServiceContext) as IEnumerable<Invoice>;
            result.AddRange(invoicesFromQBD.ToArray());
            int curItemCount = invoicesFromQBD.ToArray().Length;

            while (curItemCount > 0)
            {
                pageNumber++;
                qbdInvoiceQuery.Item = pageNumber.ToString();
                invoicesFromQBD = qbdInvoiceQuery.ExecuteQuery<Invoice>
                                     (qbServiceContext) as IEnumerable<Invoice>;
                result.AddRange(invoicesFromQBD.ToArray());
                curItemCount = invoicesFromQBD.ToArray().Length;
            }

            return result;
        }

        public List<SalesOrder> getSalesOrdersFromDate(DateTime startDate)
        {
            // uses salesOrderQuery to repeatedly get salesOrder information
            List<SalesOrder> result = new List<SalesOrder>();
            int pageNumber = 1;
            int chunkSize = 500;
            SalesOrderQuery qbdSalesOrderQuery = new SalesOrderQuery();
            qbdSalesOrderQuery.ItemElementName = ItemChoiceType4.StartPage;
            qbdSalesOrderQuery.Item = pageNumber.ToString();
            qbdSalesOrderQuery.ChunkSize = chunkSize.ToString();
            qbdSalesOrderQuery.StartCreatedTMS = startDate;
            qbdSalesOrderQuery.StartCreatedTMSSpecified = true;
            IEnumerable<SalesOrder> salesOrdersFromQBD = qbdSalesOrderQuery.ExecuteQuery<SalesOrder>
            (qbServiceContext) as IEnumerable<SalesOrder>;
            result.AddRange(salesOrdersFromQBD.ToArray());
            int curItemCount = salesOrdersFromQBD.ToArray().Length;
            while (curItemCount > 0)
            {
                pageNumber++;
                qbdSalesOrderQuery.Item = pageNumber.ToString();
                salesOrdersFromQBD = qbdSalesOrderQuery.ExecuteQuery<SalesOrder>
                                     (qbServiceContext) as IEnumerable<SalesOrder>;
                result.AddRange(salesOrdersFromQBD.ToArray());
                curItemCount = salesOrdersFromQBD.ToArray().Length;
            }

            return result;
        }

        public List<SalesRep> getActiveSalesReps()
        {
            // uses salesRepQuery to repeatedly get salesRep information
            List<SalesRep> result = new List<SalesRep>();
            int pageNumber = 1;
            int chunkSize = 500;
            SalesRepQuery qbdSalesRepQuery = new SalesRepQuery();
            qbdSalesRepQuery.ItemElementName = ItemChoiceType4.StartPage;
            qbdSalesRepQuery.Item = pageNumber.ToString();
            qbdSalesRepQuery.ChunkSize = chunkSize.ToString();
            qbdSalesRepQuery.ActiveOnly = true;
            IEnumerable<SalesRep> salesRepsFromQBD = qbdSalesRepQuery.ExecuteQuery<SalesRep>
            (qbServiceContext) as IEnumerable<SalesRep>;
            result.AddRange(salesRepsFromQBD.ToArray());
            int curItemCount = salesRepsFromQBD.ToArray().Length;
            while (curItemCount > 0)
            {
                pageNumber++;
                qbdSalesRepQuery.Item = pageNumber.ToString();
                salesRepsFromQBD = qbdSalesRepQuery.ExecuteQuery<SalesRep>
                                     (qbServiceContext) as IEnumerable<SalesRep>;
                result.AddRange(salesRepsFromQBD.ToArray());
                curItemCount = salesRepsFromQBD.ToArray().Length;
            }

            return result;
        }

        public List<Item> getActiveItems()
        {
            List<Item> result = new List<Item>();

            int pageNumber = 1;
            int chunkSize = 500;
            ItemQuery qbdItemQuery = new ItemQuery();
            qbdItemQuery.ItemElementName = ItemChoiceType4.StartPage;
            qbdItemQuery.Item = pageNumber.ToString();
            qbdItemQuery.ChunkSize = chunkSize.ToString();
            qbdItemQuery.ActiveOnly = true;
            IEnumerable<Item> itemsFromQBD = qbdItemQuery.ExecuteQuery<Item>
            (qbServiceContext) as IEnumerable<Item>;
            result.AddRange(itemsFromQBD.ToArray());
            int curItemCount = itemsFromQBD.ToArray().Length;

            while (curItemCount > 0)
            {
                pageNumber++;
                qbdItemQuery.Item = pageNumber.ToString();
                itemsFromQBD = qbdItemQuery.ExecuteQuery<Item>
                                     (qbServiceContext) as IEnumerable<Item>;
                result.AddRange(itemsFromQBD.ToArray());
                curItemCount = itemsFromQBD.ToArray().Length;
            }

            return result;
        }

        public List<Customer> getActiveCustomers()
        {
            List<Customer> result = new List<Customer>();

            int pageNumber = 1;
            int chunkSize = 500;
            CustomerQuery qbdCustomerQuery = new CustomerQuery();
            qbdCustomerQuery.ItemElementName = ItemChoiceType4.StartPage;
            qbdCustomerQuery.Item = pageNumber.ToString();
            qbdCustomerQuery.ChunkSize = chunkSize.ToString();
            qbdCustomerQuery.ActiveOnly = true;
            IEnumerable<Customer> customersFromQBD = qbdCustomerQuery.ExecuteQuery<Customer>
            (qbServiceContext) as IEnumerable<Customer>;
            result.AddRange(customersFromQBD.ToArray());
            int curCustomerCount = customersFromQBD.ToArray().Length;

            while (curCustomerCount > 0)
            {
                pageNumber++;
                qbdCustomerQuery.Item = pageNumber.ToString();
                customersFromQBD = qbdCustomerQuery.ExecuteQuery<Customer>
                                     (qbServiceContext) as IEnumerable<Customer>;
                result.AddRange(customersFromQBD.ToArray());
                curCustomerCount = customersFromQBD.ToArray().Length;
            }

            return result;
        }

        public List<Item> getAllItems()
        {
            List<Item> result = new List<Item>();

            int pageNumber = 1;
            int chunkSize = 500;
            ItemQuery qbdItemQuery = new ItemQuery();
            qbdItemQuery.ItemElementName = ItemChoiceType4.StartPage;
            qbdItemQuery.Item = pageNumber.ToString();
            qbdItemQuery.ChunkSize = chunkSize.ToString();
            IEnumerable<Item> itemsFromQBD = qbdItemQuery.ExecuteQuery<Item>
            (qbServiceContext) as IEnumerable<Item>;
            result.AddRange(itemsFromQBD.ToArray());
            int curItemCount = itemsFromQBD.ToArray().Length;

            while (curItemCount > 0)
            {
                pageNumber++;
                qbdItemQuery.Item = pageNumber.ToString();
                itemsFromQBD = qbdItemQuery.ExecuteQuery<Item>
                                     (qbServiceContext) as IEnumerable<Item>;
                result.AddRange(itemsFromQBD.ToArray());
                curItemCount = itemsFromQBD.ToArray().Length;
            }

            return result;
        }

        public List<Customer> getAllCustomers()
        {
            List<Customer> result = new List<Customer>();

            int pageNumber = 1;
            int chunkSize = 500;
            CustomerQuery qbdCustomerQuery = new CustomerQuery();
            qbdCustomerQuery.ItemElementName = ItemChoiceType4.StartPage;
            qbdCustomerQuery.Item = pageNumber.ToString();
            qbdCustomerQuery.ChunkSize = chunkSize.ToString();
            IEnumerable<Customer> customersFromQBD = qbdCustomerQuery.ExecuteQuery<Customer>
            (qbServiceContext) as IEnumerable<Customer>;
            result.AddRange(customersFromQBD.ToArray());
            int curCustomerCount = customersFromQBD.ToArray().Length;

            while (curCustomerCount > 0)
            {
                pageNumber++;
                qbdCustomerQuery.Item = pageNumber.ToString();
                customersFromQBD = qbdCustomerQuery.ExecuteQuery<Customer>
                                     (qbServiceContext) as IEnumerable<Customer>;
                result.AddRange(customersFromQBD.ToArray());
                curCustomerCount = customersFromQBD.ToArray().Length;
            }

            return result;
        }

        #endregion

        
        #region UNSAFE ACTION SECTION: create, update, (delete)
        public Invoice createInvoice(SynchRecord recordFromSynch, Dictionary<string, Item> upcToItemMap,
                                    Dictionary<int, Customer> customerIdToCustomerMap, Dictionary<int, SalesRep> accountIdToSaleRepMap, string timezone)
        {
            // creates actual invoice
            // add all the items in the record into inovice lines
            decimal balance = Decimal.Zero;
            List<InvoiceLine> listLine = new List<InvoiceLine>();
            foreach (SynchRecordLine lineFromSynch in recordFromSynch.recordLines)
            {
                // QBD uses an array pair to map attributes to their values.
                // The first array keeps track of what elements are in the second array.
                ItemsChoiceType2[] invoiceItemAttributes =
                { 
                    ItemsChoiceType2.ItemId,
                    ItemsChoiceType2.UnitPrice,
                    ItemsChoiceType2.Qty 
                };
                // Now the second array
                object[] invoiceItemValues =
                {
                    new IdType() 
                    {
                        idDomain = idDomainEnum.QB,
                        Value = upcToItemMap[lineFromSynch.upc].Id.Value
                    },
                    lineFromSynch.price,
                    new decimal(lineFromSynch.quantity) 
                };

                var invoiceLine = new InvoiceLine();
                invoiceLine.Amount = (Decimal)lineFromSynch.price * lineFromSynch.quantity;
                invoiceLine.AmountSpecified = true;
                invoiceLine.Desc = upcToItemMap[lineFromSynch.upc].Desc;
                invoiceLine.ItemsElementName = invoiceItemAttributes;
                invoiceLine.Items = invoiceItemValues;

                listLine.Add(invoiceLine);

                balance += invoiceLine.Amount;
            }

            InvoiceHeader invoiceHeader = new InvoiceHeader();
            invoiceHeader.CustomerId = new IdType()
            {
                idDomain = idDomainEnum.QB,
                Value = customerIdToCustomerMap[recordFromSynch.clientId].Id.Value
            };

            invoiceHeader.SalesRepId = new IdType()
            {
                idDomain = idDomainEnum.QB,
                Value = accountIdToSaleRepMap[recordFromSynch.accountId].Id.Value
            };

            invoiceHeader.TxnDate = DateTime.Now.AddHours(SynchTimeZoneConverter.getLocalToUtcHourDifference(DateTime.Now, config.timezone));
            invoiceHeader.TxnDateSpecified = true;

            invoiceHeader.Balance = balance;
            invoiceHeader.DueDate = DateTime.Now.AddDays(1);
            //invoiceHeader.ShipAddr = physicalAddress;
            DateTime deliveryDateTime = ((DateTimeOffset)recordFromSynch.deliveryDate).DateTime;
            invoiceHeader.ShipDate = deliveryDateTime.AddHours(SynchTimeZoneConverter.getLocalToUtcHourDifference(deliveryDateTime, config.timezone));
            invoiceHeader.ShipDateSpecified = true;

            invoiceHeader.ToBeEmailed = false;
            invoiceHeader.TotalAmt = invoiceHeader.Balance;
            invoiceHeader.Note = recordFromSynch.comment;
            Invoice invoice = new Invoice();
            invoice.Header = invoiceHeader;
            invoice.Line = listLine.ToArray();

            return qbdDataService.Add(invoice);
        }

        public SalesOrder createSalesOrder(SynchRecord recordFromSynch, Dictionary<string, Item> upcToItemMap,
                                    Dictionary<int, Customer> customerIdToCustomerMap, Dictionary<int, SalesRep> accountIdToSaleRepMap, string timezone)
        {
            // creates actual salesOrder
            // add all the items in the record into inovice lines
            decimal balance = Decimal.Zero;
            List<SalesOrderLine> listLine = new List<SalesOrderLine>();
            foreach (SynchRecordLine lineFromSynch in recordFromSynch.recordLines)
            {
                // QBD uses an array pair to map attributes to their values.
                // The first array keeps track of what elements are in the second array.
                ItemsChoiceType2[] salesOrderItemAttributes =
                { 
                    ItemsChoiceType2.ItemId,
                    ItemsChoiceType2.UnitPrice,
                    ItemsChoiceType2.Qty 
                };
                // Now the second array
                object[] salesOrderItemValues =
                {
                    new IdType() 
                    {
                        idDomain = idDomainEnum.QB,
                        Value = upcToItemMap[lineFromSynch.upc].Id.Value
                    },
                    lineFromSynch.price,
                    new decimal(lineFromSynch.quantity) 
                };

                var salesOrderLine = new SalesOrderLine();
                salesOrderLine.Amount = (Decimal)lineFromSynch.price * lineFromSynch.quantity;
                salesOrderLine.AmountSpecified = true;
                salesOrderLine.Desc = upcToItemMap[lineFromSynch.upc].Desc;
                salesOrderLine.ItemsElementName = salesOrderItemAttributes;
                salesOrderLine.Items = salesOrderItemValues;

                listLine.Add(salesOrderLine);

                balance += salesOrderLine.Amount;
            }

            SalesOrderHeader salesOrderHeader = new SalesOrderHeader();            

            salesOrderHeader.CustomerId = new IdType()
            {
                idDomain = idDomainEnum.QB,
                Value = customerIdToCustomerMap[recordFromSynch.clientId].Id.Value
            };

            salesOrderHeader.SalesRepId = new IdType()
            {
                idDomain = idDomainEnum.QB,
                Value = accountIdToSaleRepMap[recordFromSynch.accountId].Id.Value
            };

            salesOrderHeader.TxnDate = DateTime.Now.AddHours(SynchTimeZoneConverter.getLocalToUtcHourDifference(DateTime.Now, config.timezone));
            salesOrderHeader.TxnDateSpecified = true;

            salesOrderHeader.Balance = balance;
            salesOrderHeader.DueDate = DateTime.Now.AddDays(1);
            //salesOrderHeader.ShipAddr = physicalAddress;
            DateTime deliveryDateTime = ((DateTimeOffset)recordFromSynch.deliveryDate).DateTime;
            salesOrderHeader.ShipDate = deliveryDateTime.AddHours(SynchTimeZoneConverter.getLocalToUtcHourDifference(deliveryDateTime, config.timezone));
            salesOrderHeader.ShipDateSpecified = true;


            salesOrderHeader.ToBeEmailed = false;
            salesOrderHeader.TotalAmt = salesOrderHeader.Balance;
            salesOrderHeader.Note = recordFromSynch.comment;
            SalesOrder salesOrder = new SalesOrder();
            salesOrder.Header = salesOrderHeader;
            salesOrder.Line = listLine.ToArray();

            /*
            NameValue alternateId = new NameValue();
            alternateId.Name = "synchId";
            alternateId.Value = recordFromSynch.id.ToString();

            salesOrder.AlternateId.SetValue(alternateId, 0);
             */

            return qbdDataService.Add(salesOrder);
        }

        public Invoice updateInvoice(SynchRecord recordFromSynch, Dictionary<string, Item> upcToItemMap,
                                    Dictionary<int, Customer> customerIdToCustomerMap, Dictionary<int, SalesRep> accountIdToSaleRepMap, string timezone)
        {
            // creates actual Invoice
            // add all the items in the record into inovice lines
            decimal balance = Decimal.Zero;
            List<InvoiceLine> listLine = new List<InvoiceLine>();
            foreach (SynchRecordLine lineFromSynch in recordFromSynch.recordLines)
            {
                // QBD uses an array pair to map attributes to their values.
                // The first array keeps track of what elements are in the second array.
                ItemsChoiceType2[] invoiceItemAttributes =
                { 
                    ItemsChoiceType2.ItemId,
                    ItemsChoiceType2.UnitPrice,
                    ItemsChoiceType2.Qty 
                };
                // Now the second array
                object[] invoiceItemValues =
                {
                    new IdType() 
                    {
                        idDomain = idDomainEnum.QB,
                        Value = upcToItemMap[lineFromSynch.upc].Id.Value
                    },
                    lineFromSynch.price,
                    new decimal(lineFromSynch.quantity) 
                };

                var invoiceLine = new InvoiceLine();
                invoiceLine.Amount = (Decimal)lineFromSynch.price * lineFromSynch.quantity;
                invoiceLine.AmountSpecified = true;
                invoiceLine.Desc = upcToItemMap[lineFromSynch.upc].Desc;
                invoiceLine.ItemsElementName = invoiceItemAttributes;
                invoiceLine.Items = invoiceItemValues;

                listLine.Add(invoiceLine);

                balance += invoiceLine.Amount;
            }

            InvoiceHeader invoiceHeader = new InvoiceHeader();

            invoiceHeader.CustomerId = new IdType()
            {
                idDomain = idDomainEnum.QB,
                Value = customerIdToCustomerMap[recordFromSynch.clientId].Id.Value
            };

            invoiceHeader.SalesRepId = new IdType()
            {
                idDomain = idDomainEnum.QB,
                Value = accountIdToSaleRepMap[recordFromSynch.accountId].Id.Value
            };

            invoiceHeader.Balance = balance;
            invoiceHeader.DueDate = DateTime.Now.AddDays(1);
            //invoiceHeader.ShipAddr = physicalAddress;
            invoiceHeader.ShipDate = ((DateTimeOffset)recordFromSynch.deliveryDate).DateTime;
            invoiceHeader.ShipDateSpecified = true;

            invoiceHeader.ToBeEmailed = false;
            invoiceHeader.TotalAmt = invoiceHeader.Balance;
            invoiceHeader.Note = recordFromSynch.comment;

            Invoice invoice = new Invoice();
            invoice.Header = invoiceHeader;
            invoice.Line = listLine.ToArray();

            // UPDATE-required fields below
            if (String.IsNullOrEmpty(recordFromSynch.integrationId))
            {
                // treat as new sales order
                invoiceHeader.TxnDate = DateTime.Now.AddHours(SynchTimeZoneConverter.getLocalToUtcHourDifference(DateTime.Now, config.timezone));
                invoiceHeader.TxnDateSpecified = true;

                return qbdDataService.Add(invoice);
            }
            else
            {
                string qbId = recordFromSynch.integrationId.Split(':')[0];
                string syncToken = recordFromSynch.integrationId.Split(':')[1];
                invoice.Id = new IdType()
                {
                    idDomain = idDomainEnum.NG,
                    Value = qbId
                };

                invoice.SyncToken = syncToken;

                return qbdDataService.Update(invoice);
            }
        }

        public SalesOrder updateSalesOrder(SynchRecord recordFromSynch, Dictionary<string, Item> upcToItemMap,
                                    Dictionary<int, Customer> customerIdToCustomerMap, Dictionary<int, SalesRep> accountIdToSaleRepMap, string timezone)
        {
            // creates actual salesOrder
            // add all the items in the record into inovice lines
            decimal balance = Decimal.Zero;
            List<SalesOrderLine> listLine = new List<SalesOrderLine>();
            foreach (SynchRecordLine lineFromSynch in recordFromSynch.recordLines)
            {
                // QBD uses an array pair to map attributes to their values.
                // The first array keeps track of what elements are in the second array.
                ItemsChoiceType2[] salesOrderItemAttributes =
                { 
                    ItemsChoiceType2.ItemId,
                    ItemsChoiceType2.UnitPrice,
                    ItemsChoiceType2.Qty 
                };
                // Now the second array
                object[] salesOrderItemValues =
                {
                    new IdType() 
                    {
                        idDomain = idDomainEnum.QB,
                        Value = upcToItemMap[lineFromSynch.upc].Id.Value
                    },
                    lineFromSynch.price,
                    new decimal(lineFromSynch.quantity) 
                };

                var salesOrderLine = new SalesOrderLine();
                salesOrderLine.Amount = (Decimal)lineFromSynch.price * lineFromSynch.quantity;
                salesOrderLine.AmountSpecified = true;
                salesOrderLine.Desc = upcToItemMap[lineFromSynch.upc].Desc;
                salesOrderLine.ItemsElementName = salesOrderItemAttributes;
                salesOrderLine.Items = salesOrderItemValues;

                listLine.Add(salesOrderLine);

                balance += salesOrderLine.Amount;
            }

            SalesOrderHeader salesOrderHeader = new SalesOrderHeader();

            salesOrderHeader.CustomerId = new IdType()
            {
                idDomain = idDomainEnum.QB,
                Value = customerIdToCustomerMap[recordFromSynch.clientId].Id.Value
            };

            salesOrderHeader.SalesRepId = new IdType()
            {
                idDomain = idDomainEnum.QB,
                Value = accountIdToSaleRepMap[recordFromSynch.accountId].Id.Value
            };

            salesOrderHeader.Balance = balance;
            salesOrderHeader.DueDate = DateTime.Now.AddDays(1);
            //salesOrderHeader.ShipAddr = physicalAddress;
            salesOrderHeader.ShipDate = ((DateTimeOffset)recordFromSynch.deliveryDate).DateTime;
            salesOrderHeader.ShipDateSpecified = true;

            salesOrderHeader.ToBeEmailed = false;
            salesOrderHeader.TotalAmt = salesOrderHeader.Balance;
            salesOrderHeader.Note = recordFromSynch.comment;

            SalesOrder salesOrder = new SalesOrder();
            salesOrder.Header = salesOrderHeader;
            salesOrder.Line = listLine.ToArray();
            
            // UPDATE-required fields below
            if (String.IsNullOrEmpty(recordFromSynch.integrationId))
            {
                // treat as new sales order
                salesOrderHeader.TxnDate = DateTime.Now.AddHours(SynchTimeZoneConverter.getLocalToUtcHourDifference(DateTime.Now, config.timezone));
                salesOrderHeader.TxnDateSpecified = true;

                return qbdDataService.Add(salesOrder);
            }
            else
            {
                string qbId = recordFromSynch.integrationId.Split(':')[0];
                string syncToken = recordFromSynch.integrationId.Split(':')[1];
                salesOrder.Id = new IdType()
                {
                    idDomain = idDomainEnum.NG,
                    Value = qbId
                };

                salesOrder.SyncToken = syncToken;

                return qbdDataService.Update(salesOrder);
            }
        }

        #endregion
    }
}
