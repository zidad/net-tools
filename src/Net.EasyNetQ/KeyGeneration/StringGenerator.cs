using System.Globalization;

namespace Net.EasyNetQ.Persistence
{
    public class StringGenerator : IKeyGenerator<string>
    {
        private readonly IKeyGenerator<int> _intGenerator;

        public StringGenerator(IKeyGenerator<int> intGenerator)
        {
            _intGenerator = intGenerator;
        }

        public string NewKey()
        {
            return _intGenerator.NewKey().ToString(CultureInfo.InvariantCulture);
        }
    }
}