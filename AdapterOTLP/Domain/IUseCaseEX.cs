namespace Domain
{
    public interface IUseCaseEX
    {
        Task<bool> UseCaseExAsync(string caseId, string caseName);
    }
}
