using DnsClient;
using System.ComponentModel.DataAnnotations;

namespace ASPNETCoreBlog.Infrastructure
{
    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) return false;

            var email = value.ToString();
            var domain = email.Split('@').Last();
            var lookup = new LookupClient();
            var result = lookup.Query(domain, QueryType.MX);

            if (!result.Answers.MxRecords().Any())
            {
                // Fallback to A record check if no MX records found
                var aResult = lookup.Query(domain, QueryType.A);
                return aResult.Answers.ARecords().Any();
            }

            return true;
        }
    }
}
