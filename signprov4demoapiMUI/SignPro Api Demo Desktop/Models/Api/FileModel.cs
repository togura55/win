using System;
using Newtonsoft.Json;

namespace SignPro_Api_Demo_Desktop.Models.Api
{

	public class FileModel
	{

		[JsonProperty(PropertyName = "input")]
		public InputModel Input { get; set; }

		[JsonProperty(PropertyName = "output")]
		public OutputModel Output { get; set; }

		[JsonProperty(PropertyName = "authentication")]
		public AuthenticationModel Authentication { get; set; }
	}
}
