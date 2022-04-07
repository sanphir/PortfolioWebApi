namespace StudyProj.WebApp.Security
{
    public interface IPasswordHasher
    {
        string Hash(string password);

        bool IsPasswordValid(string hash, string password);
    }
}
