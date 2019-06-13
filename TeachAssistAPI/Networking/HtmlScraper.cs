using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace TeachAssistAPI.Networking {
	/// <summary>
	/// Scrapes raw html data from TeachAssist.
	/// </summary>
	public class HtmlScraper {
		/// <summary>
		/// The custom web client used to access TeachAssist
		/// </summary>
		private static readonly CookieAwareWebClient client = new CookieAwareWebClient();
		
		/// <summary>
		/// Wraps the username and password data
		/// </summary>
		private UserInfo userInfo;

		/// <summary>
		/// Creates a new HtmlScraper.
		/// </summary>
		/// <param name="userInfo">The data used to login to TeachAssist.</param>
		public HtmlScraper(UserInfo userInfo) {
			this.userInfo = userInfo;
		}

		/// <summary>
		/// Scrapes the html of the main teachassist page.
		/// </summary>
		/// <returns>A list of html pages representing each course page.</returns>
		public List<string> ScrapeHtml() {
			List<string> htmlCourses = new List<string>();

			using (var client = new CookieAwareWebClient()) {
				var loginData = new NameValueCollection
				{
					{ "subject_id", "0" },
					{ "username", userInfo.username },
					{ "password", userInfo.password },
					{ "submit", "Login" }
				};

				try {
					// login using the user data
					client.UploadValues(new Uri("https://ta.yrdsb.ca/yrdsb/"), "POST", loginData);
					string mainHTML = client.DownloadString(client.ResponseUri);

					// Extract the student id from the main url after the redirection
					var stuID = GetStudentID(client.ResponseUri);
					List<string> links = GetSubjectIDs(mainHTML);

					// Extract the subject id from each course link
					foreach (string subID in links) {
						var url = "https://ta.yrdsb.ca/live/students/viewReport.php?" + string.Format($"subject_id={subID}&student_id={stuID}");
						string responseHtml = client.DownloadString(url);
						htmlCourses.Add(responseHtml);
					}
				}catch (WebException e) {
					Debug.WriteLine("Could not connect to TeachAssist! Check internet connection. \n" + e.Message);
				}
			}
			return htmlCourses;
		}

		/// <summary>
		/// Extracts the php variable "student_id" from the main page url.
		/// </summary>
		/// <param name="mainUri">The url of the main page.</param>
		/// <returns>The student id for this account.</returns>
		private string GetStudentID(Uri mainUri) {
			return mainUri.OriginalString.Replace("https://ta.yrdsb.ca/live/students/listReports.php?student_id=", "");
		}

		/// <summary>
		/// Extracts the php variable "subject_id" using the course page links.
		/// </summary>
		/// <param name="mainHtml">The html of the main page.</param>
		/// <returns>A list of the subject ids for every available course.</returns>
		private List<string> GetSubjectIDs(string mainHtml) {
			// Load the html
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(mainHtml);

			// Extract all course page links
			List<HtmlNode> linkNodes = doc.DocumentNode.SelectNodes("//div[2]/div/table//a[@href]").ToList();
			List<string> links = linkNodes.Select(n => n.Attributes["href"].Value).ToList();

			// Extract the subject_id variable from each link
			List<string> subjectIDs = new List<string>();
			Regex r = new Regex(@"\d+");
			foreach (string link in links) {
				Match m = r.Match(link);
				if (m.Success)
					subjectIDs.Add(m.Value);
			}

			return subjectIDs;
		}
	}
}
