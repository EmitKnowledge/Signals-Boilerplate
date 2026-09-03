namespace App.Core.Contracts.Integrations.Example
{
    /// <summary>
    /// Example provider-neutral response model for an external integration.
    /// </summary>
    public class ExampleIntegrationResponse
    {
        public bool IsSuccess { get; set; }

        public string ProviderReference { get; set; }

        public string ErrorMessage { get; set; }
    }
}
