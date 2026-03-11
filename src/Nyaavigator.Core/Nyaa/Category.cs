namespace Nyaavigator.Core.Nyaa;

public class Category : SearchOption
{
    public List<Category>? SubCategories { get; }
    public Category? Parent { get; set; }

    public Category(string title, string key, List<Category>? subCategories = null) : base(title, key)
    {
        SubCategories = subCategories;
    }

    private void SetParentOfChildren()
    {
        if (SubCategories == null)
        {
            return;
        }

        foreach (Category subCategory in SubCategories)
        {
            subCategory.Parent = this;
            if (subCategory.SubCategories != null)
            {
                subCategory.SetParentOfChildren();
            }
        }
    }

    public static List<Category> GetNyaaCategories()
    {
        List<Category> categories =
        [
            new("All", "0_0"),
            new("Anime", "1_0",
            [
                new("Anime Music Video", "1_1"),
                new("English Translated", "1_2"),
                new("Non English Translated", "1_3"),
                new("Raw", "1_4")
                ]),
            new("Audio", "2_0",
                [
                    new("Lossless", "2_1"),
                    new("Lossy", "2_2"),
                ]),
            new("Literature", "3_0",
                [
                    new("English Translated", "3_1"),
                    new("Non English Translated", "3_2"),
                    new("Raw", "3_3")
                ]),
            new("Live Action", "4_0",
                [
                    new("English Translated", "4_1"),
                    new("Idol/Promotional Video", "4_2"),
                    new("Non English Translated", "4_3"),
                    new("Raw", "4_4")
                ]),
            new("Pictures", "5_0",
                [
                    new("Graphics", "5_1"),
                    new("Photos", "5_2")
                ]),
            new("Software", "6_0",
                [
                    new("Applications", "6_1"),
                    new("Games", "6_2")
                ])
        ];

        foreach (Category category in categories)
        {
            category.SetParentOfChildren();
        }

        return categories;
    }
}