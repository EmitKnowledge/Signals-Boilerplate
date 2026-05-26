using App.Core.Contracts.Integrations.Example;
using Signals.Aspects.DI.Attributes;

namespace App.Integrations.Example
{
    /// <summary>
    /// Example integration implementation. Clone this project for real external providers.
    /// </summary>
    [Export(typeof(IExampleIntegration))]
    public class ExampleIntegration : IExampleIntegration
    {
        public ExampleIntegrationResponse Send(ExampleIntegrationRequest request)
        {
            return new ExampleIntegrationResponse
            {
                IsSuccess = true,
                ProviderReference = request?.ExternalId,
                ErrorMessage = null
            };
        }
    }
}
