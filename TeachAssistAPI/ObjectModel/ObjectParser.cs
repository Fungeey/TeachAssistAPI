using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace TeachAssistAPI.ObjectModel {
	/// <summary>
	/// A class to convert raw html to a collection of objects representing Course data.
	/// </summary>
	public class ObjectParser {
		/// <summary>
		/// Extracts the mark data from raw html.
		/// </summary>
		/// <param name="htmlFiles">The Html files to extract marks from.</param>
		public List<Course> ScrapeMarks(List<string> htmlFiles) {
			List<Course> courses = new List<Course>();
			htmlFiles.ForEach(f => courses.Add(ParseCourseHtml(f)));

			return courses;
		}

		/// <summary>
		/// Extracts the HtmlNodes representing every assignment in the course.
		/// </summary>
		/// <param name="html">The html file to extract from.</param>
		/// <returns>A list of HtmlNodes representing every assignment in the course.</returns>
		private Course ParseCourseHtml(string html) {
			// Load the html
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(html);
			
			string courseCode = doc.DocumentNode.SelectSingleNode(".//h2").GetDirectInnerText().Trim();

			// Select all rows in the assignment table
			var tableRows = doc.DocumentNode.SelectSingleNode("//div/div[2]/div/div/table").ChildNodes.ToList();

			// Surround each table row in <entry> tags
			StringBuilder sb = new StringBuilder();
			tableRows.ForEach(n => 
				sb.Append("<entry>")
				.Append(n.OuterHtml)
				.Append("</entry>"));

			// Load the cleaned html
			doc.LoadHtml(sb.ToString());

			// Delete first and all empty nodes
			doc.DocumentNode.ChildNodes.ToList()
				.Where(n => n.InnerHtml.Contains("<b>") || n.InnerHtml.Contains("&nbsp;") || n.InnerHtml.Trim() == "")
				.ToList().ForEach(n => n.Remove());

			// Create the list of assessments
			List<Assessment> assessments = doc.DocumentNode.ChildNodes.ToList().Select(n => ParseAssessmentNode(n)).ToList();
			return new Course(courseCode, assessments);
		}

		/// <summary>
		/// Converts an html node into an assessment object.
		/// </summary>
		/// <param name="node">The node to scrape data from.</param>
		/// <returns>The new Assessment object.</returns>
		private Assessment ParseAssessmentNode(HtmlNode node) {
			// Extract the name of the assessment
			var name = node.SelectSingleNode(".//td").GetDirectInnerText().Trim();
			name = HttpUtility.HtmlDecode(name);

			// Extract the mark
			var marks = new List<Mark>();
			node.SelectNodes(".//table").ToList().ForEach(n => marks.Add(ParseMarkNode(n)));
			
			// Create a new Assessment object wrapping the data
			return new Assessment(name, marks);
		}

		/// <summary>
		/// Converts an html node into a Mark object.
		/// </summary>
		/// <param name="node">The node to scrape data from.</param>
		/// <returns>The new Mark object.</returns>
		private Mark ParseMarkNode(HtmlNode node) {
			// Selects the node which contains the mark and weight information
			var markNode = node.SelectSingleNode(".//*[@id]");
			var contents = markNode.GetDirectInnerText().Trim();

			// Extract the weight value ("no weight" returns a weight of 0)
			var weightString = markNode.SelectSingleNode(".//font").GetDirectInnerText().Trim();
			bool weightParsed = int.TryParse(weightString.Remove(0, 7), out var weightParse);
			var weight = weightString.Contains("n") ? 0 : weightParse;

			// Get mark category based on the box color (stripping the leading '#')
			var color = markNode.GetAttributeValue("bgcolor", "ffffaa").Replace("#", "");
			Mark.markColors.TryGetValue(color, out var markCategory);

			// Extract the percentage using regex
			Match match = Regex.Match(contents, @"\d+(?=%)");
			bool percentParsed = int.TryParse(match.Value, out var percent);
			var percentage = match.Success && percentParsed ? percent : -1;

			// Create a new Mark object wrapping the data
			return new Mark(contents, weight, percentage, markCategory);
		}
	}
}
