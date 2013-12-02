using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intuit.Ipp.Core;
using Intuit.Ipp.Security;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Data;
using Intuit.Ipp.Diagnostics;
using Intuit.Ipp.Exception;
using Intuit.Ipp.Retry;
using Intuit.Ipp.Utility;

using QuickBooksIntegrationWorker.SynchLibrary;
using QuickBooksIntegrationWorker.SynchLibrary.Models;
using QuickBooksIntegrationWorker.QuickBooksLibrary;

namespace QuickBooksIntegrationWorker.IntegrationDataflow
{
    public class IntegrationController
    {
        private int synchBusinessId;

        QbDataController qbDataController;
        SynchDatabaseController synchDatabaseController;
        SynchStorageController synchStorageController;

        Dictionary<int, Customer> customerIdToCustomerMap;
        Dictionary<string, Item> upcToItemMap;

        IntegrationStatus integrationStatus;
        IntegrationConfiguration integrationConfig;

        public IntegrationController(int businessId)
        {
            this.synchBusinessId = businessId;
        }

        /// <summary>
        /// Initializes all the objects used in the integration later on, including
        /// logic controllers and data maps. If any of the initialization or data retrieval
        /// fails, a FALSE is returned to indicate a failure. TRUE is returned upon successful
        /// intialization of all the objects.
        /// </summary>
        /// <returns></returns>
        public bool initialize()
        {
            this.synchDatabaseController = new SynchDatabaseController(synchBusinessId);
            this.synchStorageController = new SynchStorageController(synchBusinessId);

            Utility.QbCredentialEntity qbCredentialEntity = synchStorageController.getQbCredentialEntity();
            if (qbCredentialEntity == null)
                return false;
            this.qbDataController = new QbDataController(synchBusinessId, qbCredentialEntity);

            Utility.QbConfigurationEntity qbConfigurationEntity = synchStorageController.getQbConfigurationEntity();
            if (qbConfigurationEntity == null)
                return false;
            this.integrationConfig = new IntegrationConfiguration(qbConfigurationEntity);

            this.upcToItemMap = new Dictionary<string, Item>();
            this.customerIdToCustomerMap = new Dictionary<int, Customer>();

            return true;
        }

        
        #region Update QuickBooks Desktop from Synch

        
        public void createInvoiceInQbd(int recordId)
        {
            
            try
            {
                // get invoice information from Synch database
                SynchRecord recordFromSynch = synchDatabaseController.getRecord(recordId);

                Invoice newInvoice = qbDataController.createInvoice(recordFromSynch, upcToItemMap, customerIdToCustomerMap, integrationConfig.timezone);

                // create a mapping for this invoice in storage so that we won't unnecessarily sync it back
                if (newInvoice != null)
                {
                    synchStorageController.createRecordMapping(recordId, newInvoice);
                }
                else
                {
                }

            }
            catch (Exception e)
            {
                DateTime currentDateTimePST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
                System.Diagnostics.Trace.TraceError(currentDateTimePST.ToString() + ":" + e.ToString());
            }
            
        }

        public void createSalesOrderInQbd(int recordId)
        {
            try
            {
                // get invoice information from Synch database
                SynchRecord recordFromSynch = synchDatabaseController.getRecord(recordId);

                Intuit.Ipp.Data.SalesOrder newSalesOrder = qbDataController.createSalesOrder(recordFromSynch, upcToItemMap, customerIdToCustomerMap, integrationConfig.timezone);

                if (newSalesOrder != null)
                {
                    synchStorageController.createRecordMapping(recordId, newSalesOrder);
                }
                else
                {
                }
            }
            catch (Exception e)
            {
                DateTime currentDateTimePST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
                System.Diagnostics.Trace.TraceError(currentDateTimePST.ToString() + ":" + e.ToString());
            }
        }

