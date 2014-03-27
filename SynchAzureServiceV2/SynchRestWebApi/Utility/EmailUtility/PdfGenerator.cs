using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;

using SynchRestWebApi.Models;

namespace SynchRestWebApi.Utility.EmailUtility
{
    public class PdfGenerator
    {

        public PdfGenerator()
        {
        }

        public void generatePdfAtFilePath(SynchRecord record, SynchAccount account, SynchBusiness business,
                                            ISynchClient client, Dictionary<string, SynchInventory> upcToInventoryMap, string filePath)
        {
            Document document = new Document();
            document.Info.Title = record.title;
            document.Info.Subject = record.title;
            document.Info.Author = "Synch Service";

            defineStyles(ref document);

            createPage(ref document, business, record, account, client, upcToInventoryMap);

            document.UseCmykColor = true;

            // ===== Unicode encoding and font program embedding in MigraDoc is demonstrated here =====

            // A flag indicating whether to create a Unicode PDF or a WinAnsi PDF file.
            // This setting applies to all fonts used in the PDF document.
            // This setting has no effect on the RTF renderer.
            const bool unicode = false;

            // An enum indicating whether to embed fonts or not.
            // This setting applies to all font programs used in the document.
            // This setting has no effect on the RTF renderer.
            // (The term 'font program' is used by Adobe for a file containing a font. Technically a 'font file'
            // is a collection of small programs and each program renders the glyph of a character when executed.
            // Using a font in PDFsharp may lead to the embedding of one or more font programms, because each outline
            // (regular, bold, italic, bold+italic, ...) has its own fontprogram)
            const PdfFontEmbedding embedding = PdfFontEmbedding.Always;

            // ========================================================================================

            // Create a renderer for the MigraDoc document.
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(unicode, embedding);

            // Associate the MigraDoc document with a renderer
            pdfRenderer.Document = document;

            // Layout and render document to PDF
            pdfRenderer.RenderDocument();

            // Save the document...
            pdfRenderer.PdfDocument.Save(filePath);

            //using (MemoryStream ms = new MemoryStream())
            //{
            //    pdfRenderer.Save(ms, false);
            //    return ms;
            //}

        }

        private void defineStyles(ref Document document)
        {
            // Get the predefined style Normal.
            Style style = document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Verdana";

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called Table based on style Normal
            style = document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 9;

            // Create a new style called Reference based on style Normal
            style = document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
        }

