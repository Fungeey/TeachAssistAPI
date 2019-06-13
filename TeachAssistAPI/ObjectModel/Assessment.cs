using System.Collections.Generic;
using System.Linq;

namespace TeachAssistAPI.ObjectModel {
	/// <summary>
	/// Represents a single TeachAssist assessment consisting of multiple marks in different categories
	/// </summary>
	public class Assessment {

		/// <summary>
		/// The name of this assignment.
		/// </summary>
		public string name;

		/// <summary>
		/// The total percentage for this assignment.
		/// </summary>
		public float percentage;

		/// <summary>
		/// The total weight of all the marks in this assessment.
		/// </summary>
		public int totalWeightValue;

		/// <summary>
		/// The fraction of the total course that this assessment makes up.
		/// </summary>
		internal float weight;

		/// <summary>
		/// True if any of the marks have a weight of zero.
		/// </summary>
		public bool isformative;

		/// <summary>
		/// The course this assessment is a part of.
		/// </summary>
		public Course course;

		/// <summary>
		/// Represents every mark in this assignment
		/// </summary>
		public List<Mark> marks = new List<Mark>();

		/// <summary>
		/// Creates a new Assessment object.
		/// </summary>
		/// <param name="name">The name of this assessment.</param>
		/// <param name="marks">A list representing the marks for this assessment.</param>
		public Assessment(string name, List<Mark> marks) {
			this.name = name;

			this.marks = marks;
			isformative = marks.Where(m => m.weightValue == 0).Any();
			this.totalWeightValue = GetTotalWeight();
			marks.ForEach(m => m.SetAssessment(this));

			this.percentage = CalculatePercentage();
		}

		/// <summary>
		/// Sums all of the mark weights.
		/// </summary>
		/// <returns>The total weight.</returns>
		private int GetTotalWeight() {
			if (isformative) {
				return -1;
			}

			var validMarks = marks.Where(m => m.percentage != -1);
			return validMarks
				.Select(m => m.weightValue)
				.Sum();
		}

		/// <summary>
		/// Averages the marks in every category, accounting for each category weighting
		/// </summary>
		/// <returns>The total assignment average</returns>
		private float CalculatePercentage() {
			if (isformative) {
				return -1;
			}

			totalWeightValue = GetTotalWeight();

			var validMarks = marks.Where(m => m.percentage != -1);

			// (weight / totalWeight) * mark -> sum()
			float totalMarks = validMarks
				.Select(m => m.percentage * m.weight)
				.Sum();

			return totalMarks;
		}

		/// <summary>
		///	Sets the course this assessment is attached to
		/// </summary>
		public void SetCourse(Course course) {
			this.course = course;
			this.weight = (float)totalWeightValue / course.totalWeight;
		}

		/// <summary>
		/// Format: "Assignment_Name:Percentage"
		/// </summary>
		public override string ToString() {
			return string.Format($"{name}: {percentage.ToString("n2")}%");
		}
	}
}
