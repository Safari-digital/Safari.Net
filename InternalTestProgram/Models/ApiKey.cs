using Digital.Net.Authentication.Models;

namespace InternalTestProgram.Models;

public class ApiKey(string? key = null, DateTime? expiredAt = null) : ApiKeyWithGuidEntity(key, expiredAt);