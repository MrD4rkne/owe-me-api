using OweMe.Domain.Users;

namespace OweMe.Domain.Ledgers;

public class User : Participant
{
    public UserId UserId { get; set; }
}