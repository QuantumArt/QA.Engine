using Common.Widgets;

namespace Common.PageModel
{
    public class FakeAbstractItemStorageProvider : IAbstractItemStorageProvider
    {
        public AbstractItemStorage Get()
        {
            //var root = new StartPage(1, "", "Site main page",
            //                    new TextPart(101, "t1", "Test Text Part", "Above") { Text = "<b>Content of new text widget!</b>" },
            //                    new TextPage(2, "about", "about company") { Text = "Some text" },
            //                    new TextPage(3, "help", "Help page",
            //                        new TextPart(100, "t1", "Test Text Part", "Above",
            //                            new TextPart(100, "t1", "Test Text Part", "Content") { Text = "<b>Content of nested text widget</b>" })
            //                        {
            //                            Text = "<b>Content of text widget!</b>"
            //                        },
            //                        new TextPage(4, "first", "First page") { Text = "Some first page text" },
            //                        new TextPage(5, "second", "Second page") { Text = "Some second page text" },
            //                        new TextPage(6, "another", "Another page",
            //                            new TextPage(11, "test", "Test page")
            //                            )
            //                        { Text = "Some page text" }
            //                        ),
            //                    new TextPage(7, "help-new", "Help page",
            //                        new TextPage(8, "first", "First page") { Text = "Some first page text" },
            //                        new TextPage(9, "second", "Second page") { Text = "Some second page text" },
            //                        new TextPage(10, "another", "Another page") { Text = "Some page text" }
            //                        )
            //                    );
            var root = new StartPage { Id = 1, Alias = "start", Title = "Site main page" };
            var storage = new AbstractItemStorage(root);
            return storage;
        }
    }
}
