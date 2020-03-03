using System;
using Newtonsoft.Json;

namespace SignPro_Api_Demo_Desktop.Models.Api
{

	public class AuthenticationModel
	{

		[JsonProperty(PropertyName = "pdf_user_password")]
		public string PdfUserPassword { get; set; }

		[JsonProperty(PropertyName = "http_user")]
		public string HttpUser { get; set; }

		[JsonProperty(PropertyName = "http_password")]
		public string HttpPassword { get; set; }
	}
}
