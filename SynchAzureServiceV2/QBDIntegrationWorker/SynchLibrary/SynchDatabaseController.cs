﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QBDIntegrationWorker.SynchLibrary.Models;

namespace QBDIntegrationWorker.SynchLibrary
{
    class SynchDatabaseController
    {
        private int synchBusinessId;

        public SynchDatabaseController(int synchBusinessId)
        {
            this.synchBusinessId = synchBusinessId;
        }

        #region SAFE ACTION SECTION: get
        public SynchRecord getRecord(int recordId)
        {
            SynchRecord record = new SynchRecord();

            using (SynchDatabaseDataContext context = new SynchDatabaseDataContext())
	        {
		        var recordResult = context.GetRecordById(synchBusinessId, recordId);
                IEnumerator<GetRecordByIdResult> recordEnumerator = recordResult.GetEnumerator();
                if (recordEnumerator.MoveNext())
                {
                    record = new SynchRecord()
                    {
                        id = recordEnumerator.Current.id,
                        accountId = recordEnumerator.Current.accountId,
                        ownerId = recordEnumerator.Current.ownerId,
                        clientId = recordEnumerator.Current.clientId,
                        status = recordEnumerator.Current.status,
                        category = recordEnumerator.Current.category,
                        title = recordEnumerator.Current.title,
                        transactionDate = recordEnumerator.Current.transactionDate,
                        deliveryDate = recordEnumerator.Current.deliveryDate,
                        comment = recordEnumerator.Current.comment,
                    };
                }
                else
                {
                    throw new ArgumentException("Record with given Id is not found");
                }

                // get line items
                var linesResults = context.GetRecordLines(recordId, synchBusinessId);
                record.recordLines = new List<SynchRecordLine>();
                foreach (var lineResult in linesResults)
                {
                    record.recordLines.Add(new SynchRecordLine()
                    {
                        recordId = recordId,
                        upc = lineResult.upc,
                        quantity = lineResult.quantity,
                        price = lineResult.price,
                        note = lineResult.note
                    });
                }
	        }

            return record;
        }

        public List<SynchCustomer> getCustomers()
        {
            List<SynchCustomer> customers = new List<SynchCustomer>();

            using (SynchDatabaseDataContext context = new SynchDatabaseDataContext())
	        {
                var results = context.GetCustomers(synchBusinessId);

                foreach (var result in results)
                {
                    customers.Add(
                        new SynchCustomer()
                        {
                            businessId = synchBusinessId,
                            customerId = result.customerId,
                            name = result.name,
                            address = result.address,
                            email = result.email,
                            postalCode = result.postalCode,
                            phoneNumber = result.phoneNumber,
                            category = result.category
                        }
                    );
                }
	        }   // end of using block; dispose context

            return customers;
        }

        public SynchCustomer getCustomer(int customerId)
        {
            SynchCustomer customer = null;

            using(SynchDatabaseDataContext context = new SynchDatabaseDataContext())
	        {
                var results = context.GetCustomerById(synchBusinessId, customerId);
                IEnumerator<GetCustomerByIdResult> customerEnumerator = results.GetEnumerator();
                if (customerEnumerator.MoveNext())
                {
                    customer = new SynchCustomer()
                    {
                        businessId = synchBusinessId,
                        customerId = customerEnumerator.Current.customerId,
                        name = customerEnumerator.Current.name,
                        address = customerEnumerator.Current.address,
                        email = customerEnumerator.Current.email,
                        postalCode = customerEnumerator.Current.postalCode,
                        phoneNumber = customerEnumerator.Current.phoneNumber,
                        category = customerEnumerator.Current.category
                    };
                }
                else
                {
                    throw new ArgumentException("Customer with given Id is not found");
                }		 
	        }

            return customer;
        }

        public List<SynchInventory> getInventories()
        {
            List<SynchInventory> inventories = new List<SynchInventory>();

            using(SynchDatabaseDataContext context = new SynchDatabaseDataContext())
	        {
                var results = context.GetInventories(synchBusinessId);

                foreach (var result in results)
                {
                    inventories.Add(
                        new SynchInventory()
                        {
                            businessId = result.businessId,
                            name = result.name,
                            upc = result.upc,
                            defaultPrice = result.defaultPrice,
                            detail = result.detail,
                            quantityAvailable = result.quantityAvailable,
                            reorderPoint = result.reorderPoint,
                            reorderQuantity = result.reorderQuantity,
                            leadTime = (int)result.leadTime,
                            location = result.location,
                            category = (int)result.category
                        }
                    );
                }		 
	        }

            return inventories;
        }

