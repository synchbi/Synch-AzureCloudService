using SendGrid;
using SendGrid.Transport;
using System;
using System.IO;
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

using Microsoft.WindowsAzure.ServiceRuntime;
using SynchRestWebApi.Models;

namespace SynchRestWebApi.Utility.EmailUtility
{
    public class EmailController
    {

        public int businessId;
        public int accountId;

        public EmailController(int businessId, int accountId)
        {
            this.businessId = businessId;
            this.accountId = accountId;
        }

        public bool sendEmailForNewAccount(SynchDatabaseDataContext context, SynchAccount account)
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


        public bool sendEmailForNewBusiness(SynchBusiness business)
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
            text.AppendLine(
                string.Format(
                "\nYour Synch Business ID is: {0}.\n",
                business.id));
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

        
        public void sendEmailForRecord(SynchRecord record, bool presented)
        {
            // get Synch Objects ready
            SynchDatabaseController synchDatabaseController = new SynchDatabaseController(businessId);
            SynchAccount account = synchDatabaseController.getAccount(record.accountId);
            SynchBusiness business = synchDatabaseController.getBusiness();
            ISynchClient client = null;

            switch (record.category)
            {
                case (int)RecordCategory.Order:
                    client = synchDatabaseController.getCustomer(record.clientId);
                    break;
                case (int)RecordCategory.Receipt:
                    client = synchDatabaseController.getSupplier(record.clientId);
                    break;
                default:        // by default no email will be sent to inventory changes
                    return;
            }

            Dictionary<string, SynchInventory> upcToInventoryMap = synchDatabaseController.getUpcToInventoryMap();

            var message = SendGrid.Mail.GetInstance();

            message.From = new MailAddress("Synch Order Tracking Service <ordertracking@synchbi.com>");
            message.AddTo(string.Format("{0} {1} <{2}>", account.firstName, account.lastName, account.email));
            message.AddTo(string.Format("{0} <{1}>", business.name, business.email));
            message.AddTo(" synchbiorder@gmail.com");

            if (presented)
                message.Subject = "[Synch] Presented Order confirmation";
            else
                message.Subject = "[Synch] Order confirmation";

            StringBuilder text = new StringBuilder();
            text.AppendLine(record.title);
            text.AppendLine(FormatRecord(record, account, business, client, upcToInventoryMap, presented));
            text.AppendLine();
            text.AppendLine();
            text.AppendLine(
                string.Format(
                "This email was auto-generated and auto-sent to {0}. Please do not reply! ",
                business.name));
            message.Text = text.ToString();

            // create PDF attachment here
            LocalResource localResource = RoleEnvironment.GetLocalResource("EmailAttachmentStorage");

            // Define the file name and path.
            string attachmentPath = localResource.RootPath + "attachment_" + record.id + ".pdf";

            PdfGenerator pdfGenerator = new PdfGenerator();
            pdfGenerator.generatePdfAtFilePath(record, account, business, client, upcToInventoryMap, attachmentPath);

            using (FileStream fileStream = File.OpenRead(attachmentPath))
            {
                MemoryStream ms = new MemoryStream();

                ms.SetLength(fileStream.Length);

                fileStream.Read(ms.GetBuffer(), 0, (int)fileStream.Length);

                message.StreamedAttachments.Add("attachment_" + record.id + ".pdf", ms);
            }

            var username = "azure_bf33e57baacbfaae4ebfe0814f1d8a5d@azure.com";
            var password = "i6dvglzv";
            var credentials = new NetworkCredential(username, password);

            var transportSMTP = SMTP.GetInstance(credentials);

            try
            {
                transportSMTP.Deliver(message);
                
            }
            catch (Exception e)
            {
            }

            try
            {
                File.Delete(attachmentPath);
            }
            catch (Exception e)
            {

            }
        }

