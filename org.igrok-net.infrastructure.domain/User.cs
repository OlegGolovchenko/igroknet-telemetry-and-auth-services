using org.igrok.validator;

namespace org.igrok_net.infrastructure.domain
{
    public class User
    {
        public long Id { get; private set; }
        public string Mail { get; private set; }
        public long? LicenceKeyId { get; private set; }

        public User(string email, long? licenceKeyId)
        {
            EmailValidator.Activate("golovchenkoleg@gmail.com");
            EmailValidator.ValidateEmail(email);
            Mail = email;
            LicenceKeyId = licenceKeyId;
        }

        internal User(long userId, string email, long? licenceKeyId)
        {
            var mail = email;
            if(mail.Length == 254)
            {
                mail = mail.Substring(0,mail.IndexOf(' '));
            }
            Id = userId;
            Mail = mail;
            LicenceKeyId = licenceKeyId;
        }

        public void AssignLicence(long licenceId)
        {
            LicenceKeyId = licenceId;
        }

        public void ResignLicence()
        {
            LicenceKeyId = null;
        }
    }
}
