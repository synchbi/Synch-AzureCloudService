using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intuit.Ipp.Core;
using Intuit.Ipp.Security;
using Intuit.Ipp.Data;
using Intuit.Ipp.Data.Qbd;
using Intuit.Ipp.Diagnostics;
using Intuit.Ipp.Exception;
using Intuit.Ipp.Retry;
using Intuit.Ipp.Utility;

using QBDIntegrationWorker.SynchLibrary;
using QBDIntegrationWorker.SynchLibrary.Models;
using QBDIntegrationWorker.QuickBooksLibrary;
using QBDIntegrationWorker.Utility;

namespace QBDIntegrationWorker.IntegrationDataflow
{
    public class IntegrationController
    {
        private int synchBusinessId;

        QbDataController qbDataController;
        SynchDatabaseController synchDatabaseController;
        SynchStorageController synchStorageController;

        Dictionary<int, Customer> customerIdToQbCustomerMap;
        Dictionary<string, Item> upcToItemMap;
        Dictionary<int, SalesRep> accountIdToSalesRepMap;

        public IntegrationStatus integrationStatus;
        IntegrationConfiguration integrationConfig;

        public IntegrationController(int businessId)
        {
            this.synchBusinessId = businessId;

            this.customerIdToQbCustomerMap = new Dictionary<int, Customer>();
            this.upcToItemMap = new Dictionary<string, Item>();
            this.accountIdToSalesRepMap = new Dictionary<int, SalesRep>();

            integrationStatus = new IntegrationStatus(synchBusinessId);

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
            try
            {
                integrationStatus.overallSyncStatusCode = SyncStatusCode.Started;

                // Synch Side data controllers
                this.synchDatabaseController = new SynchDatabaseController(synchBusinessId);
                this.synchStorageController = new SynchStorageController(synchBusinessId);

                // QBD Side data controllers
                Utility.QbCredentialEntity qbCredentialEntity = synchStorageController.getQbCredentialEntity();
                if (qbCredentialEntity == null)
                    return false;
                this.qbDataController = new QbDataController(synchBusinessId, qbCredentialEntity);

                Utility.QbConfigurationEntity qbConfigurationEntity = synchStorageController.getQbConfigurationEntity();
                if (qbConfigurationEntity == null)
                    return false;
                this.integrationConfig = new IntegrationConfiguration(qbConfigurationEntity);

                return true;
            }
            catch (Exception e)
            {
                integrationStatus.overallSyncStatusCode = SyncStatusCode.ConnectionFailure;
                integrationStatus.registerException(e);
                return false;
            }
        }

        public void finalize()
        {
            integrationStatus.finish();
            synchStorageController.updateStatusEntity(integrationStatus);
        }

        
        #region Update QuickBooks Desktop from Synch

        public int createRecordInQbd(int recordId)
        {
            if (integrationConfig.syncOrderAsInvoice)
                return createInvoiceInQbd(recordId);
            else
                return createSalesOrderInQb(recordId);
        }

        public int createInvoiceInQbd(int recordId)
        {   
            try
            {
                System.Diagnostics.Trace.TraceInformation("QBD: creating Invoice for record " + recordId);
                integrationStatus.invoiceSyncFromSynchStatusCode = SyncStatusCode.Started;

                // get invoice information from Synch database
                SynchRecord recordFromSynch = synchDatabaseController.getRecord(recordId);

                Invoice newInvoice = qbDataController.createInvoice(recordFromSynch, upcToItemMap,
                    customerIdToQbCustomerMap, accountIdToSalesRepMap, integrationConfig.timezone);

                // create a mapping for this invoice in storage so that we won't unnecessarily sync it back
                if (newInvoice != null)
                {
                    synchStorageController.createRecordMapping(recordId, newInvoice);
                    System.Diagnostics.Trace.TraceInformation("QBD: creating Invoice for record " + recordId);
                }
                else
                {
                    DateTime currentDateTimePST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
                    System.Diagnostics.Trace.TraceError(currentDateTimePST.ToString() + ":" + "failed to create invoice\n" + integrationStatus.ToString());
                }

                integrationStatus.invoiceSyncFromSynchStatusCode = SyncStatusCode.SyncSuccess;

                return 0;
            }
            catch (Exception e)
            {
                integrationStatus.invoiceSyncFromSynchStatusCode = SyncStatusCode.SyncFailure;
                integrationStatus.registerException(e);

                return 1;
            }
            
        }

