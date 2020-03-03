using System;
using Newtonsoft.Json;

namespace SignPro_Api_Demo_Desktop.Models.Api
{
	public class CheckboxModel
	{

		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "value")]
		public bool Value { get; set; }

		[JsonProperty(PropertyName = "location")]
		public LocationModel Location { get; set; }
	}
}
