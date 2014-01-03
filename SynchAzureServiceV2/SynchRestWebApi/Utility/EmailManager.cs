using SendGrid;
using SendGrid.Transport;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.ServiceModel.Web;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

using SynchRestWebApi.Models;

namespace SynchRestWebApi.Utility
{
    public static class EmailManager
    {
        public static bool sendEmailForNewAccount(SynchDatabaseDataContext context, SynchAccount account)
        {
            SynchBusiness business = new SynchBusiness();
            var getBusinessResult = context.GetBusinessById(account.businessId);

            IEnumerator<GetBusinessByIdResult> businessResultEnum = getBusinessResult.GetEnumerator();
            if (businessResultEnum.MoveNext())
            {
                business.id = businessResultEnum.Current.id;
                business.name = businessResultEnum.Current.name;
                business.address = businessResultEnum.Current.address;
                business.email = businessResultEnum.Current.email;
                business.phoneNumber = businessResultEnum.Current.phoneNumber;
                business.postalCode = businessResultEnum.Current.postalCode;
            }
            else
                throw new WebFaultException<string>("business for this account is not found", HttpStatusCode.NotFound);

            var message = SendGrid.Mail.GetInstance();

            message.From = new MailAddress("Synch Customer Service <customerservice@synchbi.com>");
            message.AddTo(string.Format("{0} {1} <{2}>", account.firstName, account.lastName, account.email));
            message.AddTo(string.Format("{0} <{1}>", business.name, business.email));
            message.AddTo("changhao.han@gmail.com");
            message.Subject = "[Synch] Welcome to Synch. Let\'s get you started";
            StringBuilder text = new StringBuilder();
            text.AppendLine(
                string.Format(
                "Welcome to Synch, {0} {1} from {2}.\n",
                account.firstName, account.lastName, business.name));
            text.AppendLine(string.Format("Your username for your new account at Synch is {0}.\n",
                account.login));
            text.AppendLine(string.Format("To join your business family at Synch, please visit {0}.\n", ApplicationConstants.DASHBOARD_LINK));
            
            message.Text = text.ToString();
            string html = getHtmlTemplate(text.ToString());
            message.Html = html;

            var username = "azure_bf33e57baacbfaae4ebfe0814f1d8a5d@azure.com";
            var password = "i6dvglzv";
            var credentials = new NetworkCredential(username, password);

            var transportSMTP = SMTP.GetInstance(credentials);

            try
            {
                transportSMTP.Deliver(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool sendEmailForNewBusiness(SynchBusiness business)
        {
            var message = SendGrid.Mail.GetInstance();
            
            message.From = new MailAddress("Synch Customer Service <customerservice@synchbi.com>");
            message.AddTo(string.Format("{0} <{1}>", business.name, business.email));
            message.AddTo("changhao.han@gmail.com");
            message.Subject = "[Synch] Welcome to Synch. Let\'s get you started";
            StringBuilder text = new StringBuilder();
            text.AppendLine(
                string.Format(
                "Welcome to Synch, {0}.\n",
                business.name));
            text.AppendLine(string.Format("Your email address {0} will be used as the primary contact for communication between you and Synch.\n",
                business.email));
            text.AppendLine(string.Format("To get started with your business service at Synch, please visit {0}.\n", ApplicationConstants.DASHBOARD_LINK));
            message.Text = text.ToString();
            string html = getHtmlTemplate(text.ToString());
            message.Html = html;

            var username = "azure_bf33e57baacbfaae4ebfe0814f1d8a5d@azure.com";
            var password = "i6dvglzv";
            var credentials = new NetworkCredential(username, password);

            var transportSMTP = SMTP.GetInstance(credentials);
            
            try
            {
                transportSMTP.Deliver(message);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool sendEmailForPresentedRecord(SynchDatabaseDataContext context, SynchRecord record)
        {
            SynchAccount account = new SynchAccount();
            var getAccountResult = context.GetAccountById(record.accountId);

            IEnumerator<GetAccountByIdResult> accountResultEnum = getAccountResult.GetEnumerator();
            if (accountResultEnum.MoveNext())
            {
                account.id = accountResultEnum.Current.id;
                account.firstName = accountResultEnum.Current.firstName;
                account.lastName = accountResultEnum.Current.lastName;
                account.email = accountResultEnum.Current.email;
                account.tier = (int)accountResultEnum.Current.tier;
                account.phoneNumber = accountResultEnum.Current.phoneNumber;
            }
            else
                throw new WebFaultException<string>("account in this record is not found", HttpStatusCode.NotFound);

            SynchBusiness business = new SynchBusiness();
            var getBusinessResult = context.GetBusinessById(record.ownerId);

            IEnumerator<GetBusinessByIdResult> businessResultEnum = getBusinessResult.GetEnumerator();
            if (businessResultEnum.MoveNext())
            {
                business.id = businessResultEnum.Current.id;
                business.name = businessResultEnum.Current.name;
                business.address = businessResultEnum.Current.address;
                business.email = businessResultEnum.Current.email;
                business.phoneNumber = businessResultEnum.Current.phoneNumber;
                business.postalCode = businessResultEnum.Current.postalCode;
            }
            else
                throw new WebFaultException<string>("owner in this record is not found", HttpStatusCode.NotFound);

            var message = SendGrid.Mail.GetInstance();

            message.From = new MailAddress("Synch Order Tracking Service <ordertracking@synchbi.com>");
            message.AddTo(string.Format("{0} {1} <{2}>", account.firstName, account.lastName, account.email));
            message.AddTo(" synchbiorder@gmail.com");
            message.Subject = "[Synch] Presented Order confirmation";
            StringBuilder text = new StringBuilder();
            text.AppendLine(record.title);
            text.AppendLine(FormatPresentedRecord(record, account, business, context));
            text.AppendLine();
            text.AppendLine();
            text.AppendLine(
                string.Format(
                "This email was auto-generated and auto-sent to {0}. Please do not reply! ",
                business.name));
            message.Text = text.ToString();

            var username = "azure_bf33e57baacbfaae4ebfe0814f1d8a5d@azure.com";
            var password = "i6dvglzv";
            var credentials = new NetworkCredential(username, password);

            var transportSMTP = SMTP.GetInstance(credentials);

            try
            {
                transportSMTP.Deliver(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string FormatPresentedRecord(SynchRecord record, SynchAccount account, SynchBusiness business, SynchDatabaseDataContext context)
        {
            StringBuilder builder = new StringBuilder();
            DateTime transactionDateTimePST = TimeZoneInfo.ConvertTimeFromUtc(
                record.transactionDate.ToUniversalTime().DateTime, TimeZoneInfo.FindSystemTimeZoneById(ApplicationConstants.DEFAULT_TIME_ZONE));
            DateTime deliveryDateTimePST = TimeZoneInfo.ConvertTimeFromUtc(
                ((DateTimeOffset)record.deliveryDate).ToUniversalTime().DateTime, TimeZoneInfo.FindSystemTimeZoneById(ApplicationConstants.DEFAULT_TIME_ZONE));

            builder.AppendLine(string.Format("Transaction Date (PST): {0}", transactionDateTimePST));

            switch (record.category)
            {
                case (int)RecordCategory.Order:
                    var customerResults = context.GetCustomerById(record.ownerId, record.clientId);
                    SynchCustomer customer = null;
                    IEnumerator<GetCustomerByIdResult> customerEnumerator = customerResults.GetEnumerator();
                    if (customerEnumerator.MoveNext())
                    {
                        customer = new SynchCustomer()
                        {
                            name = customerEnumerator.Current.name
                        };
                    }
                    else
                    {
                        throw new WebFaultException<string>("Customer with given Id is not found", HttpStatusCode.NotFound);
                    }
                    builder.AppendLine(string.Format("Customer: {0}", customer.name));
                    builder.AppendLine(string.Format("Presented Order from {0} {1}", account.firstName, account.lastName));
                    break;
                case (int)RecordCategory.Receipt:
                    var supplierResults = context.GetSupplierById(record.ownerId, record.clientId);
                    SynchSupplier supplier = null;
                    IEnumerator<GetSupplierByIdResult> supplierEnumerator = supplierResults.GetEnumerator();
                    if (supplierEnumerator.MoveNext())
                    {
                        supplier = new SynchSupplier()
                        {
                            name = supplierEnumerator.Current.name
                        };
                    }
                    else
                    {
                        throw new WebFaultException<string>("Supplier with given Id is not found", HttpStatusCode.NotFound);
                    }
                    builder.AppendLine(string.Format("Supplier: {0}", supplier.name));
                    builder.AppendLine(string.Format("Receipt for {0} {1}", account.firstName, account.lastName));
                    break;
                case (int)RecordCategory.PhysicalDamage:
                    builder.AppendLine("Inventory Change (Physical Damange)");
                    break;
                case (int)RecordCategory.PhysicalInventory:
                    builder.AppendLine("Inventory Change (Physical Inventory)");
                    break;
                case (int)RecordCategory.QualityIssue:
                    builder.AppendLine("Inventory Change (Quality Issue)");
                    break;
                case (int)RecordCategory.CycleCount:
                    builder.AppendLine("Inventory Change (Cycle Count)");
                    break;
                case (int)RecordCategory.Return:
                    builder.AppendLine("Inventory Change (Return)");
                    break;
                case (int)RecordCategory.SalesSample:
                    builder.AppendLine("Inventory Change (Sales Sample)");
                    break;
                case (int)RecordCategory.Stolen:
                    builder.AppendLine("Inventory Change (Stolen)");
                    break;
                default:
                    break;
            }

            builder.AppendLine(string.Format("Delivery Date: {0}", deliveryDateTimePST));
            builder.AppendLine();
            builder.AppendLine(string.Format("{0,-20}{1,-60}{2, -60}{3,-20}{4,-20}{5,-20}", "UPC", "Product Number", "Product Description", "Quantity", "Price", "Location"));

            SynchInventory[] sortedInventories = sortLinesByLocation(record, context);
            decimal totalPrice = 0.0m;

            for (int i = 0; i < sortedInventories.Length; i++)
            {
                string location = sortedInventories[i].location;
                string upc = sortedInventories[i].upc;
                string name = sortedInventories[i].name;
                string detail = sortedInventories[i].detail;
                SynchRecordLine line = getRecordLineByUpc(upc, record.recordLines);

                totalPrice += line.price * line.quantity;

                switch (record.category)
                {
                    case (int)RecordCategory.Order:
                        builder.AppendLine(string.Format("{0,-20}{1,-60}{2, -60}{3,-20}{4,-20}{5,-20}",
                                                                                            upc,
                                                                                            name,
                                                                                            detail,
                                                                                            line.quantity,
                                                                                            line.price,
                                                                                            location
                                                                                            ));
                        break;
                    case (int)RecordCategory.Receipt:
                        builder.AppendLine(string.Format("{0,-20}{1,-60}{2, -60}{3,-20}{4,-20}{5,-20}",
                                                                                            upc,
                                                                                            name,
                                                                                            detail,
                                                                                            line.quantity,
                                                                                            line.price,
                                                                                            location
                                                                                            ));
                        break;
                    default:
                        break;
                }

                builder.AppendLine();
            }

            builder.AppendLine("Total price: " + totalPrice);
            builder.AppendLine();
            builder.AppendLine(string.Format("Memo: {0}", record.comment));
            return builder.ToString();
        }

        public static bool sendEmailForRecord(SynchDatabaseDataContext context, SynchRecord record)
        {
            SynchAccount account = new SynchAccount();
            var getAccountResult = context.GetAccountById(record.accountId);

            IEnumerator<GetAccountByIdResult> accountResultEnum = getAccountResult.GetEnumerator();
            if (accountResultEnum.MoveNext())
            {
                account.id = accountResultEnum.Current.id;
                account.firstName = accountResultEnum.Current.firstName;
                account.lastName = accountResultEnum.Current.lastName;
                account.email = accountResultEnum.Current.email;
                account.tier = (int)accountResultEnum.Current.tier;
                account.phoneNumber = accountResultEnum.Current.phoneNumber;
            }
            else
                throw new WebFaultException<string>("account in this record is not found", HttpStatusCode.NotFound);

            SynchBusiness business = new SynchBusiness();
            var getBusinessResult = context.GetBusinessById(record.ownerId);

            IEnumerator<GetBusinessByIdResult> businessResultEnum = getBusinessResult.GetEnumerator();
            if (businessResultEnum.MoveNext())
            {
                business.id = businessResultEnum.Current.id;
                business.name = businessResultEnum.Current.name;
                business.address = businessResultEnum.Current.address;
                business.email = businessResultEnum.Current.email;
                business.phoneNumber = businessResultEnum.Current.phoneNumber;
                business.postalCode = businessResultEnum.Current.postalCode;
            }
            else
                throw new WebFaultException<string>("owner in this record is not found", HttpStatusCode.NotFound);

            var message = SendGrid.Mail.GetInstance();

            message.From = new MailAddress("Synch Order Tracking Service <ordertracking@synchbi.com>");
            message.AddTo(string.Format("{0} {1} <{2}>", account.firstName, account.lastName, account.email));
            message.AddTo(string.Format("{0} <{1}>", business.name, business.email));
            message.AddTo(" synchbiorder@gmail.com");
            message.Subject = "[Synch] Order confirmation";
            StringBuilder text = new StringBuilder();
            text.AppendLine(record.title);
            text.AppendLine(FormatRecord(record, account, business, context));
            text.AppendLine();
            text.AppendLine();
            text.AppendLine(
                string.Format(
                "This email was auto-generated and auto-sent to {0}. Please do not reply! ",
                business.name));
            message.Text = text.ToString();

            var username = "azure_bf33e57baacbfaae4ebfe0814f1d8a5d@azure.com";
            var password = "i6dvglzv";
            var credentials = new NetworkCredential(username, password);

            var transportSMTP = SMTP.GetInstance(credentials);

            try
            {
                transportSMTP.Deliver(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string FormatRecord(SynchRecord record, SynchAccount account, SynchBusiness business, SynchDatabaseDataContext context)
        {
            StringBuilder builder = new StringBuilder();
            DateTime transactionDateTimePST = TimeZoneInfo.ConvertTimeFromUtc(
                record.transactionDate.ToUniversalTime().DateTime, TimeZoneInfo.FindSystemTimeZoneById(ApplicationConstants.DEFAULT_TIME_ZONE));
            DateTime deliveryDateTimePST = TimeZoneInfo.ConvertTimeFromUtc(
                ((DateTimeOffset)record.deliveryDate).ToUniversalTime().DateTime, TimeZoneInfo.FindSystemTimeZoneById(ApplicationConstants.DEFAULT_TIME_ZONE));

            builder.AppendLine(string.Format("Transaction Date (PST): {0}", transactionDateTimePST));

            switch (record.category)
            {
                case (int)RecordCategory.Order:
                    var customerResults = context.GetCustomerById(record.ownerId, record.clientId);
                    SynchCustomer customer = null;
                    IEnumerator<GetCustomerByIdResult> customerEnumerator = customerResults.GetEnumerator();
                    if (customerEnumerator.MoveNext())
                    {
                        customer = new SynchCustomer()
                        {
                            name = customerEnumerator.Current.name
                        };
                    }
                    else
                    {
                        throw new WebFaultException<string>("Customer with given Id is not found", HttpStatusCode.NotFound);
                    }
                    builder.AppendLine(string.Format("Customer: {0}", customer.name));
                    builder.AppendLine(string.Format("Order from {0} {1}", account.firstName, account.lastName));
                    break;
                case (int)RecordCategory.Receipt:
                    var supplierResults = context.GetSupplierById(record.ownerId, record.clientId);
                    SynchSupplier supplier = null;
                    IEnumerator<GetSupplierByIdResult> supplierEnumerator = supplierResults.GetEnumerator();
                    if (supplierEnumerator.MoveNext())
                    {
                        supplier = new SynchSupplier()
                        {
                            name = supplierEnumerator.Current.name
                        };
                    }
                    else
                    {
                        throw new WebFaultException<string>("Supplier with given Id is not found", HttpStatusCode.NotFound);
                    }
                    builder.AppendLine(string.Format("Supplier: {0}", supplier.name));
                    builder.AppendLine(string.Format("Receipt for {0} {1}", account.firstName, account.lastName));
                    break;
                case (int)RecordCategory.PhysicalDamage:
                    builder.AppendLine("Inventory Change (Physical Damange)");
                    break;
                case (int)RecordCategory.PhysicalInventory:
                    builder.AppendLine("Inventory Change (Physical Inventory)");
                    break;
                case (int)RecordCategory.QualityIssue:
                    builder.AppendLine("Inventory Change (Quality Issue)");
                    break;
                case (int)RecordCategory.CycleCount:
                    builder.AppendLine("Inventory Change (Cycle Count)");
                    break;
                case (int)RecordCategory.Return:
                    builder.AppendLine("Inventory Change (Return)");
                    break;
                case (int)RecordCategory.SalesSample:
                    builder.AppendLine("Inventory Change (Sales Sample)");
                    break;
                case (int)RecordCategory.Stolen:
                    builder.AppendLine("Inventory Change (Stolen)");
                    break;
                default:
                    break;
            }

            builder.AppendLine(string.Format("Delivery Date: {0}", deliveryDateTimePST));
            builder.AppendLine();
            builder.AppendLine(string.Format("{0,-20}{1,-60}{2, -60}{3,-20}{4,-20}{5,-20}", "UPC", "Product Number", "Product Description", "Quantity", "Price", "Location"));

            SynchInventory[] sortedInventories = sortLinesByLocation(record, context);
            decimal totalPrice = 0.0m;

            for (int i = 0; i < sortedInventories.Length; i++)
            {
                string location = sortedInventories[i].location;
                string upc = sortedInventories[i].upc;
                string name = sortedInventories[i].name;
                string detail = sortedInventories[i].detail;
                SynchRecordLine line = getRecordLineByUpc(upc, record.recordLines);

                totalPrice += line.price * line.quantity;

                switch (record.category)
                {
                    case (int)RecordCategory.Order:
                        builder.AppendLine(string.Format("{0,-20}{1,-60}{2, -60}{3,-20}{4,-20}{5,-20}",
                                                                                            upc,
                                                                                            name,
                                                                                            detail,
                                                                                            line.quantity,
                                                                                            line.price,
                                                                                            location
                                                                                            ));
                        break;
                    case (int)RecordCategory.Receipt:
                        builder.AppendLine(string.Format("{0,-20}{1,-60}{2, -60}{3,-20}{4,-20}{5,-20}",
                                                                                            upc,
                                                                                            name,
                                                                                            detail,
                                                                                            line.quantity,
                                                                                            line.price,
                                                                                            location
                                                                                            ));
                        break;
                    default:
                        break;
                }

                builder.AppendLine();
            }

            builder.AppendLine("Total price: " + totalPrice);
            builder.AppendLine();
            builder.AppendLine(string.Format("Memo: {0}", record.comment));
            return builder.ToString();
        }

        private static SynchRecordLine getRecordLineByUpc(string upc, List<SynchRecordLine> lines)
        {
            foreach (SynchRecordLine line in lines)
            {
                if (upc == line.upc)
                    return line;
            }

            return null;
        }

        private static SynchInventory[] sortLinesByLocation(SynchRecord record, SynchDatabaseDataContext context)
        {
            List<string> locationList = new List<string>();
            SynchInventory[] sortedInventories = new SynchInventory[record.recordLines.Count];

            string pattern = "\\W+";
            string replacement = String.Empty;
            Regex rgx = new Regex(pattern);

            foreach (SynchRecordLine line in record.recordLines)
            {
                var results = context.GetInventoryByUpc(record.ownerId, line.upc);
                IEnumerator<GetInventoryByUpcResult> inventoryEnumerator = results.GetEnumerator();
                if (inventoryEnumerator.MoveNext())
                {
                    GetInventoryByUpcResult target = inventoryEnumerator.Current;
                    SynchInventory inventory = new SynchInventory()
                    {
                        name = target.name,
                        location = target.location,
                        detail = target.detail,
                        upc = target.upc
                    };

                    string comparableLocation = rgx.Replace(inventory.location, replacement);
                    locationList.Add(comparableLocation);

                }
            }

            locationList.Sort();

            foreach (SynchRecordLine line in record.recordLines)
            {
                var results = context.GetInventoryByUpc(record.ownerId, line.upc);
                IEnumerator<GetInventoryByUpcResult> inventoryEnumerator = results.GetEnumerator();
                if (inventoryEnumerator.MoveNext())
                {
                    GetInventoryByUpcResult target = inventoryEnumerator.Current;
                    SynchInventory inventory = new SynchInventory()
                    {
                        name = target.name,
                        location = target.location,
                        detail = target.detail,
                        upc = target.upc
                    };

                    string comparableLocation = rgx.Replace(inventory.location, replacement);
                    for (int i = 0; i < locationList.Count; i++)
                    {
                        if (comparableLocation == locationList[i])
                        {
                            while (sortedInventories[i] != null)
                                i++;
                            sortedInventories[i] = inventory;
                            break;
                        }
                    }

                }
            }


            return sortedInventories;
        }

        private static string getHtmlTemplate(string text)
        {
            string template = "<html>\n\t<body>\n";
            foreach (string line in text.Split('\n'))
                template += "\t\t<p>" + line + "</p>\n";

            template += "\t\t<div class=\"row\">\n\t\t\t<div class=\"span4\">\n\t\t\t\t<h2>Capture Field Orders</h2>\n\t\t\t\t<h4>No More Paperwork</h4>\n\t\t\t\t<p> With our app, you can capture sales orders on the go without paperwork or calling in orders.  You now communicate in an instant.</p>\n\t\t\t</div>\n\t\t\t<!-- /.span4 -->\n\t\t\t<div class=\"span4\">\n\t\t\t\t<h2>See Your Inventory</h2>\n\t\t\t\t<h4>Always Updated</h4>\n\t\t\t\t<p>Know exactly what you have in inventory from the office and the field instantly.  No more running to the warehouse or calling, just be in Synch!</p>\n\t\t\t</div><!-- /.span4 -->\n\t\t\t<div class=\"span4\">\n\t\t\t\t<h2>See the Trends</h2>\n\t\t\t\t<h4>Be informed</h4>\n\t\t\t\t<p>Access up to date business intelligence about previous sales, seasonal trends and more. Synch helps you make the right decisions from the palm of your hand.</p>\n\t\t\t</div><!-- /.span4 -->\n\t\t</div><!-- /.row -->\n\t</body>\n</html>\n\t\t";

            return template;
        }
    }
}