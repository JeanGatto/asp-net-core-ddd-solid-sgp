using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using SGP.Shared.Helpers;
using SGP.Shared.Messages;

namespace SGP.Application.Requests.AuthenticationRequests
{
    public class LogInRequest : BaseRequest
    {
        public LogInRequest(string email, string password)
        {
            Email = email;
            Password = password;
        }

        [Required]
        [DataType(DataType.EmailAddress)]
        [MaxLength(100)]
        public string Email { get; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(4)]
        public string Password { get; }

        public override async Task ValidateAsync()
        {
            ValidationResult = await ValidatorHelper.ValidateAsync<LogInRequestValidator>(this);
        }
    }
}