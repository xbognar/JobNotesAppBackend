namespace DataAccess.Interfaces
{
	public interface IAuthService
	{
		string Authenticate(string username, string password);
	}
}