        [Obsolete]
        public void createBusinessInQbd(int otherBid, bool isCustomer)
        {
            /*
            // get invoice information from Synch database
            SynchDatabaseDataContext synchDataContext = new SynchDatabaseDataContext();
            var result = synchDataContext.GetBusiness(otherBid);
            IEnumerator<GetBusinessResult> businessEnumerator = result.GetEnumerator();
            SynchBusiness businessInSynch = null;
            if (businessEnumerator.MoveNext())
            {
                GetBusinessResult retrievedBusiness = businessEnumerator.Current;

                businessInSynch = new SynchBusiness()
                {
                    id = retrievedBusiness.id,
                    name = retrievedBusiness.name,
                    address = retrievedBusiness.address,
                    zip = (int)retrievedBusiness.zip,
                    email = retrievedBusiness.email,
                    phoneNumber = retrievedBusiness.phone_number
                };
            }
            else
                return;     // no business found

            string name = businessInSynch.name;
            string[] address = businessInSynch.address.Split(',');
            string email = businessInSynch.email;
            string zipCode = businessInSynch.zip.ToString();
            string stateCode = address[address.Length - 1].Trim();
            string phoneNumber = businessInSynch.phoneNumber;
            if (isCustomer)
            {
                // create a customer in Qbd
                Intuit.Ipp.Data.Qbd.Customer newCustomer = new Intuit.Ipp.Data.Qbd.Customer();
                newCustomer.Name = name;

                // add address
                Intuit.Ipp.Data.Qbd.PhysicalAddress ippAddress = new Intuit.Ipp.Data.Qbd.PhysicalAddress();
                ippAddress.Line1 = address[0];
                if (address.Length >= 4)
                    ippAddress.Line2 = address[1];
                ippAddress.City = address[address.Length - 2];
                ippAddress.CountrySubDivisionCode = stateCode;
                ippAddress.Country = "USA";
                ippAddress.PostalCode = zipCode;
                ippAddress.Tag = new string[] { "Billing" };
                newCustomer.Address = new Intuit.Ipp.Data.Qbd.PhysicalAddress[] { ippAddress };

                // add phone number
                Intuit.Ipp.Data.Qbd.TelephoneNumber ippPhoneNumber = new Intuit.Ipp.Data.Qbd.TelephoneNumber();
                ippPhoneNumber.FreeFormNumber = phoneNumber;
                ippPhoneNumber.Tag = new string[] { "Business" };
                newCustomer.Phone = new Intuit.Ipp.Data.Qbd.TelephoneNumber[] { ippPhoneNumber };

                // add email address
                Intuit.Ipp.Data.Qbd.EmailAddress ippEmail = new Intuit.Ipp.Data.Qbd.EmailAddress();
                ippEmail.Address = email;
                ippEmail.Tag = new string[] { "Business" };
                newCustomer.Email = new Intuit.Ipp.Data.Qbd.EmailAddress[] { ippEmail };

                Intuit.Ipp.Data.Qbd.Customer addedCustomer = commonService.Add(newCustomer);
            }
            else
            {
                // create a vendor in Qbd
                Intuit.Ipp.Data.Qbd.Vendor newVendor = new Intuit.Ipp.Data.Qbd.Vendor();
                newVendor.Name = name;

                // add address
                Intuit.Ipp.Data.Qbd.PhysicalAddress ippAddress = new Intuit.Ipp.Data.Qbd.PhysicalAddress();
                ippAddress.Line1 = address[0];
                if (address.Length >= 4)
                    ippAddress.Line2 = address[1];
                ippAddress.City = address[address.Length - 2];
                ippAddress.CountrySubDivisionCode = stateCode;
                ippAddress.Country = "USA";
                ippAddress.PostalCode = zipCode;
                ippAddress.Tag = new string[] { "Billing" };
                newVendor.Address = new Intuit.Ipp.Data.Qbd.PhysicalAddress[] { ippAddress };

                // add phone number
                Intuit.Ipp.Data.Qbd.TelephoneNumber ippPhoneNumber = new Intuit.Ipp.Data.Qbd.TelephoneNumber();
                ippPhoneNumber.FreeFormNumber = phoneNumber;
                newVendor.Phone = new Intuit.Ipp.Data.Qbd.TelephoneNumber[] { ippPhoneNumber };

                // add email address
                Intuit.Ipp.Data.Qbd.EmailAddress ippEmail = new Intuit.Ipp.Data.Qbd.EmailAddress();
                ippEmail.Address = email;
                newVendor.Email = new Intuit.Ipp.Data.Qbd.EmailAddress[] { ippEmail };

                Intuit.Ipp.Data.Qbd.Vendor addedVendor = commonService.Add(newVendor);
            }
             */
        }

