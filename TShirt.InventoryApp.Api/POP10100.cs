//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TShirt.InventoryApp.Api
{
    using System;
    using System.Collections.Generic;
    
    public partial class POP10100
    {
        public string PONUMBER { get; set; }
        public short POSTATUS { get; set; }
        public short STATGRP { get; set; }
        public short POTYPE { get; set; }
        public string USER2ENT { get; set; }
        public string CONFIRM1 { get; set; }
        public System.DateTime DOCDATE { get; set; }
        public System.DateTime LSTEDTDT { get; set; }
        public System.DateTime LSTPRTDT { get; set; }
        public System.DateTime PRMDATE { get; set; }
        public System.DateTime PRMSHPDTE { get; set; }
        public System.DateTime REQDATE { get; set; }
        public System.DateTime REQTNDT { get; set; }
        public string SHIPMTHD { get; set; }
        public string TXRGNNUM { get; set; }
        public decimal REMSUBTO { get; set; }
        public decimal SUBTOTAL { get; set; }
        public decimal TRDISAMT { get; set; }
        public decimal FRTAMNT { get; set; }
        public decimal MSCCHAMT { get; set; }
        public decimal TAXAMNT { get; set; }
        public string VENDORID { get; set; }
        public string VENDNAME { get; set; }
        public decimal MINORDER { get; set; }
        public string VADCDPAD { get; set; }
        public short CMPANYID { get; set; }
        public string PRBTADCD { get; set; }
        public string PRSTADCD { get; set; }
        public string CMPNYNAM { get; set; }
        public string CONTACT { get; set; }
        public string ADDRESS1 { get; set; }
        public string ADDRESS2 { get; set; }
        public string ADDRESS3 { get; set; }
        public string CITY { get; set; }
        public string STATE { get; set; }
        public string ZIPCODE { get; set; }
        public string CCode { get; set; }
        public string COUNTRY { get; set; }
        public string PHONE1 { get; set; }
        public string PHONE2 { get; set; }
        public string PHONE3 { get; set; }
        public string FAX { get; set; }
        public string PYMTRMID { get; set; }
        public decimal DSCDLRAM { get; set; }
        public short DSCPCTAM { get; set; }
        public decimal DISAMTAV { get; set; }
        public System.DateTime DISCDATE { get; set; }
        public System.DateTime DUEDATE { get; set; }
        public decimal TRDPCTPR { get; set; }
        public string CUSTNMBR { get; set; }
        public short TIMESPRT { get; set; }
        public System.DateTime CREATDDT { get; set; }
        public System.DateTime MODIFDT { get; set; }
        public decimal PONOTIDS_1 { get; set; }
        public decimal PONOTIDS_2 { get; set; }
        public decimal PONOTIDS_3 { get; set; }
        public decimal PONOTIDS_4 { get; set; }
        public decimal PONOTIDS_5 { get; set; }
        public decimal PONOTIDS_6 { get; set; }
        public decimal PONOTIDS_7 { get; set; }
        public decimal PONOTIDS_8 { get; set; }
        public decimal PONOTIDS_9 { get; set; }
        public decimal PONOTIDS_10 { get; set; }
        public decimal PONOTIDS_11 { get; set; }
        public decimal PONOTIDS_12 { get; set; }
        public decimal PONOTIDS_13 { get; set; }
        public decimal PONOTIDS_14 { get; set; }
        public decimal PONOTIDS_15 { get; set; }
        public string COMMNTID { get; set; }
        public decimal CANCSUB { get; set; }
        public string CURNCYID { get; set; }
        public short CURRNIDX { get; set; }
        public string RATETPID { get; set; }
        public string EXGTBLID { get; set; }
        public decimal XCHGRATE { get; set; }
        public System.DateTime EXCHDATE { get; set; }
        public System.DateTime TIME1 { get; set; }
        public short RATECALC { get; set; }
        public decimal DENXRATE { get; set; }
        public short MCTRXSTT { get; set; }
        public decimal OREMSUBT { get; set; }
        public decimal ORSUBTOT { get; set; }
        public decimal Originating_Canceled_Sub { get; set; }
        public decimal ORTDISAM { get; set; }
        public decimal ORFRTAMT { get; set; }
        public decimal OMISCAMT { get; set; }
        public decimal ORTAXAMT { get; set; }
        public decimal ORDDLRAT { get; set; }
        public decimal ODISAMTAV { get; set; }
        public string BUYERID { get; set; }
        public decimal ONORDAMT { get; set; }
        public decimal ORORDAMT { get; set; }
        public byte HOLD { get; set; }
        public System.DateTime ONHOLDDATE { get; set; }
        public string ONHOLDBY { get; set; }
        public System.DateTime HOLDREMOVEDATE { get; set; }
        public string HOLDREMOVEBY { get; set; }
        public byte ALLOWSOCMTS { get; set; }
        public short DISGRPER { get; set; }
        public short DUEGRPER { get; set; }
        public short Revision_Number { get; set; }
        public short Change_Order_Flag { get; set; }
        public byte[] PO_Field_Changes { get; set; }
        public short PO_Status_Orig { get; set; }
        public string TAXSCHID { get; set; }
        public short TXSCHSRC { get; set; }
        public byte TXENGCLD { get; set; }
        public byte BSIVCTTL { get; set; }
        public short Purchase_Freight_Taxable { get; set; }
        public short Purchase_Misc_Taxable { get; set; }
        public string FRTSCHID { get; set; }
        public string MSCSCHID { get; set; }
        public decimal FRTTXAMT { get; set; }
        public decimal ORFRTTAX { get; set; }
        public decimal MSCTXAMT { get; set; }
        public decimal ORMSCTAX { get; set; }
        public decimal BCKTXAMT { get; set; }
        public decimal OBTAXAMT { get; set; }
        public decimal BackoutFreightTaxAmt { get; set; }
        public decimal OrigBackoutFreightTaxAmt { get; set; }
        public decimal BackoutMiscTaxAmt { get; set; }
        public decimal OrigBackoutMiscTaxAmt { get; set; }
        public short Flags { get; set; }
        public decimal BackoutTradeDiscTax { get; set; }
        public decimal OrigBackoutTradeDiscTax { get; set; }
        public string POPCONTNUM { get; set; }
        public System.DateTime CONTENDDTE { get; set; }
        public short CNTRLBLKTBY { get; set; }
        public string PURCHCMPNYNAM { get; set; }
        public string PURCHCONTACT { get; set; }
        public string PURCHADDRESS1 { get; set; }
        public string PURCHADDRESS2 { get; set; }
        public string PURCHADDRESS3 { get; set; }
        public string PURCHCITY { get; set; }
        public string PURCHSTATE { get; set; }
        public string PURCHZIPCODE { get; set; }
        public string PURCHCCode { get; set; }
        public string PURCHCOUNTRY { get; set; }
        public string PURCHPHONE1 { get; set; }
        public string PURCHPHONE2 { get; set; }
        public string PURCHPHONE3 { get; set; }
        public string PURCHFAX { get; set; }
        public decimal BLNKTLINEEXTQTYSUM { get; set; }
        public byte CBVAT { get; set; }
        public short Workflow_Approval_Status { get; set; }
        public short Workflow_Priority { get; set; }
        public System.DateTime DEX_ROW_TS { get; set; }
        public int DEX_ROW_ID { get; set; }
    }
}
