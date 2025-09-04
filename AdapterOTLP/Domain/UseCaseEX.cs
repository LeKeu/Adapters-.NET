using System.Diagnostics;
using System.Text.Json;

namespace Domain
{
    public class UseCaseEX : IUseCaseEX
    {
        public async Task<bool> UseCaseExAsync(string caseId, string caseName)
        {
            using var _activity = Activity.Current?.Source.StartActivity($"UseCase Exemplo - {caseId}");

            try
            {
                _activity?.SetTag("case.id", caseId);
                _activity?.SetTag("case.name", caseName);
                _activity?.SetTag("operation.type", "use-case");

                await Task.Delay(100);

                _activity?.SetStatus(ActivityStatusCode.Ok);
                _activity?.SetTag("operation.result", "success");
            }
            catch (Exception ex)
            {
                _activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                _activity?.SetTag("StackTrace", ex.StackTrace);
            }

            return true;
        }
    }
}
