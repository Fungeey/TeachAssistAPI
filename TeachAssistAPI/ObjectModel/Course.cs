using System.Collections.Generic;
using System.Linq;

namespace TeachAssistAPI.ObjectModel {
	/// <summary>
	/// Represents a Teachassist course.
	/// </summary>
	public class Course {

		/// <summary>
		/// The course code of this course.
		/// </summary>
		public string code;

		/// <summary>
		/// The course average so far.
		/// </summary>
		public float average;

		/// <summary>
		/// The total weight of every assessment.
		/// </summary>
		internal int totalWeight;

		/// <summary>
		/// Represents all of the Assessment objects in this course so far.
		/// </summary>
		public List<Assessment> assessments = new List<Assessment>();

		/// <summary>
		/// Creates a new Course object.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="assessments"></param>
		public Course(string name, List<Assessment> assessments) {
			this.code = name;

			this.assessments = assessments;
			this.totalWeight = GetTotalWeight();
			assessments.ForEach(a => a.SetCourse(this));
			this.average = CalculateAverage();
		}

		/// <summary>
		/// Sums all of the Assessment weights.
		/// </summary>
		/// <returns>The total weight of all the assessments.</returns>
		private int GetTotalWeight() {
			var validAssessments = assessments.Where(a => !a.isformative);
			return validAssessments
				.Select(a => a.totalWeightValue)
				.Sum();
		}

		/// <summary>
		/// Averages the marks of every assessment, accounting for it's weight in the final mark.
		/// </summary>
		/// <returns>The total course average.</returns>
		private float CalculateAverage() {
			totalWeight = GetTotalWeight();

			var summatives = assessments.Where(a => !a.isformative);
			if (summatives.Count() == 0) {
				return -1;
			}

			// (assignment weight / total weight) * assignment -> sum()
			return summatives
				.Select(a => (a.percentage * a.weight))
				.Sum();
		}

		/// <summary>
		/// Format: "Course_Code:Percentage".
		/// </summary>
		public override string ToString() {
			return string.Format($"{code}: {average.ToString("n2")}%");
		}
	}
}
