using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;

namespace DataSetSample
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string sConnectionString;
                sConnectionString = "Password=myPassword;User ID=myUserID;"
                                    + "Initial Catalog=pubs;"
                                    + "Data Source=(local)";
                SqlConnection objConn
                    = new SqlConnection(sConnectionString);
                objConn.Open();

                SqlDataAdapter daAuthors
                    = new SqlDataAdapter("Select * From Authors", objConn);
                DataSet dsPubs = new DataSet("Pubs");
                daAuthors.FillSchema(dsPubs, SchemaType.Source, "Authors");
                daAuthors.Fill(dsPubs, "Authors");

                DataTable tblAuthors;
                tblAuthors = dsPubs.Tables["Authors"];

                foreach (DataRow drCurrent in tblAuthors.Rows)
                {
                    Console.WriteLine("{0} {1}",
                        drCurrent["au_fname"].ToString(),
                        drCurrent["au_lname"].ToString());
                }
                Console.ReadLine();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
