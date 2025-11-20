using Bogus;

namespace GeoCidadao.TestShared.Fixtures
{
    public class TestFixtures
    {
        public static int GetRandomInt(int min = 1, int max = 20) => new Faker().Random.Int(min, max);
        public static string GetRandomWord() => new Faker().Lorem.Word();
        public static string GetRandomName() => new Faker().Person.FullName;
        public static string GetRandomCity() => new Faker().Address.City();
        public static string GetRandomDistrict() => new Faker().Address.Country();
        public static string? GetRandomComments()
        {
            int random = new Faker().Random.Int(0, 1);
            return random == 0 ? null : new Faker().Lorem.Sentence(new Faker().Random.Int(1, 20));
        }

        public static T GetRandomEnumValue<T>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(new Faker().Random.Int(0, values.Length - 1))!;
        }
        public static string GetRandomDate() => new Faker().Date.Past().ToString("yyyy-MM-dd");
        public static string GetRandomTime() => new Faker().Date.Recent().ToString("HH:mm");

        public static string GetRandomEmail() => new Faker().Internet.Email();
    }
}