[ApiController, Route("webhooks")]
public class WebhookController : ControllerBase
{
    private const string SecretKey = "chave-secreta-personalizada";
    private static readonly HashSet<string> _processedRequests = new();
    private readonly Channel<string> _queue = Channel.CreateUnbounded<string>();

    [HttpPost("banco")]
    public async Task<IActionResult> Post(
        [FromHeader(Name = "X-Signature")] string signature,
        [FromHeader(Name = "X-Timestamp")] string timestamp,
        [FromHeader(Name = "X-Request-Id")] string requestId)
    {       
        if (!long.TryParse(timestamp, out var ts))
            return Unauthorized("Timestamp inválido.");

        var requestTime = DateTimeOffset.FromUnixTimeSeconds(ts);
        if (Math.Abs((DateTimeOffset.UtcNow - requestTime).TotalMinutes) > 5)
            return Unauthorized("Timestamp expirado.");
    
        if (!_processedRequests.Add(requestId))
            return Ok("Já processado.");

        var body = await new StreamReader(Request.Body).ReadToEndAsync();
        var expectedSignature = ComputeHmac($"{timestamp}.{body}");

        if (!CryptographicOperations.FixedTimeEquals(
            Convert.FromHexString(signature.Replace("sha256=", "")),
            Convert.FromHexString(expectedSignature)))
            return Unauthorized("Assinatura inválida.");

        await _queue.Writer.WriteAsync(body);
        _ = Task.Run(() => ProcessAsync(body));

        return Ok();
    }

    private static string ComputeHmac(string data)
    {
        var key = Encoding.UTF8.GetBytes(SecretKey);
        var payload = Encoding.UTF8.GetBytes(data);
        var hash = HMACSHA256.HashData(key, payload);
        return Convert.ToHexString(hash).ToLower();
    }

    private static async Task ProcessAsync(string body)
    {
        await Task.CompletedTask;
    }
}
