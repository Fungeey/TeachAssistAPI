# TeachAssistAPI
A simple mark scraper / API for TeachAssist

### Setup

TeachAssistApi can be installed as a [nuget package](https://www.nuget.org/packages/TeachAssistAPI/), or manually as a dll.

### Quickstart:

```c#
MarkScraper scraper = new MarkScraper();

var userInfo = new UserInfo("your_username", "your_password");
string markDataJSON = scraper.ScrapeMarks(userInfo);
```

### Notes

Currently, Teachassist doesn’t have a public api, so the api has to login using a post request, then manually scrape and parse all of the actual page content. Also, I can only guarantee that this will work on my school’s Teachassist instance. 

