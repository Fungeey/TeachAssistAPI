using System;
using System.Net;

namespace TeachAssistAPI.Networking {
	// WebClient is a child of Component, making this class a VS "Design Class" or something
	// This attribute ensures that opening the file opens the code as usual
	[System.ComponentModel.DesignerCategory("Code")]
	class CookieAwareWebClient : WebClient {

		/// <summary>
		/// Stores the cookies 
		/// </summary>
		public CookieContainer CookieContainer { get; private set; }

		public CookieAwareWebClient() {
			CookieContainer = new CookieContainer();
		}

		/// <summary>
		/// An overrided function to add the cookies
		/// </summary>
		protected override WebRequest GetWebRequest(Uri address) {
			// Get the base request being made 
			var request = (HttpWebRequest)base.GetWebRequest(address);



			// yoink cookies and save them to a text file so we dont have to login again?
			// Change timeout so it never times out



			// Add the existing cookie container to the Request
			request.CookieContainer = CookieContainer;

			// Continue on our merry way
			return request;
		}

		/// <summary>
		/// The uri of the web page after a redirection
		/// </summary>
		public Uri ResponseUri { get; private set; }

		/// <summary>
		/// n overrided function to extract the url of the redirected web page
		/// </summary>
		protected override WebResponse GetWebResponse(WebRequest request) {
			// Get the base response being made 
			WebResponse response = base.GetWebResponse(request);

			// Save the url for reference
			ResponseUri = response.ResponseUri;

			// Continue on our merry way
			return response;
		}
	}
}