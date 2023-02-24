using MyMindset.Cache.WebAPI.Models;

namespace MyMindset.Cache.WebAPI.Infrastracture
{
    public static class Seeder
    {
        public static async Task SeedAsync(this KeyAndValueContext keyAndValueContext)
        {
            if (!keyAndValueContext.KeyAndValues.Any())
            {
                int i = 1;
                foreach (string letter in Alphabet)
                {
                    keyAndValueContext.Add(new KeyValueModel
                    {
                        Key = letter,
                        Value = $"The letter \"{letter}\" is at position {i++} in the alphabet"
                    });
                }

                await keyAndValueContext.SaveChangesAsync();
            }
        }

        public static IEnumerable<string> Alphabet
        {
            get
            {
                for (char letter = 'a'; letter <= 'z'; letter++)
                {
                    yield return letter.ToString();
                }
            }
        }
    }
}