        public int createSalesOrderInQb(int recordId)
        {
            try
            {
                System.Diagnostics.Trace.TraceInformation("QBD: creating Sales Order for record " + recordId);
                integrationStatus.invoiceSyncFromSynchStatusCode = SyncStatusCode.Started;

                // get invoice information from Synch database
                SynchRecord recordFromSynch = synchDatabaseController.getRecord(recordId);

                SalesOrder newSalesOrder = qbDataController.createSalesOrder(recordFromSynch, upcToItemMap,
                    customerIdToQbCustomerMap, accountIdToSalesRepMap, integrationConfig.timezone);

                if (newSalesOrder != null)
                {
                    synchStorageController.createRecordMapping(recordId, newSalesOrder);
                    System.Diagnostics.Trace.TraceInformation("QBD: finished creating Sales Order for record " + recordId);
                }
                else
                {
                    DateTime currentDateTimePST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
                    System.Diagnostics.Trace.TraceError(currentDateTimePST.ToString() + ":" + "failed to create sales order\n" + integrationStatus.ToString());
                }

                integrationStatus.invoiceSyncFromSynchStatusCode = SyncStatusCode.SyncSuccess;

                return 0;

            }
            catch (Exception e)
            {
                integrationStatus.invoiceSyncFromSynchStatusCode = SyncStatusCode.SyncFailure;

                integrationStatus.registerException(e);

                return 1;
            }
        }

        public int updateRecordInQbd(int recordId)
        {
            try
            {
                System.Diagnostics.Trace.TraceInformation("QBD: updating Invoice for record " + recordId);
                integrationStatus.salesOrderSyncFromSynchStatusCode = SyncStatusCode.Started;

                // get invoice information from Synch database
                SynchRecord recordFromSynch = synchDatabaseController.getRecord(recordId);

                qbDataController.updateSalesOrder(recordFromSynch, upcToItemMap,
                    customerIdToQbCustomerMap, accountIdToSalesRepMap, integrationConfig.timezone);

                integrationStatus.invoiceSyncFromSynchStatusCode = SyncStatusCode.SyncSuccess;

                return 0;
            }
            catch (Exception e)
            {
                integrationStatus.salesOrderSyncFromSynchStatusCode = SyncStatusCode.SyncFailure;
                integrationStatus.registerException(e);

                return 1;
            }

        }

        [Obsolete]
        public void createBusinessInQb(int otherBid, bool isCustomer)
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
        public void createItemInQb(string upc)
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
        public void updateItemInQb(string upc)
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

        public void updateSalesRepsFromQb()
        {
            try
            {
                integrationStatus.salesRepSyncFromQbStatusCode = SyncStatusCode.Started;

                Dictionary<string, ERPAccountMapEntity> qbIdToEntityMap = synchStorageController.getQbSalesRepIdToEntityMap();
                IEnumerable<SalesRep> salesRepsFromQbd = qbDataController.getActiveSalesReps();
                foreach (SalesRep salesRep in salesRepsFromQbd)
                {
                    
                    if (qbIdToEntityMap.ContainsKey(salesRep.Id.Value))
                    {
                        // this mapping exists and the server assumes it is up-to-date from the Dashboard manual linking;
                        int accountId = qbIdToEntityMap[salesRep.Id.Value].accountIdFromSynch;
                        if (!accountIdToSalesRepMap.ContainsKey(accountId))
                            accountIdToSalesRepMap.Add(accountId, salesRep);
                    }
                    else
                    {
                        // this sales rep does not exist in our mapping;
                        // create this new sales rep in the mapping and map it to default right now
                        synchStorageController.createAccountMapping(integrationConfig.defaultAccountId, salesRep);
                        if (!accountIdToSalesRepMap.ContainsKey(integrationConfig.defaultAccountId))
                            accountIdToSalesRepMap.Add(integrationConfig.defaultAccountId, salesRep);
                    }
                        
                }

                integrationStatus.salesRepSyncFromQbStatusCode = SyncStatusCode.SyncSuccess;

            }
            catch (Exception e)
            {
                integrationStatus.salesRepSyncFromQbStatusCode = SyncStatusCode.SyncFailure;

                integrationStatus.registerException(e);
            }
        }

