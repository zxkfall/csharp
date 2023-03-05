namespace WebUseQuartzEventEf.Domain;

public class UpdateAccountEvent
{
    public string EntityId { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}