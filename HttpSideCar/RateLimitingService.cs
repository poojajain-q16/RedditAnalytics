using Polly;
using Polly.CircuitBreaker;
using System.Net;

namespace HttpSideCar
{

    public class RateLimitingService
    {
        private readonly HttpClient _httpClient;
        private double _remainingLimit;
        private TimeSpan _resetTime;
        private AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;
        private readonly AuthenticationService authService;

        public RateLimitingService(AuthenticationService authService)
        {
            _httpClient = new HttpClient();
            this.authService = authService;
            // Initialize _remainingLimit and _resetTime based on the rate limit headers
            _remainingLimit = double.MaxValue;
            _resetTime = TimeSpan.Zero;

            _circuitBreakerPolicy = Policy
              .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests)
              .CircuitBreakerAsync(2, TimeSpan.FromMinutes(1));
        }

        public async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request)
        {
            if (_remainingLimit <= 0)
            {
                // Wait until the reset time if no more requests are allowed
                await Task.Delay(_resetTime);
            } else
            {
                // If we have remaining requests, spread them evenly over the remaining time
                double delayInSeconds = _resetTime.TotalSeconds / (_remainingLimit + 1);
                TimeSpan delay = TimeSpan.FromSeconds(delayInSeconds);
                await Task.Delay(delay);
            }

            HttpResponseMessage response = null;

            try
            {
                string accessToken = await this.authService.GetAccessTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Postman");
                response = await _circuitBreakerPolicy.ExecuteAsync(() => _httpClient.SendAsync(request));
            }
            catch (BrokenCircuitException)
            {
                Console.WriteLine("Circuit Breaker is currently open. Please wait.");
            }
            catch (Exception ex)
            {

            }

            if (response != null)
            {
                if (response.Headers.Contains("x-ratelimit-remaining"))
                {
                    _remainingLimit = double.Parse(response.Headers.GetValues("x-ratelimit-remaining").First());
                }

                if (response.Headers.Contains("x-ratelimit-reset"))
                {
                    _resetTime = TimeSpan.FromSeconds(double.Parse(response.Headers.GetValues("x-ratelimit-reset").First()));
                }
            }

            return response;
        }
    }
}


public class TooManyRequestsException: Exception { }