        [Obsolete]
        public void createItemInQbd(string upc)
        {
            /*
            // get invoice information from Synch database
            SynchDatabaseDataContext synchDataContext = new SynchDatabaseDataContext();
            var result = synchDataContext.GetInventoryByUpc(synchBusinessId, upc);
            IEnumerator<GetInventoryByUpcResult> productEnumerator = result.GetEnumerator();
            SynchProduct newItemInSynch = null;
            if (productEnumerator.MoveNext())
            {
                GetInventoryByUpcResult target = productEnumerator.Current;
                newItemInSynch = new SynchProduct()
                {
                    upc = target.upc,
                    name = target.name,
                    detail = target.detail,
                    quantity = (int)target.quantity,
                    location = target.location,
                    owner = synchBusinessId,
                    leadTime = (int)target.lead_time,
                    price = (double)target.default_price
                };
            }

            if (newItemInSynch != null)
            {
                Intuit.Ipp.Data.Qbd.Item newItem = new Intuit.Ipp.Data.Qbd.Item();
                newItem.Active = true;
                newItem.Name = newItemInSynch.name;
                newItem.Desc = newItemInSynch.detail;
                newItem.QtyOnHand = newItemInSynch.quantity;
                newItem.QtyOnHandSpecified = true;
                newItem.Type = Intuit.Ipp.Data.Qbd.ItemTypeEnum.Inventory;
                newItem.TypeSpecified = true;

                // assign income account
                Intuit.Ipp.Data.Qbd.AccountRef incomeAccountRef = new Intuit.Ipp.Data.Qbd.AccountRef();
                incomeAccountRef.AccountName = "AB Sales-Wholesale:Wines Sold:Importer Wines:Hand of God";
                newItem.IncomeAccountRef = incomeAccountRef;

                // assign COGS account
                Intuit.Ipp.Data.Qbd.AccountRef cogsAccountRef = new Intuit.Ipp.Data.Qbd.AccountRef();
                cogsAccountRef.AccountName = "Cost of Inventory Sold:Cost of Wine:Importer Wines:Hand of God";
                newItem.COGSAccountRef = cogsAccountRef;

                // assign asset account
                Intuit.Ipp.Data.Qbd.AccountRef assetAccountRef = new Intuit.Ipp.Data.Qbd.AccountRef();
                assetAccountRef.AccountName = "Inventory Asset";
                newItem.AssetAccountRef = assetAccountRef;

                // assign money
                Intuit.Ipp.Data.Qbd.Money ippMoney = new Intuit.Ipp.Data.Qbd.Money();
                ippMoney.Amount = (decimal)newItemInSynch.price;
                ippMoney.CurrencyCode = Intuit.Ipp.Data.Qbd.currencyCode.USD;
                newItem.AvgCost = ippMoney;

                newItem.Name = newItem.Name.Substring(0, 31);

                Intuit.Ipp.Data.Qbd.Item addedItem = commonService.Add(newItem);
            }
             */
        }

        [Obsolete]
        public void updateItemInQbd(string upc)
        {
            /*
            SynchDatabaseDataContext synchDataContext = new SynchDatabaseDataContext();
            var result = synchDataContext.GetInventoryByUpc(synchBusinessId, upc);
            IEnumerator<GetInventoryByUpcResult> productEnumerator = result.GetEnumerator();
            SynchProduct itemInSynch = null;
            if (productEnumerator.MoveNext())
            {
                GetInventoryByUpcResult target = productEnumerator.Current;
                itemInSynch = new SynchProduct()
                {
                    upc = target.upc,
                    name = target.name,
                    detail = target.detail,
                    quantity = (int)target.quantity,
                    location = target.location,
                    owner = synchBusinessId,
                    leadTime = (int)target.lead_time,
                    price = (double)target.default_price
                };
            }

            if (itemInSynch != null)
            {
                createItemNameToItemMap();
                Intuit.Ipp.Data.Qbd.Item currentItem = itemNameToItemMap[itemInSynch.name];

            }*/
        }

        #endregion


        #region Update Synch from QuickBooks Desktop
        