        public void updateItemsFromQb()
        {
            try
            {
                integrationStatus.productSyncFromQbStatusCode = SyncStatusCode.Started;

                // 1: get current inventory list
                Dictionary<string, SynchInventory> upcToInventoryMap = synchDatabaseController.getUpcToInventoryMap();
                string autoUpcPrefix = synchBusinessId + "AUTO";
                int autoUpcCounter = getAutoUpcCounter(autoUpcPrefix, upcToInventoryMap.Keys);
                Dictionary<string, ERPProductMapEntity> qbIdToEntityMap = synchStorageController.getQbItemIdToEntityMap();

                // 2: get updated information from Qbd side
                IEnumerable<Item> itemsFromQbd = qbDataController.getActiveItems();

                // logic of matching item information
                foreach ( Item item in itemsFromQbd)
                {
                    // checks if this is a legitimate product we want to sync
                    if (String.IsNullOrEmpty(item.Name))
                        continue;
                    if (!item.Active)
                        continue;
                    if (String.IsNullOrEmpty(item.Desc))
                        continue;
                    if (item.Type != ItemTypeEnum.Inventory && item.Type != ItemTypeEnum.Service)
                        continue;
                    if (!item.QtyOnHandSpecified)
                        continue;
                    if (item.Item1 == null)     // ignore items with no price information
                        continue;

                    SynchInventory inventoryFromQb = new SynchInventory()
                    {
                        name = item.Name,
                        defaultPrice = Convert.ToDecimal(((Intuit.Ipp.Data.Qbd.Money)item.Item1).Amount),
                        detail = item.Desc,
                        quantityAvailable = Convert.ToInt32(item.QtyOnHand),
                        businessId = synchBusinessId,
                        category = 0,
                        leadTime = 7,
                        location = "temporary location"
                    };

                    // default values for these fields
                    int reorderPointFromQbd = 20;

                    // takes into account the quantity on sales order, which includes
                    // orders generated from Synch as well as orders generated from QuickBooks directly
                    if (item.QtyOnSalesOrderSpecified)
                        inventoryFromQb.quantityAvailable -= Convert.ToInt32(item.QtyOnSalesOrder);

                    if (item.ReorderPointSpecified)
                        inventoryFromQb.reorderPoint = Convert.ToInt32(item.ReorderPoint);
                    inventoryFromQb.reorderQuantity = reorderPointFromQbd / 2;

                    if (item.QtyOnPurchaseOrderSpecified)
                        inventoryFromQb.quantityOnPurchaseOrder = Convert.ToInt32(item.QtyOnPurchaseOrder);

                    // now get current product linking information from Table Storage mapping,
                    // or create a new mapping if no mapping exists.
                    if (!qbIdToEntityMap.ContainsKey(item.Id.Value))
                    {
                        string upc = matchInfoWithCurrentInventory(inventoryFromQb.name, inventoryFromQb.detail, upcToInventoryMap.Values);

                        if (String.IsNullOrEmpty(upc))
                        {
                            // CASE 1:
                            // when no mapping exists and no product with same name/detail exist in our database,
                            // we create new one for them
                            autoUpcCounter++;
                            inventoryFromQb.upc = autoUpcPrefix + autoUpcCounter;
                            synchDatabaseController.createNewInventory(inventoryFromQb);
                            synchStorageController.createProductMapping(inventoryFromQb.upc, item);

                            upcToItemMap.Add(inventoryFromQb.upc, item);
                        }
                        else
                        {
                            // CASE 2:
                            // when we have the same product with missing/incorrect mapping information in storage
                            synchDatabaseController.updateInventoryFromQb(inventoryFromQb, upcToInventoryMap[upc]);
                            upcToInventoryMap.Remove(upc);
                            synchStorageController.createProductMapping(upc, item);

                            upcToItemMap.Add(upc, item);
                        }
                    }
                    else
                    {
                        // the mapping in storage exists
                        string upc = qbIdToEntityMap[item.Id.Value].upc;
                        qbIdToEntityMap.Remove(item.Id.Value);

                        if (upcToInventoryMap.ContainsKey(upc))
                        {
                            // CASE 3:
                            // this product with correct upc exists in Synch, update if needed
                            synchDatabaseController.updateInventoryFromQb(inventoryFromQb, upcToInventoryMap[upc]);
                            upcToInventoryMap.Remove(upc);

                            upcToItemMap.Add(upc, item);
                        }
                        else
                        {
                            // CASE 4:
                            // this upc does not exist in this business's inventory;
                            // create one new product+inventory or link existing one
                            inventoryFromQb.upc = upc;
                            synchDatabaseController.createNewInventory(inventoryFromQb);

                            upcToItemMap.Add(upc, item);
                        }
                    }
                }

                // 3. After matching all the products from Qbd, we delete excessive/inactive products in Synch
                foreach (string upc in upcToInventoryMap.Keys)
                    synchDatabaseController.deleteInventory(upc);

                foreach (ERPProductMapEntity entity in qbIdToEntityMap.Values)
                    synchStorageController.deleteProductMapping(entity);

                integrationStatus.productSyncFromQbStatusCode = SyncStatusCode.SyncSuccess;

            }
            catch (Exception e)
            {
                integrationStatus.productSyncFromQbStatusCode = SyncStatusCode.SyncFailure;
                integrationStatus.registerException(e);
            }
        }

