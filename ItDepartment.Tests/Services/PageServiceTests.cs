using Xunit;
using Moq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using it_department_site.Services;
using it_department_site.Database;
using it_department_site.Repositories;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

public class PageServiceTests
{
    private readonly Mock<IPageRepository> _pageRepoMock;
    private readonly EncryptionService _encryption;
    private readonly PageService _service;

    public PageServiceTests()
    {
        _pageRepoMock = new Mock<IPageRepository>();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Encryption:Key", "1234567890123456" },
                { "Encryption:IV", "1234567890123456" }
            }).Build();

        _encryption = new EncryptionService(config);
        _service = new PageService(_pageRepoMock.Object, _encryption);
    }

    [Fact]
    public void GetPageById_ReturnsNull_WhenNotFound()
    {
        _pageRepoMock.Setup(r => r.GetById(99)).Returns((Page)null);

        var result = _service.GetPageById(99);

        Assert.Null(result);
    }

    [Fact]
    public void GetPageById_DecryptsContentCorrectly()
    {
        var json = "{\"main\":\"hello\"}";
        var encrypted = _encryption.Encrypt(json);

        var page = new Page
        {
            Id = 1,
            Title = "Test",
            Slug = "test",
            ContentJson = encrypted,
            LayoutType = "no-sidebar"
        };

        _pageRepoMock.Setup(r => r.GetById(1)).Returns(page);

        var result = _service.GetPageById(1);

        Assert.Equal("hello", result.Content.main);
    }

    [Fact]
    public void GetAllPages_CallsRepositoryOnce()
    {
        _pageRepoMock.Setup(r => r.GetAll()).Returns(new List<Page>());

        var result = _service.GetAllPages();

        _pageRepoMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public void CreatePage_EncryptsContentBeforeSave()
    {
        Page page = new Page
        {
            Title = "Encrypted",
            Slug = "enc",
            LayoutType = "no-sidebar",
            Content = new ContentSections { main = "secret" }
        };

        _pageRepoMock.Setup(r => r.Create(It.IsAny<Page>()));

        _service.CreatePage(page);

        _pageRepoMock.Verify(r => r.Create(It.Is<Page>(p => p.ContentJson != null && p.ContentJson != "")));
    }

    [Fact]
    public void GetPageBySlug_AdminCanViewDraft()
    {
        var page = new Page
        {
            Title = "Draft Page",
            Slug = "draft-page",
            Status = PageStatus.Draft,
            ContentJson = _encryption.Encrypt("{\"main\":\"draft\"}"),
            LayoutType = "no-sidebar"
        };

        _pageRepoMock.Setup(r => r.GetBySlug("draft-page")).Returns(page);

        var result = _service.GetPageBySlug("draft-page", isAdmin: true);

        Assert.NotNull(result);
    }

    [Fact]
    public void GetPageBySlug_NonAdminCannotViewDraft()
    {
        var page = new Page
        {
            Title = "Draft Page",
            Slug = "draft-page",
            Status = PageStatus.Draft,
            ContentJson = _encryption.Encrypt("{\"main\":\"draft\"}"),
            LayoutType = "no-sidebar"
        };

        _pageRepoMock.Setup(r => r.GetBySlug("draft-page")).Returns(page);

        var result = _service.GetPageBySlug("draft-page", isAdmin: false);

        Assert.Null(result);
    }

}