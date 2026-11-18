using StokPanel.Domain.Common;

namespace StokPanel.Domain.Categories
{
    public sealed class Category :AggregateRoot
    {
        //EF core ve Frontend kullanım noktası
        public const int NameMaxLength = 100;
        public const int DescriptionMaxLength = 500;

        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }

        private Category() { }
        public static Category Create(string name, string? description)
        {
            var category = new Category();
            category.SetDetails(name, description);
            category.Raise(new CategoryCreatedEvent(category.Id, category.Name));
            return category;
        }

        public void Update(string name, string? description) 
        {
            SetDetails(name, description);
            Raise(new CategoryUpdateEvent(Id, Name));
        }

        private void SetDetails(string name, string? description)
        {
            //kontroller
            var trimmedName = name?.Trim() ?? string.Empty;
            var trimmedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

            if (trimmedName.Length == 0)
                throw new DomainException("Kategori adı boş olamaz");

            if (trimmedName.Length > NameMaxLength)
                throw new DomainException($"Kategori adı en fazla {NameMaxLength} karakter olabilir. ");

            if (trimmedDescription is not null && trimmedDescription.Length > DescriptionMaxLength)
                throw new DomainException($"Açıklama en fazla {DescriptionMaxLength} karakter olabilir.");

            Name = trimmedName;
            Description = trimmedDescription;
        }

    }
}
