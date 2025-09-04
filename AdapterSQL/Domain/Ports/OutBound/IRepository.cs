namespace Domain.Ports.OutBound
{
    public interface IRepository
    {
        public Task<string> ExecutarProcedure(object transaction);
    }
}
