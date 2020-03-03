using System;
using Newtonsoft.Json;

namespace SignPro_Api_Demo_Desktop.Models.Api
{

	public class SignatureModel
	{

		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "signer")]
		public string Signer { get; set; }

		[JsonProperty(PropertyName = "reason")]
		public string Reason { get; set; }

		[JsonProperty(PropertyName = "type")]
		public string Type { get; set; }

		[JsonProperty(PropertyName = "biometric")]
		public bool Biometric { get; set; }

		[JsonProperty(PropertyName = "required")]
		public bool Required { get; set; }

		[JsonProperty(PropertyName = "location")]
		public LocationModel Location { get; set; }

		public SignatureModel()
		{
		}

		public SignatureModel(SignatureModel signature)
		{
			this.Name = signature.Name;
			this.Signer = signature.Signer;
			this.Reason = signature.Reason;
			this.Type = signature.Type;
			this.Biometric = signature.Biometric;
			this.Required = signature.Required;
			this.Location = new LocationModel
			{
				Page = signature.Location.Page,
				H = signature.Location.H,
				W = signature.Location.W,
				X = signature.Location.X,
				Y = signature.Location.Y
			};
		}
	}
}