        public void updateCustomersFromQb()
        {
            try
            {
                integrationStatus.customerSyncFromQbStatusCode = SyncStatusCode.Started;

                // get mapping and customer list from Synch first
                Dictionary<int, SynchCustomer> synchIdToSynchCustomerMap = synchDatabaseController.getCustomerIdToCustomerMap();
                Dictionary<string, ERPBusinessMapEntity> qbIdToEntityMap = synchStorageController.getQbBusinessIdToEntityMap();
                Dictionary<string, ERPAccountMapEntity> qbSalesRepIdToEntityMap = synchStorageController.getQbSalesRepIdToEntityMap();

                IEnumerable<Customer> customersFromQbd = qbDataController.getActiveCustomers();
                foreach (Customer customer in customersFromQbd)
                {

                    if (String.IsNullOrEmpty(customer.Name))
                        continue;

                    SynchCustomer customerFromQb = new SynchCustomer()
                    {
                        name = customer.Name,
                        address = "empty address",
                        businessId = synchBusinessId,
                        email = "sample_email@synchbi.com",
                        phoneNumber = "000-000-0000",
                        postalCode = "98101",
                        category = 0
                    };

                    if (customer.Address != null)
                    {
                        if (customer.Address[0].PostalCode != null)
                            customerFromQb.postalCode = customer.Address[0].PostalCode;
                        if (customer.Address[0].Line1 == null)
                            customerFromQb.address = customer.Address[0].City + ", " + customer.Address[0].CountrySubDivisionCode;
                        else
                        {
                            customerFromQb.address = customer.Address[0].Line1 + ", ";
                            if (customer.Address[0].Line2 == null)
                                customerFromQb.address += customer.Address[0].City + ", " + customer.Address[0].CountrySubDivisionCode;
                            else
                                customerFromQb.address += customer.Address[0].Line2
                                                    + ", " + customer.Address[0].City + ", "
                                                    + customer.Address[0].CountrySubDivisionCode;

                        }
                    }

                    if (customer.Email != null)
                        customerFromQb.email = customer.Email[0].Address;
                    if (customer.Phone != null)
                        customerFromQb.phoneNumber = customer.Phone[0].FreeFormNumber;

                    if (customer.SalesRepId != null && qbSalesRepIdToEntityMap.ContainsKey(customer.SalesRepId.Value))
                    {
                        customerFromQb.accountId = qbSalesRepIdToEntityMap[customer.SalesRepId.Value].accountIdFromSynch;
                    }
                    else
                        customerFromQb.accountId = integrationConfig.defaultAccountId;

                    // compare and update information now
                    if (!qbIdToEntityMap.ContainsKey(customer.Id.Value))
                    {
                        // CASE 1
                        // not in table storage right now, and considered not in Synch database
                        // create new business in Synch and a new mapping
                        int newCustomerId = synchDatabaseController.createNewCustomer(customerFromQb);
                        if (newCustomerId != -1)
                        {
                            synchIdToSynchCustomerMap.Remove(newCustomerId);
                            synchStorageController.createBusinessMapping(newCustomerId, customer);

                            customerIdToQbCustomerMap.Add(newCustomerId, customer);
                        }
                    }
                    else
                    {
                        // in table storage; get business info from Synch
                        int idFromSynch = qbIdToEntityMap[customer.Id.Value].idFromSynch;
                        qbIdToEntityMap.Remove(customer.Id.Value);

                        SynchCustomer customerFromSynch = null;

                        if (!synchIdToSynchCustomerMap.ContainsKey(idFromSynch))
                        {
                            // CASE 2:
                            // business mapping exists, but business id in Synch is outdated;
                            // create new business in Synch and a new mapping; later on delete outdated ones
                            int newCustomerId = synchDatabaseController.createNewCustomer(customerFromQb);
                            if (newCustomerId != -1)
                            {
                                synchStorageController.createBusinessMapping(newCustomerId, customer);

                                customerIdToQbCustomerMap.Add(newCustomerId, customer);
                            }
                        }
                        else
                        {
                            // CASE 3:
                            // business mapping exist and business id is up-to-date;
                            // check and update business information
                            customerFromSynch = synchIdToSynchCustomerMap[idFromSynch];
                            synchIdToSynchCustomerMap.Remove(idFromSynch);
                            synchDatabaseController.updateCustomerFromQb(customerFromQb, customerFromSynch);

                            customerIdToQbCustomerMap.Add(idFromSynch, customer);
                        }
                    }   // end if mapping in storage

                }
                // 3. After matching all the customers from Qbd, we delete excessive/inactive customers in Synch
                foreach (int id in synchIdToSynchCustomerMap.Keys)
                {
                    synchDatabaseController.deleteCustomer(id);
                }

                foreach (ERPBusinessMapEntity entity in qbIdToEntityMap.Values)
                {
                    synchStorageController.deleteBusinessMapping(entity);
                }

                integrationStatus.customerSyncFromQbStatusCode = SyncStatusCode.SyncSuccess;
            }
            catch (Exception e)
            {
                integrationStatus.customerSyncFromQbStatusCode = SyncStatusCode.SyncFailure;

                integrationStatus.registerException(e);
            }
            
        }

