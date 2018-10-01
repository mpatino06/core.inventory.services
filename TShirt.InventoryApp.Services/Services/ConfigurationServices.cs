﻿using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TShirt.InventoryApp.Services;
using System.Configuration;
using TShirt.InventoryApp.Services.Properties;

namespace TShirt.InventoryApp.Services.Services
{
  public class ConfigurationServices
  {

    HttpClient client;
    private string PATHSERVER { get; set; }

    public ConfigurationServices()
    {
      client = new HttpClient();
      client.MaxResponseContentBufferSize = 256000;
      PATHSERVER = Settings.Default.PathFile;

    }


    public async Task<Models.Configuration> Get()
    {
      var items = new Models.Configuration();
      string uri = "http://" + PATHSERVER + "/tshirt/Configuration/GetAll";

      try
      {
        var result = await client.GetAsync(uri);
        if (result.IsSuccessStatusCode)
        {
          var content = await result.Content.ReadAsStringAsync();
          items = JsonConvert.DeserializeObject<Models.Configuration>(content);
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine(@"				ERROR {0}", ex.Message);
      }
      return items;
    }

    public async Task<bool> Save(Configuration items)
    {
      bool sample = true;
      string url = "http://" + PATHSERVER + "/tshirt/Configuration/Save";

      try
      {
        var json = JsonConvert.SerializeObject(items);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage result = null;

        result = await client.PostAsync(url, content);

        if (result.IsSuccessStatusCode)
        {
          var x = await result.Content.ReadAsStringAsync();
          sample = JsonConvert.DeserializeObject<bool>(x);

        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine(@"				ERROR {0}", ex.Message);
      }
      return sample;
    }

  }
}