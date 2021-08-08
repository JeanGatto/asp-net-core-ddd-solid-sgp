using System.ComponentModel.DataAnnotations;
using SGP.Shared.Messages;

namespace SGP.Application.Requests.AuthenticationRequests
{
    public class RefreshTokenRequest : BaseRequest
    {
        public RefreshTokenRequest(string token)
        {
            Token = token;
        }

        [Required]
        public string Token { get; }

        public override void Validate()
        {
            ValidationResult = new RefreshTokenRequestValidator().Validate(this);
        }
    }
}