        public void updateInvoicesFromQb()
        {
            try
            {
                integrationStatus.invoiceSyncFromQbStatusCode = SyncStatusCode.Started;

                // get product mapping information from Qbd
                Dictionary<string, ERPBusinessMapEntity> qbCustomerIdToEntityMap = synchStorageController.getQbBusinessIdToEntityMap();
                Dictionary<string, ERPProductMapEntity> qbItemIdToEntityMap = synchStorageController.getQbItemIdToEntityMap();
                Dictionary<string, ERPRecordMapEntity> qbTransactionIdToEntityMap = synchStorageController.getQbTransactionIdToEntityMap();
                Dictionary<string, ERPAccountMapEntity> qbSalesRepIdToEntityMap = synchStorageController.getQbSalesRepIdToEntityMap();

                IEnumerable<Invoice> invoicesFromQbd = qbDataController.getInvoicesFromDate(integrationConfig.historyStartDate);

                int successCount = 0;
                int noCustomerCount = 0;
                int missingInfoCount = 0;
                int noUpcCount = 0;

                foreach (Invoice invoice in invoicesFromQbd)
                {
                    if (!invoice.Header.TxnDateSpecified)
                        continue;

                    if (qbTransactionIdToEntityMap.ContainsKey(invoice.Id.Value))
                    {
                        // this invoice exists;
                        // check if updates needed.
                    }
                    else
                    {
                        SynchRecord recordFromQb = new SynchRecord()
                        {
                            accountId = integrationConfig.defaultAccountId,
                            ownerId = synchBusinessId,
                            status = (int)RecordStatus.closed,
                            title = "Invoiced: " + invoice.Header.CustomerName,
                            comment = "Invoiced From QuickBooks: " + invoice.Header.Note,
                            transactionDate = invoice.Header.TxnDate,
                            deliveryDate = invoice.Header.TxnDate.AddDays(1),
                            category = (int)RecordCategory.Order
                        };
                        recordFromQb.recordLines = new List<SynchRecordLine>();

                        if (invoice.Header.SalesRepId != null)
                            if (qbSalesRepIdToEntityMap.ContainsKey(invoice.Header.SalesRepId.Value))
                                recordFromQb.accountId = qbSalesRepIdToEntityMap[invoice.Header.SalesRepId.Value].accountIdFromSynch;

                        if (invoice.Header.ShipDateSpecified)
                            recordFromQb.deliveryDate = invoice.Header.ShipDate;

                        if (qbCustomerIdToEntityMap.ContainsKey(invoice.Header.CustomerId.Value))
                        {
                            // this customer exists
                            recordFromQb.clientId = qbCustomerIdToEntityMap[invoice.Header.CustomerId.Value].idFromSynch;

                            foreach (Intuit.Ipp.Data.Qbd.InvoiceLine curLine in invoice.Line)
                            {
                                string upc = null;
                                int quantity = 0;
                                decimal price = 0.0m;
                                if (curLine.ItemsElementName != null && curLine.Items != null)
                                {
                                    for (int i = 0; i < curLine.ItemsElementName.Length; i++)
                                    {
                                        if (curLine.ItemsElementName[i].ToString() == "ItemId")
                                        {
                                            string itemId = ((Intuit.Ipp.Data.Qbd.IdType)curLine.Items[i]).Value;
                                            if (qbItemIdToEntityMap.ContainsKey(itemId))
                                                upc = qbItemIdToEntityMap[itemId].upc;
                                            else
                                                noUpcCount++;
                                        }

                                        if (curLine.ItemsElementName[i].ToString() == "UnitPrice")
                                            price = Decimal.Parse(curLine.Items[i].ToString());

                                        if (curLine.ItemsElementName[i].ToString() == "Qty")
                                            quantity = Int32.Parse(curLine.Items[i].ToString());
                                    }

                                    if (upc != null && quantity != 0 && price != 0.0m)
                                    {
                                        // now create this line item in database
                                        SynchRecordLine recordLine = new SynchRecordLine()
                                        {
                                            upc = upc,
                                            note = "",
                                            price = price,
                                            quantity = quantity,
                                            recordId = 0
                                        };
                                        recordFromQb.recordLines.Add(recordLine);
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

                            int rid = synchDatabaseController.createNewRecord(recordFromQb);
                            if (rid > 0)
                            {
                                synchStorageController.createRecordMapping(rid, invoice);
                                successCount++;
                            }
                            else
                            {
                                missingInfoCount++;
                            }
                        }
                        else
                            noCustomerCount++;

                    }   // end new invoice
                }

                integrationStatus.invoiceSyncFromQbStatusCode = SyncStatusCode.SyncSuccess;
            }
            catch (Exception e)
            {
                integrationStatus.invoiceSyncFromQbStatusCode = SyncStatusCode.SyncFailure;

                integrationStatus.registerException(e);
            }

        }

        public void updateSalesOrdersFromQb()
        {
            try
            {
                integrationStatus.salesOrderSyncFromQbStatusCode = SyncStatusCode.Started;

                // get product mapping information from Qbd
                Dictionary<string, ERPBusinessMapEntity> qbCustomerIdToEntityMap = synchStorageController.getQbBusinessIdToEntityMap();
                Dictionary<string, ERPProductMapEntity> qbItemIdToEntityMap = synchStorageController.getQbItemIdToEntityMap();
                Dictionary<string, ERPRecordMapEntity> qbTransactionIdToEntityMap = synchStorageController.getQbTransactionIdToEntityMap();
                Dictionary<string, ERPAccountMapEntity> qbSalesRepIdToEntityMap = synchStorageController.getQbSalesRepIdToEntityMap();

                IEnumerable<SalesOrder> salesOrdersFromQbd = qbDataController.getSalesOrdersFromDate(integrationConfig.historyStartDate);

                int successCount = 0;
                int noCustomerCount = 0;
                int missingInfoCount = 0;
                int noUpcCount = 0;

                foreach (SalesOrder salesOrder in salesOrdersFromQbd)
                {
                    if (!salesOrder.Header.TxnDateSpecified)
                        continue;

                    if (qbTransactionIdToEntityMap.ContainsKey(salesOrder.Id.Value))
                    {
                        // this salesOrder exists;
                        // check if it is closed;
                        // if YES then delete sales order record from Synch
                        if (salesOrder.Header.Status == "FullyInvoiced" ||
                            salesOrder.Header.Status == "Manually Closed" ||
                            salesOrder.Header.Status == "Trash" ||
                            salesOrder.Header.Status == "Paid")
                        {
                            synchDatabaseController.deleteRecord(qbTransactionIdToEntityMap[salesOrder.Id.Value].rid);

                            synchStorageController.deleteRecordMapping(qbTransactionIdToEntityMap[salesOrder.Id.Value]);
                        }
                    }
                    else
                    {
                        // skip sales orders that are already closed
                        if (salesOrder.Header.Status == "FullyInvoiced" ||
                            salesOrder.Header.Status == "Manually Closed" ||
                            salesOrder.Header.Status == "Trash" ||
                            salesOrder.Header.Status == "Paid")
                            continue;

                        SynchRecord recordFromQb = new SynchRecord()
                        {
                            accountId = integrationConfig.defaultAccountId,
                            ownerId = synchBusinessId,
                            status = (int)RecordStatus.sent,
                            title = "S.O. from QBD: " + salesOrder.Header.CustomerName,
                            comment = "S.O. From QuickBooks: " + salesOrder.Header.Note,
                            transactionDate = salesOrder.Header.TxnDate,
                            deliveryDate = salesOrder.Header.TxnDate.AddDays(1),
                            category = (int)RecordCategory.Order
                        };
                        recordFromQb.recordLines = new List<SynchRecordLine>();

                        if (salesOrder.Header.SalesRepId != null)
                            if (qbSalesRepIdToEntityMap.ContainsKey(salesOrder.Header.SalesRepId.Value))
                                recordFromQb.accountId = qbSalesRepIdToEntityMap[salesOrder.Header.SalesRepId.Value].accountIdFromSynch;

                        if (salesOrder.Header.ShipDateSpecified)
                            recordFromQb.deliveryDate = salesOrder.Header.ShipDate;

                        if (qbCustomerIdToEntityMap.ContainsKey(salesOrder.Header.CustomerId.Value))
                        {
                            // this customer exists
                            recordFromQb.clientId = qbCustomerIdToEntityMap[salesOrder.Header.CustomerId.Value].idFromSynch;

                            foreach (Intuit.Ipp.Data.Qbd.SalesOrderLine curLine in salesOrder.Line)
                            {
                                string upc = null;
                                int quantity = 0;
                                decimal price = 0.0m;
                                if (curLine.ItemsElementName != null && curLine.Items != null)
                                {
                                    for (int i = 0; i < curLine.ItemsElementName.Length; i++)
                                    {
                                        if (curLine.ItemsElementName[i].ToString() == "ItemId")
                                        {
                                            string itemId = ((Intuit.Ipp.Data.Qbd.IdType)curLine.Items[i]).Value;
                                            if (qbItemIdToEntityMap.ContainsKey(itemId))
                                                upc = qbItemIdToEntityMap[itemId].upc;
                                            else
                                                noUpcCount++;
                                        }

                                        if (curLine.ItemsElementName[i].ToString() == "UnitPrice")
                                            price = Decimal.Parse(curLine.Items[i].ToString());

                                        if (curLine.ItemsElementName[i].ToString() == "Qty")
                                            quantity = Int32.Parse(curLine.Items[i].ToString());
                                    }

                                    if (upc != null && quantity != 0 && price != 0.0m)
                                    {
                                        // now create this line item in database
                                        SynchRecordLine recordLine = new SynchRecordLine()
                                        {
                                            upc = upc,
                                            note = "",
                                            price = price,
                                            quantity = quantity,
                                            recordId = 0
                                        };
                                        recordFromQb.recordLines.Add(recordLine);
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

                            int rid = synchDatabaseController.createNewRecord(recordFromQb);
                            if (rid > 0)
                            {
                                synchStorageController.createRecordMapping(rid, salesOrder);
                                successCount++;
                            }
                            else
                            {
                                missingInfoCount++;
                            }
                        }
                        else
                            noCustomerCount++;

                    }   // end new salesOrder
                }

                integrationStatus.salesOrderSyncFromQbStatusCode = SyncStatusCode.SyncSuccess;
            }
            catch (Exception e)
            {
                integrationStatus.salesOrderSyncFromQbStatusCode = SyncStatusCode.SyncFailure;

                integrationStatus.registerException(e);
            }

        }

        #endregion

        #region private helper methods that are not always used in the service
        private int getAutoUpcCounter(string autoUpcPrefix, Dictionary<string, SynchInventory>.KeyCollection keyCollection)
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

            return maxCurrentCount;
        }

        private string matchInfoWithCurrentInventory(string nameFromQbd, string detailFromQbd,
            Dictionary<string, SynchInventory>.ValueCollection synchInventories)
        {
            foreach (SynchInventory i in synchInventories)
            {
                if (detailFromQbd == i.detail)
                    return i.upc;
            }

            return null;
        }

        #endregion

    }
}
