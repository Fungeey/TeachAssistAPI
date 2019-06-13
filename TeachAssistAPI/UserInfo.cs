namespace TeachAssistAPI {
	/// <summary>
	/// A struct wrapping the login data of a TeachAssist account.
	/// </summary>
	public struct UserInfo {
		/// <summary>
		/// The username of the account.
		/// </summary>
		public string username { get; private set; }

		/// <summary>
		/// The password of the account.
		/// </summary>
		public string password { get; private set; }

		/// <summary>
		/// Creates a new UserInfo struct.
		/// </summary>
		/// <param name="username">The username of the account.</param>
		/// <param name="password">The password of the account.</param>
		public UserInfo(string username, string password) {
			this.username = username;
			this.password = password;
		}
	}
}
