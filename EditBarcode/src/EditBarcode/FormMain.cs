using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace EditBarcode
{
    public partial class FormMain : Form
    {

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.Text = "EditBarcode"; // ToDo: dynamically read from the program name
            labelHigh.Text = "High";
            labelMiddle.Text = "Middle";
            labelLow.Text = "Low";
            PbtnRead.Text = "Read";
            PbtnWrite.Text = "Write";

            // 2-digit
            textBoxHigh.MaxLength = 2;
            textBoxMiddle.MaxLength = 2;
            textBoxLow.MaxLength = 2;
        }

        private void PbtnRead_Click(object sender, EventArgs e)
        {
            ClbBarcode cb = new ClbBarcode();
            string f = string.Empty;
            if (cb.GetUserConfigXml(ref f)) // Get barcode values from UserConfig
            {
                textBoxHigh.Text = cb.BarcodeValues[0];
                textBoxMiddle.Text = cb.BarcodeValues[1];
                textBoxLow.Text = cb.BarcodeValues[2];
            }
            else
            {
                MessageBox.Show(f, this.Text, MessageBoxButtons.OK);
            }
        }

        private void PbtnWrite_Click(object sender, EventArgs e)
        {
            ClbBarcode cb = new ClbBarcode();
            cb.BarcodeValues[0] = textBoxHigh.Text;
            cb.BarcodeValues[1] = textBoxMiddle.Text;
            cb.BarcodeValues[2] = textBoxLow.Text;

            // write back to a config file
            string f = string.Empty;
            if (cb.SetUserConfigXml(ref f))
            {
                MessageBox.Show("Updated.", this.Text, MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(f, this.Text, MessageBoxButtons.OK);
            }
        }

        public void ReadBaxter()
        {
            try
            {
                Reader reader = new Reader();   // BaXter
//                StreamReader stream = new StreamReader(path, Encoding.GetEncoding("euc-jp"));
                StreamReader stream = new StreamReader(path, Encoding.GetEncoding("UTF-8"));

                reader.readFromStream(stream.BaseStream);

                //Now the document has been parsed, it can be read or modified via the reader class.
                //Check the metadata we're interested in exists in the document
                if (reader.document.exists(ElementNames.AUTHORING_TOOL))
                {
                    // Read the Authoring Tool properties
                    richTextBoxResult.AppendText("Before " + reader.document.authoringToolName + " " + reader.document.authoringToolVersion + Environment.NewLine);

                    // Edit the Authoring Tool property
                    reader.document.authoringToolVersion = "v3";
                    richTextBoxResult.AppendText("Edited " + reader.document.authoringToolName + " " + reader.document.authoringToolVersion + Environment.NewLine);
                }

                // We can easily erase whole elements
                if (reader.document.exists(ElementNames.SMARTPAD))
                {
                    reader.document.eraseElement(ElementNames.SMARTPAD);
                }
                // Then re-add them
                reader.document.smartPadID = "12345";
                reader.document.smartPadDeviceName = "Wacom Clipboard";
                richTextBoxResult.AppendText(reader.document.smartPadDeviceName + Environment.NewLine);

                // As the document metadata has been edited, we should regenerate the XMP
                // for the client to insert back into the PDF.
                var new_xmp = reader.document.toXMP();

                //All Document Level Metadata is accessible via the document object
                var page_ids = reader.document.pageIDList;
                //Pages with Metadata will be listed in the PageIDList
                richTextBoxResult.AppendText("Active Pages: \t" + Environment.NewLine);
                foreach (var page_id in page_ids)
                {
                    richTextBoxResult.AppendText("PDF #" + page_id.Item1 + " UUID " + page_id.Item2 + Environment.NewLine);
                }

                //Page objects are accessed in the order they were discovered.
                var page = reader.document.pages[0];
                richTextBoxResult.AppendText("Got Page by vector with UUID " + page.uuid + Environment.NewLine);

                //Using our page object reference, we can access page level metadata
                if (page.exists(ElementNames.PAGE_ID))
                {
                    richTextBoxResult.AppendText("Page with UUID " + page.uuid + " belongs to PDF page " + page.pdfPage + Environment.NewLine);
                }

                //Accessing Fields within a Page is much the same as accessing Pages within the Document
                var field_ids = page.fieldIDList;
                richTextBoxResult.AppendText("Found Fields \t" + Environment.NewLine);
                foreach (var field_id in field_ids)
                {
                    richTextBoxResult.AppendText(field_id + "\t" + Environment.NewLine);
                }

                //We can iterate through a Page's fields vector to find any signatures / handwriting etc.
                foreach (var field in page.fields)
                {
                    if (field.type == "Signature")
                    {
                        richTextBoxResult.AppendText("Found a signature Field " + field.pdfID + Environment.NewLine);
                        richTextBoxResult.AppendText("\t Encrypted " + (field.encrypted ? "YES" : "NO") + Environment.NewLine);
                        richTextBoxResult.AppendText("\t Required " + (field.required ? "YES" : "NO") + Environment.NewLine);
                        richTextBoxResult.AppendText("\t Signatory Time  " + field.completionTime + Environment.NewLine);
                        richTextBoxResult.AppendText("\t FSS Data " + field.data + Environment.NewLine);
                    }
                    else if (field.type == "Text")
                    {
                        richTextBoxResult.AppendText("Found a text Field: " + field.pdfID + Environment.NewLine);
                        richTextBoxResult.AppendText("\t Tag: " + field.tag + Environment.NewLine);
                        richTextBoxResult.AppendText("\t Handwriting Recognition Data: " + field.data + Environment.NewLine);
                        richTextBoxResult.AppendText("\t Location XYHW: "
                            + field.locationX + ", "
                            + field.locationY + ", "
                            + field.locationH + ", "
                            + field.locationW + ", "
                            + Environment.NewLine);
                        richTextBoxResult.AppendText("\t Completion Time: "
                            + field.completionTime + Environment.NewLine);

                        csv += (field.pdfID
                            + "," + field.tag
                            + "," + field.data
                            + ", " + field.locationX
                            + ", " + field.locationY
                            + ", " + field.locationH
                            + ", " + field.locationW
                            + Environment.NewLine);
                    }
                }

                pbtnExport.Enabled = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}
