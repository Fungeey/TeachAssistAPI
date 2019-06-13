using System.Collections.Generic;

namespace TeachAssistAPI.ObjectModel {
	/// <summary>
	/// Represents an assignment mark in any category.
	/// </summary>
	public class Mark {

		/// <summary>
		/// The different categories that any mark can belong to.
		/// </summary>
		public enum MarkCategories {
			Knowledge,
			Thinking,
			Communication,
			Application,
			Other
		}

		/// <summary>
		/// A lookup dictionary to categorize marks by their colour.
		/// </summary>
		// Make sure to strip the leading '#'
		internal static readonly Dictionary<string, MarkCategories> markColors = new Dictionary<string, MarkCategories>() {
			{ "ffffaa", MarkCategories.Knowledge },
			{ "c0fea4", MarkCategories.Thinking },
			{ "afafff", MarkCategories.Communication },
			{ "ffd490", MarkCategories.Application },
			{ "dedede", MarkCategories.Other }
		};

		/// <summary>
		/// The category this mark belongs to.
		/// </summary>
		public MarkCategories markCategory;

		/// <summary>
		/// The actual weight value, according to teachassist.
		/// </summary>
		public int weightValue;

		/// <summary>
		/// The weight multiplier applied to this mark depending on the other marks.
		/// </summary>
		internal float weight;

		/// <summary>
		/// The percentage of this mark.
		/// </summary>
		public int percentage;

		/// <summary>
		/// The contents of the mark on TeachAssist (eg "8 / 9 = 88%")
		/// </summary>
		public string contents;

		/// <summary>
		/// The Assessment object this mark is associated with
		/// </summary>
		public Assessment assessment;

		/// <summary>
		/// Creates a new Mark Object.
		/// </summary>
		/// <param name="contents"></param>
		/// <param name="weight"></param>
		/// <param name="percentage"></param>
		/// <param name="markCategory"></param>
		public Mark(string contents, int weight, int percentage, MarkCategories markCategory) {
			this.contents = contents;
			this.weightValue = weight;
			this.percentage = percentage;
			this.markCategory = markCategory;
		}

		/// <summary>
		/// Attaches this mark to an assignment.
		/// </summary>
		/// <param name="assessment">The assignment to associate this mark with.</param>
		internal void SetAssessment(Assessment assessment) {
			this.assessment = assessment;
			this.weight = (float)weightValue / assessment.totalWeightValue;
		}
	}
}
