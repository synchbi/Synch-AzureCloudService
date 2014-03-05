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
                return createSalesOrderInQbd(recordId);
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
                    System.Diagnostics.Trace.TraceInformation("QBD: creating Invoice for record " + recordId);
                    recordFromSynch.integrationId = newInvoice.Id.Value + ":" + newInvoice.SyncToken;
                    recordFromSynch.status = (int)RecordStatus.syncedInvoice;
                    synchDatabaseController.updateRecord(recordFromSynch);

                }
                else
                {
                    synchDatabaseController.updateRecordStatus(recordId, (int)RecordStatus.rejected);

                    DateTime currentDateTimePST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
                    System.Diagnostics.Trace.TraceError(currentDateTimePST.ToString() + ":" + "failed to create invoice\n" + integrationStatus.ToString());

                    return 1;
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

        public int createSalesOrderInQbd(int recordId)
        {
            try
            {
                System.Diagnostics.Trace.TraceInformation("QBD: creating Sales Order for record " + recordId);
                integrationStatus.invoiceSyncFromSynchStatusCode = SyncStatusCode.Started;

                // get invoice information from Synch database
                SynchRecord recordFromSynch = synchDatabaseController.getRecord(recordId);

                SalesOrder newSalesOrder = qbDataController.createSalesOrder(recordFromSynch, upcToItemMap,
                    customerIdToQbCustomerMap, accountIdToSalesRepMap, integrationConfig.timezone);

                // create a mapping for this invoice in storage so that we won't unnecessarily sync it back
                if (newSalesOrder != null)
                {
                    System.Diagnostics.Trace.TraceInformation("QBD: creating sales order for record " + recordId);
                    recordFromSynch.integrationId = newSalesOrder.Id.Value + ":" + newSalesOrder.SyncToken;
                    recordFromSynch.status = (int)RecordStatus.syncedSalesOrder;
                    synchDatabaseController.updateRecord(recordFromSynch);

                }
                else
                {

                    DateTime currentDateTimePST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
                    System.Diagnostics.Trace.TraceError(currentDateTimePST.ToString() + ":" + "failed to create invoice\n" + integrationStatus.ToString());

                    synchDatabaseController.updateRecordStatus(recordId, (int)RecordStatus.rejected);

                    return 1;

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

                // we only update sales orders; throw exception for invoices
                if (recordFromSynch.status != (int)RecordStatus.syncedSalesOrder && recordFromSynch.status != (int)RecordStatus.sentFromSynch)
                    throw new ApplicationException("Only sales order can be updated.");

                SalesOrder updatedSalesOrder = qbDataController.updateSalesOrder(recordFromSynch, upcToItemMap,
                    customerIdToQbCustomerMap, accountIdToSalesRepMap, integrationConfig.timezone);
                recordFromSynch.integrationId = updatedSalesOrder.Id.Value + ":" + updatedSalesOrder.SyncToken;
                recordFromSynch.status = (int)RecordStatus.syncedSalesOrder;
                synchDatabaseController.updateRecord(recordFromSynch);

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
                Dictionary<string, SynchInventory> integrationIdToSynchInventoryMap = synchDatabaseController.getIntegrationIdToInventoryMap();
                string autoUpcPrefix = synchBusinessId + "AUTO";
                int autoUpcCounter = getAutoUpcCounter(autoUpcPrefix, integrationIdToSynchInventoryMap.Values);

                // 2: get updated information from Qbd side
                IEnumerable<Item> itemsFromQbd = qbDataController.getAllItems();

                // logic of matching item information
                foreach ( Item item in itemsFromQbd)
                {
                    // checks if this is a legitimate product we want to sync
                    if (String.IsNullOrEmpty(item.Name))
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
                        location = "temporary location",
                        integrationId = item.Id.Value,
                        reorderPoint = 20
                    };


                    if (item.ActiveSpecified && item.Active)
                        inventoryFromQb.status = (int)InventoryStatus.active;
                    else
                        inventoryFromQb.status = (int)InventoryStatus.inactive;
                        

                    // default values for these fields

                    // takes into account the quantity on sales order, which includes
                    // orders generated from Synch as well as orders generated from QuickBooks directly
                    if (item.QtyOnSalesOrderSpecified)
                        inventoryFromQb.quantityAvailable -= Convert.ToInt32(item.QtyOnSalesOrder);

                    if (item.ReorderPointSpecified)
                        inventoryFromQb.reorderPoint = Convert.ToInt32(item.ReorderPoint);
                    inventoryFromQb.reorderQuantity = inventoryFromQb.reorderPoint / 2;

                    if (item.QtyOnPurchaseOrderSpecified)
                        inventoryFromQb.quantityOnPurchaseOrder = Convert.ToInt32(item.QtyOnPurchaseOrder);

                    if (item.PurchaseCost != null)
                        inventoryFromQb.purchasePrice = Convert.ToDecimal(((Intuit.Ipp.Data.Qbd.Money)item.PurchaseCost).Amount);
                    else if (item.AvgCost != null)
                        inventoryFromQb.purchasePrice = Convert.ToDecimal(((Intuit.Ipp.Data.Qbd.Money)item.AvgCost).Amount);
                    else
                        inventoryFromQb.purchasePrice = 0.0m;

                    // now get current product linking information from Table Storage mapping,
                    // or create a new mapping if no mapping exists.
                    if (!integrationIdToSynchInventoryMap.ContainsKey(item.Id.Value))
                    {
                        string upc = matchInfoWithCurrentInventory(inventoryFromQb.name, inventoryFromQb.detail, integrationIdToSynchInventoryMap.Values);

                        if (String.IsNullOrEmpty(upc))
                        {
                            // CASE 1:
                            // when no matching product information exists in our database
                            // we create a new inventory item
                            autoUpcCounter++;
                            inventoryFromQb.upc = autoUpcPrefix + autoUpcCounter;
                            synchDatabaseController.createNewInventory(inventoryFromQb);

                            upcToItemMap.Add(inventoryFromQb.upc, item);
                        }
                        else
                        {
                            // CASE 2:
                            // when we have outdated integration id for this inventory item
                            synchDatabaseController.updateInventoryFromQb(inventoryFromQb, integrationIdToSynchInventoryMap[item.Id.Value]);
                            integrationIdToSynchInventoryMap.Remove(item.Id.Value);

                            upcToItemMap.Add(upc, item);
                        }
                    }
                    else
                    {
                        // update existing item
                        string upc = integrationIdToSynchInventoryMap[item.Id.Value].upc;

                        synchDatabaseController.updateInventoryFromQb(inventoryFromQb, integrationIdToSynchInventoryMap[item.Id.Value]);
                        integrationIdToSynchInventoryMap.Remove(item.Id.Value);

                        upcToItemMap.Add(upc, item);

                        
                    }
                }

                // 3. After matching all the products from Qbd, we delete excessive/inactive products in Synch
                foreach (SynchInventory inventory in integrationIdToSynchInventoryMap.Values)
                {
                    string upc = inventory.upc;
                    synchDatabaseController.deleteInventory(upc);
                }

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
                Dictionary<string, SynchCustomer> integrationIdToSynchCustomerMap = synchDatabaseController.getIntegrationIdToCustomerMap();
                Dictionary<string, ERPAccountMapEntity> qbSalesRepIdToEntityMap = synchStorageController.getQbSalesRepIdToEntityMap();

                IEnumerable<Customer> customersFromQbd = qbDataController.getAllCustomers();
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
                        category = 0,
                        integrationId = customer.Id.Value
                    };

                    // assign "active/inactive" status
                    if (customer.ActiveSpecified && customer.Active)
                        customerFromQb.status = (int)CustomerStatus.active;
                    else
                        customerFromQb.status = (int)CustomerStatus.inactive;

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
                    if (!integrationIdToSynchCustomerMap.ContainsKey(customer.Id.Value))
                    {
                        // CASE 1
                        // not in our database now
                        int newCustomerId = synchDatabaseController.createNewCustomer(customerFromQb);
                        if (newCustomerId != -1)
                        {
                            integrationIdToSynchCustomerMap.Remove(customer.Id.Value);

                            customerIdToQbCustomerMap.Add(newCustomerId, customer);
                        }
                    }
                    else
                    {
                        // in our database; update
                        SynchCustomer customerFromSynch = integrationIdToSynchCustomerMap[customer.Id.Value];
                        integrationIdToSynchCustomerMap.Remove(customer.Id.Value);

                        synchDatabaseController.updateCustomerFromQb(customerFromQb, customerFromSynch);

                        customerIdToQbCustomerMap.Add(customerFromSynch.customerId, customer);
                    }   // end if mapping in storage

                }

                // 3. After matching all the customers from Qbd, we delete excessive/inactive customers in Synch
                foreach (SynchCustomer customer in integrationIdToSynchCustomerMap.Values)
                {
                    synchDatabaseController.deleteCustomer(customer.customerId);
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
                Dictionary<string, SynchCustomer> integrationIdToSynchCustomerMap = synchDatabaseController.getIntegrationIdToCustomerMap();
                Dictionary<string, SynchInventory> integrationIdToSynchInventoryMap = synchDatabaseController.getIntegrationIdToInventoryMap();
                Dictionary<string, SynchRecord> integrationIdToSynchRecordMap = synchDatabaseController.getIntegrationIdToRecordMap();
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

                    if (integrationIdToSynchRecordMap.ContainsKey(invoice.Id.Value))
                    {
                        // this invoice exists;
                        // check if updates needed.

                        // if sync token is not the same anymore, it needs updates
                        string currentSyncToken = integrationIdToSynchRecordMap[invoice.Id.Value].integrationId.Split(':')[1];

                        if (currentSyncToken != invoice.SyncToken)
                        {
                            SynchRecord recordFromQb = new SynchRecord()
                            {
                                id = integrationIdToSynchRecordMap[invoice.Id.Value].id,
                                accountId = integrationConfig.defaultAccountId,
                                ownerId = synchBusinessId,
                                status = (int)RecordStatus.syncedInvoice,
                                title = "Invoiced: " + invoice.Header.CustomerName,
                                comment = invoice.Header.Note,
                                transactionDate = invoice.Header.TxnDate,
                                deliveryDate = invoice.Header.TxnDate.AddDays(1),
                                category = (int)RecordCategory.Order,
                                integrationId = invoice.Id.Value + ":" + invoice.SyncToken
                            };
                            recordFromQb.recordLines = new List<SynchRecordLine>();

                            if (invoice.Header.SalesRepId != null)
                                if (qbSalesRepIdToEntityMap.ContainsKey(invoice.Header.SalesRepId.Value))
                                    recordFromQb.accountId = qbSalesRepIdToEntityMap[invoice.Header.SalesRepId.Value].accountIdFromSynch;

                            if (invoice.Header.ShipDateSpecified)
                                recordFromQb.deliveryDate = invoice.Header.ShipDate;

                            if (!integrationIdToSynchCustomerMap.ContainsKey(invoice.Header.CustomerId.Value))
                            {
                                noCustomerCount++;
                                continue;
                            }

                            recordFromQb.clientId = integrationIdToSynchCustomerMap[invoice.Header.CustomerId.Value].customerId;

                            //if (integrationIdToSynchCustomerMap[invoice.Header.CustomerId.Value].status == (int)CustomerStatus.inactive)
                            //    isImplicitlyInactive = true;

                            foreach (Intuit.Ipp.Data.Qbd.InvoiceLine curLine in invoice.Line)
                            {
                                string upc = null;
                                int quantity = 0;
                                decimal price = 0.0m;

                                if (curLine.ItemsElementName == null || curLine.Items == null)
                                {
                                    missingInfoCount++;
                                    continue;
                                }

                                for (int i = 0; i < curLine.ItemsElementName.Length; i++)
                                {
                                    if (curLine.ItemsElementName[i].ToString() == "ItemId")
                                    {
                                        string itemId = ((Intuit.Ipp.Data.Qbd.IdType)curLine.Items[i]).Value;
                                        if (integrationIdToSynchInventoryMap.ContainsKey(itemId))
                                        {
                                            upc = integrationIdToSynchInventoryMap[itemId].upc;

                                            //if (integrationIdToSynchInventoryMap[itemId].status == (int)InventoryStatus.inactive)
                                            //    isImplicitlyInactive = true;
                                        }
                                        else
                                        {
                                            noUpcCount++;
                                        }
                                    }

                                    if (curLine.ItemsElementName[i].ToString() == "UnitPrice")
                                        price = Decimal.Parse(curLine.Items[i].ToString());

                                    if (curLine.ItemsElementName[i].ToString() == "Qty")
                                        quantity = Int32.Parse(curLine.Items[i].ToString());
                                }

                                if (upc != null && quantity >= 0 && price >= 0.0m)
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

                            }   // end foreach line item

                            synchDatabaseController.updateRecord(recordFromQb);

                        }

                    }
                    else
                    {
                        SynchRecord recordFromQb = new SynchRecord()
                        {
                            accountId = integrationConfig.defaultAccountId,
                            ownerId = synchBusinessId,
                            status = (int)RecordStatus.syncedInvoice,
                            title = "Invoiced: " + invoice.Header.CustomerName,
                            comment = invoice.Header.Note,
                            transactionDate = invoice.Header.TxnDate,
                            deliveryDate = invoice.Header.TxnDate.AddDays(1),
                            category = (int)RecordCategory.Order,
                            integrationId = invoice.Id.Value + ":" + invoice.SyncToken
                        };
                        recordFromQb.recordLines = new List<SynchRecordLine>();

                        if (invoice.Header.SalesRepId != null)
                            if (qbSalesRepIdToEntityMap.ContainsKey(invoice.Header.SalesRepId.Value))
                                recordFromQb.accountId = qbSalesRepIdToEntityMap[invoice.Header.SalesRepId.Value].accountIdFromSynch;

                        if (invoice.Header.ShipDateSpecified)
                            recordFromQb.deliveryDate = invoice.Header.ShipDate;

                        if (!integrationIdToSynchCustomerMap.ContainsKey(invoice.Header.CustomerId.Value))
                        {
                            noCustomerCount++;
                            continue;
                        }

                        recordFromQb.clientId = integrationIdToSynchCustomerMap[invoice.Header.CustomerId.Value].customerId;

                        //if (integrationIdToSynchCustomerMap[invoice.Header.CustomerId.Value].status == (int)CustomerStatus.inactive)
                        //    isImplicitlyInactive = true;

                        foreach (Intuit.Ipp.Data.Qbd.InvoiceLine curLine in invoice.Line)
                        {
                            string upc = null;
                            int quantity = 0;
                            decimal price = 0.0m;

                            if (curLine.ItemsElementName == null || curLine.Items == null)
                            {
                                missingInfoCount++;
                                continue;
                            }

                            for (int i = 0; i < curLine.ItemsElementName.Length; i++)
                            {
                                if (curLine.ItemsElementName[i].ToString() == "ItemId")
                                {
                                    string itemId = ((Intuit.Ipp.Data.Qbd.IdType)curLine.Items[i]).Value;
                                    if (integrationIdToSynchInventoryMap.ContainsKey(itemId))
                                    {
                                        upc = integrationIdToSynchInventoryMap[itemId].upc;

                                        //if (integrationIdToSynchInventoryMap[itemId].status == (int)InventoryStatus.inactive)
                                        //    isImplicitlyInactive = true;
                                    }
                                    else
                                    {
                                        noUpcCount++;
                                    }
                                }

                                if (curLine.ItemsElementName[i].ToString() == "UnitPrice")
                                    price = Decimal.Parse(curLine.Items[i].ToString());

                                if (curLine.ItemsElementName[i].ToString() == "Qty")
                                    quantity = Int32.Parse(curLine.Items[i].ToString());
                            }

                            if (upc != null && quantity >= 0 && price >= 0.0m)
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

                        }   // end foreach line item

                        int rid = synchDatabaseController.createNewRecord(recordFromQb);

                        if (rid > 0)
                        {
                            successCount++;
                        }
                        else
                        {
                            missingInfoCount++;
                        }

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
                Dictionary<string, SynchCustomer> integrationIdToSynchCustomerMap = synchDatabaseController.getIntegrationIdToCustomerMap();
                Dictionary<string, SynchInventory> integrationIdToSynchInventoryMap = synchDatabaseController.getIntegrationIdToInventoryMap();
                Dictionary<string, SynchRecord> integrationIdToSynchRecordMap = synchDatabaseController.getIntegrationIdToRecordMap();
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

                    if (integrationIdToSynchRecordMap.ContainsKey(salesOrder.Id.Value))
                    {
                        if (salesOrder.Header.Status == "FullyInvoiced" ||
                            salesOrder.Header.Status == "Manually Closed" ||
                            salesOrder.Header.Status == "Trash" ||
                            salesOrder.Header.Status == "Paid")
                        {
                            synchDatabaseController.updateRecordStatus(integrationIdToSynchRecordMap[salesOrder.Id.Value].id, (int)RecordStatus.closed);
                        }
                        else
                        {
                            string currentSyncToken = integrationIdToSynchRecordMap[salesOrder.Id.Value].integrationId.Split(':')[1];
                            if (currentSyncToken != salesOrder.SyncToken)
                            {
                                SynchRecord recordFromQb = new SynchRecord()
                                {
                                    id = integrationIdToSynchRecordMap[salesOrder.Id.Value].id,
                                    accountId = integrationConfig.defaultAccountId,
                                    ownerId = synchBusinessId,
                                    status = (int)RecordStatus.syncedSalesOrder,
                                    title = "S.O.: " + salesOrder.Header.CustomerName,
                                    comment = salesOrder.Header.Note,
                                    transactionDate = salesOrder.Header.TxnDate,
                                    deliveryDate = salesOrder.Header.TxnDate.AddDays(1),
                                    category = (int)RecordCategory.Order,
                                    integrationId = salesOrder.Id.Value + ":" + salesOrder.SyncToken
                                };
                                recordFromQb.recordLines = new List<SynchRecordLine>();

                                if (salesOrder.Header.SalesRepId != null)
                                    if (qbSalesRepIdToEntityMap.ContainsKey(salesOrder.Header.SalesRepId.Value))
                                        recordFromQb.accountId = qbSalesRepIdToEntityMap[salesOrder.Header.SalesRepId.Value].accountIdFromSynch;

                                if (salesOrder.Header.ShipDateSpecified)
                                    recordFromQb.deliveryDate = salesOrder.Header.ShipDate;

                                if (!integrationIdToSynchCustomerMap.ContainsKey(salesOrder.Header.CustomerId.Value))
                                {
                                    noCustomerCount++;
                                    continue;
                                }

                                recordFromQb.clientId = integrationIdToSynchCustomerMap[salesOrder.Header.CustomerId.Value].customerId;

                                //if (integrationIdToSynchCustomerMap[salesOrder.Header.CustomerId.Value].status == (int)CustomerStatus.inactive)
                                //    isImplicitlyInactive = true;

                                foreach (Intuit.Ipp.Data.Qbd.SalesOrderLine curLine in salesOrder.Line)
                                {
                                    string upc = null;
                                    int quantity = 0;
                                    decimal price = 0.0m;

                                    if (curLine.ItemsElementName == null || curLine.Items == null)
                                    {
                                        missingInfoCount++;
                                        continue;
                                    }

                                    for (int i = 0; i < curLine.ItemsElementName.Length; i++)
                                    {
                                        if (curLine.ItemsElementName[i].ToString() == "ItemId")
                                        {
                                            string itemId = ((Intuit.Ipp.Data.Qbd.IdType)curLine.Items[i]).Value;
                                            if (integrationIdToSynchInventoryMap.ContainsKey(itemId))
                                            {
                                                upc = integrationIdToSynchInventoryMap[itemId].upc;

                                                //if (integrationIdToSynchInventoryMap[itemId].status == (int)InventoryStatus.inactive)
                                                //    isImplicitlyInactive = true;
                                            }
                                            else
                                            {
                                                noUpcCount++;
                                            }
                                        }

                                        if (curLine.ItemsElementName[i].ToString() == "UnitPrice")
                                            price = Decimal.Parse(curLine.Items[i].ToString());

                                        if (curLine.ItemsElementName[i].ToString() == "Qty")
                                            quantity = Int32.Parse(curLine.Items[i].ToString());
                                    }

                                    if (upc != null && quantity >= 0 && price >= 0.0m)
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

                                }   // end foreach line item

                                synchDatabaseController.updateRecord(recordFromQb);

                            }
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
                            status = (int)RecordStatus.syncedSalesOrder,
                            title = "S.O.: " + salesOrder.Header.CustomerName,
                            comment = salesOrder.Header.Note,
                            transactionDate = salesOrder.Header.TxnDate,
                            deliveryDate = salesOrder.Header.TxnDate.AddDays(1),
                            category = (int)RecordCategory.Order,
                            integrationId = salesOrder.Id.Value + ":" + salesOrder.SyncToken
                        };
                        recordFromQb.recordLines = new List<SynchRecordLine>();

                        if (salesOrder.Header.SalesRepId != null)
                            if (qbSalesRepIdToEntityMap.ContainsKey(salesOrder.Header.SalesRepId.Value))
                                recordFromQb.accountId = qbSalesRepIdToEntityMap[salesOrder.Header.SalesRepId.Value].accountIdFromSynch;

                        if (salesOrder.Header.ShipDateSpecified)
                            recordFromQb.deliveryDate = salesOrder.Header.ShipDate;

                        if (!integrationIdToSynchCustomerMap.ContainsKey(salesOrder.Header.CustomerId.Value))
                        {
                            noCustomerCount++;
                            continue;
                        }

                        recordFromQb.clientId = integrationIdToSynchCustomerMap[salesOrder.Header.CustomerId.Value].customerId;

                        //if (integrationIdToSynchCustomerMap[salesOrder.Header.CustomerId.Value].status == (int)CustomerStatus.inactive)
                        //    isImplicitlyInactive = true;

                        foreach (Intuit.Ipp.Data.Qbd.SalesOrderLine curLine in salesOrder.Line)
                        {
                            string upc = null;
                            int quantity = 0;
                            decimal price = 0.0m;

                            if (curLine.ItemsElementName == null || curLine.Items == null)
                            {
                                missingInfoCount++;
                                continue;
                            }

                            for (int i = 0; i < curLine.ItemsElementName.Length; i++)
                            {
                                if (curLine.ItemsElementName[i].ToString() == "ItemId")
                                {
                                    string itemId = ((Intuit.Ipp.Data.Qbd.IdType)curLine.Items[i]).Value;
                                    if (integrationIdToSynchInventoryMap.ContainsKey(itemId))
                                    {
                                        upc = integrationIdToSynchInventoryMap[itemId].upc;

                                        //if (integrationIdToSynchInventoryMap[itemId].status == (int)InventoryStatus.inactive)
                                        //    isImplicitlyInactive = true;
                                    }
                                    else
                                    {
                                        noUpcCount++;
                                    }
                                }

                                if (curLine.ItemsElementName[i].ToString() == "UnitPrice")
                                    price = Decimal.Parse(curLine.Items[i].ToString());

                                if (curLine.ItemsElementName[i].ToString() == "Qty")
                                    quantity = Int32.Parse(curLine.Items[i].ToString());
                            }

                            if (upc != null && quantity >= 0 && price >= 0.0m)
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

                        }   // end foreach line item

                        //if (isImplicitlyInactive)
                        //    recordFromQb.status = (int)RecordStatus.inactive;

                        int rid = synchDatabaseController.createNewRecord(recordFromQb);
                        
                        if (rid > 0)
                        {
                            successCount++;
                        }
                        else
                        {
                            missingInfoCount++;
                        }

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
        private int getAutoUpcCounter(string autoUpcPrefix, Dictionary<string, SynchInventory>.ValueCollection inventories)
        {
            int maxCurrentCount = 0;
            foreach (SynchInventory inventory in inventories)
            {
                string upc = inventory.upc;
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
                if (nameFromQbd == i.name && detailFromQbd == i.detail)
                    return i.upc;
            }

            return null;
        }

        #endregion

    }
}
