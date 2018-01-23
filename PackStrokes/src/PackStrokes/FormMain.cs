using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Wacom.Ink;
using BaXterX;
using System.IO;
using System.Diagnostics;

namespace PackStrokes
{
    using InkPath = Wacom.Ink.Path; // resolve ref to System.IO.Path
    

    public partial class FormMain : Form
    {
        string csv = string.Empty;

        StrokeCollection sc;

        public FormMain()
        {
            InitializeComponent();

            sc = new StrokeCollection();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.Text = @"PackStroke";
            PbtnStart.Text = @"Start";
            PbtnFileOpen.Text = @"File...";
        }

        private void PbtnStart_Click(object sender, EventArgs e)
        {
            // Define Regions
            sc.CreateRegion(10, 10, 110, 60);
            sc.CreateRegion(200, 10, 300, 60);

            // Simurate Input Strokes
            List<float> data;
            uint stride;
            InkPath p;

            // 1st stroke
            data = new List<float> { 20,21,1, 20,22,1};
            stride = 3;
            p = new InkPath(data, stride, PathFormat.XYA);
            sc.CreateStroke(p);

            // 2nd stroke
            data = new List<float> { 21,30,1, 25,30,1, 24,40,1 };
            stride = 3;
            p = new InkPath(data, stride, PathFormat.XYA);
            sc.CreateStroke(p);

            // 3rd stroke
            data = new List<float> { 210,13,1, 212,30,1, 220,23,1 };
            stride = 3;
            p = new InkPath(data, stride, PathFormat.XYA);
            sc.CreateStroke(p);


            // ストロークをリージョンに当てはめる
            // 後から当てはめるか、
            // リアルタイムに当てはめるか sc.Regionがあるかどうか
            sc.StrokesToRegion();
        }

        private void PbtnFileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                FileName = string.Empty,
                //           ofd.InitialDirectory = @"C:\";
                InitialDirectory = string.Empty,  // current directory
                Filter = "PDF File (*.pdf)|*.pdf|All Files(*.*)|*.*",
                FilterIndex = 2,
                //set the title
                Title = "Please set the File",
                RestoreDirectory = true,
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string path = string.Empty;
                path = ofd.FileName;    // full path + filename + extension
                                        //               textBoxReadFile.Text = path;
                ReadBaxter(path);
            }
        }

        public void ReadBaxter(string path)
        {
            try
            {
                Reader reader = new Reader();   // BaXter
                StreamReader stream = new StreamReader(path, Encoding.GetEncoding("UTF-8"));

                reader.readFromStream(stream.BaseStream);

                //Now the document has been parsed, it can be read or modified via the reader class.
                //Check the metadata we're interested in exists in the document
                if (reader.document.exists(ElementNames.AUTHORING_TOOL))
                {
                    // Read the Authoring Tool properties
                    //                    richTextBoxResult.AppendText("Before " + reader.document.authoringToolName + " " + reader.document.authoringToolVersion + Environment.NewLine);

                    // Edit the Authoring Tool property
                    reader.document.authoringToolVersion = "v3";
                    //                    richTextBoxResult.AppendText("Edited " + reader.document.authoringToolName + " " + reader.document.authoringToolVersion + Environment.NewLine);
                }

                // We can easily erase whole elements
                if (reader.document.exists(ElementNames.SMARTPAD))
                {
                    reader.document.eraseElement(ElementNames.SMARTPAD);
                }
                // Then re-add them
                reader.document.smartPadID = "12345";
                reader.document.smartPadDeviceName = "Wacom Clipboard";
                //                richTextBoxResult.AppendText(reader.document.smartPadDeviceName + Environment.NewLine);

                // As the document metadata has been edited, we should regenerate the XMP
                // for the client to insert back into the PDF.
                var new_xmp = reader.document.toXMP();

                //All Document Level Metadata is accessible via the document object
                var page_ids = reader.document.pageIDList;
                //Pages with Metadata will be listed in the PageIDList
                //                richTextBoxResult.AppendText("Active Pages: \t" + Environment.NewLine);
                foreach (var page_id in page_ids)
                {
                    //                    richTextBoxResult.AppendText("PDF #" + page_id.Item1 + " UUID " + page_id.Item2 + Environment.NewLine);
                }

                //Page objects are accessed in the order they were discovered.
                var page = reader.document.pages[0];
                //                richTextBoxResult.AppendText("Got Page by vector with UUID " + page.uuid + Environment.NewLine);

                //Using our page object reference, we can access page level metadata
                if (page.exists(ElementNames.PAGE_ID))
                {
                    //                    richTextBoxResult.AppendText("Page with UUID " + page.uuid + " belongs to PDF page " + page.pdfPage + Environment.NewLine);
                }

                //Accessing Fields within a Page is much the same as accessing Pages within the Document
                var field_ids = page.fieldIDList;
                //                richTextBoxResult.AppendText("Found Fields \t" + Environment.NewLine);
                foreach (var field_id in field_ids)
                {
                    //                    richTextBoxResult.AppendText(field_id + "\t" + Environment.NewLine);
                }

                //We can iterate through a Page's fields vector to find any signatures / handwriting etc.
                foreach (var field in page.fields)
                {
                    if (field.type == "Signature")
                    {
                        //                        richTextBoxResult.AppendText("Found a signature Field " + field.pdfID + Environment.NewLine);
                        //                        richTextBoxResult.AppendText("\t Encrypted " + (field.encrypted ? "YES" : "NO") + Environment.NewLine);
                        //                        richTextBoxResult.AppendText("\t Required " + (field.required ? "YES" : "NO") + Environment.NewLine);
                        //                        richTextBoxResult.AppendText("\t Signatory Time  " + field.completionTime + Environment.NewLine);
                        //                        richTextBoxResult.AppendText("\t FSS Data " + field.data + Environment.NewLine);
                    }
                    else if (field.type == "Text")
                    {
                        //                        richTextBoxResult.AppendText("Found a text Field: " + field.pdfID + Environment.NewLine);
                        //                        richTextBoxResult.AppendText("\t Tag: " + field.tag + Environment.NewLine);
                        //                        richTextBoxResult.AppendText("\t Handwriting Recognition Data: " + field.data + Environment.NewLine);
                        //                        richTextBoxResult.AppendText("\t Location XYHW: "
                        //                            + field.locationX + ", "
                        //                            + field.locationY + ", "
                        //                            + field.locationH + ", "
                        //                            + field.locationW + ", "
                        //                            + Environment.NewLine);
                        //                        richTextBoxResult.AppendText("\t Completion Time: "
                        //                            + field.completionTime + Environment.NewLine);

                        sc.CreateRegion(float.Parse(field.locationX),
                            float.Parse(field.locationY),
                            float.Parse(field.locationX) + float.Parse(field.locationW),
                            float.Parse(field.locationY) + float.Parse(field.locationH),
                            field.tag, field.data, field.pdfID
                            );

                        //csv += (field.pdfID
                        //    + "," + field.tag
                        //    + "," + field.data
                        //    + ", " + field.locationX
                        //    + ", " + field.locationY
                        //    + ", " + field.locationH
                        //    + ", " + field.locationW
                        //    + Environment.NewLine);
                    }
                }

                //                pbtnExport.Enabled = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
