namespace App.Core.Contracts.Integrations.Example
{
    /// <summary>
    /// Example integration contract. Replace this with a real provider-neutral contract.
    /// </summary>
    public interface IExampleIntegration
    {
        /// <summary>
        /// Sends a sample request to an external system.
        /// </summary>
        ExampleIntegrationResponse Send(ExampleIntegrationRequest request);
    }
}
