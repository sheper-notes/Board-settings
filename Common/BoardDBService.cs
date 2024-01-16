using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Common
{
    public class BoardDBService
    {
		IConfiguration Configuration { get; set; }

		public BoardDBService(IConfiguration configuration)
		{
			Configuration = configuration;
		}
        public async Task DeleteBoard(long id, string jwt)
        {
			try
			{
				var httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");
				var res = await httpClient.DeleteAsync($"{Configuration.GetValue<string>("boardDBServiceURL")}/boardObject/board/{id}"); 
				res.EnsureSuccessStatusCode();
			}
			catch (Exception)
			{

				throw;
			}

        }
    }
}
