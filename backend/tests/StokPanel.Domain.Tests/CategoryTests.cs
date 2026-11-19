using StokPanel.Domain.Categories;
using StokPanel.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StokPanel.Domain.Tests
{
    public class CategoryTests
    {
        [Fact]
        public void Create_TrimsName_AndBlankDescriptionBecomesNull()
        {
            var category = Category.Create("Kırtasiye", "  ");
            Assert.Equal("Kırtasiye", category.Name);
            Assert.Null(category.Description);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void Create_EmptyName_Throws(string name)
        {
            Assert.Throws<DomainException>(() => Category.Create(name, null));
        }

        [Fact]
        public void Create_TooLongName_Throws()
        {
            var longName = new string('a', Category.NameMaxLength + 1);

            Assert.Throws<DomainException>(() => Category.Create(longName, null));
        }

        [Fact]
        public void Update_RaisesUpdateEvent()
        {
            var category = Category.Create("Kırtasiye", null);
            category.ClearDomainEvents();

            category.Update("Ofis Malzemeleri", null);

            var domainEvent = Assert.IsType<CategoryUpdateEvent>(Assert.Single(category.DomainEvents));
            Assert.Equal("Ofis Malzemeleri", domainEvent.Name);

        }
    }
}
