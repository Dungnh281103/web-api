using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyAPI.Data;
using MyAPI.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAPI.Db
{
    public static class DataSeeder
    {
        public static async Task SeedData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

            
            await context.Database.MigrateAsync();

         
            await SeedUsers(userManager, roleManager);

           
            await SeedCategories(context);

           
            await SeedTags(context);

           
            await SeedStories(context);

         
            await context.SaveChangesAsync();
        }

        private static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            // Seed roles if they don't exist
            string[] roleNames = { "Admin", "Moderator", "User", "Author" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new AppRole { Name = roleName });
                }
            }

            // Seed admin user
            if (await userManager.FindByEmailAsync("admin@example.com") == null)
            {
                var admin = new AppUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    EmailConfirmed = true,
                    Nickname = "Admin",
                    Avatar = "/images/avatars/admin.jpg",
                    CreatedAt = DateTime.Now
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // Seed sample authors
            var authors = new List<(string email, string nickname, string password, string avatar)>
            {
                ("wuxia_master@example.com", "Võ Hiệp", "Author@123", "/images/avatars/wuxia_master.jpg"),
                ("immortal_jade@example.com", "Tiên Ngọc", "Author@123", "/images/avatars/immortal_jade.jpg"),
                ("sword_poet@example.com", "Kiếm Thi", "Author@123", "/images/avatars/sword_poet.jpg"),
                ("dao_seeker@example.com", "Đạo Tìm", "Author@123", "/images/avatars/dao_seeker.jpg"),
                ("phoenix_feather@example.com", "Phượng Vũ", "Author@123", "/images/avatars/phoenix_feather.jpg")
            };

            foreach (var author in authors)
            {
                if (await userManager.FindByEmailAsync(author.email) == null)
                {
                    var user = new AppUser
                    {
                        UserName = author.email,
                        Email = author.email,
                        EmailConfirmed = true,
                        Nickname = author.nickname,
                        Avatar = author.avatar,
                        CreatedAt = DateTime.Now.AddDays(-new Random().Next(1, 365))
                    };

                    var result = await userManager.CreateAsync(user, author.password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Author");
                    }
                }
            }

            // Seed sample regular users
            var users = new List<(string email, string nickname, string password, string avatar)>
            {
                ("reader1@example.com", "Độc Giả 1", "User@123", "/images/avatars/reader1.jpg"),
                ("reader2@example.com", "Độc Giả 2", "User@123", "/images/avatars/reader2.jpg"),
                ("reader3@example.com", "Độc Giả 3", "User@123", "/images/avatars/reader3.jpg")
            };

            foreach (var user in users)
            {
                if (await userManager.FindByEmailAsync(user.email) == null)
                {
                    var newUser = new AppUser
                    {
                        UserName = user.email,
                        Email = user.email,
                        EmailConfirmed = true,
                        Nickname = user.nickname,
                        Avatar = user.avatar,
                        CreatedAt = DateTime.Now.AddDays(-new Random().Next(1, 180))
                    };

                    var result = await userManager.CreateAsync(newUser, user.password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newUser, "User");
                    }
                }
            }
        }

        private static async Task SeedCategories(MyDbContext context)
        {
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category
                    {
                        Id = "tien-hiep",
                        Name = "Tiên Hiệp",
                        Description = "Thể loại truyện có yếu tố tu tiên, luyện đan, phi thăng thành tiên..."
                    },
                    new Category
                    {
                        Id = "kiem-hiep",
                        Name = "Kiếm Hiệp",
                        Description = "Thể loại truyện võ thuật kiếm hiệp trong thế giới cổ trang"
                    },
                    new Category
                    {
                        Id = "tu-chan",
                        Name = "Tu Chân",
                        Description = "Thể loại truyện tu luyện, trau dồi đạo tâm để đạt đến cảnh giới cao hơn"
                    },
                    new Category
                    {
                        Id = "huyen-huyen",
                        Name = "Huyền Huyễn",
                        Description = "Thể loại truyện thần thoại, huyền ảo"
                    },
                    new Category
                    {
                        Id = "di-gioi",
                        Name = "Dị Giới",
                        Description = "Thể loại truyện xuất hiện nhiều thế giới khác nhau"
                    },
                    new Category
                    {
                        Id = "trong-sinh",
                        Name = "Trọng Sinh",
                        Description = "Thể loại truyện nhân vật chính được tái sinh lại với ký ức từ kiếp trước"
                    }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedTags(MyDbContext context)
        {
            if (!context.Tags.Any())
            {
                var tags = new List<Tag>
                {
                    new Tag { Id = "tu-luyen", Name = "Tu Luyện", StoryCount = 0 },
                    new Tag { Id = "xuyen-khong", Name = "Xuyên Không", StoryCount = 0 },
                    new Tag { Id = "vo-cong", Name = "Võ Công", StoryCount = 0 },
                    new Tag { Id = "linh-dan", Name = "Linh Đan", StoryCount = 0 },
                    new Tag { Id = "phi-thang", Name = "Phi Thăng", StoryCount = 0 },
                    new Tag { Id = "co-trang", Name = "Cổ Trang", StoryCount = 0 },
                    new Tag { Id = "harem", Name = "Harem", StoryCount = 0 },
                    new Tag { Id = "ngon-tinh", Name = "Ngôn Tình", StoryCount = 0 },
                    new Tag { Id = "mat-the", Name = "Mạt Thế", StoryCount = 0 },
                    new Tag { Id = "nguoc", Name = "Ngược", StoryCount = 0 },
                    new Tag { Id = "sung", Name = "Sủng", StoryCount = 0 },
                    new Tag { Id = "he-thong", Name = "Hệ Thống", StoryCount = 0 },
                    new Tag { Id = "phong-van", Name = "Phong Vân", StoryCount = 0 },
                    new Tag { Id = "chinh-do", Name = "Chinh Đồ", StoryCount = 0 }
                };

                await context.Tags.AddRangeAsync(tags);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedStories(MyDbContext context)
        {
            if (!context.Stories.Any())
            {
                var random = new Random();
                DateTime now = DateTime.Now;

                // Fetch categories and tags
                var categories = await context.Categories.ToListAsync();
                var tags = await context.Tags.ToListAsync();

                var stories = new List<Story>
                {
                    new Story
                    {
                        Id = "kiem-dao-doc-ton",
                        Title = "Kiếm Đạo Độc Tôn",
                        Author = "Kiếm Thi",
                        Translator = "Phượng Vũ",
                        CoverUrl = "/images/covers/kiem-dao-doc-ton.jpg",
                        Description = "Một thanh niên bị đày xuống vực sâu vạn trượng, tìm được một thanh cổ kiếm và bắt đầu con đường tu luyện kiếm đạo, phấn đấu trở thành kiếm đạo độc tôn, xưng bá thiên hạ.",
                        Status = "Đang ra",
                        IsRecommended = true,
                        TotalChapters = 10,
                        TotalViews = random.Next(5000, 50000),
                        BookmarksCount = random.Next(100, 1000),
                        CreatedAt = now.AddDays(-random.Next(30, 100)),
                        UpdatedAt = now.AddDays(-random.Next(1, 10)),
                        LastChapterUpdate = now.AddDays(-random.Next(1, 5)),
                        Rating = new RatingSummary { Average = 4.7, Count = random.Next(50, 500) }
                    },
                    new Story
                    {
                        Id = "dai-dao-chu-tien",
                        Title = "Đại Đạo Chủ Tiên",
                        Author = "Tiên Ngọc",
                        Translator = "Đạo Tìm",
                        CoverUrl = "/images/covers/dai-dao-chu-tien.jpg",
                        Description = "Vì một cơ duyên mà Lý Đạo được thừa hưởng ký ức của tiền bối tu tiên. Từ đó, hắn bước lên con đường tu tiên đầy chông gai, với mục tiêu trở thành Đại Đạo Chủ Tiên.",
                        Status = "Đang ra",
                        IsRecommended = true,
                        TotalChapters = 15,
                        TotalViews = random.Next(8000, 80000),
                        BookmarksCount = random.Next(200, 2000),
                        CreatedAt = now.AddDays(-random.Next(60, 120)),
                        UpdatedAt = now.AddDays(-random.Next(1, 10)),
                        LastChapterUpdate = now.AddDays(-random.Next(1, 3)),
                        Rating = new RatingSummary { Average = 4.8, Count = random.Next(100, 800) }
                    },
                    new Story
                    {
                        Id = "ngu-han-phong-than",
                        Title = "Ngự Hàn Phong Thần",
                        Author = "Võ Hiệp",
                        Translator = "Kiếm Thi",
                        CoverUrl = "/images/covers/ngu-han-phong-than.jpg",
                        Description = "Thân mang hàn khí, Lâm Hàn bị coi là yêu nghiệt từ nhỏ. Trong lúc sắp bị tiêu diệt, một lão tiền bối đã truyền cho y bí kíp Ngự Hàn Công, từ đó mở ra một hành trình tu tiên đầy sóng gió.",
                        Status = "Hoàn thành",
                        IsRecommended = false,
                        TotalChapters = 8,
                        TotalViews = random.Next(3000, 30000),
                        BookmarksCount = random.Next(50, 500),
                        CreatedAt = now.AddDays(-random.Next(150, 200)),
                        UpdatedAt = now.AddDays(-random.Next(30, 60)),
                        LastChapterUpdate = now.AddDays(-random.Next(30, 60)),
                        Rating = new RatingSummary { Average = 4.5, Count = random.Next(30, 300) }
                    },
                    new Story
                    {
                        Id = "bach-luyen-thanh-tien",
                        Title = "Bách Luyện Thành Tiên",
                        Author = "Đạo Tìm",
                        Translator = "Võ Hiệp",
                        CoverUrl = "/images/covers/bach-luyen-thanh-tien.jpg",
                        Description = "Trương Thiên là một thanh niên bình thường ở thế kỷ 21, vô tình được một vị tiên nhân truyền thụ công pháp. Từ đó, hắn bắt đầu con đường tu luyện, trải qua trăm ngàn khó khăn để đạt đến cảnh giới tiên nhân.",
                        Status = "Đang ra",
                        IsRecommended = true,
                        TotalChapters = 12,
                        TotalViews = random.Next(10000, 100000),
                        BookmarksCount = random.Next(300, 3000),
                        CreatedAt = now.AddDays(-random.Next(40, 90)),
                        UpdatedAt = now.AddDays(-random.Next(1, 10)),
                        LastChapterUpdate = now.AddDays(-random.Next(1, 5)),
                        Rating = new RatingSummary { Average = 4.9, Count = random.Next(150, 1000) }
                    },
                    new Story
                    {
                        Id = "vo-than-ky",
                        Title = "Võ Thần Ký",
                        Author = "Phượng Vũ",
                        Translator = "Tiên Ngọc",
                        CoverUrl = "/images/covers/vo-than-ky.jpg",
                        Description = "Một câu chuyện về Lý Thần, người được mệnh danh là thiên tài võ học nhưng lại đột ngột mất đi võ công. Anh phải bắt đầu lại từ đầu và khám phá ra những bí mật về thân thế cùng những âm mưu lớn trong giới võ lâm.",
                        Status = "Đang ra",
                        IsRecommended = false,
                        TotalChapters = 7,
                        TotalViews = random.Next(2000, 20000),
                        BookmarksCount = random.Next(40, 400),
                        CreatedAt = now.AddDays(-random.Next(20, 50)),
                        UpdatedAt = now.AddDays(-random.Next(1, 10)),
                        LastChapterUpdate = now.AddDays(-random.Next(1, 5)),
                        Rating = new RatingSummary { Average = 4.3, Count = random.Next(20, 200) }
                    }
                };

                // Add stories to context
                await context.Stories.AddRangeAsync(stories);
                await context.SaveChangesAsync();

                // Assign categories and tags to stories
                var storyDict = await context.Stories.ToDictionaryAsync(s => s.Id);

                // Kiếm Đạo Độc Tôn - Kiếm hiệp, Tu chân
                await AssignCategoriesToStory(context, storyDict["kiem-dao-doc-ton"],
                    new[] { "kiem-hiep", "tu-chan" });
                await AssignTagsToStory(context, storyDict["kiem-dao-doc-ton"],
                    new[] { "tu-luyen", "vo-cong", "co-trang", "phi-thang" });

                // Đại Đạo Chủ Tiên - Tiên hiệp, Tu chân
                await AssignCategoriesToStory(context, storyDict["dai-dao-chu-tien"],
                    new[] { "tien-hiep", "tu-chan" });
                await AssignTagsToStory(context, storyDict["dai-dao-chu-tien"],
                    new[] { "tu-luyen", "linh-dan", "phi-thang", "he-thong" });

                // Ngự Hàn Phong Thần - Tiên hiệp, Huyền huyễn
                await AssignCategoriesToStory(context, storyDict["ngu-han-phong-than"],
                    new[] { "tien-hiep", "huyen-huyen" });
                await AssignTagsToStory(context, storyDict["ngu-han-phong-than"],
                    new[] { "tu-luyen", "linh-dan", "ngon-tinh", "sung" });

                // Bách Luyện Thành Tiên - Tiên hiệp, Trọng sinh
                await AssignCategoriesToStory(context, storyDict["bach-luyen-thanh-tien"],
                    new[] { "tien-hiep", "trong-sinh" });
                await AssignTagsToStory(context, storyDict["bach-luyen-thanh-tien"],
                    new[] { "tu-luyen", "xuyen-khong", "phi-thang", "he-thong" });

                // Võ Thần Ký - Kiếm hiệp, Dị giới
                await AssignCategoriesToStory(context, storyDict["vo-than-ky"],
                    new[] { "kiem-hiep", "di-gioi" });
                await AssignTagsToStory(context, storyDict["vo-than-ky"],
                    new[] { "vo-cong", "co-trang", "chinh-do", "phong-van" });

                await context.SaveChangesAsync();

                // Seed chapters for each story
                await SeedChapters(context);

                // Seed comments and ratings
                await SeedCommentsAndRatings(context);

                // Update tag counts
                await UpdateTagCounts(context);
            }
        }

        private static async Task AssignCategoriesToStory(MyDbContext context, Story story, string[] categoryIds)
        {
            var categories = await context.Categories
                .Where(c => categoryIds.Contains(c.Id))
                .ToListAsync();

            story.Categories = categories;
            await context.SaveChangesAsync();
        }

        private static async Task AssignTagsToStory(MyDbContext context, Story story, string[] tagIds)
        {
            var tags = await context.Tags
                .Where(t => tagIds.Contains(t.Id))
                .ToListAsync();

            story.Tags = tags;
            await context.SaveChangesAsync();
        }

        private static async Task SeedChapters(MyDbContext context)
        {
            var rand = new Random();
            var stories = await context.Stories.ToListAsync();
            var now = DateTime.Now;
            var chapters = new List<Chapter>();

            foreach (var story in stories)
            {
                var chapterCount = story.TotalChapters;
                var baseDate = story.CreatedAt;

                for (int i = 1; i <= chapterCount; i++)
                {
                    var chapterId = $"{story.Id}-chuong-{i}";
                    var chapterTitle = $"Chương {i}: {GetRandomChapterTitle(i, story.Title)}";
                    var createdAt = baseDate.AddDays(i * rand.Next(1, 3));
                    var updatedAt = createdAt.AddHours(rand.Next(1, 12));

                    chapters.Add(new Chapter
                    {
                        Id = chapterId,
                        StoryId = story.Id,
                        ChapterNumber = i,
                        Title = chapterTitle,
                        Content = GenerateChapterContent(i, story.Title),
                        Views = 36,
                        CreatedAt = createdAt,
                        UpdatedAt = updatedAt,
                        CommentsCount = rand.Next(0, 20)
                    });

                    // Update story's last chapter update time
                    if (i == chapterCount)
                    {
                        story.LastChapterUpdate = updatedAt;
                    }
                }
            }

            await context.Chapters.AddRangeAsync(chapters);
            await context.SaveChangesAsync();
        }

        private static async Task SeedCommentsAndRatings(MyDbContext context)
        {
            var rand = new Random();
            var stories = await context.Stories.ToListAsync();
            var chapters = await context.Chapters.ToListAsync();
            var users = await context.Users.ToListAsync();

            if (!users.Any()) return;

            var comments = new List<Comment>();
            var ratings = new List<Rating>();
            var commentContents = new[]
            {
                "Truyện hay quá! Mong tác giả ra chương mới sớm.",
                "Nhân vật chính thật ngầu, thích cách tác giả xây dựng tính cách.",
                "Cảm ơn tác giả và dịch giả đã mang đến tác phẩm tuyệt vời.",
                "Tình tiết hơi lê thê, mong tác giả cải thiện.",
                "Chương này hơi ngắn, mong chương sau dài hơn.",
                "Cảm xúc dâng trào khi đọc đoạn này.",
                "Truyện có nhiều tình tiết bất ngờ, rất hấp dẫn.",
                "Nhân vật phụ đáng yêu quá!",
                "Văn phong của tác giả rất cuốn hút.",
                "Mong truyện này được chuyển thể thành phim.",
                "Tu vi của nhân vật chính tăng hơi nhanh.",
                "Mong tác giả sẽ viết thêm ngoại truyện.",
                "Truyện có nhiều tình huống hài hước.",
                "Trận chiến vừa rồi mô tả rất chi tiết.",
                "Tôi thích cách nhân vật chính giải quyết vấn đề."
            };

            // Add comments to stories and chapters
            foreach (var story in stories)
            {
                var commentCount = rand.Next(5, 20);

                for (int i = 0; i < commentCount; i++)
                {
                    var user = users[rand.Next(users.Count)];
                    var content = commentContents[rand.Next(commentContents.Length)];
                    var createdAt = DateTime.Now.AddDays(-rand.Next(1, 30));

                    comments.Add(new Comment
                    {
                        Id = Guid.NewGuid().ToString(),
                        StoryId = story.Id,
                        UserId = user.Id,
                        Content = content,
                        CreatedAt = createdAt,
                    });
                }

                // Add some replies
                for (int i = 0; i < comments.Count / 5; i++)
                {
                    var parentComment = comments[rand.Next(comments.Count)];
                    if (string.IsNullOrEmpty(parentComment.ParentCommentId)) // Only if it's not already a reply
                    {
                        var user = users[rand.Next(users.Count)];
                        var content = $"Đồng ý với bạn. {commentContents[rand.Next(commentContents.Length)]}";
                        var createdAt = parentComment.CreatedAt.AddHours(rand.Next(1, 48));

                        comments.Add(new Comment
                        {
                            Id = Guid.NewGuid().ToString(),
                            StoryId = parentComment.StoryId,
                            ChapterId = parentComment.ChapterId,
                            ParentCommentId = parentComment.Id,
                            UserId = user.Id,
                            Content = content,
                            CreatedAt = createdAt,
                        });
                    }
                }

                // Add some chapter comments
                var storyChapters = chapters.Where(c => c.StoryId == story.Id).ToList();
                foreach (var chapter in storyChapters)
                {
                    var chapterCommentCount = rand.Next(0, 5); // Some chapters may have no comments

                    for (int i = 0; i < chapterCommentCount; i++)
                    {
                        var user = users[rand.Next(users.Count)];
                        var content = commentContents[rand.Next(commentContents.Length)];
                        var createdAt = chapter.CreatedAt.AddDays(rand.Next(0, (DateTime.Now - chapter.CreatedAt).Days));

                        comments.Add(new Comment
                        {
                            Id = Guid.NewGuid().ToString(),
                            StoryId = story.Id,
                            ChapterId = chapter.Id,
                            UserId = user.Id,
                            Content = content,
                            CreatedAt = createdAt,
                        });
                    }
                }

                // Add ratings for stories
                var ratingCount = story.Rating.Count;
                var ratingSum = ratingCount * story.Rating.Average;

                for (int i = 0; i < ratingCount; i++)
                {
                    // Try to keep the average close to the desired value
                    double ratingValue;
                    if (i < ratingCount * 0.7) // 70% of ratings close to the average
                    {
                        ratingValue = Math.Round(story.Rating.Average + rand.NextDouble() * 0.5 - 0.25, 1);
                    }
                    else // 30% more varied
                    {
                        ratingValue = Math.Round(rand.NextDouble() * 5, 1);
                    }

                    // Keep rating between 1 and 5
                    ratingValue = Math.Max(1, Math.Min(5, ratingValue));

                    var user = users[rand.Next(users.Count)];
                    var createdAt = story.CreatedAt.AddDays(rand.Next(0, (DateTime.Now - story.CreatedAt).Days));

                    ratings.Add(new Rating
                    {
                        Id = Guid.NewGuid().ToString(),
                        StoryId = story.Id,
                        UserId = user.Id,
                        Score = 4,
                        CreatedAt = createdAt
                    });
                }
            }

            await context.Comments.AddRangeAsync(comments);
            await context.Ratings.AddRangeAsync(ratings);
            await context.SaveChangesAsync();

            // Update comment counts for chapters
            foreach (var chapter in chapters)
            {
                chapter.CommentsCount = comments.Count(c => c.ChapterId == chapter.Id);
            }

            await context.SaveChangesAsync();
        }

        private static async Task UpdateTagCounts(MyDbContext context)
        {
            var tags = await context.Tags.Include(t => t.Stories).ToListAsync();

            foreach (var tag in tags)
            {
                tag.StoryCount = tag.Stories.Count;
            }

            await context.SaveChangesAsync();
        }

        private static string GetRandomChapterTitle(int chapterNumber, string storyTitle)
        {
            var titles = new List<string>
            {
                "Bắt Đầu Tu Luyện",
                "Gặp Gỡ Kỳ Nhân",
                "Bí Kíp Thất Truyền",
                "Đại Chiến Trường Môn",
                "Đột Phá Cảnh Giới",
                "Linh Dược Hiếm Có",
                "Thách Đấu Quần Hùng",
                "Bí Ẩn Huyết Mạch",
                "Truy Tìm Cổ Vật",
                "Ngộ Đạo Tiên Cảnh",
                "Huyền Môn Đại Hội",
                "Phá Giải Phong Ấn",
                "Tập Kích Bất Ngờ",
                "Tàng Bảo Khố",
                "Hạ San Lịch Lãm",
                "Thiên Kiếp Giáng Lâm",
                "Đoạt Bảo Chi Chiến",
                "Tà Ma Xuất Thế",
                "Nhất Kiếm Trảm Thiên",
                "Tiên Môn Nội Đấu"
            };

            // For later chapters, add specific progression
            if (chapterNumber > 10)
            {
                titles.AddRange(new[]
                {
                    "Vượt Qua Giới Hạn",
                    "Long Phượng Song Tu",
                    "Tiên Đạo Khó Cầu",
                    "Đại Năng Truy Sát",
                    "Đạo Tâm Kiên Định"
                });
            }

            var random = new Random();
            return titles[random.Next(titles.Count)];
        }

        private static string GenerateChapterContent(int chapterNumber, string storyTitle)
        {
            var contentParts = new List<string>
            {
                $"# {storyTitle} - Chương {chapterNumber}\n\n",
                "Mặt trời dần lên cao, ánh dương xuyên qua tầng mây mỏng chiếu xuống mặt đất. Không khí trong lành của buổi sáng sớm khiến người ta cảm thấy tỉnh táo.\n\n",
                "\"Đã đến lúc rồi,\" một thanh niên khẽ thì thầm, ánh mắt kiên định nhìn về phía chân trời xa xăm.\n\n"
            };

            // Add content based on chapter position
            if (chapterNumber == 1)
            {
                contentParts.Add("Đây là bước đầu tiên trên con đường tu luyện đầy chông gai. Không ai có thể ngờ rằng, một kẻ vô danh tiểu tốt như hắn lại có thể đi được đến tận cùng con đường này.\n\n");
                contentParts.Add("\"Ta nhất định phải thành công!\" Hắn nắm chặt tay, quyết tâm khắc sâu vào tâm khảm.\n\n");
            }
            else if (chapterNumber < 5)
            {
                contentParts.Add($"Sau những thử thách ban đầu, tu vi của hắn đã có những tiến bộ đáng kể. Nhưng con đường phía trước vẫn còn rất dài.\n\n");
                contentParts.Add($"\"Cảnh giới Luyện Khí tầng {chapterNumber} sao... Còn lâu mới đủ.\" Hắn thầm nghĩ, mắt nhìn xa xăm.\n\n");
                contentParts.Add($"Lão nhân đứng bên cạnh khẽ thở dài: \"Ngươi còn phải học nhiều lắm. Kiếm đạo vô cùng, đừng nóng vội.\"\n\n");
            }
            else if (chapterNumber < 10)
            {
                contentParts.Add($"Đối thủ trước mặt không hề đơn giản. Đôi mắt lạnh lẽo như băng của hắn ta khiến người khác phải rùng mình.\n\n");
                contentParts.Add($"\"Ngươi dám cản đường ta sao? Không biết sống chết!\" Đối phương gằn giọng, một luồng khí thế mạnh mẽ tỏa ra.\n\n");
                contentParts.Add($"\"Ta không sợ. Hôm nay, hoặc là ngươi ngã xuống, hoặc là ta vong mạng tại đây!\" Hắn rút kiếm chỉ thẳng về phía trước.\n\n");
                contentParts.Add($"Một trận chiến kinh thiên động địa sắp bắt đầu...\n\n");
            }
            else
            {
                contentParts.Add($"Trải qua vô số khó khăn, hắn đã đạt đến cảnh giới mà trước đây chỉ dám mơ ước. Nhưng thiên đạo vô tình, càng lên cao càng cô độc.\n\n");
                contentParts.Add($"\"Sư phụ, người đã đi rồi, đệ tử nhất định sẽ hoàn thành tâm nguyện của người.\" Hắn quỳ xuống, lòng tràn đầy bi thương lẫn quyết tâm.\n\n");
                contentParts.Add($"Bầu trời đêm đầy sao, dường như đang chứng kiến lời thề này. Một ngôi sao băng vụt qua, như điềm báo cho những biến cố sắp tới.\n\n");
                contentParts.Add($"Đại chiến sắp nổ ra, vận mệnh của muôn vạn sinh linh sẽ thay đổi từ đây...\n\n");
            }

            // Add cultivation scene
            contentParts.Add("Ngồi xếp bằng giữa sân, hắn bắt đầu vận chuyển công pháp. Từng luồng linh khí theo các kinh mạch trong cơ thể lưu chuyển, mỗi một vòng tuần hoàn là một bước tiến nhỏ trên con đường tu tiên.\n\n");

            // Add some dialog
            contentParts.Add("\"Ngươi nghĩ rằng chỉ bằng chút tài năng ấy đã có thể thay đổi cục diện sao?\" Lão nhân áo trắng cười nhạt.\n\n");
            contentParts.Add("\"Đệ tử biết mình còn kém cỏi, nhưng xin sư phụ hãy tin tưởng con. Con nhất định sẽ không làm người thất vọng!\" Hắn chắp tay cung kính đáp.\n\n");
            contentParts.Add("\"Hừm, lời nói suông ai cũng nói được. Ta chỉ tin vào những gì mắt ta thấy được mà thôi.\" Lão nhân phẩy tay áo, bỏ đi.\n\n");

            // Add chapter ending
            contentParts.Add("Ánh trăng dần lên cao, soi rọi cả thế gian. Trong bóng đêm mênh mang, liệu còn bao nhiêu bí mật đang chờ đợi được khám phá?\n\n");
            contentParts.Add("Hắn không biết tương lai sẽ ra sao, nhưng chỉ biết rằng, con đường mình đã chọn, dù có gian nan đến đâu cũng phải bước tiếp...\n\n");
            contentParts.Add("(Còn tiếp)\n\n");

            // Shuffle some parts to make chapters different
            var random = new Random();
            var middleParts = contentParts.Skip(3).Take(contentParts.Count - 5).ToList();
            var shuffledMiddle = middleParts.OrderBy(x => random.Next()).ToList();

            return string.Concat(
                contentParts.Take(3).Concat(shuffledMiddle).Concat(contentParts.TakeLast(2))
            );
        }
    }
}