﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.IO;
using System.Globalization;

namespace TShirt.InventoryApp.Integration.To.GP
{
    class TransactionMakeInvoice: Transaccion, ITransaccion
    {
        private string file;
        private string fname;

        private string server;
        private string db;

        public TransactionMakeInvoice(string strcon, string nin, string nout, string hin, string hout, string fil, string srv, string datab,string itbms)
            : base("TransactionMakeInvoice", 11, strcon, nin, nout, hin, hout,itbms)
        {
            file = fil;
            fname = file.Split('\\').Last().Split('/').Last().Replace(nin, "");
            server = srv;
            db = datab;
        }

        #region Miembros de ITransaccion

        void ITransaccion.Execute()
        {
            GenerateDocument();
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
                    DOCID = "2"; //CHANGED GPHelper.returnDOCID(this.StringConnection).Trim();
                }
                else
                {
                    DOCID = "2"; //CHANGED header["DOCID"].ToString();
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
                NodeElement.InnerText = DOCID+DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("SOPNUMBE");
                NodeElement.InnerText = "1023"; //CHANGED header["DOCNUMBE"].ToString();
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("ORIGNUMB");
                NodeElement.InnerText = "1024"; //CHANGED header["ORIGNUMB"].ToString();
                taSopHdrNode.AppendChild(NodeElement);

                NodeElement = eConnectXMLDocument.CreateElement("ORIGTYPE");
                NodeElement.InnerText = "1025"; //CHANGED header["ORIGTYPE"].ToString();
                taSopHdrNode.AppendChild(NodeElement);

                if(header["TAXSCHID"].ToString().Length<=0)
                {
                    NodeElement = eConnectXMLDocument.CreateElement("TAXSCHID");
                    NodeElement.InnerText = "1024"; //CHANGED this.ITBMS;
                    taSopHdrNode.AppendChild(NodeElement);
                }
                else
                {
                    NodeElement = eConnectXMLDocument.CreateElement("TAXSCHID");
                    NodeElement.InnerText = "1024"; //CHANGED header["TAXSCHID"].ToString();
                    taSopHdrNode.AppendChild(NodeElement);
                }
                

                //NodeElement = eConnectXMLDocument.CreateElement("FRTSCHID");
                //NodeElement.InnerText = header["FRTSCHID"].ToString();
                //taSopHdrNode.AppendChild(NodeElement);

                //NodeElement = eConnectXMLDocument.CreateElement("MSCSCHID");
                //NodeElement.InnerText = header["MSCSCHID"].ToString();
                //taSopHdrNode.AppendChild(NodeElement);

                //NodeElement = eConnectXMLDocument.CreateElement("SHIPMTHD");
                //NodeElement.InnerText = header["SHIPMTHD"].ToString();
                //taSopHdrNode.AppendChild(NodeElement);
               

                //NodeElement = eConnectXMLDocument.CreateElement("LOCNCODE");
                //NodeElement.InnerText = header["LOCNCODE"].ToString();
                //taSopHdrNode.AppendChild(NodeElement);

                //NodeElement = eConnectXMLDocument.CreateElement("DOCDATE");
                //NodeElement.InnerText = DateTime.Parse(header["DOCDATE"].ToString()).Year + "-" + DateTime.Parse(header["DOCDATE"].ToString()).Month + "-" + DateTime.Parse(header["DOCDATE"].ToString()).Day;
                //taSopHdrNode.AppendChild(NodeElement);

                //NodeElement = eConnectXMLDocument.CreateElement("FREIGHT");
                //NodeElement.InnerText = header["FREIGHT"].ToString();
                //taSopHdrNode.AppendChild(NodeElement);

                //NodeElement = eConnectXMLDocument.CreateElement("MISCAMNT");
                //NodeElement.InnerText = header["MISCAMNT"].ToString();
                //taSopHdrNode.AppendChild(NodeElement);

                //NodeElement = eConnectXMLDocument.CreateElement("TRDISAMT");
                //NodeElement.InnerText = header["TRDISAMT"].ToString();
                //taSopHdrNode.AppendChild(NodeElement);

                //NodeElement = eConnectXMLDocument.CreateElement("CUSTNMBR");
                //NodeElement.InnerText = header["CUSTOMER"].ToString();
                //taSopHdrNode.AppendChild(NodeElement);

                //NodeElement = eConnectXMLDocument.CreateElement("CSTPONBR");
                //NodeElement.InnerText = header["CSTPONBR"].ToString();
                //taSopHdrNode.AppendChild(NodeElement);

                //NodeElement = eConnectXMLDocument.CreateElement("SUBTOTAL");
                //NodeElement.InnerText = header["SUBTOTAL"].ToString();
                //taSopHdrNode.AppendChild(NodeElement);

                //NodeElement = eConnectXMLDocument.CreateElement("DOCAMNT");
                //NodeElement.InnerText = header["DOCAMNT"].ToString();
                //taSopHdrNode.AppendChild(NodeElement);

                //NodeElement = eConnectXMLDocument.CreateElement("SALSTERR");
                //NodeElement.InnerText = header["SALSTERR"].ToString();
                //taSopHdrNode.AppendChild(NodeElement);

                //NodeElement = eConnectXMLDocument.CreateElement("SLPRSNID");
                //NodeElement.InnerText = header["SLPRSNID"].ToString();
                //taSopHdrNode.AppendChild(NodeElement);


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
                    NodeElement.InnerText = "1010"; //CHANGED header["DOCNUMBE"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("CUSTNMBR");
                    NodeElement.InnerText = "1"; //CHANGED header["CUSTOMER"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("DOCDATE");
                    NodeElement.InnerText = DateTime.Parse(header["DOCDATE"].ToString()).Year + "-" + DateTime.Parse(header["DOCDATE"].ToString()).Month + "-" + DateTime.Parse(header["DOCDATE"].ToString()).Day;
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("LOCNCODE");
                    NodeElement.InnerText = "1"; //CHANGED item["LOCNCODE"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("ITEMNMBR");
                    NodeElement.InnerText = "1"; //CHANGEDitem["ITEMNMBR"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("UNITPRCE");
                    NodeElement.InnerText = "1"; //CHANGED item["UNITPRCE"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("XTNDPRCE");
                    NodeElement.InnerText = item["XTNDPRCE"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("QUANTITY");
                    NodeElement.InnerText = "23"; //CHANGED item["QUANTITY"].ToString();
                    taSopLineNode.AppendChild(NodeElement);


                    NodeElement = eConnectXMLDocument.CreateElement("MRKDNAMT");
                    NodeElement.InnerText = "1"; //CHANGED item["MRKDNAMT"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("UOFM");
                    NodeElement.InnerText = "0"; //CHANGED item["UOFM"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("USERDEF1");
                    NodeElement.InnerText = "1"; //CHANGED item["USERDEF1"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("USERDEF2");
                    NodeElement.InnerText = "2"; //CHANGED item["USERDEF2"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("USERDEF3");
                    NodeElement.InnerText = "3"; //CHANGED item["USERDEF3"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("USERDEF4");
                    NodeElement.InnerText = "4"; //CHANGED item["USERDEF4"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    NodeElement = eConnectXMLDocument.CreateElement("USERDEF5");
                    NodeElement.InnerText = "5"; //CHANGED item["USERDEF5"].ToString();
                    taSopLineNode.AppendChild(NodeElement);

                    taSopLine_ItemsNode.AppendChild(taSopLineNode);
                }
                #endregion
                // Payments
                // Details
                Decimal totalPaid = 0;

                //taCreateSopPaymentInsertRecord_Items = eConnectXMLDocument.CreateElement("taCreateSopPaymentInsertRecord_Items");

                //foreach (DataRow item in data.Tables["PAYMENTS"].Rows)
                //{
                //    taCreateSopPaymentInsertRecord = eConnectXMLDocument.CreateElement("taCreateSopPaymentInsertRecord");
                //    NodeElement = eConnectXMLDocument.CreateElement("SOPTYPE");
                //    NodeElement.InnerText = "3"; //return
                //    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                //    NodeElement = eConnectXMLDocument.CreateElement("SOPNUMBE");
                //    NodeElement.InnerText = header["DOCNUMBE"].ToString();
                //    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                //    NodeElement = eConnectXMLDocument.CreateElement("CUSTNMBR");
                //    NodeElement.InnerText = header["CUSTOMER"].ToString();
                //    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);


                //    NodeElement = eConnectXMLDocument.CreateElement("DOCDATE");
                //    NodeElement.InnerText = DateTime.Parse(item["DOCDATE"].ToString()).ToString("yyyy-MM-dd");
                //    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                //    NodeElement = eConnectXMLDocument.CreateElement("DOCAMNT");
                //    NodeElement.InnerText = item["DOCAMNT"].ToString();
                //    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                //    totalPaid += (Decimal)item["DOCAMNT"];

                //    NodeElement = eConnectXMLDocument.CreateElement("CHEKBKID");
                //    NodeElement.InnerText = item["CHEKBKID"].ToString();
                //    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                //    NodeElement = eConnectXMLDocument.CreateElement("CARDNAME");
                //    NodeElement.InnerText = item["CARDNAME"].ToString();
                //    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                //    NodeElement = eConnectXMLDocument.CreateElement("CHEKNMBR");
                //    NodeElement.InnerText = item["CHEKNMBR"].ToString();
                //    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                //    NodeElement = eConnectXMLDocument.CreateElement("RCTNCCRD");
                //    NodeElement.InnerText = item["RCTNCCRD"].ToString();
                //    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                //    NodeElement = eConnectXMLDocument.CreateElement("AUTHCODE");
                //    NodeElement.InnerText = item["AUTHCODE"].ToString();
                //    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                //    NodeElement = eConnectXMLDocument.CreateElement("EXPNDATE");
                //    NodeElement.InnerText = ((DateTime)item["EXPNDATE"]).ToString("yyyy-MM-dd");
                //    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                //    NodeElement = eConnectXMLDocument.CreateElement("PYMTTYPE");
                //    NodeElement.InnerText = item["PYMTTYPE"].ToString();
                //    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                //    NodeElement = eConnectXMLDocument.CreateElement("DOCNUMBR");
                //    NodeElement.InnerText = item["DOCNUMBR"].ToString();
                //    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                //    NodeElement = eConnectXMLDocument.CreateElement("USRDEFND1");
                //    NodeElement.InnerText = item["USERDEF1"].ToString();
                //    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                //    NodeElement = eConnectXMLDocument.CreateElement("USERDEF2");
                //    NodeElement.InnerText = item["USERDEF2"].ToString();
                //    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                //    NodeElement = eConnectXMLDocument.CreateElement("USERDEF3");
                //    NodeElement.InnerText = item["USERDEF3"].ToString();
                //    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                //    NodeElement = eConnectXMLDocument.CreateElement("USERDEF4");
                //    NodeElement.InnerText = item["USERDEF4"].ToString();
                //    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                //    NodeElement = eConnectXMLDocument.CreateElement("USERDEF5");
                //    NodeElement.InnerText = item["USERDEF5"].ToString();
                //    taCreateSopPaymentInsertRecord.AppendChild(NodeElement);

                //    taCreateSopPaymentInsertRecord_Items.AppendChild(taCreateSopPaymentInsertRecord);
                //}

                //NodeElement = eConnectXMLDocument.CreateElement("PYMTRCVD");
                //NodeElement.InnerText = totalPaid.ToString().Replace(",",".").Replace(".",".");
                //taSopHdrNode.AppendChild(NodeElement);

                //NodeSchema.AppendChild(taSopLine_ItemsNode);
                //NodeSchema.AppendChild(taCreateSopPaymentInsertRecord_Items);
                //NodeSchema.AppendChild(taSopHdrNode);

                // Document
                NodeeConnect.AppendChild(NodeSchema);
                eConnectXMLDocument.AppendChild(NodeeConnect);



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
        
        private DataSet parseXML(string filename)
        {
            DataSet data = new DataSet("INVOICE");
            data.Tables.Add("HEADER");

            data.Tables["HEADER"].Columns.Add("DOCNUMBE", typeof(string));
            data.Tables["HEADER"].Columns.Add("DOCID", typeof(string));
            data.Tables["HEADER"].Columns.Add("ORIGNUMB", typeof(string));
            data.Tables["HEADER"].Columns.Add("ORIGTYPE", typeof(int));
            data.Tables["HEADER"].Columns.Add("TAXSCHID", typeof(string));
            data.Tables["HEADER"].Columns.Add("FRTSCHID", typeof(string));
            data.Tables["HEADER"].Columns.Add("MSCSCHID", typeof(string));
            data.Tables["HEADER"].Columns.Add("SHIPMTHD", typeof(string));
            data.Tables["HEADER"].Columns.Add("TAXAMNT", typeof(Decimal));
            data.Tables["HEADER"].Columns.Add("LOCNCODE", typeof(string));
            data.Tables["HEADER"].Columns.Add("DOCDATE", typeof(DateTime));
            data.Tables["HEADER"].Columns.Add("FREIGHT", typeof(Decimal));
            data.Tables["HEADER"].Columns.Add("MISCAMNT", typeof(Decimal));
            data.Tables["HEADER"].Columns.Add("TRDISAMT", typeof(Decimal));
            data.Tables["HEADER"].Columns.Add("CUSTOMER", typeof(string));
            data.Tables["HEADER"].Columns.Add("CSTPONBR", typeof(string));
            data.Tables["HEADER"].Columns.Add("SUBTOTAL", typeof(Decimal));
            data.Tables["HEADER"].Columns.Add("DOCAMNT", typeof(Decimal));
            data.Tables["HEADER"].Columns.Add("SALSTERR", typeof(string));
            data.Tables["HEADER"].Columns.Add("SLPRSNID", typeof(string));
            data.Tables["HEADER"].Columns.Add("FRTTXAMT", typeof(Decimal));
            data.Tables["HEADER"].Columns.Add("COO", typeof(string));
            data.Tables["HEADER"].Columns.Add("INVOICE", typeof(string));
            data.Tables["HEADER"].Columns.Add("DATEGEN", typeof(DateTime));
            data.Tables["HEADER"].Columns.Add("SERIALPRINTER", typeof(string));

            data.Tables.Add("LINES");
            data.Tables["LINES"].Columns.Add("LOCNCODE", typeof(string));
            data.Tables["LINES"].Columns.Add("ITEMNMBR", typeof(string));
            data.Tables["LINES"].Columns.Add("UNITPRCE", typeof(Decimal));
            data.Tables["LINES"].Columns.Add("XTNDPRCE", typeof(Decimal));
            data.Tables["LINES"].Columns.Add("QUANTITY", typeof(Decimal));
            data.Tables["LINES"].Columns.Add("MRKDNAMT", typeof(Decimal));
            data.Tables["LINES"].Columns.Add("UOFM", typeof(string));
            data.Tables["LINES"].Columns.Add("USERDEF1", typeof(string));
            data.Tables["LINES"].Columns.Add("USERDEF2", typeof(string));
            data.Tables["LINES"].Columns.Add("USERDEF3", typeof(string));
            data.Tables["LINES"].Columns.Add("USERDEF4", typeof(string));
            data.Tables["LINES"].Columns.Add("USERDEF5", typeof(string));

            data.Tables.Add("PAYMENTS");
            data.Tables["PAYMENTS"].Columns.Add("DOCAMNT", typeof(Decimal));
            data.Tables["PAYMENTS"].Columns.Add("PYMTTYPE", typeof(int));
            data.Tables["PAYMENTS"].Columns.Add("CHEKBKID", typeof(string));
            data.Tables["PAYMENTS"].Columns.Add("CHEKNMBR", typeof(string));
            data.Tables["PAYMENTS"].Columns.Add("DOCDATE", typeof(DateTime));
            data.Tables["PAYMENTS"].Columns.Add("DOCNUMBR", typeof(string));
            data.Tables["PAYMENTS"].Columns.Add("CARDNAME", typeof(string));
            data.Tables["PAYMENTS"].Columns.Add("RCTNCCRD", typeof(string));
            data.Tables["PAYMENTS"].Columns.Add("EXPNDATE", typeof(DateTime));
            data.Tables["PAYMENTS"].Columns.Add("AUTHCODE", typeof(string));
            data.Tables["PAYMENTS"].Columns.Add("USERDEF1", typeof(string));
            data.Tables["PAYMENTS"].Columns.Add("USERDEF2", typeof(string));
            data.Tables["PAYMENTS"].Columns.Add("USERDEF3", typeof(string));
            data.Tables["PAYMENTS"].Columns.Add("USERDEF4", typeof(string));
            data.Tables["PAYMENTS"].Columns.Add("USERDEF5", typeof(string));

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


            XmlNode header = doc.SelectSingleNode("/GPDocument/Head");

            DataRow HeaderRow = data.Tables["HEADER"].NewRow();

            foreach (XmlNode n in header.ChildNodes)
            {
                try
                {

                    switch (n.Name.Trim().ToUpper())
                    {
                        case "DOCUNMBE":

                            HeaderRow["DOCNUMBE"] = n.InnerText;
                            break;
                        case "DOCID":

                            HeaderRow["DOCID"] = n.InnerText;
                            break;
                        case "DOCNUMBE":

                            HeaderRow["DOCNUMBE"] = n.InnerText;
                            break;
                        case "ORIGNUMB":
                            HeaderRow["ORIGNUMB"] = n.InnerText;
                            break;
                        case "ORIGTYPE":
                            if (n.InnerText == "")
                                HeaderRow["ORIGTYPE"] = 0;
                            else
                                HeaderRow["ORIGTYPE"] = int.Parse(n.InnerText);
                            break;
                        case "TAXSCHID":
                            HeaderRow["TAXSCHID"] = n.InnerText;
                            break;
                        case "FRTSCHID":
                            HeaderRow["FRTSCHID"] = n.InnerText;
                            break;
                        case "MSCSCHID":
                            HeaderRow["MSCSCHID"] = n.InnerText;
                            break;
                        case "SHIPMTHD":
                            HeaderRow["SHIPMTHD"] = n.InnerText;
                            break;
                        case "TAXAMNT":
                            if (n.InnerText == "")
                                HeaderRow["TAXAMNT"] = 0;
                            else
                                HeaderRow["TAXAMNT"] = Decimal.Parse(n.InnerText.Replace(".", ".").Replace(",", "."));
                            break;
                        case "LOCNCODE":
                            HeaderRow["LOCNCODE"] = n.InnerText;
                            break;
                        case "DOCDATE":
                            if (n.InnerText == "")
                                HeaderRow["DOCDATE"] = "01-01-1991";
                            else
                                HeaderRow["DOCDATE"] = DateTime.Parse(n.InnerText);
                            break;
                        case "FREIGHT":
                            if (n.InnerText == "")
                                HeaderRow["FREIGHT"] = 0;
                            else
                                HeaderRow["FREIGHT"] = Decimal.Parse(n.InnerText.Replace(".", ".").Replace(",", "."));
                            break;
                        case "MISCAMNT":
                            if (n.InnerText == "")
                                HeaderRow["MISCAMNT"] = 0;
                            else
                                HeaderRow["MISCAMNT"] = Decimal.Parse(n.InnerText.Replace(".", ".").Replace(",", "."));
                            break;
                        case "TRDISAMT":
                            if (n.InnerText == "")
                                HeaderRow["TRDISAMT"] = 0;
                            else
                                HeaderRow["TRDISAMT"] = Decimal.Parse(n.InnerText.Replace(".", ".").Replace(",", "."));
                            break;
                        case "CUSTOMER":
                            HeaderRow["CUSTOMER"] = n.InnerText;
                            break;
                        case "CSTPONBR":
                            HeaderRow["CSTPONBR"] = n.InnerText;
                            break;
                        case "SUBTOTAL":
                            if (n.InnerText == "")
                                HeaderRow["SUBTOTAL"] = 0;
                            else
                                HeaderRow["SUBTOTAL"] = Decimal.Parse(n.InnerText.Replace(".", ".").Replace(",", "."));
                            break;
                        case "DOCAMNT":
                            if (n.InnerText == "")
                                HeaderRow["DOCAMNT"] = 0;
                            else
                                HeaderRow["DOCAMNT"] = Decimal.Parse(n.InnerText.Replace(".", ".").Replace(",", "."));
                            break;
                        case "SALSTERR":
                            HeaderRow["SALSTERR"] = n.InnerText;
                            break;
                        case "SLPRSNID":
                            HeaderRow["SLPRSNID"] = n.InnerText;
                            break;
                        case "FRTTXAMT":
                            if (n.InnerText == "")
                                HeaderRow["FRTTXAMT"] = 0;
                            else
                                HeaderRow["FRTTXAMT"] = Decimal.Parse(n.InnerText.Replace(".", ".").Replace(",", "."));
                            break;
                        case "COO":
                            HeaderRow["COO"] = n.InnerText;
                            break;
                        case "INVOICE":
                            HeaderRow["INVOICE"] = n.InnerText;
                            break;
                        case "DATEGEN":
                            if (n.InnerText == "")
                                HeaderRow["DATEGEN"] = "01-01-1991";
                            else
                                HeaderRow["DATEGEN"] = DateTime.Parse(n.InnerText);
                            break;
                        case "SERIALPRINTER":
                            HeaderRow["SERIALPRINTER"] = n.InnerText;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ((IEvent)(new ErrorEvent("", "", "El archivo está corrupto!." + ex.Message))).Publish();
                }
            }
            data.Tables["HEADER"].Rows.Add(HeaderRow);

            XmlNode lines = doc.SelectSingleNode("/GPDocument/Lines");
            foreach (XmlNode nde in lines.ChildNodes)
            {
                DataRow lnerow = data.Tables["LINES"].NewRow();

                foreach (XmlNode n in nde.ChildNodes)
                {
                    try
                    {
                        switch (n.Name.Trim().ToUpper())
                        {
                            case "LOCNCODE":
                                lnerow["LOCNCODE"] = n.InnerText;
                                break;
                            case "ITEMNMBR":
                                lnerow["ITEMNMBR"] = n.InnerText;
                                break;
                            case "UNITPRCE":
                                if (n.InnerText == "")
                                    lnerow["UNITPRCE"] = 0;
                                else
                                    lnerow["UNITPRCE"] = Decimal.Parse(n.InnerText.Replace(".", ".").Replace(",", "."));
                                break;
                            case "XTNDPRCE":
                                if (n.InnerText == "")
                                    lnerow["XTNDPRCE"] = 0;
                                else
                                    lnerow["XTNDPRCE"] = Decimal.Parse(n.InnerText.Replace(".", ".").Replace(",", "."));
                                break;
                            case "QUANTITY":
                                if (n.InnerText == "")
                                    lnerow["QUANTITY"] = 0;
                                else
                                    lnerow["QUANTITY"] = Decimal.Parse(n.InnerText.Replace(".", ".").Replace(",", "."));
                                break;
                            case "MRKDNAMT":
                                if (n.InnerText == "")
                                    lnerow["MRKDNAMT"] = 0;
                                else
                                    lnerow["MRKDNAMT"] = Decimal.Parse(n.InnerText.Replace(".", ".").Replace(",", "."));
                                break;
                            case "UOFM":
                                lnerow["UOFM"] = n.InnerText;
                                break;
                            case "USERDEF1":
                                lnerow["USERDEF1"] = n.InnerText;
                                break;
                            case "USERDEF2":
                                lnerow["USERDEF2"] = n.InnerText;
                                break;
                            case "USERDEF3":
                                lnerow["USERDEF3"] = n.InnerText;
                                break;
                            case "USERDEF4":
                                lnerow["USERDEF4"] = n.InnerText;
                                break;
                            case "USERDEF5":
                                lnerow["USERDEF5"] = n.InnerText;
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        ((IEvent)(new ErrorEvent("", "", "El archivo está corrupto!." + ex.Message))).Publish();
                    }
                }
                data.Tables["LINES"].Rows.Add(lnerow);
            }

            // Payments

            XmlNode payments = doc.SelectSingleNode("/GPDocument/Payments");
            foreach (XmlNode nde in payments.ChildNodes)
            {
                DataRow lnerow = data.Tables["PAYMENTS"].NewRow();

                foreach (XmlNode n in nde.ChildNodes)
                {
                    try
                    {
                        switch (n.Name.Trim().ToUpper())
                        {
                            case "DOCAMNT":
                                if (n.InnerText == "")
                                    lnerow["DOCAMNT"] = 0.00;
                                else
                                    lnerow["DOCAMNT"] = Decimal.Parse(n.InnerText.Replace(".", ".").Replace(",", "."));
                                break;
                            case "PYMTTYPE":
                                lnerow["PYMTTYPE"] = n.InnerText;
                                break;
                            case "CHEKBKID":
                                lnerow["CHEKBKID"] = n.InnerText;
                                break;
                            case "CHEKNMBR":
                                lnerow["CHEKNMBR"] = n.InnerText;
                                break;
                            case "DOCDATE":
                                if (n.InnerText == "")
                                    lnerow["DOCDATE"] = "1-1-1991";
                                else
                                    lnerow["DOCDATE"] = DateTime.Parse(n.InnerText);
                                break;
                            case "DOCNUMBR":
                                lnerow["DOCNUMBR"] = n.InnerText;
                                break;
                            case "CARDNAME":
                                lnerow["CARDNAME"] = n.InnerText;
                                break;
                            case "RCTNCCRD":
                                lnerow["RCTNCCRD"] = n.InnerText;
                                break;
                            case "EXPNDATE":
                                if (n.InnerText == "")
                                    lnerow["EXPNDATE"] = "1-1-1991";
                                else
                                    lnerow["EXPNDATE"] = DateTime.Parse(n.InnerText);
                                break;
                            case "AUTHCODE":
                                lnerow["AUTHCODE"] = n.InnerText;
                                break;
                            case "USERDEF1":
                                lnerow["USERDEF1"] = n.InnerText;
                                break;
                            case "USERDEF2":
                                lnerow["USERDEF2"] = n.InnerText;
                                break;
                            case "USERDEF3":
                                lnerow["USERDEF3"] = n.InnerText;
                                break;
                            case "USERDEF4":
                                lnerow["USERDEF4"] = n.InnerText;
                                break;
                            case "USERDEF5":
                                lnerow["USERDEF5"] = n.InnerText;
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        ((IEvent)(new ErrorEvent("", "", "El archivo está corrupto!." + ex.Message))).Publish();
                    }
                }
                data.Tables["PAYMENTS"].Rows.Add(lnerow);
            }



            return data;

        }
    }
}
