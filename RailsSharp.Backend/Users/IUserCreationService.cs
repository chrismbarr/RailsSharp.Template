namespace RailsSharp.Backend.Users
{
	public interface IUserCreationService
	{
		void Create(string email, string password);
	}
}