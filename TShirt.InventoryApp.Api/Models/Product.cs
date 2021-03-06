﻿using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TShirt.InventoryApp.Api.Models
{
  [Table("Product")]
  public class Product
  {
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Code { get; set; }
    public string BarCode { get; set; }
    public string Description { get; set; }
    public string Value1 { get; set; }
    public string Value2 { get; set; }
    public string Value3 { get; set; }
    public string Value4 { get; set; }
    public string Value5 { get; set; }
  }
}