        /*
        public void updateInvoicesFromQbd()
        {
            // get product mapping information from Qbd
            Dictionary<string, string> itemIdToUpcMap = synchStorageReader.getItemIdToUpcMap(ApplicationConstants.ERP_Qbd_TABLE_PRODUCT);
            Dictionary<string, int> customerIdToSynchCidMap = synchStorageReader.getCustomerIdToSynchCidMap(ApplicationConstants.ERP_Qbd_TABLE_BUSINESS);

            if (transactionStartDateFilter == null)
                transactionStartDateFilter = new DateTime(2013, 1, 1);

            List<Intuit.Ipp.Data.Qbd.Invoice> invoicesFromQbd = QbdDataReader.getInvoicesFromDate(transactionStartDateFilter);

            int successCount = 0;
            int noCustomerCount = 0;
            int missingInfoCount = 0;
            int noUpcCount = 0;

            for (int invoiceIndex = 0; invoiceIndex < invoicesFromQbd.Count; invoiceIndex++)
            {
                Intuit.Ipp.Data.Qbd.Invoice curInvoice = invoicesFromQbd[invoiceIndex];

                if (transactionIdToEntityMap.ContainsKey(curInvoice.Id.Value))
                {
                    // this invoice exists;
                    // check if updates needed.
                }
                else
                {
                    string customerIdFromQbd = curInvoice.Header.CustomerId.Value;
                    string invoiceTitle = "Invoiced: " + curInvoice.Header.CustomerName;
                    string invoiceComment = "Invoiced From QuickBooks: " + curInvoice.Header.Msg;
                    string transactionDateString = String.Empty;
                    if (curInvoice.Header.TxnDateSpecified)
                    {
                        transactionDateString = curInvoice.Header.TxnDate.Year.ToString();
                        if (curInvoice.Header.TxnDate.Month > 9)
                            transactionDateString += curInvoice.Header.TxnDate.Month.ToString();
                        else
                            transactionDateString += "0" + curInvoice.Header.TxnDate.Month.ToString();

                        if (curInvoice.Header.TxnDate.Day > 9)
                            transactionDateString += curInvoice.Header.TxnDate.Day.ToString();
                        else
                            transactionDateString += "0" + curInvoice.Header.TxnDate.Day.ToString();

                        transactionDateString += "000000";
                    }
                    long transactionDateLong = (transactionDateString == String.Empty) ? 20111231000000 : long.Parse(transactionDateString);

                    if (customerIdToSynchCidMap.ContainsKey(customerIdFromQbd))
                    {
                        // this customer exists
                        int customerIdFromSynch = customerIdToSynchCidMap[customerIdFromQbd];

                        List<string> upcList = new List<string>();
                        List<int> quantityList = new List<int>();
                        List<double> priceList = new List<double>();
                        foreach (Intuit.Ipp.Data.Qbd.InvoiceLine curLine in curInvoice.Line)
                        {
                            string upc = null;
                            int quantity = 0;
                            double price = 0.0;
                            if (curLine.ItemsElementName != null && curLine.Items != null)
                            {
                                for (int i = 0; i < curLine.ItemsElementName.Length; i++)
                                {
                                    if (curLine.ItemsElementName[i].ToString() == "ItemId")
                                    {
                                        string itemId = ((Intuit.Ipp.Data.Qbd.IdType)curLine.Items[i]).Value;
                                        if (itemIdToUpcMap.ContainsKey(itemId))
                                            upc = itemIdToUpcMap[itemId];
                                        else
                                            noUpcCount++;
                                    }
                                    if (curLine.ItemsElementName[i].ToString() == "UnitPrice")
                                        price = Double.Parse(curLine.Items[i].ToString());

                                    if (curLine.ItemsElementName[i].ToString() == "Qty")
                                        quantity = Int32.Parse(curLine.Items[i].ToString());
                                }

                                if (upc != null && quantity != 0 && price != 0.0)
                                {
                                    // now create this line item in database
                                    upcList.Add(upc);
                                    quantityList.Add(quantity);
                                    priceList.Add(price);
                                }
                                else
                                {
                                    missingInfoCount++;
                                }
                            }   // if item information exists
                            else
                            {
                                missingInfoCount++;
                            }
                        }   // end foreach line item

                        if (upcList.Count > 0)
                        {
                            int rid = synchDatabaseUpdater.createNewRecord(invoiceTitle, 0, (int)RecordStatus.closed, invoiceComment, 42, transactionDateLong, customerIdFromSynch,
                                                                                upcList, quantityList, priceList);
                            if (rid > 0)
                            {
                                synchStorageUpdater.createRecordMapping(ApplicationConstants.ERP_Qbd_TABLE_RECORD, rid, curInvoice.Id.Value);
                            }
                            successCount++;

                        }
                    }   // if this customer exists
                    else
                        noCustomerCount++;

                }   // end new invoice
            }

        }

        public void updateSalesOrdersFromQbd()
        {
            // get product mapping information from Qbd
            Dictionary<string, string> itemIdToUpcMap = synchStorageReader.getItemIdToUpcMap(ApplicationConstants.ERP_Qbd_TABLE_PRODUCT);
            Dictionary<string, int> customerIdToSynchCidMap = synchStorageReader.getCustomerIdToSynchCidMap(ApplicationConstants.ERP_Qbd_TABLE_BUSINESS);

            if (transactionStartDateFilter == null)
                transactionStartDateFilter = new DateTime(2013, 1, 1);
            
            List<Intuit.Ipp.Data.Qbd.SalesOrder> salesOrdersFromQbd = QbdDataReader.getSalesOrdersFromDate(transactionStartDateFilter);

            int successCount = 0;
            int noCustomerCount = 0;
            int missingInfoCount = 0;
            int noUpcCount = 0;

            for (int salesOrderIndex = 0; salesOrderIndex < salesOrdersFromQbd.Count; salesOrderIndex++)
            {
                Intuit.Ipp.Data.Qbd.SalesOrder curSalesOrder = salesOrdersFromQbd[salesOrderIndex];

                if (transactionIdToEntityMap.ContainsKey(curSalesOrder.Id.Value))
                {
                    // this salesOrder exists;
                    // check if updates needed.
                }
                else
                {
                    string customerIdFromQbd = curSalesOrder.Header.CustomerId.Value;
                    string salesOrderTitle = "From QuickBooks: " + curSalesOrder.Header.CustomerName;
                    string salesOrderComment = "From QuickBooks: " + curSalesOrder.Header.Msg;
                    string transactionDateString = String.Empty;
                    if (curSalesOrder.Header.TxnDateSpecified)
                    {
                        transactionDateString = curSalesOrder.Header.TxnDate.Year.ToString();
                        if (curSalesOrder.Header.TxnDate.Month > 9)
                            transactionDateString += curSalesOrder.Header.TxnDate.Month.ToString();
                        else
                            transactionDateString += "0" + curSalesOrder.Header.TxnDate.Month.ToString();

                        if (curSalesOrder.Header.TxnDate.Day > 9)
                            transactionDateString += curSalesOrder.Header.TxnDate.Day.ToString();
                        else
                            transactionDateString += "0" + curSalesOrder.Header.TxnDate.Day.ToString();

                        transactionDateString += "000000";
                    }
                    long transactionDateLong = (transactionDateString == String.Empty) ? 20111231000000 : long.Parse(transactionDateString);

                    if (customerIdToSynchCidMap.ContainsKey(customerIdFromQbd))
                    {
                        // this customer exists
                        int customerIdFromSynch = customerIdToSynchCidMap[customerIdFromQbd];

                        List<string> upcList = new List<string>();
                        List<int> quantityList = new List<int>();
                        List<double> priceList = new List<double>();
                        foreach (Intuit.Ipp.Data.Qbd.SalesOrderLine curLine in curSalesOrder.Line)
                        {
                            string upc = null;
                            int quantity = 0;
                            double price = 0.0;
                            if (curLine.ItemsElementName != null && curLine.Items != null)
                            {
                                for (int i = 0; i < curLine.ItemsElementName.Length; i++)
                                {
                                    if (curLine.ItemsElementName[i].ToString() == "ItemId")
                                    {
                                        string itemId = ((Intuit.Ipp.Data.Qbd.IdType)curLine.Items[i]).Value;
                                        if (itemIdToUpcMap.ContainsKey(itemId))
                                            upc = itemIdToUpcMap[itemId];
                                        else
                                            noUpcCount++;
                                    }
                                    if (curLine.ItemsElementName[i].ToString() == "UnitPrice")
                                        price = Double.Parse(curLine.Items[i].ToString());

                                    if (curLine.ItemsElementName[i].ToString() == "Qty")
                                        quantity = Int32.Parse(curLine.Items[i].ToString());
                                }

                                if (upc != null && quantity != 0 && price != 0.0)
                                {
                                    // now create this line item in database
                                    //context.CreateProductInRecord(recordId, upc, synchBusinessId, customerIdFromSynch, quantity, note, price);
                                    upcList.Add(upc);
                                    quantityList.Add(quantity);
                                    priceList.Add(price);
                                }
                                else
                                {
                                    missingInfoCount++;
                                }
                            }   // if item information exists
                            else
                            {
                                missingInfoCount++;
                            }
                        }   // end foreach line item

                        if (upcList.Count > 0)
                        {
                            int rid = synchDatabaseUpdater.createNewRecord(salesOrderTitle, 0, (int)RecordStatus.sent, salesOrderComment, 42, transactionDateLong, customerIdFromSynch,
                                                                                upcList, quantityList, priceList);
                            if (rid > 0)
                            {
                                synchStorageUpdater.createRecordMapping(ApplicationConstants.ERP_Qbd_TABLE_RECORD, rid, curSalesOrder.Id.Value);
                            }
                            successCount++;
                        }
                    }   // if this customer exists
                    else
                        noCustomerCount++;

                }   // end new salesOrder
            }
        }

        
        public void updateItemsFromQbd()
        {
            // 1: get current inventory list
            Dictionary<string, SynchProduct> upcToInventoryMap = synchDatabaseReader.getUpcToInventoryMap();
            getAutoUpcCounter(upcToInventoryMap.Keys);
            Dictionary<string, string> itemIdToUpcMap = synchStorageReader.getItemIdToUpcMap(ApplicationConstants.ERP_Qbd_TABLE_PRODUCT);

            // 2: get updated information from Qbd side
            List<Intuit.Ipp.Data.Qbd.Item> itemsFromQbd = QbdDataReader.getItems();

            // logic of matching item information
            for (int i = 0; i < itemsFromQbd.Count(); i++)
            {
                Intuit.Ipp.Data.Qbd.Item curItem = itemsFromQbd[i];

                // checks if this is a legitimate product we want to sync
                if (String.IsNullOrEmpty(curItem.Name))
                    continue;
                if (!curItem.Active)
                    continue;
                if (String.IsNullOrEmpty(curItem.Desc))
                    continue;
                if (curItem.Type != Intuit.Ipp.Data.Qbd.ItemTypeEnum.Product && curItem.Type != Intuit.Ipp.Data.Qbd.ItemTypeEnum.Inventory)
                    continue;
                if (!curItem.QtyOnHandSpecified)
                    continue;

                string nameFromQbd = curItem.Name;
                string itemId = curItem.Id.Value;
                string detailFromQbd = curItem.Desc;
                double priceFromQbd = 0.99;         // default price
                int quantityFromQbd = Convert.ToInt32(curItem.QtyOnHand);

                // takes into account the quantity on sales order, which includes
                // orders generated from Synch as well as orders generated from QuickBooks directly
                if (curItem.QtyOnSalesOrderSpecified)
                    quantityFromQbd -= Convert.ToInt32(curItem.QtyOnSalesOrder);

                Intuit.Ipp.Data.Qbd.Money costFromQbd = (Intuit.Ipp.Data.Qbd.Money)curItem.Item1;
                if (costFromQbd != null)
                    priceFromQbd = Convert.ToDouble(costFromQbd.Amount);

                // now get current product linking information from Table Storage mapping,
                // or create a new mapping if no mapping exists.
                if (!itemIdToUpcMap.ContainsKey(itemId))
                {
                    string upc = null;
                    upc = matchNameAndDetailWithInventory(nameFromQbd, detailFromQbd, upcToInventoryMap.Values);

                    if (upc == null)
                    {
                        // when no mapping exist and no product with same name/detail exist in our database,
                        // we create new one for them
                        autoUpcCounter++;
                        string autoUpc = autoUpcPrefix + autoUpcCounter;
                        synchDatabaseUpdater.createNewInventory(autoUpc, nameFromQbd, detailFromQbd, "temporary location", quantityFromQbd, 7, priceFromQbd, 0);
                        synchStorageUpdater.createProductMapping(ApplicationConstants.ERP_Qbd_TABLE_PRODUCT, autoUpc, itemId);
                    }
                    else
                    {
                        // when we have the same product with missing/incorrect mapping information in storage
                        upcToInventoryMap.Remove(upc);
                        synchStorageUpdater.createProductMapping(ApplicationConstants.ERP_Qbd_TABLE_PRODUCT, upc, itemId);
                    }
                }
                else
                {
                    string upc = itemIdToUpcMap[itemId];
                    itemIdToUpcMap.Remove(itemId);

                    if (upcToInventoryMap.ContainsKey(upc))
                    {
                        // this product with correct upc exists in Synch, update if needed
                        SynchProduct itemFromSynch = upcToInventoryMap[upc];
                        upcToInventoryMap.Remove(upc);
                        if (detailFromQbd != itemFromSynch.detail ||
                            priceFromQbd != itemFromSynch.price ||
                            quantityFromQbd != itemFromSynch.quantity ||
                            nameFromQbd != itemFromSynch.name)
                        {
                            synchDatabaseUpdater.updateInventory(itemFromSynch.upc, detailFromQbd, quantityFromQbd, priceFromQbd, synchBusinessId, nameFromQbd);

                        }
                    }
                    else
                    {
                        // this upc does not exist in Synch, create new one
                        synchDatabaseUpdater.createNewInventory(upc, nameFromQbd, detailFromQbd, "Unassigned", quantityFromQbd, 7, priceFromQbd, 0);
                    }
                }
            }

            // 3. After matching all the products from Qbd, we delete excessive/inactive products in Synch
            foreach (string upc in upcToInventoryMap.Keys)
                synchDatabaseUpdater.deleteInventory(upc);

            foreach (string itemId in itemIdToUpcMap.Keys)
                synchStorageUpdater.deleteProductMapping(ApplicationConstants.ERP_Qbd_TABLE_PRODUCT, itemId);
        }

        private void getAutoUpcCounter(Dictionary<string, SynchProduct>.KeyCollection keyCollection)
        {
            string[] upcs = keyCollection.ToArray<string>();
            int maxCurrentCount = 0;
            foreach (string upc in upcs)
            {
                if (upc.StartsWith(autoUpcPrefix))
                {
                    int curCount = 0;
                    Int32.TryParse(upc.Split('O')[1], out curCount);
                    if (curCount > maxCurrentCount)
                        maxCurrentCount = curCount;
                }
            }

            autoUpcCounter = maxCurrentCount;
        }
        */

