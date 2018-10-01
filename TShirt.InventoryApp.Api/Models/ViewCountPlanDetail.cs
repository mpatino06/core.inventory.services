﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQLite.Net.Attributes;

namespace TShirt.InventoryApp.Api.Models
{
  public class ViewCountPlanDetail
  {
    public int Id { get; set; }
    public int IdCountPlan { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ProductCode { get; set; }
    public int Quantity { get; set; }
    public int? TotalCounted { get; set; }
    public string BarCode { get; set; }
    public string ProductDescription { get; set; }
    public int TotalProduct { get; set; }
    public string Warehouse { get; set; }
  }
}