        public Dictionary<string, SynchInventory> getUpcToInventoryMap()
        {
            Dictionary<string, SynchInventory> map = new Dictionary<string, SynchInventory>();
            List<SynchInventory> inventories = getInventories();
            foreach (SynchInventory i in inventories)
            {
                map.Add(i.upc, i);
            }

            return map;
        }

        public Dictionary<int, SynchCustomer> getCustomerIdToCustomerMap()
        {
            Dictionary<int, SynchCustomer> map = new Dictionary<int, SynchCustomer>();
            List<SynchCustomer> customers = getCustomers();
            foreach (SynchCustomer c in customers)
            {
                map.Add(c.customerId, c);
            }

            return map;
        }

        public List<SynchAccount> getAccounts()
        {
            List<SynchAccount> accounts = new List<SynchAccount>();

            using (SynchDatabaseDataContext context = new SynchDatabaseDataContext())
            {
                var results = context.GetAccounts(synchBusinessId);

                foreach (var result in results)
                {
                    accounts.Add(
                        new SynchAccount()
                        {
                            businessId = synchBusinessId,
                            id = result.id,
                            email = result.email,
                            firstName = result.firstName,
                            lastName = result.lastName,
                            login = result.login,
                            phoneNumber = result.phoneNumber,
                            tier = (int)result.tier,
                            deviceId = result.deviceId,

                            // intentionally left empty
                            sessionId = null,
                            password = null
                        }
                    );
                }
            }   // end of using block; dispose context

            return accounts;
        }

        public Dictionary<int, SynchAccount> getAccountIdToAccountMap()
        {
            Dictionary<int, SynchAccount> map = new Dictionary<int, SynchAccount>();
            List<SynchAccount> accounts = getAccounts();
            foreach (SynchAccount a in accounts)
            {
                map.Add(a.id, a);
            }

            return map;
        }

        #endregion

        #region UNSAFE ACTION SECTION: create, update, delete
        public void createNewInventory(SynchInventory newInventory)
        {
            using (SynchDatabaseDataContext context = new SynchDatabaseDataContext())
	        {
		        context.CreateProduct(newInventory.upc);
                context.CreateInventory(synchBusinessId, newInventory.upc, newInventory.name, newInventory.defaultPrice, newInventory.detail,
                                        newInventory.leadTime, newInventory.quantityAvailable, newInventory.reorderQuantity,
                                        newInventory.reorderPoint, newInventory.category, newInventory.location, newInventory.quantityOnPurchaseOrder);

	        }

        }

        /// <summary>
        /// Update inventory information from QuickBooks to Synch.
        /// This method only updates information we can collect from QuickBooks; other information will be left
        /// as before.
        /// Specifically, we update:
        /// 1. name
        /// 2. detail
        /// 3. price
        /// 4. quantity
        /// 5. reorder point
        /// 6. quantity on P.O.
        /// </summary>
        /// <param name="inventoryFromQb"></param>
        /// <param name="currentInventory"></param>
        /// <returns>true if an update has been performed; false if no update to Synch has been performed</returns>
        public bool updateInventoryFromQb(SynchInventory inventoryFromQb, SynchInventory currentInventory)
        {
            if (inventoryFromQb.name != currentInventory.name ||
                inventoryFromQb.detail != currentInventory.detail ||
                inventoryFromQb.defaultPrice != currentInventory.defaultPrice ||
                inventoryFromQb.quantityAvailable != currentInventory.quantityAvailable ||
                inventoryFromQb.reorderPoint != currentInventory.reorderPoint ||
                inventoryFromQb.quantityOnPurchaseOrder != currentInventory.quantityOnPurchaseOrder)
            {
                using (SynchDatabaseDataContext context = new SynchDatabaseDataContext())
	            {
		            context.UpdateInventory(synchBusinessId, currentInventory.upc, inventoryFromQb.name, inventoryFromQb.defaultPrice,
                                            inventoryFromQb.detail, currentInventory.leadTime, inventoryFromQb.quantityAvailable,
                                            currentInventory.reorderQuantity, inventoryFromQb.reorderPoint, currentInventory.category,
                                            currentInventory.location, inventoryFromQb.quantityOnPurchaseOrder);
	                return true;
                }
            }

            return false;
        }

