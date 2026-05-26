namespace App.Core.Contracts.Integrations.Example
{
    /// <summary>
    /// Example provider-neutral request model for an external integration.
    /// </summary>
    public class ExampleIntegrationRequest
    {
        public string ExternalId { get; set; }

        public string Payload { get; set; }
    }
}