        private void createPage(ref Document document, SynchBusiness business, SynchRecord record, SynchAccount account,
                                ISynchClient client, Dictionary<string, SynchInventory> upcToInventoryMap)
        {
            // Each MigraDoc document needs at least one section.
            Section section = document.AddSection();

            // Create footer
            Paragraph paragraph = section.Footers.Primary.AddParagraph();
            paragraph.AddText(business.name + " · " + business.address + " · " + business.postalCode + " · " + business.phoneNumber);
            paragraph.Format.Font.Size = 9;
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            // Add the print date field
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "8cm";
            paragraph.Style = "Reference";
            paragraph.AddFormattedText("TEMPORARY INVOICE", TextFormat.Bold);
            paragraph.AddTab();
            paragraph.AddText(account.firstName + " " + account.lastName + " at " + business.name + ", ");
            paragraph.AddDateField("MM.dd.yyyy");

            // Add from and to fields
            MigraDoc.DocumentObjectModel.Shapes.TextFrame addressFrame = section.AddTextFrame();
            addressFrame.Height = "4.0cm";
            addressFrame.Width = "7.0cm";
            addressFrame.Left = MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Left;
            addressFrame.RelativeHorizontal = MigraDoc.DocumentObjectModel.Shapes.RelativeHorizontal.Margin;
            addressFrame.Top = "5.0cm";
            addressFrame.RelativeVertical = MigraDoc.DocumentObjectModel.Shapes.RelativeVertical.Page;

            paragraph = addressFrame.AddParagraph("Name and Address of Client");
            paragraph.Format.Font.Name = "Times New Roman";
            paragraph.Format.Font.Size = 10;
            paragraph.Format.SpaceAfter = 3;

            paragraph = addressFrame.AddParagraph();
            paragraph.AddText(client.name);
            paragraph.AddLineBreak();
            paragraph.AddText(client.address);
            paragraph.AddLineBreak();
            paragraph.AddText(client.postalCode);
            paragraph.AddLineBreak();
            paragraph.AddText(client.phoneNumber);

            // Create the item table
            MigraDoc.DocumentObjectModel.Tables.Table table = section.AddTable();
            table.Style = "Table";
            table.Borders.Width = 0.25;
            table.Borders.Left.Width = 0.5;
            table.Borders.Right.Width = 0.5;
            table.Rows.LeftIndent = 0;

            // Before you can add a row, you must define the columns
            // item #
            MigraDoc.DocumentObjectModel.Tables.Column column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("7cm");    // item description
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("2cm");    // item quantity
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("2cm");  // item rate
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("2cm");    // item amount
            column.Format.Alignment = ParagraphAlignment.Center;

            // Create the header of the table
            MigraDoc.DocumentObjectModel.Tables.Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Cells[0].AddParagraph("Item #");
            row.Cells[1].AddParagraph("Item Description");
            row.Cells[2].AddParagraph("Quantity");
            row.Cells[3].AddParagraph("Rate");
            row.Cells[4].AddParagraph("Amount");

            table.SetEdge(0, 0, 5, 1, MigraDoc.DocumentObjectModel.Tables.Edge.Box, BorderStyle.Single, 0.75, Color.Empty);


            // Iterate the invoice items
            decimal totalPrice = 0.0m;

            foreach (SynchRecordLine line in record.recordLines)
            {
                SynchInventory inventory = upcToInventoryMap[line.upc];

                totalPrice += line.price * line.quantity;

                MigraDoc.DocumentObjectModel.Tables.Row curRow = table.AddRow();
                curRow.Format.Alignment = ParagraphAlignment.Left;
                curRow.Cells[0].AddParagraph(inventory.name);
                curRow.Cells[1].AddParagraph(inventory.detail);
                curRow.Cells[2].AddParagraph(line.quantity.ToString());
                curRow.Cells[3].AddParagraph("$" + line.price.ToString());
                curRow.Cells[4].AddParagraph("$" + (line.price * line.quantity).ToString());
                curRow.Cells[0].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                curRow.Cells[1].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                curRow.Cells[2].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                curRow.Cells[3].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                curRow.Cells[4].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;

                table.SetEdge(0, table.Rows.Count - 1, 5, 1, MigraDoc.DocumentObjectModel.Tables.Edge.Box, BorderStyle.Single, 0.75);

                if (!String.IsNullOrEmpty(line.note))
                {
                    MigraDoc.DocumentObjectModel.Tables.Row lineNoteRow = table.AddRow();
                    lineNoteRow.Format.Alignment = ParagraphAlignment.Left;
                    lineNoteRow.Cells[0].AddParagraph(line.note);
                    lineNoteRow.Cells[0].MergeRight = 4;
                    lineNoteRow.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.LightGray;

                }
            }

            // Add an invisible row as a space line to the table
            MigraDoc.DocumentObjectModel.Tables.Row spacing = table.AddRow();
            spacing.Borders.Visible = false;

            // Add the total price row
            MigraDoc.DocumentObjectModel.Tables.Row totalPriceRow = table.AddRow();
            totalPriceRow.Cells[0].Borders.Visible = false;
            totalPriceRow.Cells[0].AddParagraph("Total Price");
            totalPriceRow.Cells[0].Format.Font.Bold = true;
            totalPriceRow.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            totalPriceRow.Cells[0].MergeRight = 3;
            totalPriceRow.Cells[4].AddParagraph("$" + totalPrice.ToString());

            // Add the notes paragraph
            if (!String.IsNullOrEmpty(record.comment))
            {
                MigraDoc.DocumentObjectModel.Shapes.TextFrame memoFrame = section.AddTextFrame();
                memoFrame.Height = "4.0cm";
                memoFrame.Width = "18.0cm";
                memoFrame.Left = MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Left;
                memoFrame.RelativeHorizontal = MigraDoc.DocumentObjectModel.Shapes.RelativeHorizontal.Margin;
                memoFrame.Top = "2.0cm";
                memoFrame.RelativeVertical = MigraDoc.DocumentObjectModel.Shapes.RelativeVertical.Paragraph;

                paragraph = memoFrame.AddParagraph("Memo:");
                paragraph.Format.Font.Name = "Times New Roman";
                paragraph.Format.Font.Size = 10;
                paragraph.Format.SpaceAfter = 3;

                paragraph = memoFrame.AddParagraph();
                paragraph.AddText(record.comment);
            }

        }
    }

}