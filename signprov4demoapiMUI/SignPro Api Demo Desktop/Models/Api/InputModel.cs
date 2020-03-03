using System;
using Newtonsoft.Json;

namespace SignPro_Api_Demo_Desktop.Models.Api
{

	public class InputModel
	{

		[JsonProperty(PropertyName = "filesystem")]
		public string FileSystem { get; set; }


		[JsonProperty(PropertyName = "http_get")]
		public string HttpGet { get; set; }
	}
}
