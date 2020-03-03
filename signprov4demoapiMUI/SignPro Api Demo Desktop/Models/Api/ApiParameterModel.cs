using System;
using Newtonsoft.Json;

namespace SignPro_Api_Demo_Desktop.Models.Api
{
	public class ApiParameterModel
	{

		[JsonProperty(PropertyName = "file")]
		public FileModel File { get; set; }

		[JsonProperty(PropertyName = "configuration")]
		public ConfigurationModel Configuration { get; set; }

		[JsonProperty(PropertyName = "signatures")]
		public SignatureModel[] Signatures { get; set; }

		[JsonProperty(PropertyName = "initials")]
		public InitialsModel[] Initials { get; set; }

		[JsonProperty(PropertyName = "checkboxes")]
		public CheckboxModel[] Checkboxes { get; set; }

		[JsonProperty(PropertyName = "radiobuttons")]
		public RadiobuttonModel[] RadioButtons { get; set; }

		[JsonProperty(PropertyName = "textfields")]
		public TextModel[] TextFields { get; set; }

		[JsonProperty(PropertyName = "dates")]
		public DateModel[] DateFields { get; set; }
	}
}
