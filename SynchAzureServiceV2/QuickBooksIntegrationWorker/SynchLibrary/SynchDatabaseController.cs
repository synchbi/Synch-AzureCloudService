using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QuickBooksIntegrationWorker.SynchLibrary.Models;

namespace QuickBooksIntegrationWorker.SynchLibrary
{
    class SynchDatabaseController
    {
        private int synchBusinessId;

        public SynchDatabaseController(int synchBusinessId)
        {
            this.synchBusinessId = synchBusinessId;
        }

        //#region SAFE ACTION SECTION: get
        public SynchRecord getRecord(int recordId)
        {
            SynchRecord record = new SynchRecord();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
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
            catch (Exception e)
            {
                record = null;
                DateTime currentDateTimePST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
                System.Diagnostics.Trace.TraceError(currentDateTimePST.ToString() + ":" + e.ToString());
                throw e;

            }
            finally
            {
                context.Dispose();
            }

            return record;
        }

        public SynchCustomer getCustomer(int customerId)
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();
            SynchCustomer customer = null;

            try
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
            catch (Exception e)
            {
                customer = null;
                DateTime currentDateTimePST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
                System.Diagnostics.Trace.TraceError(currentDateTimePST.ToString() + ":" + e.ToString());
                throw e;
            }
            finally
            {
                context.Dispose();
            }

            return customer;
        }

        public List<SynchInventory> getInventories()
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();
            List<SynchInventory> inventories = new List<SynchInventory>();

            try
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
            catch (Exception e)
            {
                inventories = null;
                DateTime currentDateTimePST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
                System.Diagnostics.Trace.TraceError(currentDateTimePST.ToString() + ":" + e.ToString());
                throw e;
            }
            finally
            {
                context.Dispose();
            }

            return inventories;
        }

        /*
        public Dictionary<string, SynchInventory> getUpcToInventoryMap()
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();
            Dictionary<string, SynchProduct> result = new Dictionary<string, SynchProduct>();
            try
            {
                var results = context.GetAllInventory(synchBusinessId);
                foreach (var inventory in results)
                {
                    result.Add(inventory.upc,
                        new SynchProduct()
                        {
                            name = inventory.name,
                            upc = inventory.upc,
                            detail = inventory.detail,
                            location = inventory.location,
                            quantity = (int)inventory.quantity,
                            leadTime = (int)inventory.lead_time,
                            price = (double)inventory.default_price
                        });
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                context.Dispose();
            }

            return result;
        }

        public Dictionary<int, SynchBusiness> getBidToCustomerMap()
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();
            Dictionary<int, SynchBusiness> result = new Dictionary<int, SynchBusiness>();
            try
            {
                var results = context.GetAllCustomers(synchBusinessId);
                foreach (var customer in results)
                {
                    result.Add(customer.id,
                        new SynchBusiness()
                        {
                            id = customer.id,
                            name = customer.name,
                            address = customer.address,
                            email = customer.email,
                            zip = (int)customer.zip,
                            phoneNumber = customer.phone_number
                        });
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                context.Dispose();
            }

            return result;
        }

        #endregion

        #region UNSAFE ACTION SECTION: create, update, delete
        public void createNewInventory(string upc, string name, string detail, string location, int quantity, int leadTime, double price, int category)
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                context.CreateProduct(upc, name, detail);
                context.CreateInventory(synchBusinessId, upc, location, quantity, leadTime, price, category);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                context.Dispose();
            }

        }

        public int createNewRecord(string invoiceTitle, int category, int status, string invoiceComment, int accountId, long transactionDateLong,
                                    int customerId, List<string> upcList, List<int> quantityList, List<double> priceList)
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();
            int rid = 0;

            try
            {
                rid = context.CreateHistoryRecord(synchBusinessId, invoiceTitle, category, status, invoiceComment, accountId, transactionDateLong);
                for (int i = 0; i < upcList.Count; i++)
                {
                    context.CreateProductInRecord(rid, upcList[i], 1, customerId, quantityList[i], "", priceList[i]);
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                context.Dispose();
            }

            return rid;
        }


        internal void updateInventory(string upc, string detailFromQbd, int quantityFromQbd, double priceFromQbd, int synchBusinessId, string nameFromQbd)
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                context.UpdateInventoryByUpc(upc, detailFromQbd, quantityFromQbd, priceFromQbd, synchBusinessId, nameFromQbd);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                context.Dispose();
            }

        }

        public int createCustomer(string name, string address, int zip, string email, string category, int integration, int tier, string phoneNumber)
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();
            int newCustomerId = -1;

            try
            {
                newCustomerId = context.CreateBusiness(name, address, zip, email, category, integration, tier, phoneNumber);
                context.CreateSupplies(synchBusinessId, newCustomerId, synchBusinessId);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                context.Dispose();
            }

            return newCustomerId;

        }

        public void updateBusinessById(int otherBid, string address, int zip, string email, string category, string phoneNumber)
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                context.UpdateBusinessById(otherBid, address, zip, email, category, phoneNumber);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                context.Dispose();
            }

        }

        public void deleteCustomer(int cid)
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                context.DeleteSupplies(synchBusinessId, cid, synchBusinessId);
                context.DeleteBusinessById(cid);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                context.Dispose();
            }

        }

        public void deleteInventory(string upc)
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                context.DeleteInventoryByUpc(upc, synchBusinessId);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                context.Dispose();
            }
        }
        
        #endregion
         */
    }
}
