﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
//using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace TShirt.InventoryApp.Integration.To.GR
{
     class TransactionAddItem : Transaccion, ITransaccion
    {
        private string file;
        private string fname;

        private string server;
        private string db;

        public TransactionAddItem(string strcon, string nin, string nout, string hin, string hout, string fil, string srv, string datab, string itbms)
            : base("TransactionAddItem", 11, strcon, nin, nout, hin, hout, itbms)
        {
            file = fil;
            fname = file.Split('\\').Last().Split('/').Last().Replace(nin, "").Replace("\\","");
            server = srv;
            db = datab;
        }

        #region Miembros de ITransaccion

        void ITransaccion.Execute()
        {
            // GenerateDocument();
            Models.Item item = getFromFile(file);
            if(item!=null)
                if(InsertItem(item))
                {
                    try {
                        if (File.Exists(this.HistPathOut + "\\OK_" + fname))
                            File.Delete(this.HistPathOut + "\\OK_" + fname);

                        File.Move(file, this.HistPathOut + "\\OK_" + fname); } catch { }
                }
                else
                {
                    try {
                        if (File.Exists(this.HistPathOut + "\\ERR_" + fname))
                            File.Delete(this.HistPathOut + "\\ERR_" + fname);

                        File.Move(file, this.HistPathOut + "\\ERR_" + fname); } catch { }
                }
        }

        #endregion

        private void GenerateDocument()
        {
            DataSet data = parseXML(file);

            string eConnectConnectionString = string.Format("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog={1};Data Source={0};", this.server, this.db);

            string DOCID = GPHelper.InvoiceDOCID(this.StringConnection).Trim();


            //Now we create the eConnect XML Document
            XmlDocument eConnectXMLDocument = new XmlDocument();
            XmlNode NodeeConnect;
            XmlNode NodeSchema;
            XmlNode taSopHdrNode;
            XmlNode taSopLine_ItemsNode;
            XmlNode taSopLineNode;
            XmlNode taCreateSopPaymentInsertRecord_Items;
            XmlNode taCreateSopPaymentInsertRecord;

            XmlNode NodeElement;

            if (data.Tables["HEADER"].Rows.Count > 0 && data.Tables["LINES"].Rows.Count > 0)
            {
                DataRow header = data.Tables["HEADER"].Rows[0];


                if (header["DOCID"].ToString().Length <= 0)
                {
                    DOCID = GPHelper.returnDOCID(this.StringConnection).Trim();
                }
                else
                {
                    DOCID = header["DOCID"].ToString();
                }

                if (header["DOCNUMBE"].ToString() == "")
                {
                    IEvent w = new WarningEvent("", "", "El documento no posee código, se intentará generar uno para proceder con el mismo.");
                    w.Publish();


                    header["DOCNUMBE"] = "";
                }
                NodeeConnect = eConnectXMLDocument.CreateElement("eConnect");
                NodeSchema = eConnectXMLDocument.CreateElement("SOPTransactionType");

                //Header
                taSopHdrNode = eConnectXMLDocument.CreateElement("taSopHdrIvcInsert");

                NodeElement = eConnectXMLDocument.CreateElement("SOPTYPE");
                NodeElement.InnerText = "3"; // INVOICE
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("DOCID");
                NodeElement.InnerText = DOCID;
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("DEFPRICING");
                NodeElement.InnerText = "1";
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("CREATETAXES");
                NodeElement.InnerText = "1";
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("DEFTAXSCHDS");
                NodeElement.InnerText = "1";
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("BACHNUMB");
                NodeElement.InnerText = DOCID + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("SOPNUMBE");
                NodeElement.InnerText = header["DOCNUMBE"].ToString();
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("ORIGNUMB");
                NodeElement.InnerText = header["ORIGNUMB"].ToString();
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("ORIGTYPE");
                NodeElement.InnerText = header["ORIGTYPE"].ToString();
                taSopHdrNode.AppendChild(NodeElement);

                if (header["TAXSCHID"].ToString().Length <= 0)
                {
                    NodeElement = eConnectXMLDocument.CreateElement("TAXSCHID");
                    NodeElement.InnerText = this.ITBMS;
                    taSopHdrNode.AppendChild(NodeElement);
                }
                else
                {
                    NodeElement = eConnectXMLDocument.CreateElement("TAXSCHID");
                    NodeElement.InnerText = header["TAXSCHID"].ToString();
                    taSopHdrNode.AppendChild(NodeElement);
                }


                NodeElement = eConnectXMLDocument.CreateElement("FRTSCHID");
                NodeElement.InnerText = header["FRTSCHID"].ToString();
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("MSCSCHID");
                NodeElement.InnerText = header["MSCSCHID"].ToString();
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("SHIPMTHD");
                NodeElement.InnerText = header["SHIPMTHD"].ToString();
                taSopHdrNode.AppendChild(NodeElement);

                //if ((Decimal)header["TAXAMNT"] > 0)
                //{
                //    NodeElement = eConnectXMLDocument.CreateElement("TAXAMNT");
                //    NodeElement.InnerText = header["TAXAMNT"].ToString();
                //    taSopHdrNode.AppendChild(NodeElement);
                //}


                NodeElement = eConnectXMLDocument.CreateElement("LOCNCODE");
                NodeElement.InnerText = header["LOCNCODE"].ToString();
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("DOCDATE");
                NodeElement.InnerText = DateTime.Parse(header["DOCDATE"].ToString()).Year + "-" + DateTime.Parse(header["DOCDATE"].ToString()).Month + "-" + DateTime.Parse(header["DOCDATE"].ToString()).Day;
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("FREIGHT");
                NodeElement.InnerText = header["FREIGHT"].ToString();
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("MISCAMNT");
                NodeElement.InnerText = header["MISCAMNT"].ToString();
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("TRDISAMT");
                NodeElement.InnerText = header["TRDISAMT"].ToString();
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("CUSTNMBR");
                NodeElement.InnerText = header["CUSTOMER"].ToString();
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("CSTPONBR");
                NodeElement.InnerText = header["CSTPONBR"].ToString();
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("SUBTOTAL");
                NodeElement.InnerText = header["SUBTOTAL"].ToString();
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("DOCAMNT");
                NodeElement.InnerText = header["DOCAMNT"].ToString();
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("SALSTERR");
                NodeElement.InnerText = header["SALSTERR"].ToString();
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("SLPRSNID");
                NodeElement.InnerText = header["SLPRSNID"].ToString();
                taSopHdrNode.AppendChild(NodeElement);


                #region Lines
                // Details

                taSopLine_ItemsNode = eConnectXMLDocument.CreateElement("taSopLineIvcInsert_Items");

                foreach (DataRow item in data.Tables["LINES"].Rows)
                {
                    taSopLineNode = eConnectXMLDocument.CreateElement("taSopLineIvcInsert");
                    NodeElement = eConnectXMLDocument.CreateElement("SOPTYPE");
                    NodeElement.InnerText = "3"; //return
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("DEFPRICING");
                    NodeElement.InnerText = "1";
                    taSopHdrNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("SOPNUMBE");
                    NodeElement.InnerText = header["DOCNUMBE"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("CUSTNMBR");
                    NodeElement.InnerText = header["CUSTOMER"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("DOCDATE");
                    NodeElement.InnerText = DateTime.Parse(header["DOCDATE"].ToString()).Year + "-" + DateTime.Parse(header["DOCDATE"].ToString()).Month + "-" + DateTime.Parse(header["DOCDATE"].ToString()).Day;
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("LOCNCODE");
                    NodeElement.InnerText = item["LOCNCODE"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("ITEMNMBR");
                    NodeElement.InnerText = item["ITEMNMBR"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("UNITPRCE");
                    NodeElement.InnerText = item["UNITPRCE"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("XTNDPRCE");
                    NodeElement.InnerText = item["XTNDPRCE"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("QUANTITY");
                    NodeElement.InnerText = item["QUANTITY"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    //NodeElement = eConnectXMLDocument.CreateElement("QTYONHND");
                    //NodeElement.InnerText = item["QUANTITY"].ToString();
                    //taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("MRKDNAMT");
                    NodeElement.InnerText = item["MRKDNAMT"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("UOFM");
                    NodeElement.InnerText = item["UOFM"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("USERDEF1");
                    NodeElement.InnerText = item["USERDEF1"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("USERDEF2");
                    NodeElement.InnerText = item["USERDEF2"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("USERDEF3");
                    NodeElement.InnerText = item["USERDEF3"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("USERDEF4");
                    NodeElement.InnerText = item["USERDEF4"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("USERDEF5");
                    NodeElement.InnerText = item["USERDEF5"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    taSopLine_ItemsNode.AppendChild(taSopLineNode);
                }
                #endregion
                // Payments
                // Details
                Decimal totalPaid = 0;

                taCreateSopPaymentInsertRecord_Items = eConnectXMLDocument.CreateElement("taCreateSopPaymentInsertRecord_Items");

                foreach (DataRow item in data.Tables["PAYMENTS"].Rows)
                {
                    taCreateSopPaymentInsertRecord = eConnectXMLDocument.CreateElement("taCreateSopPaymentInsertRecord");
                    NodeElement = eConnectXMLDocument.CreateElement("SOPTYPE");
                    NodeElement.InnerText = "3"; //return
                    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("SOPNUMBE");
                    NodeElement.InnerText = header["DOCNUMBE"].ToString();
                    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("CUSTNMBR");
                    NodeElement.InnerText = header["CUSTOMER"].ToString();
                    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);


                    NodeElement = eConnectXMLDocument.CreateElement("DOCDATE");
                    NodeElement.InnerText = DateTime.Parse(item["DOCDATE"].ToString()).Year + "-" + DateTime.Parse(header["DOCDATE"].ToString()).Month + "-" + DateTime.Parse(header["DOCDATE"].ToString()).Day;
                    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("DOCAMNT");
                    NodeElement.InnerText = item["DOCAMNT"].ToString();
                    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                    totalPaid += (Decimal)item["DOCAMNT"];

                    NodeElement = eConnectXMLDocument.CreateElement("CHEKBKID");
                    NodeElement.InnerText = item["CHEKBKID"].ToString();
                    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("CARDNAME");
                    NodeElement.InnerText = item["CARDNAME"].ToString();
                    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("CHEKNMBR");
                    NodeElement.InnerText = item["CHEKNMBR"].ToString();
                    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("RCTNCCRD");
                    NodeElement.InnerText = item["RCTNCCRD"].ToString();
                    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                    //NodeElement = eConnectXMLDocument.CreateElement("QTYONHND");
                    //NodeElement.InnerText = item["QUANTITY"].ToString();
                    //taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("AUTHCODE");
                    NodeElement.InnerText = item["AUTHCODE"].ToString();
                    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("EXPNDATE");
                    NodeElement.InnerText = ((DateTime)item["EXPNDATE"]).ToString("yyyy-MM-dd");
                    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("PYMTTYPE");
                    NodeElement.InnerText = item["PYMTTYPE"].ToString();
                    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("DOCNUMBR");
                    NodeElement.InnerText = item["DOCNUMBR"].ToString();
                    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("USRDEFND1");
                    NodeElement.InnerText = item["USERDEF1"].ToString();
                    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("USERDEF2");
                    NodeElement.InnerText = item["USERDEF2"].ToString();
                    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("USERDEF3");
                    NodeElement.InnerText = item["USERDEF3"].ToString();
                    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("USERDEF4");
                    NodeElement.InnerText = item["USERDEF4"].ToString();
                    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("USERDEF5");
                    NodeElement.InnerText = item["USERDEF5"].ToString();
                    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                    taCreateSopPaymentInsertRecord_Items.AppendChild(taCreateSopPaymentInsertRecord);
                }

                NodeElement = eConnectXMLDocument.CreateElement("PYMTRCVD");
                NodeElement.InnerText = totalPaid.ToString().Replace(",", ".").Replace(".", ".");
                taSopHdrNode.AppendChild(NodeElement);

                NodeSchema.AppendChild(taSopLine_ItemsNode);
                NodeSchema.AppendChild(taCreateSopPaymentInsertRecord_Items);
                NodeSchema.AppendChild(taSopHdrNode);

                // Document
                NodeeConnect.AppendChild(NodeSchema);
                eConnectXMLDocument.AppendChild(NodeeConnect);

                //Dim eConnectXMLDocString As String = eConnectXMLDocument.OuterXml;
                //MsgBox(eConnectXMLDocString)


                if (!GPHelper.eConnectSendToGP(eConnectXMLDocument.OuterXml, eConnectConnectionString))
                {
                    ((IEvent)(new ErrorEvent("", "", "Se detectó un error en la transacción e connect, se procederá con el rollback de la transación."))).Publish();
                    if (File.Exists(this.HistPathIn + "\\ERR_" + this.fname.Replace("\\", "")))
                    {
                        File.Delete(this.HistPathIn + "\\ERR_" + this.fname.Replace("\\", ""));
                    }
                    File.Move(this.file, this.HistPathIn + "\\ERR_" + this.fname.Replace("\\", ""));
                }
                else
                {
                    ((IEvent)(new InfoEvent("", "", "La transacción culminó satisfactoriamente."))).Publish();
                    if (File.Exists(this.HistPathIn + "\\OK_" + this.fname.Replace("\\", "")))
                    {
                        File.Delete(this.HistPathIn + "\\OK_" + this.fname.Replace("\\", ""));
                    }
                    File.Move(this.file, this.HistPathIn + "\\OK_" + this.fname.Replace("\\", ""));

                    /* FALTA AHCER LA CONSULTA PARA AÑADIR LA INFORMACIÓN FISCAL */
                    GPHelper.insertFiscalInfo(header["DOCNUMBE"].ToString(),
                        header["COO"].ToString(),
                        header["DATEGEN"].ToString(),
                        header["SERIALPRINTER"].ToString(),
                        StringConnection);
                    /* FALTA AHCER LA CONSULTA PARA AÑADIR LA INFORMACIÓN FISCAL */

                }
            }
        }

        private Models.Item getFromFile(string filename)
        {
            return Models.Item.LoadFrom(filename);
        }

        private bool ItemExist(Models.Item item)
        {
            bool res = false;

            SqlCommand cmd = new SqlCommand("SELECT COUNT([No]) FROM [Item] WHERE [No]=@No", new SqlConnection(this.StringConnection));
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@No", item.No);

            try
            {
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                res = ((int)cmd.ExecuteScalar()) > 0;

            }catch(Exception ex)
            {
            
                IEvent e = new ErrorEvent("", "", "TransactionAddItemID::" + ex.Message);
                e.Publish();
            }
            finally
            {
                if (cmd.Connection.State == ConnectionState.Open)
                    cmd.Connection.Close();
            }

            return res;
        }
        

        private bool InsertItem(Models.Item item)
        {
            bool res = false;
            SqlCommand command;

            if (!ItemExist(item))
                command = new SqlCommand("dbo.spInsertItem", new SqlConnection(this.StringConnection));
            else
                command = new SqlCommand("dbo.spUpdateItem", new SqlConnection(this.StringConnection));

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@No", item.No);
            command.Parameters.AddWithValue("@Description", item.Description);
            command.Parameters.AddWithValue("@Descripcion2", item.Descripcion2);
            command.Parameters.AddWithValue("@Qty_Inventory", item.Qty_Inventory);
            command.Parameters.AddWithValue("@Is_Use", 1);
            command.Parameters.AddWithValue("@Billed_Units", 0);
            command.Parameters.AddWithValue("@Qty_Purshase", 0);
            command.Parameters.AddWithValue("@Qty_Sales", 0);
            command.Parameters.AddWithValue("@Amount_Purchases", 0);
            command.Parameters.AddWithValue("@Amount_Sales", 0);
            command.Parameters.AddWithValue("@Qty_Positive_adjustment", 0);
            command.Parameters.AddWithValue("@Qty_Negative_adjustment", 0);
            command.Parameters.AddWithValue("@Amount_Positive_adjustment", 0);
            command.Parameters.AddWithValue("@Amount_Negative_adjustment", 0);
            command.Parameters.AddWithValue("@Qty_in_Order", 0);
            command.Parameters.AddWithValue("@Qty_transfer", 0);
            command.Parameters.AddWithValue("@Transfer_amount", 0);
            command.Parameters.AddWithValue("@Price_without_VAT", item.PricewithoutVAT);
            command.Parameters.AddWithValue("@Price_including_VAT", item.PriceincludingVAT);
            command.Parameters.AddWithValue("@VAT", item.VAT);
            command.Parameters.AddWithValue("@Qty_in_Transit", 0);
            command.Parameters.AddWithValue("@Qty_Sale_Returns", 0);
            command.Parameters.AddWithValue("@Qty_Purchase_Returns", 0);
            command.Parameters.AddWithValue("@Allow_Negative_Inventory", 0);
            command.Parameters.AddWithValue("@Qty_Negative", 0);
            command.Parameters.AddWithValue("@Auto_Code", "");
            command.Parameters.AddWithValue("@Variant", "");
            command.Parameters.AddWithValue("@Weight", 0);
            command.Parameters.AddWithValue("@Base_Unit_of_Measure_Purched", "");
            command.Parameters.AddWithValue("@Base_Unit_of_Measure_Sales", "");
            command.Parameters.AddWithValue("@Barcode", "");
            command.Parameters.AddWithValue("@Allow_Discount", 1);
            command.Parameters.AddWithValue("@Cost_Unity", 0);
            command.Parameters.AddWithValue("@Cost_Standard", "0");
            command.Parameters.AddWithValue("@Last_cost", 0);
            command.Parameters.AddWithValue("@date_last_cost", DateTime.Now);
            command.Parameters.AddWithValue("@Cost_indirect",0);
            command.Parameters.AddWithValue("@Cost_Ajust", 0);
            command.Parameters.AddWithValue("@Profit", 0);
            command.Parameters.AddWithValue("@Profit_percentage",0);
            command.Parameters.AddWithValue("@Special_Groups", "");
            command.Parameters.AddWithValue("@Sustitute", "");
            command.Parameters.AddWithValue("@Cross_References", "");
            command.Parameters.AddWithValue("@Additional_text","");
            command.Parameters.AddWithValue("@Imagen", DBNull.Value).SqlDbType = SqlDbType.Image;
            command.Parameters.AddWithValue("@Translations", "");
            command.Parameters.AddWithValue("@Block",0);
            command.Parameters.AddWithValue("@No_Vendor", "");
            command.Parameters.AddWithValue("@Reorder_point", 0);
            command.Parameters.AddWithValue("@Qty_Reorder_point", 0);
            command.Parameters.AddWithValue("@Inventory_Security", 0);
            command.Parameters.AddWithValue("@Reserve", 0);
            command.Parameters.AddWithValue("@Lot_size",0);
            command.Parameters.AddWithValue("@No_Serial", "");
            command.Parameters.AddWithValue("@No_Lot", "");
            command.Parameters.AddWithValue("@Calculation_Due","");
            command.Parameters.AddWithValue("@Due_Date", DateTime.Now);
            command.Parameters.AddWithValue("@Creation_date", DateTime.Now);
            command.Parameters.AddWithValue("@Created_by_User","");
            command.Parameters.AddWithValue("@Last_modified_date", DateTime.Now);
            command.Parameters.AddWithValue("@Last_modified_by_the_user", "");
            command.Parameters.AddWithValue("@Warning_shortages", 0);
            command.Parameters.AddWithValue("@Location", "");
            command.Parameters.AddWithValue("@Start_Date_counting", DateTime.Now);
            command.Parameters.AddWithValue("@End_Date_counting", DateTime.Now);
            command.Parameters.AddWithValue("@VAT_Prod_Posting_Group", "01");
            command.Parameters.AddWithValue("@VAT_Bus_Posting_Group","01");
            command.Parameters.AddWithValue("@Item_Category_Code", "01");
            command.Parameters.AddWithValue("@Product_Group_Code","01");
            

            try
            {
                if (command.Connection.State != ConnectionState.Open)
                    command.Connection.Open();

                res = (command.ExecuteNonQuery() >= 0);

            }
            catch (Exception ex)
            {
                res = false;
                
                IEvent e = new ErrorEvent("", "", "TransactionAddItemID::" + ex.Message);
                e.Publish();
            }
            finally
            {
                if (command.Connection.State != ConnectionState.Closed)
                    command.Connection.Close();
            }


            return res;
        }

        private DataSet parseXML(string filename)
        {
            DataSet data = new DataSet("CUSTOMERS");
            data.Tables.Add("CUSTOMER");

            data.Tables["CUSTOMER"].Columns.Add("No", typeof(string));
            data.Tables["CUSTOMER"].Columns.Add("Name", typeof(string));
            data.Tables["CUSTOMER"].Columns.Add("Name2", typeof(string));
            data.Tables["CUSTOMER"].Columns.Add("CodeAddress", typeof(string));
            data.Tables["CUSTOMER"].Columns.Add("VAT_Registration_No", typeof(string));
            data.Tables["CUSTOMER"].Columns.Add("Customer_Invoice_No", typeof(string));
            data.Tables["CUSTOMER"].Columns.Add("Price_Group", typeof(string));
            data.Tables["CUSTOMER"].Columns.Add("Currency", typeof(string));
            data.Tables["CUSTOMER"].Columns.Add("Language", typeof(string));
            data.Tables["CUSTOMER"].Columns.Add("Price_including_VAT", typeof(decimal));
            data.Tables["CUSTOMER"].Columns.Add("Price_without_VAT", typeof(decimal));

            data.Tables["CUSTOMER"].Columns.Add("VAT", typeof(decimal));
            data.Tables["CUSTOMER"].Columns.Add("Salesperson", typeof(string));
            data.Tables["CUSTOMER"].Columns.Add("Discounts_Invoice", typeof(decimal));
            data.Tables["CUSTOMER"].Columns.Add("Line_Discounts", typeof(decimal));

            data.Tables["CUSTOMER"].Columns.Add("Reserve", typeof(int));
            data.Tables["CUSTOMER"].Columns.Add("Payment_terms", typeof(string));
            data.Tables["CUSTOMER"].Columns.Add("Allow_discount", typeof(int));
            data.Tables["CUSTOMER"].Columns.Add("Block", typeof(int));
            data.Tables["CUSTOMER"].Columns.Add("VAT_Bus_Posting_Group", typeof(string));
            data.Tables["CUSTOMER"].Columns.Add("Creation_date", typeof(DateTime));
            data.Tables["CUSTOMER"].Columns.Add("Created_by_user", typeof(string));
            data.Tables["CUSTOMER"].Columns.Add("Last_modified_date", typeof(DateTime));
            data.Tables["CUSTOMER"].Columns.Add("Last_modified_by_the_user", typeof(string));


            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(filename);

            }
            catch (Exception ex)
            {
                ((IEvent)(new ErrorEvent("", "", ex.Message))).Publish();
                return data;
            }


            XmlNode header = doc.SelectSingleNode("/GPDocument");

            DataRow HeaderRow = data.Tables["CUSTOMER"].NewRow();

            foreach (XmlNode n in header.ChildNodes)
            {
                try
                {

                    switch (n.Name.Trim().ToUpper())
                    {
                        case "Created_by_user":
                            HeaderRow["Created_by_user"] = n.InnerText;
                            break;
                        case "No":

                            HeaderRow["No"] = n.InnerText;
                            break;
                        case "Name":

                            HeaderRow["Name"] = n.InnerText;
                            break;
                        case "Name2":

                            HeaderRow["Name2"] = n.InnerText;
                            break;
                        case "CodeAddress":
                            HeaderRow["CodeAddress"] = n.InnerText;
                            break;
                        //case "VAT_Registration_Neo":
                        //    if (n.InnerText == "")
                        //        HeaderRow["ORIGTYPE"] = 0;
                        //    else
                        //        HeaderRow["ORIGTYPE"] = int.Parse(n.InnerText);
                        //    break;
                        case "VAT_Registration_No":
                            HeaderRow["VAT_Registration_No"] = n.InnerText;
                            break;
                        case "Customer_Invoice_No":
                            HeaderRow["Customer_Invoice_No"] = n.InnerText;
                            break;
                        case "Price_Group":
                            HeaderRow["Price_Group"] = n.InnerText;
                            break;
                        case "Currency":
                            HeaderRow["Currency"] = n.InnerText;
                            break;
                        case "Price_without_VAT":
                            if (n.InnerText == "")
                                HeaderRow["Price_without_VAT"] = 0;
                            else
                                HeaderRow["Price_without_VAT"] = Decimal.Parse(n.InnerText.Replace(".", ".").Replace(",", "."));
                            break;
                        case "Language":
                            HeaderRow["Language"] = n.InnerText;
                            break;
                        case "Creation_date":
                            if (n.InnerText == "")
                                HeaderRow["Creation_date"] = "01-01-1991";
                            else
                                HeaderRow["Creation_date"] = DateTime.Parse(n.InnerText);
                            break;
                        case "Price_including_VAT":
                            if (n.InnerText == "")
                                HeaderRow["Price_including_VAT"] = 0;
                            else
                                HeaderRow["Price_including_VAT"] = Decimal.Parse(n.InnerText.Replace(".", ".").Replace(",", "."));
                            break;
                        case "VAT":
                            if (n.InnerText == "")
                                HeaderRow["VAT"] = 0;
                            else
                                HeaderRow["VAT"] = Decimal.Parse(n.InnerText.Replace(".", ".").Replace(",", "."));
                            break;
                        case "Discounts_Invoice":
                            if (n.InnerText == "")
                                HeaderRow["Discounts_Invoice"] = 0;
                            else
                                HeaderRow["Discounts_Invoice"] = Decimal.Parse(n.InnerText.Replace(".", ".").Replace(",", "."));
                            break;
                        case "Allow_discount":
                            HeaderRow["Allow_discount"] = n.InnerText;
                            break;
                        case "Last_modified_by_the_user":
                            HeaderRow["Last_modified_by_the_user"] = n.InnerText;
                            break;
                        case "Line_Discounts":
                            if (n.InnerText == "")
                                HeaderRow["Line_Discounts"] = 0;
                            else
                                HeaderRow["Line_Discounts"] = Decimal.Parse(n.InnerText.Replace(".", ".").Replace(",", "."));
                            break;

                        case "Salesperson":
                            HeaderRow["Salesperson"] = n.InnerText;
                            break;
                        case "Payment_terms":
                            HeaderRow["Payment_terms"] = n.InnerText;
                            break;

                        case "Reserve":
                            HeaderRow["Reserve"] = n.InnerText;
                            break;
                        case "Block":
                            HeaderRow["Block"] = n.InnerText;
                            break;
                        case "Last_modified_date":
                            if (n.InnerText == "")
                                HeaderRow["Last_modified_date"] = "01-01-1991";
                            else
                                HeaderRow["Last_modified_date"] = DateTime.Parse(n.InnerText);
                            break;
                        case "VAT_Bus_Posting_Group":
                            HeaderRow["VAT_Bus_Posting_Group"] = n.InnerText;
                            break;
                    }

                    data.Tables["CUSTOMER"].Rows.Add(HeaderRow);
                }
                catch (Exception ex)
                {
                    ((IEvent)(new ErrorEvent("", "", "El archivo está corrupto!." + ex.Message))).Publish();
                }
            }

            return data;

        }
    }
}
