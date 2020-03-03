using System;
using Newtonsoft.Json;

namespace SignPro_Api_Demo_Desktop.Models.Api
{

	public class InitialsModel
	{

		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }


		[JsonProperty(PropertyName = "signer")]
		public string Signer { get; set; }


		[JsonProperty(PropertyName = "reason")]
		public string Reason { get; set; }


		[JsonProperty(PropertyName = "required")]
		public bool Required { get; set; }


		[JsonProperty(PropertyName = "location")]
		public LocationModel Location { get; set; }


		public InitialsModel()
		{
		}

		public InitialsModel(InitialsModel initials)
		{
			this.Name = initials.Name;
			this.Signer = initials.Signer;
			this.Reason = initials.Reason;
			this.Required = initials.Required;
			this.Location = new LocationModel
			{
				Page = initials.Location.Page,
				H = initials.Location.H,
				W = initials.Location.W,
				X = initials.Location.X,
				Y = initials.Location.Y
			};
		}
	}
}