        public int createNewRecord(SynchRecord newRecord)
        {
            int recordId = -1;
            using (SynchDatabaseDataContext context = new SynchDatabaseDataContext())
            {
                newRecord.ownerId = synchBusinessId;

                recordId = context.CreateRecord(
                    newRecord.category,
                    newRecord.accountId,
                    newRecord.ownerId,
                    newRecord.clientId,
                    newRecord.status,
                    newRecord.title,
                    newRecord.comment,
                    newRecord.transactionDate,
                    newRecord.deliveryDate);

                if (recordId < 0)
                    throw new ApplicationException("unable to create record");

                foreach (SynchRecordLine recordLine in newRecord.recordLines)
                {
                    context.CreateRecordLine(recordId, recordLine.upc, recordLine.quantity, recordLine.price, recordLine.note);
                }
            }
            return recordId;
        }

        /// <summary>
        /// Creates new customer into Synch's database.
        /// By default we use {name, postalCode} as a tuple to uniquely identify businesses in Synch.
        /// If we find the same tuple exists, this method will not create a new business; rather it will
        /// find that existing tuple's businessId and create a new customer for current business.
        /// If the same tuple also exists in this current business's customer list, it updates the customer information
        /// </summary>
        /// <param name="newCustomer">SynchCustomer object containing new customer's information</param>
        /// <returns>either a newly created customer Id, or an existing customer Id that gets newly linked or its information newly updated</returns>
        public int createNewCustomer(SynchCustomer newCustomer)
        {
            int customerId = -1;

            using (SynchDatabaseDataContext context = new SynchDatabaseDataContext())
            {
                customerId = context.CreateBusiness(newCustomer.name, 0, 0, newCustomer.address, newCustomer.postalCode, newCustomer.email, newCustomer.phoneNumber);
                if (customerId < 0)
                {
                    var result = context.GetBusinessByNameAndPostalCode(newCustomer.name, newCustomer.postalCode);
                    IEnumerator<GetBusinessByNameAndPostalCodeResult> businessEnumerator = result.GetEnumerator();
                    if (businessEnumerator.MoveNext())
                    {
                        customerId = businessEnumerator.Current.id;
                    }
                    else
                        throw new ApplicationException("failed to create new business on server, and no business with same name and postal code is found");
                }

                context.CreateCustomer(synchBusinessId, customerId, newCustomer.address, newCustomer.email, newCustomer.phoneNumber, newCustomer.category, newCustomer.accountId);
            }

            return customerId;

        }

        /// <summary>
        /// Update customer information from QuickBooks to Synch.
        /// This method only updates information we can collect from QuickBooks; other information will be left
        /// as before.
        /// Specifically, we update:
        /// 1. address
        /// 2. email
        /// 3. phone number
        /// 4. account Id
        /// </summary>
        /// <param name="customerFromQb"></param>
        /// <param name="currentCustomer"></param>
        /// <returns>true if an update has been performed; false if no update to Synch has been performed</returns>
        public bool updateCustomerFromQb(SynchCustomer customerFromQb, SynchCustomer currentCustomer)
        {
            if (customerFromQb.address != currentCustomer.address ||
                customerFromQb.email != currentCustomer.email ||
                customerFromQb.phoneNumber != currentCustomer.phoneNumber ||
                customerFromQb.accountId != currentCustomer.accountId)
            {
                using (SynchDatabaseDataContext context = new SynchDatabaseDataContext())
                {
                    context.UpdateCustomer(synchBusinessId, currentCustomer.customerId, customerFromQb.address, customerFromQb.email,
                                            customerFromQb.phoneNumber, currentCustomer.category, customerFromQb.accountId);
                    return true;
                }
            }

            return false;
        }

        public void deleteCustomer(int cid)
        {
            using (SynchDatabaseDataContext context = new SynchDatabaseDataContext())
            {
                context.DeleteCustomerById(cid, synchBusinessId);
            }
        }

        public void deleteInventory(string upc)
        {
            using (SynchDatabaseDataContext context = new SynchDatabaseDataContext())
            {
                context.DeleteInventoryByUpc(synchBusinessId, upc);

            }
        }

        public void deleteRecord(int rid)
        {
            using (SynchDatabaseDataContext context = new SynchDatabaseDataContext())
            {
                context.DeleteRecordById(rid);

            }
        }
        
        #endregion
    }
}
