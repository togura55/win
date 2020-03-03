using System;
using Newtonsoft.Json;

namespace SignPro_Api_Demo_Desktop.Models.Api
{

	public class OutputModel
	{

		[JsonProperty(PropertyName = "filesystem")]
		public string FileSystem { get; set; }


		[JsonProperty(PropertyName = "http_post")]
		public string HttpPost { get; set; }
	}
}