        public void updateCustomersFromQbd()
        {
            IEnumerable<Customer> customersFromQbd = qbDataController.getActiveCustomers();
            foreach (Customer customerFromQbd in customersFromQbd)
            {
                if (String.IsNullOrEmpty(customerFromQbd.CompanyName))
                    continue;

                string nameFromQbd = customerFromQbd.CompanyName;
                string idFromQbd = customerFromQbd.Id;

                string addressFromQbd = "empty address";
                string postalCodeFromQbd = "98105";
                if (customerFromQbd.ShipAddr != null)
                {
                    postalCodeFromQbd = (customerFromQbd.ShipAddr.PostalCode == null) ? "98105" :
                                    customerFromQbd.ShipAddr.PostalCode;

                    if (customerFromQbd.ShipAddr.Line1 == null)
                        addressFromQbd = customerFromQbd.ShipAddr.City + ", " + customerFromQbd.ShipAddr.CountrySubDivisionCode;
                    else
                    {
                        addressFromQbd = customerFromQbd.ShipAddr.Line1 + ", ";
                        if (customerFromQbd.ShipAddr.Line2 == null)
                            addressFromQbd += customerFromQbd.ShipAddr.City + ", " + customerFromQbd.ShipAddr.CountrySubDivisionCode;
                        else
                            addressFromQbd += customerFromQbd.ShipAddr.Line2
                                                + ", " + customerFromQbd.ShipAddr.City + ", "
                                                + customerFromQbd.ShipAddr.CountrySubDivisionCode;

                    }
                }

                string emailFromQbd = (customerFromQbd.PrimaryEmailAddr == null) ? "changhao.han@gmail.com" : customerFromQbd.PrimaryEmailAddr.Address;
                string phoneNumFromQbd = (customerFromQbd.PrimaryPhone == null) ? "206-407-9494" : customerFromQbd.PrimaryPhone.FreeFormNumber;
            }


            // 1: get current customer list
            /*
            Dictionary<int, SynchBusiness> bidToSynchBusinessMap = synchDatabaseReader.getBidToCustomerMap();

            // 2: get information from table storage
            Dictionary<string, int> customerIdToSynchBidMap = synchStorageReader.getCustomerIdToSynchCidMap(ApplicationConstants.ERP_Qbd_TABLE_BUSINESS); 

            // 3: get updated information from Qbd side
            List<Intuit.Ipp.Data.Qbd.Customer> customersFromQbd = QbdDataReader.getCustomers();

            // logic of matching customer info
            for (int i = 0; i < customersFromQbd.Count(); i++)
            {
                Intuit.Ipp.Data.Qbd.Customer curCustomer = customersFromQbd[i];

                if (String.IsNullOrEmpty(curCustomer.Name))
                    continue;

                string nameFromQbd = curCustomer.Name;
                string idFromQbd = curCustomer.Id.Value;

                string addressFromQbd = "empty address";
                string zipFromQbd = "98105";
                if (curCustomer.Address != null)
                {
                    zipFromQbd = (curCustomer.Address[0].PostalCode == null) ? "98105" :
                                    curCustomer.Address[0].PostalCode.Split('-')[0];

                    if (curCustomer.Address[0].Line1 == null)
                        addressFromQbd = curCustomer.Address[0].City + ", " + curCustomer.Address[0].CountrySubDivisionCode;
                    else
                    {
                        addressFromQbd = curCustomer.Address[0].Line1 + ", ";
                        if (curCustomer.Address[0].Line2 == null)
                            addressFromQbd += curCustomer.Address[0].City + ", " + curCustomer.Address[0].CountrySubDivisionCode;
                        else
                            addressFromQbd += curCustomer.Address[0].Line2
                                                + ", " + curCustomer.Address[0].City + ", "
                                                + curCustomer.Address[0].CountrySubDivisionCode;

                    }
                }

                int intZipFromQbd = -1;
                if (!Int32.TryParse(zipFromQbd, out intZipFromQbd))
                    intZipFromQbd = 98105;
                string emailFromQbd = (curCustomer.Email == null) ? "changhao.han@gmail.com" : curCustomer.Email[0].Address;
                string categoryFromQbd = (curCustomer.Category == null) ? "empty_category" : curCustomer.Category;
                string phoneNumFromQbd = (curCustomer.Phone == null) ? "206-407-9494" : curCustomer.Phone[0].FreeFormNumber;

                // compare information now
                // 1. try to get cid from table storage mapping
                if (!customerIdToSynchBidMap.ContainsKey(idFromQbd))
                {
                    // not in table storage right now, and considered not in Synch database
                    // create new business in Synch and a new mapping
                    int newCustomerId = synchDatabaseUpdater.createCustomer(nameFromQbd, addressFromQbd, intZipFromQbd, emailFromQbd, categoryFromQbd, 0, 0, phoneNumFromQbd);
                    if (newCustomerId != -1)
                        synchStorageUpdater.createBusinessMapping(ApplicationConstants.ERP_Qbd_TABLE_BUSINESS, newCustomerId, idFromQbd);
                }
                else
                {
                    // in table storage; get business info from Synch
                    int idFromSynch = customerIdToSynchBidMap[idFromQbd];
                    SynchBusiness currentBusinessFromSynch = null;

                    if (!bidToSynchBusinessMap.ContainsKey(idFromSynch))
                    {
                        // business mapping exists, but business id in Synch is outdated;
                        // create new business in Synch and a new mapping; later on delete outdated ones
                        int newCustomerId = synchDatabaseUpdater.createCustomer(nameFromQbd, addressFromQbd, intZipFromQbd, emailFromQbd, categoryFromQbd, 0, 0, phoneNumFromQbd);
                        if (newCustomerId != -1)
                            synchStorageUpdater.createBusinessMapping(ApplicationConstants.ERP_Qbd_TABLE_BUSINESS, newCustomerId, idFromQbd);
                    }
                    else
                    {
                        // business mapping exist and business id is update;
                        // check and update business information
                        currentBusinessFromSynch = bidToSynchBusinessMap[idFromSynch];
                        bidToSynchBusinessMap.Remove(idFromSynch);
                        customerIdToSynchBidMap.Remove(idFromQbd);

                        if (addressFromQbd != currentBusinessFromSynch.address
                            || intZipFromQbd != currentBusinessFromSynch.zip
                            || emailFromQbd != currentBusinessFromSynch.email
                            || phoneNumFromQbd != currentBusinessFromSynch.phoneNumber)
                        {
                            // update new info into Synch's database
                            synchDatabaseUpdater.updateBusinessById(currentBusinessFromSynch.id, addressFromQbd, intZipFromQbd, emailFromQbd, categoryFromQbd, phoneNumFromQbd);
                        }
                    }
                }   // end if mapping in storage

            }

            // 3. After matching all the customers from Qbd, we delete excessive/inactive customers in Synch
            foreach (int cid in bidToSynchBusinessMap.Keys)
            {
                synchDatabaseUpdater.deleteCustomer(cid);
            }

            foreach (string customerId in customerIdToSynchBidMap.Keys)
            {
                synchStorageUpdater.deleteBusinessMapping(ApplicationConstants.ERP_Qbd_TABLE_BUSINESS, customerId);
            }
             */

        }
        #endregion


        #region private helper methods that are not always used in the service

        private string matchNameAndDetailWithInventory(string nameFromQbd, string detailFromQbd,
            Dictionary<string, SynchInventory>.ValueCollection synchInventories)
        {
            foreach (SynchInventory i in synchInventories)
            {
                if (nameFromQbd == i.name || detailFromQbd == i.detail)
                    return i.upc;
            }

            return null;
        }

        #endregion

    }
}
