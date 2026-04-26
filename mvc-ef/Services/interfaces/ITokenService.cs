namespace mvc_ef.Service.interfaces
{
    public interface ITokenService
    {
        string CreateToken(int userId, string email);
    }
}