        private string FormatRecord(SynchRecord record, SynchAccount account, SynchBusiness business,
                                    ISynchClient client, Dictionary<string, SynchInventory> upcToInventoryMap, bool presented)
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
                    builder.AppendLine(string.Format("Customer: {0}", client.name));
                    if (presented)
                        builder.AppendLine(string.Format("Presented Order from {0} {1}", account.firstName, account.lastName));
                    else
                        builder.AppendLine(string.Format("Order from {0} {1}", account.firstName, account.lastName));
                    break;
                case (int)RecordCategory.Receipt:
                    builder.AppendLine(string.Format("Supplier: {0}", client.name));
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

            SynchInventory[] sortedInventories = sortLinesByLocation(record, upcToInventoryMap);
            decimal totalPrice = 0.0m;
            int totalNumberOfItems = 0;

            for (int i = 0; i < sortedInventories.Length; i++)
            {
                string location = sortedInventories[i].location;
                string upc = sortedInventories[i].upc;
                string name = sortedInventories[i].name;
                string detail = sortedInventories[i].detail;
                SynchRecordLine line = getRecordLineByUpc(upc, record.recordLines);

                totalPrice += line.price * line.quantity;
                totalNumberOfItems += line.quantity;

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
            builder.AppendLine("Total number of units: " + totalNumberOfItems);
            builder.AppendLine();
            builder.AppendLine("Total number of lines: " + sortedInventories.Length);
            builder.AppendLine();

            builder.AppendLine(string.Format("Memo: {0}", record.comment));
            return builder.ToString();
        }

        private SynchRecordLine getRecordLineByUpc(string upc, List<SynchRecordLine> lines)
        {
            foreach (SynchRecordLine line in lines)
            {
                if (upc == line.upc)
                    return line;
            }

            return null;
        }

        private SynchInventory[] sortLinesByLocation(SynchRecord record, Dictionary<string, SynchInventory> upcToInventoryMap)
        {
            List<string> locationList = new List<string>();
            SynchInventory[] sortedInventories = new SynchInventory[record.recordLines.Count];

            string pattern = "\\W+";
            string replacement = String.Empty;
            Regex rgx = new Regex(pattern);

            foreach (SynchRecordLine line in record.recordLines)
            {
                SynchInventory inventory = upcToInventoryMap[line.upc];

                string comparableLocation = rgx.Replace(inventory.location, replacement);
                locationList.Add(comparableLocation);
            }

            locationList.Sort();

            foreach (SynchRecordLine line in record.recordLines)
            {
                SynchInventory inventory = upcToInventoryMap[line.upc];

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


            return sortedInventories;
        }

        private string getHtmlTemplate(string text)
        {
            string template = "<html>\n\t<body>\n";
            foreach (string line in text.Split('\n'))
                template += "\t\t<p>" + line + "</p>\n";

            template += "\t\t<div class=\"row\">\n\t\t\t<div class=\"span4\">\n\t\t\t\t<h2>Capture Field Orders</h2>\n\t\t\t\t<h4>No More Paperwork</h4>\n\t\t\t\t<p> With our app, you can capture sales orders on the go without paperwork or calling in orders.  You now communicate in an instant.</p>\n\t\t\t</div>\n\t\t\t<!-- /.span4 -->\n\t\t\t<div class=\"span4\">\n\t\t\t\t<h2>See Your Inventory</h2>\n\t\t\t\t<h4>Always Updated</h4>\n\t\t\t\t<p>Know exactly what you have in inventory from the office and the field instantly.  No more running to the warehouse or calling, just be in Synch!</p>\n\t\t\t</div><!-- /.span4 -->\n\t\t\t<div class=\"span4\">\n\t\t\t\t<h2>See the Trends</h2>\n\t\t\t\t<h4>Be informed</h4>\n\t\t\t\t<p>Access up to date business intelligence about previous sales, seasonal trends and more. Synch helps you make the right decisions from the palm of your hand.</p>\n\t\t\t</div><!-- /.span4 -->\n\t\t</div><!-- /.row -->\n\t</body>\n</html>\n\t\t";

            return template;
        }
    }
}