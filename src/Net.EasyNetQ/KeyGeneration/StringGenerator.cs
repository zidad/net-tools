using System.Globalization;

namespace Net.EasyNetQ.KeyGeneration
{
    public class StringGenerator : IKeyGenerator<string>
    {
        readonly IKeyGenerator<int> intGenerator;

        public StringGenerator(IKeyGenerator<int> intGenerator)
        {
            this.intGenerator = intGenerator;
        }

        public string NewKey()
        {
            return intGenerator.NewKey().ToString(CultureInfo.InvariantCulture);
        }
    }
}