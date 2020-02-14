using IGNKeyGen;

namespace org.igrok_net.infrastructure.domain
{
    public class LicenceKey
    {
        public long Id { get; private set; }
        public string Key { get; private set; }
        public bool IsUsed { get; private set; }

        public LicenceKey()
        {
            Key = KeyGen.GetKey();
            IsUsed = false;
        }

        internal LicenceKey(long id, string key, bool isUsed)
        {
            Id = id;
            Key = key;
            IsUsed = isUsed;
        }
    }
}
