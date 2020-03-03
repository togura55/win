using System;
using Newtonsoft.Json;

namespace SignPro_Api_Demo_Desktop.Models.Api
{

	public class ConfigurationModel
	{

		[JsonProperty(PropertyName = "api_key")]
		public string ApiKey { get; set; }

		[JsonProperty(PropertyName = "show_annotate")]
		public bool ShowAnnotate { get; set; } = true;

		[JsonProperty(PropertyName = "show_manual_signature")]
		public bool ShowManualSignature { get; set; } = true;

		[JsonProperty(PropertyName = "error_handler_url")]
		public string ErrorHandlerUrl { get; set; }

		[JsonProperty(PropertyName = "required_signatures")]
		public int RequiredSignatures { get; set; }

		[JsonProperty(PropertyName = "process_text_tags")]
		public bool ProcessTextTags { get; set; }
	}
}
