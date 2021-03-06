﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TShirt.InventoryApp.Services.Models
{
  public class OutputDetail
  {
    public int Id { get; set; }
    public string ProductCode { get; set; }
    public int Quantity { get; set; }
    public int OutputId { get; set; }
    public int Warehouse { get; set; }
    [NotMapped]
    public string ProductDescription { get; set; }
    [NotMapped]
    public int QuantityAvailable { get; set; }
    [NotMapped]
    public string WarehouseName { get; set; }
  }
}