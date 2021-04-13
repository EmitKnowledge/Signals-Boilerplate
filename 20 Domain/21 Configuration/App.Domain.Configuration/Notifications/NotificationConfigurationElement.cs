namespace App.Domain.Configuration.Notifications
{
	/// <summary>
	/// Windows azure service bus configuration
	/// </summary>
	public sealed class NotificationConfigurationElement
	{
		/// <summary>
		/// Represents the name of the table in the database where the messages are stored
		/// </summary>
		public string DbTableName { get; set; }
	}
}