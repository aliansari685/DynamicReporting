namespace Tests.Application.Tests.Services;

public class FakeUser
{
    [SwaggerSchema("نام کاربر")]
    public string UserName { get; set; } = null!;
}